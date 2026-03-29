using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public static UnityEvent OnClientGameInitiated = new();
    public static UnityEvent<int> OnClientTimerUpdated = new();
    public static UnityEvent OnClientGameStarted = new();
    public static UnityEvent<int> OnClientGameOver = new();

    [Space]

    [SerializeField][Min(0)] private int _playersSpawnTime = 5;
    [SerializeField][Min(0)] private int _gameTime = 60;
    public int GameTime => _gameTime;

    [Space]

    [SerializeField] private Transform _spawnPointsParent;
    private List<Transform> _freeSpawnPoints = new();

    public GameObject NetworkTimerPrefab { get; set; }

    private NetworkTimer _playersSpawnTimer;
    private NetworkTimer _gameTimer;

    private bool _gameStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    [Server]
    public IEnumerator ServerInitAndStartGame()
    {
        if (!isServer)
            yield break;

        _spawnPointsParent = GameObject.Find("Spawn Points").transform;

        _freeSpawnPoints = Enumerable.Range(0, _spawnPointsParent.childCount)
            .Select(i => _spawnPointsParent.GetChild(i))
            .ToList();

        var playersSpawnTimerObj = Instantiate(NetworkTimerPrefab);
        NetworkServer.Spawn(playersSpawnTimerObj);
        _playersSpawnTimer = playersSpawnTimerObj.GetComponent<NetworkTimer>();

        var gameTimerObj = Instantiate(NetworkTimerPrefab);
        NetworkServer.Spawn(gameTimerObj);
        _gameTimer = gameTimerObj.GetComponent<NetworkTimer>();

        ServerStartGame();
    }

    [Server]
    public void ServerStartGame()
    {
        if (!isServer)
            return;

        RpcGameInitiated();

        foreach (var conn in NetworkServer.connections.Values)
            TargetRpcSubscribeToTimer(conn, _playersSpawnTimer.netId);

        _playersSpawnTimer.OnTimeRanOut.AddListener(ServerSpawnPlayers);
        _playersSpawnTimer.ServerStartTimer(_playersSpawnTime);
    }

    [ClientRpc]
    public void RpcGameInitiated()
    {
        OnClientGameInitiated.Invoke();
    }

    [TargetRpc]
    public void TargetRpcSubscribeToTimer(NetworkConnectionToClient conn, uint timerNetworkId)
    {
        var timers = FindObjectsByType<NetworkTimer>(FindObjectsSortMode.None);
        var timerToSubscribeTo = timers.FirstOrDefault(timer => timer.netId == timerNetworkId);

        if (timerToSubscribeTo == null)
        {
            // We should leave the session here, because such situation should not happen at all.
            NetworkClient.Disconnect();
            return;
        }

        timerToSubscribeTo.OnUpdated.AddListener(ClientTimerUpdated);
        timerToSubscribeTo.OnTimeRanOut.AddListener(() => ClientUnsubscribeFromTimer(timerToSubscribeTo));
    }

    [Client]
    public void ClientTimerUpdated(int seconds)
    {
        if (!isClient)
            return;

        OnClientTimerUpdated.Invoke(seconds);
    }

    [Client]
    public void ClientUnsubscribeFromTimer(NetworkTimer timer)
    {
        if (!isClient)
            return;

        timer.OnUpdated.RemoveListener(ClientTimerUpdated);
    }

    [Server]
    private void ServerSpawnPlayers()
    {
        if (!isServer)
            return;

        foreach (var conn in NetworkServer.connections.Values)
        {
            var spawnPoint = _freeSpawnPoints.RandomItem();
            _ = _freeSpawnPoints.Remove(spawnPoint);

            LobbyNetworkManager.Instance.ServerSpawnGamePlayer(conn, spawnPoint.position, spawnPoint.rotation);
        }

        RpcGameStarted();

        foreach (var conn in NetworkServer.connections.Values)
            TargetRpcSubscribeToTimer(conn, _gameTimer.netId);

        Invoke(nameof(AssignRandomLeaderAndStart), 1f);
    }

    [ClientRpc]
    public void RpcGameStarted()
    {
        OnClientGameStarted.Invoke();
    }

    [Server]
    public void AssignRandomLeaderAndStart()
    {
        var playersData = FindObjectsByType<PlayerData>(FindObjectsSortMode.None);
        var leaderData = playersData.RandomItem();
        leaderData.Status = PlayerStatus.Leader;

        _gameTimer.OnTimeRanOut.AddListener(ServerGameOver);
        _gameTimer.ServerStartTimer(_gameTime);

        _gameStarted = true;
    }

    [Server]
    private void ServerGameOver()
    {
        if (!isServer)
            return;

        if (!_gameStarted)
            return;

        LobbyNetworkManager.Instance.ConnectedPlayersData
            .GroupBy(p => p.TotalLeadingTime)
            .OrderBy(g => g.Key)
            .SelectMany((g, i) =>
                g.Select(playerData =>
                (
                    conn: LobbyNetworkManager.Instance.GetConnectionForPlayerData(playerData),
                    place: i + 1
                ))
            )
            .ForEach(x => TargetRpcGameOver(x.conn, x.place));
    }

    [TargetRpc]
    public void TargetRpcGameOver(NetworkConnectionToClient conn, int place)
    {
        OnClientGameOver.Invoke(place);
    }
}
