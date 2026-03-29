using Mirror;
using UnityEngine;

public class PauseMenu : Menu
{
    private PauseManager _pauseManager;

    protected override void OnActivate()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    protected override void OnDeactivate()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ActivateFrom(PauseManager pauseManager)
    {
        _pauseManager = pauseManager;
        Activate();
    }

    public void Continue()
    {
        if (_pauseManager != null)
            _pauseManager.Unpause();
        else
            Deactivate();
    }

    public void Quit()
    {
        NetworkClient.Disconnect();
    }
}
