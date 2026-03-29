using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class LobbyPlayer : NetworkBehaviour
{
    private void Awake()
    {
        // Force the object to scene root, in case it was instantiated as a child of something in the scene,
        // since DDOL is only allowed for scene root objects.
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}
