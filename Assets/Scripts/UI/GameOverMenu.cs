using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverMenu : Menu
{
    [SerializeField] private TMP_Text _placeText;
    [SerializeField] private TMP_Text _leadingTimeText;

    protected override void OnStart()
    {
        _placeText.text = string.Empty;
        _leadingTimeText.text = string.Empty;

        LobbyNotifier.OnSceneReady.AddListener(SetupGameManagerEvents);
    }

    private void SetupGameManagerEvents()
    {
        GameManager.OnClientGameOver.AddListener(OnGameOver);
    }

    private void OnGameOver(int place)
    {
        string placeEnding = place.ToString().Last() switch
        {
            '1' => "st",
            '2' => "nd",
            '3' => "rd",
            _ => "th"
        };

        _placeText.text = $"You took {place}{placeEnding} place!";

        int leadingSeconds = (int) Mathf.Clamp
        (
            value: Mathf.Floor(PlayerData.Local.TotalLeadingTime),
            min: 0f,
            max: GameManager.Instance.GameTime
        );
        int leadingMinutes = leadingSeconds / 60;
        int leadingLeftoverSeconds = leadingSeconds % 60;

        int totalSeconds = GameManager.Instance.GameTime;
        int totalMinutes = totalSeconds / 60;
        int totalLeftoverSeconds = totalSeconds % 60;

        _leadingTimeText.text = $"You've been leading for {leadingMinutes}:{leadingLeftoverSeconds:00} out of {totalMinutes}:{totalLeftoverSeconds:00}";

        Activate();

        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit()
    {
        NetworkClient.Disconnect();
    }
}
