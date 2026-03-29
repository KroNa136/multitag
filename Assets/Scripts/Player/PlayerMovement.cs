using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(CameraController))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] protected Transform _cameraRoot;

    private CharacterController _controller;
    private InputManager _inputManager;
    private CameraController _cameraController;
    private Animator _animator;

    [Space]

    [SerializeField] private InputActionAsset _inputActions;

    [Space]

    [SerializeField] private float _groundDistanceOffset = 0.05f;
    [SerializeField] private LayerMask _groundMask;

    private bool _isGrounded;
    private Collider[] _groundOverlaps;

    [Space]

    [SerializeField] private float _terminalFallingVelocity = -50f;
    [SerializeField] private float _defaultVerticalForce = -1.5f;

    private float _verticalSpeed;

    [Space]

    [SerializeField] private float _runningSpeed = 3f;

    [Space]

    [SerializeField] private ParticleSystem _onBecomeLeaderParticleSystem;
    [SerializeField] private float _touchedPlayersCheckControllerRadiusInflation = 0.1f;
    [SerializeField] private float _invincibilityAfterLeadershipPassDuration = 5f;

    private Collider[] _playerOverlaps;

    private float TopY => transform.position.y + (_controller.height * 0.5f);
    private float BottomY => transform.position.y - (_controller.height * 0.5f);

    [Space]

    [SerializeField] private float _maxNetworkPositionError = 0.05f;
    [SerializeField] private bool _smoothLocalMovement = true;

    private readonly Queue<PlayerInputData> _clientInputBuffer = new();
    private readonly Queue<PlayerInputData> _serverInputBuffer = new();
    private readonly Queue<PlayerStateData> _stateBuffer = new();

    private float _tickRate = 1f / 30f;

    private Vector3 _previousSimulatedPosition;
    private Vector3 _currentSimulatedPosition;
    private float _motionInterpolationTimer;

    public bool CanBeControlledByPlayer = true;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cameraController = GetComponent<CameraController>();
        _animator = GetComponent<Animator>();

        _isGrounded = false;
        _groundOverlaps = new Collider[10];

        _verticalSpeed = 0f;

        _playerOverlaps = new Collider[10];

        var tickSystem = TickSystem.Instance;
        _tickRate = tickSystem.TickRate;
        tickSystem.OnTick.AddListener(Tick);

        _previousSimulatedPosition = transform.position;
        _currentSimulatedPosition = transform.position;
        _motionInterpolationTimer = 0f;

        GameManager.OnClientGameOver.AddListener(OnGameOver);
    }

    private void OnGameOver(int _)
    {
        CanBeControlledByPlayer = false;
    }

    [Client]
    public override void OnStartAuthority()
    {
        if (!isLocalPlayer)
            return;

        var childRenderers = GetComponentsInChildren<Renderer>().AsEnumerable();

        if (TryGetComponent(out Renderer renderer))
            childRenderers = childRenderers.Prepend(renderer);

        childRenderers.NonNullItems().ForEach(rend => rend.enabled = false);

        Camera.main.transform.SetParent(_cameraRoot);
        Camera.main.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        var playerInput = gameObject.AddComponent<PlayerInput>();
        playerInput.actions = _inputActions;
        playerInput.defaultActionMap = _inputActions.actionMaps[0].name;
        playerInput.neverAutoSwitchControlSchemes = false;
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;
        playerInput.ActivateInput();

        _inputManager = gameObject.AddComponent<InputManager>();
    }

    [Server]
    public override void OnStartServer()
    {
        if (!isServer)
            return;

        var playerData = LobbyNetworkManager.Instance.GetPlayerDataForConnection(connectionToClient);
        _ = StartCoroutine(CallRpcSubscribeToLeadChangeAfterDelay(playerData.netId));
    }

    [Server]
    public IEnumerator CallRpcSubscribeToLeadChangeAfterDelay(uint playerDataNetId)
    {
        yield return new WaitForSeconds(0.1f);
        RpcSubscribeToLeadChange(playerDataNetId);
    }

    [ClientRpc]
    public void RpcSubscribeToLeadChange(uint playerDataNetId)
    {
        var playersData = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        var playerDataToSubscribeTo = playersData.FirstOrDefault(p => p.netId == playerDataNetId);

        if (playerDataToSubscribeTo == null)
        {
            // We should leave the session here, because such situation should not happen at all.
            NetworkClient.Disconnect();
            return;
        }

        playerDataToSubscribeTo.OnStatusChanged.AddListener(ClientStatusChanged);
    }

    [Client]
    public void ClientStatusChanged(PlayerStatus newStatus)
    {
        if (!isClient)
            return;

        if (newStatus is PlayerStatus.Leader)
        {
            _ = _onBecomeLeaderParticleSystem
                .Bind(ps => { if (ps.isPlaying) ps.Stop(); })
                .Bind(ps => ps.Play());
        }

        if (isLocalPlayer)
            return;

        var renderers = GetComponentsInChildren<Renderer>().AsEnumerable();

        if (TryGetComponent(out Renderer renderer))
            renderers = renderers.Prepend(renderer);

        renderers
            .NonNullItems()
            .SelectMany(renderer => renderer.materials)
            .NonNullItems()
            .ForEach(mat => mat.color = newStatus switch
            {
                PlayerStatus.Leader => Color.red,
                PlayerStatus.Invincible => Color.yellow,
                _ => Color.white
            });
    }

    private void Update()
    {
        if (!isClient)
            return;

        Vector3 positionBeforeMovement = transform.position;

        if (_smoothLocalMovement)
        {
            if (isLocalPlayer && CanBeControlledByPlayer)
                ClientSmoothMove();
            else
                ClientInterpolateMovement();
        }

        if (!isLocalPlayer)
        {
            bool isMoving = positionBeforeMovement.x != transform.position.x || positionBeforeMovement.z != transform.position.z;
            _ = _animator.Bind(animator => animator.SetBool("Moving", isMoving));
        }
    }

    [Client]
    public void ClientSmoothMove()
    {
        if (!isClient)
            return;

        if (!_smoothLocalMovement)
            return;

        // Here we move the player immediately just for visual smoothness.
        // When the client ticks, this movement will be used to calculate a "desired move" from the simulated position to the current position.
        // The desired move will be then simulated normally using tick rate.

        Vector2 move = new(_inputManager.Horizontal, _inputManager.Vertical);

        PlayerInputData input = new()
        {
            Tick = -1,
            Look = _cameraController.Look,
            Move = move
        };

        PredictSimulation(input, Time.deltaTime);
    }

    [Client]
    public void ClientInterpolateMovement()
    {
        if (!isClient)
            return;

        if (isLocalPlayer)
            return;

        if (!_smoothLocalMovement)
            return;

        _motionInterpolationTimer += Time.deltaTime;

        float t = Mathf.Clamp01(_motionInterpolationTimer / _tickRate);
        var targetPosition = Vector3.Lerp(_previousSimulatedPosition, _currentSimulatedPosition, t);

        TeleportTo(targetPosition);
    }

    private void Tick(int tick)
    {
        // First the client needs to tick, then the server.
        // This is necessary for simulating client-server-client communication on the host.

        if (isLocalPlayer)
            ClientTick(tick);

        if (isServer)
            ServerTick(tick);
    }

    [Client]
    public void ClientTick(int tick)
    {
        if (!isLocalPlayer)
            return;

        Vector2 look = _cameraController.Look;

        Vector2 move = !CanBeControlledByPlayer ? Vector2.zero :
            _smoothLocalMovement ?
            CalculateDesiredMove(_currentSimulatedPosition, transform.position, _tickRate) :
            new Vector2(_inputManager.Horizontal, _inputManager.Vertical);

        PlayerInputData input = new()
        {
            Tick = tick,
            Look = look,
            Move = move
        };

        _clientInputBuffer.Enqueue(input);

        if (_smoothLocalMovement)
            TeleportTo(_currentSimulatedPosition);

        Simulate(input, _tickRate);

        PlayerStateData state = new()
        {
            Tick = tick,
            Look = look,
            Position = transform.position
        };

        _stateBuffer.Enqueue(state);

        if (isServer)
            ServerProcessInput(input);
        else
            CmdProcessInput(input);
    }

    [Command]
    public void CmdProcessInput(PlayerInputData input)
    {
        ServerProcessInput(input);
    }

    [Server]
    public void ServerProcessInput(PlayerInputData input)
    {
        if (!isServer)
            return;

        input.Move.x = Mathf.Clamp(input.Move.x, -1f, 1f);
        input.Move.y = Mathf.Clamp(input.Move.y, -1f, 1f);

        _serverInputBuffer.Enqueue(input);
    }

    [Server]
    public void ServerTick(int tick)
    {
        if (!isServer)
            return;

        if (_serverInputBuffer.Count == 0)
            return;

        Vector3 savedPosition = transform.position;
        Vector2 savedLook = _cameraController.Look;

        if (isClient)
        {
            Vector3 positionToSimulateFrom = isLocalPlayer ? _previousSimulatedPosition : _currentSimulatedPosition;
            TeleportTo(positionToSimulateFrom);
        }

        PlayerInputData lastReceivedInput = new();

        while (_serverInputBuffer.TryDequeue(out var input))
        {
            _cameraController.SetLook(input.Look);

            Simulate(input, _tickRate);

            var playerData = LobbyNetworkManager.Instance.GetPlayerDataForConnection(connectionToClient);

            if (playerData != null && playerData.Status is PlayerStatus.Leader)
            {
                var touchedVulnerablePlayersData = GetTouchedPlayersData()
                    .Where(p => p.Status is PlayerStatus.Normal);

                if (touchedVulnerablePlayersData.Any())
                {
                    touchedVulnerablePlayersData.RandomItem().Status = PlayerStatus.Leader;
                    playerData.Status = PlayerStatus.Invincible;
                    _ = StartCoroutine(MakeVulnerableAfterDelay(playerData, _invincibilityAfterLeadershipPassDuration));
                }
            }

            lastReceivedInput = input;
        }

        PlayerStateData state = new()
        {
            Tick = lastReceivedInput.Tick,
            Look = lastReceivedInput.Look,
            Position = transform.position
        };

        if (isClient)
        {
            Vector3 positionToRollbackTo = isLocalPlayer ? savedPosition : _previousSimulatedPosition;
            TeleportTo(positionToRollbackTo);

            if (isLocalPlayer)
                _cameraController.SetLook(savedLook);

            ClientReconcileState(state);
        }

        RpcReconcileState(state);
    }

    public IEnumerable<PlayerData> GetTouchedPlayersData()
    {
        Vector3 point0 = new(transform.position.x, TopY - _controller.radius, transform.position.z);
        Vector3 point1 = new(transform.position.x, BottomY + _controller.radius, transform.position.z);
        float inflatedRadius = _controller.radius + _touchedPlayersCheckControllerRadiusInflation;

        int overlaps = Physics.OverlapCapsuleNonAlloc(point0, point1, inflatedRadius, _playerOverlaps);

        var touchedPlayersData = _playerOverlaps
            .Take(overlaps)
            .Where(col => col != null)
            .Where(col => col.gameObject != null)
            .Select(col =>
            {
                if (col.gameObject.TryGetComponent(out PlayerMovement otherPlayerMovement) && otherPlayerMovement != this)
                    return LobbyNetworkManager.Instance.GetPlayerDataForConnection(otherPlayerMovement.connectionToClient);
                else
                    return null;
            })
            .NonNullItems()
            .ToList();

        return touchedPlayersData;
    }

    [Server]
    public IEnumerator MakeVulnerableAfterDelay(PlayerData playerData, float delay)
    {
        if (!isServer || playerData == null || playerData.Status is not PlayerStatus.Invincible)
            yield break;

        yield return new WaitForSeconds(delay);

        if (playerData != null)
            playerData.Status = PlayerStatus.Normal;
    }

    [ClientRpc]
    public void RpcReconcileState(PlayerStateData state)
    {
        if (isServer)
            return;

        ClientReconcileState(state);
    }

    [Client]
    public void ClientReconcileState(PlayerStateData serverState)
    {
        if (!isClient)
            return;

        if (isLocalPlayer)
        {
            int serverTick = serverState.Tick;

            while (_clientInputBuffer.TryPeek(out var input) && input.Tick <= serverTick)
                _ = _clientInputBuffer.Dequeue();

            while (_stateBuffer.TryPeek(out var state) && state.Tick < serverTick)
                _ = _stateBuffer.Dequeue();

            Vector3 positionOnServerTick = _stateBuffer.TryDequeue(out var stateOnServerTick) ?
                stateOnServerTick.Position :
                // fallback to current position
                transform.position;

            float error = Vector3.Distance(positionOnServerTick, serverState.Position);

            if (error < _maxNetworkPositionError)
                return;

            Vector3 savedPosition = transform.position;
            Vector2 savedLook = _cameraController.Look;

            TeleportTo(serverState.Position);

            _stateBuffer.Clear();

            while (_clientInputBuffer.TryDequeue(out var input))
            {
                _cameraController.SetLook(input.Look);
                Simulate(input, _tickRate);

                PlayerStateData state = new()
                {
                    Tick = input.Tick,
                    Look = input.Look,
                    Position = transform.position
                };

                _stateBuffer.Enqueue(state);
            }

            _cameraController.SetLook(savedLook);
        }
        else
        {
            if (!isServer)
            {
                _ = _cameraController.Bind((controller, look) => controller.SetLook(look), serverState.Look);

                _previousSimulatedPosition = _currentSimulatedPosition;
                _currentSimulatedPosition = serverState.Position;
            }

            _motionInterpolationTimer = 0f;
        }
    }

    protected void TeleportTo(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }

    private void Simulate(PlayerInputData input, float deltaTime)
    {
        _previousSimulatedPosition = _currentSimulatedPosition;

        MoveVertically(deltaTime);
        CheckGround();
        Move(input, deltaTime);

        _currentSimulatedPosition = transform.position;
    }

    private void PredictSimulation(PlayerInputData input, float deltaTime)
    {
        MoveVertically(deltaTime);
        CheckGround();
        Move(input, deltaTime);
    }

    private void CheckGround()
    {
        Vector3 groundCheckPosition = new(0f, BottomY + _controller.radius - _groundDistanceOffset, 0f);

        int overlapCount = Physics.OverlapSphereNonAlloc
        (
            position: groundCheckPosition,
            radius: _controller.radius,
            results: _groundOverlaps,
            layerMask: _groundMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );

        _isGrounded = overlapCount > 0;
    }

    private void MoveVertically(float deltaTime)
    {
        _verticalSpeed = _isGrounded ?
            _defaultVerticalForce :
            Mathf.Clamp
            (
                value: _verticalSpeed + Physics.gravity.y * deltaTime,
                min: _terminalFallingVelocity,
                max: float.PositiveInfinity
            );

        _ = _controller.Move(_verticalSpeed * deltaTime * Vector3.up);
    }

    private void Move(PlayerInputData input, float deltaTime)
    {
        Vector3 clampedMove = Vector3.ClampMagnitude(transform.right * input.Move.x + transform.forward * input.Move.y, 1f);
        _ = _controller.Move(_runningSpeed * deltaTime * clampedMove);
    }

    private Vector2 CalculateDesiredMove(Vector3 startPosition, Vector3 endPosition, float deltaTime)
    {
        Vector3 startPositionWithoutY = new(startPosition.x, 0f, startPosition.z);
        Vector3 endPositionWithoutY = new(endPosition.x, 0f, endPosition.z);

        Vector3 positionDelta = endPositionWithoutY - startPositionWithoutY;

        float forwardProjection = Vector3.Dot(positionDelta, transform.forward);
        float rightProjection = Vector3.Dot(positionDelta, transform.right);

        return new Vector2(rightProjection, forwardProjection) / (_runningSpeed * deltaTime);
    }
}
