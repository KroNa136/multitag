using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NetworkTimer : NetworkBehaviour
{
    public UnityEvent OnStarted = new();
    public UnityEvent<int> OnUpdated = new();
    public UnityEvent OnStopped = new();
    public UnityEvent OnTimeRanOut = new();

    [SyncVar(hook = nameof(OnClientCurrentSecondsChanged))]
    public int CurrentSeconds;

    private readonly WaitForSeconds _waitForOneSecond = new(1f);
    private bool _isRunning = false;

    private Coroutine _timerCoroutine;

    [Server]
    public void ServerStartTimer(int seconds)
    {
        if (!isServer)
            return;

        if (_isRunning)
        {
            Debug.LogError("Attempted to start a Network Timer that is already running.");
            return;
        }

        if (seconds <= 0)
        {
            Debug.LogError("Attempted to start a Network Timer with 0 or less seconds.");
            return;
        }

        _isRunning = true;
        _timerCoroutine = StartCoroutine(ServerTimerCoroutine(seconds));

        OnStarted.Invoke();
    }

    [Server]
    public void ServerStopTimer()
    {
        if (!isServer)
            return;

        if (!_isRunning)
            return;

        _isRunning = false;

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        OnStopped.Invoke();
    }

    [Server]
    public IEnumerator ServerTimerCoroutine(int seconds)
    {
        if (!isServer)
            yield break;

        CurrentSeconds = seconds;

        while (CurrentSeconds > 0)
        {
            yield return _waitForOneSecond;
            CurrentSeconds--;
        }

        _isRunning = false;

        OnTimeRanOut.Invoke();
    }

    [Client]
    public void OnClientCurrentSecondsChanged(int oldValue, int newValue)
    {
        if (!isClient)
            return;

        OnUpdated.Invoke(newValue);
    }
}
