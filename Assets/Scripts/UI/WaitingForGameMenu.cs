using TMPro;
using UnityEngine;

public class WaitingForGameMenu : Menu
{
    [SerializeField] private TMP_Text _gameStatusText;

    protected override void OnStart()
    {
        _gameStatusText.text = "Waiting for server to start the game...";

        LobbyNotifier.OnSceneReady.AddListener(SetupGameManagerEvents);
    }

    private void SetupGameManagerEvents()
    {
        GameManager.OnClientGameInitiated.AddListener(OnGameInitiated);
        GameManager.OnClientTimerUpdated.AddListener(UpdateTimer);
        GameManager.OnClientGameStarted.AddListener(OnGameStarted);
    }

    private void OnGameInitiated()
    {
        _gameStatusText.text = $"Game starts in: --:--";
    }

    private void UpdateTimer(int seconds)
    {
        int minutes = seconds / 60;
        int leftoverSeconds = seconds % 60;

        _gameStatusText.text = $"Game starts in: {minutes}:{leftoverSeconds:00}";
    }

    private void OnGameStarted()
    {
        Deactivate();
    }
}
