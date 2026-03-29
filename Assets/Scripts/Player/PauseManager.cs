using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private PauseMenu _pauseMenu;
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;
    private CameraController _cameraController;

    private bool _canPause = true;

    private void Start()
    {
        _pauseMenu = GameObject.Find("Pause Menu").GetComponent<PauseMenu>();
        _ = TryGetComponent(out _inputManager);
        _ = TryGetComponent(out _playerMovement);
        _ = TryGetComponent(out _cameraController);

        GameManager.OnClientGameOver.AddListener(OnGameOver);
    }

    private void OnGameOver(int _)
    {
        _canPause = false;
    }

    private void Update()
    {
        if (!_canPause || _inputManager == null)
            return;

        if (_inputManager.Pause)
        {
            if (_pauseMenu.IsActive)
                Unpause();
            else
                Pause();
        }
    }

    public void Pause()
    {
        _pauseMenu.ActivateFrom(this);
        _playerMovement.CanBeControlledByPlayer = false;
        _cameraController.CanBeControlledByPlayer = false;
    }

    public void Unpause()
    {
        _pauseMenu.Deactivate();
        _playerMovement.CanBeControlledByPlayer = true;
        _cameraController.CanBeControlledByPlayer = true;
    }
}
