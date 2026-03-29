using UnityEngine;
using UnityEngine.Events;

public class TickSystem : MonoBehaviour
{
    public static TickSystem Instance;

    public UnityEvent<int> OnTick = new();

    public float TickRate { get; private set; }
    public int CurrentTick { get; private set; }

    private float _accumulator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        TickRate = 1f / LobbyNetworkManager.Instance.sendRate;
    }

    private void Update()
    {
        _accumulator += Time.deltaTime;

        while (_accumulator >= TickRate)
        {
            _accumulator -= TickRate;
            CurrentTick++;
            OnTick.Invoke(CurrentTick);
        }
    }
}
