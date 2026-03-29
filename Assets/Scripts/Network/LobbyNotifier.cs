using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class LobbyNotifier : NetworkBehaviour
{
    public static LobbyNotifier Instance;

    //public static UnityEvent<uint> OnLeadingPlayerChanged = new();
    public static UnityEvent OnSceneReady = new();

    //[SyncVar(hook = nameof(OnClientChangeLeadingPlayerNetId))]
    //public uint LeadingPlayerNetId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /*
    [Client]
    public void OnClientChangeLeadingPlayerNetId(uint oldValue, uint newValue)
    {
        if (!isClient)
            return;

        OnLeadingPlayerChanged.Invoke(newValue);
    }
    */

    [ClientRpc]
    public void RpcSceneReady()
    {
        OnSceneReady.Invoke();
    }
}
