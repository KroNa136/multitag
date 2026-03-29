using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartServerMenu : Menu
{
    [SerializeField] private Toggle _publicServerToggle;
    [SerializeField] private Button _startServerButton;
    [SerializeField] private StatusText _serverStatusText;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private Button _copyJoinCodeButton;
    [SerializeField] private Button _stopServerButton;

    private bool _isSessionRunning = false;

    protected override void OnStart()
    {
        OnActivated.AddListener(() =>
        {
            _publicServerToggle.isOn = false;
            _joinCodeInputField.text = string.Empty;
            _serverStatusText.SetMessage(string.Empty);
            UnlockUI();

            if (LobbyNetworkManager.Instance.DEBUG_MODE)
            {
                _publicServerToggle.isOn = true;
                StartServer();
            }
        });

        LobbyNetworkManager.OnServerAllPlayersReady.AddListener(OnLastPlayerJoined);
    }

    public void StartServer()
    {
        LockUI();
        _stopServerButton.interactable = true;

        _serverStatusText.SetMessage("Starting server...");

        if (SessionManager.Instance.IsSignedIn)
            StartServerAuthorized();
        else
            SessionManager.OnSignedIn.AddListener(StartServerAuthorized);
    }

    private async void StartServerAuthorized()
    {
        SessionManager.OnSignedIn.RemoveListener(StartServerAuthorized);

        _stopServerButton.interactable = false;

        bool isPublic = _publicServerToggle.isOn;

        var startSessionStatus = isPublic ?
            await SessionManager.Instance.StartPublicSessionAsHost() :
            await SessionManager.Instance.StartPrivateSessionAsHost();

        if (startSessionStatus is StartSessionStatus.Failed)
        {
            UnlockUI();
            _serverStatusText.SetError("Failed to start server.");
            return;
        }

        _isSessionRunning = true;

        _joinCodeInputField.text = SessionManager.Instance.ActiveSession.Code;
        _serverStatusText.SetSuccess("Success! Waiting for players...");

        UnlockUI();
    }

    private void OnLastPlayerJoined()
    {
        _serverStatusText.SetMessage("Creating lobby...");
    }

    public async void StopServer()
    {
        SessionManager.OnSignedIn.RemoveListener(StartServerAuthorized);

        LockUI();

        if (!_isSessionRunning)
        {
            Deactivate();
            return;
        }

        _serverStatusText.SetMessage("Stopping server...");

        var stopSessionStatus = await SessionManager.Instance.StopSession();

        if (stopSessionStatus is StopSessionStatus.Failed)
        {
            UnlockUI();
            _serverStatusText.SetError("Failed to stop the server.");
            return;
        }

        _isSessionRunning = false;

        _serverStatusText.SetSuccess("Server stopped.");
        _joinCodeInputField.text = string.Empty;

        Deactivate();
    }

    private void LockUI()
    {
        _publicServerToggle.interactable = false;
        _startServerButton.interactable = false;
        _copyJoinCodeButton.interactable = false;
        _stopServerButton.interactable = false;
    }

    private void UnlockUI()
    {
        _publicServerToggle.interactable = !_isSessionRunning;
        _startServerButton.interactable = !_isSessionRunning;
        _copyJoinCodeButton.interactable = !string.IsNullOrEmpty(_joinCodeInputField.text);
        _stopServerButton.interactable = true;
    }
}
