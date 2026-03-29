using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class PlayerData : NetworkBehaviour
{
    public UnityEvent<PlayerStatus> OnStatusChanged = new();

    [SyncVar(hook = nameof(StatusChanged))]
    public PlayerStatus Status;

    [SyncVar]
    public float TotalLeadingTime;

    public static PlayerData Local { get; private set; } = null;
    public bool IsLocal { get; private set; } = false;

    private float _tickRate = 1f / 30f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public override void OnStartServer()
    {
        Status = PlayerStatus.Normal;
        TotalLeadingTime = 0f;

        _ = StartCoroutine(SubscribeToTickSystemWhenAvailable());
    }

    [Server]
    public IEnumerator SubscribeToTickSystemWhenAvailable()
    {
        if (!isServer)
            yield break;

        while (TickSystem.Instance == null)
            yield return null;

        var tickSystem = TickSystem.Instance;
        _tickRate = tickSystem.TickRate;
        tickSystem.OnTick.AddListener(ServerTick);
    }

    [Client]
    public override void OnStartAuthority()
    {
        Local = this;
        IsLocal = true;
    }

    [Server]
    public void ServerTick(int tick)
    {
        if (!isServer)
            return;

        if (Status is PlayerStatus.Leader)
            TotalLeadingTime += _tickRate;
    }

    [Client]
    public void StatusChanged(PlayerStatus oldValue, PlayerStatus newValue)
    {
        if (!isClient)
            return;

        OnStatusChanged.Invoke(newValue);
    }

    private void OnDestroy()
    {
        Local = null;
    }
}
