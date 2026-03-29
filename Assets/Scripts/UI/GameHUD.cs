using Mirror;
using TMPro;
using UnityEngine;

public class GameHUD : Menu
{
    [SerializeField] private TMP_Text _gameTimerText;
    [SerializeField] private TMP_Text _playerStatusText;

    protected override void OnStart()
    {
        _gameTimerText.text = $"--:--";
        _playerStatusText.text = string.Empty;

        PlayerData.Local.OnStatusChanged.AddListener(OnStatusChanged);
        LobbyNotifier.OnSceneReady.AddListener(SetupGameManagerEvents);
    }

    private void SetupGameManagerEvents()
    {
        GameManager.OnClientGameStarted.AddListener(OnGameStarted);
        GameManager.OnClientTimerUpdated.AddListener(UpdateTimer);
        GameManager.OnClientGameOver.AddListener(OnGameOver);
    }

    private void OnGameStarted()
    {
        Activate();
    }

    private void UpdateTimer(int seconds)
    {
        int minutes = seconds / 60;
        int leftoverSeconds = seconds % 60;

        _gameTimerText.text = $"{minutes}:{leftoverSeconds:00}";
    }

    private void OnGameOver(int place)
    {
        Deactivate();
    }

    private void OnStatusChanged(PlayerStatus newStatus)
    {
        _playerStatusText.text = newStatus switch
        {
            PlayerStatus.Leader => "You're leading! Touch a player to pass the lead to them.",
            PlayerStatus.Invincible => "You're temporarily invinsible! Use this time to run away.",
            _ => string.Empty
        };
    }
}
