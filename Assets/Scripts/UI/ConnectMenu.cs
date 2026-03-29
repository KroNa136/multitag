using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectMenu : Menu
{
    [SerializeField] private Button _connectToRandomServerButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private Button _pasteJoinCodeButton;
    [SerializeField] private Button _connectToServerByCodeButton;
    [SerializeField] private StatusText _connectionStatusText;
    [SerializeField] private Button _cancelButton;

    private bool _connected = false;

    protected override void OnStart()
    {
        OnActivated.AddListener(() =>
        {
            _joinCodeInputField.text = string.Empty;
            _connectionStatusText.SetMessage(string.Empty);
            UnlockUI();

            if (LobbyNetworkManager.Instance.DEBUG_MODE)
            {
                ConnectToRandomServer();
            }
        });

        LobbyNetworkManager.OnClientDisconnected.AddListener(OnDisconnectedFromGame);
    }

    public void ConnectToRandomServer()
    {
        LockUI();
        _cancelButton.interactable = true;

        _connectionStatusText.SetMessage("Searching for a server...");

        if (SessionManager.Instance.IsSignedIn)
            ConnectToRandomServerAuthorized();
        else
            SessionManager.OnSignedIn.AddListener(ConnectToRandomServerAuthorized);
    }

    private async void ConnectToRandomServerAuthorized()
    {
        SessionManager.OnSignedIn.RemoveListener(ConnectToRandomServerAuthorized);

        _cancelButton.interactable = false;

        var (findSessionsStatus, sessions) = await SessionManager.Instance.FindSessions();

        if (findSessionsStatus is FindSessionsStatus.Failed)
        {
            UnlockUI();
            _connectionStatusText.SetError("Failed to find a server. Try again later.");
            return;
        }

        if (sessions.Count == 0)
        {
            UnlockUI();
            _connectionStatusText.SetMessage("There are no free public servers. Start your own server or try again later.");
            return;
        }

        _connectionStatusText.SetMessage("Connecting...");

        var joinSessionStatus = await SessionManager.Instance.JoinSession(sessions[0]);

        if (joinSessionStatus is JoinSessionStatus.SessionIsFull)
        {
            UnlockUI();
            _connectionStatusText.SetError("! Found a full session !");
            return;
        }

        if (joinSessionStatus is JoinSessionStatus.NotFound or JoinSessionStatus.Failed)
        {
            UnlockUI();
            _connectionStatusText.SetError("Connection error. Try again later.");
            return;
        }

        _connected = true;
        _connectionStatusText.SetSuccess("Success! Waiting for the server to create a lobby...");
    }

    public void ConnectToServerByCode()
    {
        LockUI();
        _cancelButton.interactable = true;

        string code = _joinCodeInputField.text;

        if (code.ToCharArray().Length == 0)
        {
            UnlockUI();
            _connectionStatusText.SetError("Enter the code.");
            return;
        }

        if (code.ToCharArray().Length != 6)
        {
            UnlockUI();
            _connectionStatusText.SetError("The code must contain exactly 6 characters.");
            return;
        }

        _connectionStatusText.SetMessage("Connecting...");

        if (SessionManager.Instance.IsSignedIn)
            ConnectToServerByCodeAuthorized();
        else
            SessionManager.OnSignedIn.AddListener(ConnectToServerByCodeAuthorized);
    }

    private async void ConnectToServerByCodeAuthorized()
    {
        SessionManager.OnSignedIn.RemoveListener(ConnectToServerByCodeAuthorized);

        _cancelButton.interactable = false;

        var joinSessionStatus = await SessionManager.Instance.JoinSession(_joinCodeInputField.text);

        if (joinSessionStatus is JoinSessionStatus.SessionIsFull)
        {
            UnlockUI();
            _connectionStatusText.SetError("The server is already full.");
            return;
        }

        if (joinSessionStatus is JoinSessionStatus.NotFound)
        {
            UnlockUI();
            _connectionStatusText.SetError("Could not find a server with such join code.");
            return;
        }

        if (joinSessionStatus is JoinSessionStatus.Failed)
        {
            UnlockUI();
            _connectionStatusText.SetError("Connection error. Try again later.");
            return;
        }

        _connected = true;
        _connectionStatusText.SetSuccess("Success! Waiting for the server to create a lobby...");
    }

    private void OnDisconnectedFromGame()
    {
        UnlockUI();
        _connectionStatusText.SetError("Server terminated the connection.");
        _connected = false;
    }

    public void Cancel()
    {
        if (_connected)
        {
            Debug.LogWarning("Attempted to cancel connecting to a server when already connected.");
            return;
        }

        SessionManager.OnSignedIn.RemoveListener(ConnectToRandomServerAuthorized);
        SessionManager.OnSignedIn.RemoveListener(ConnectToServerByCodeAuthorized);

        LockUI();
        Deactivate();
    }

    private void LockUI()
    {
        _connectToRandomServerButton.interactable = false;
        _joinCodeInputField.readOnly = true;
        _pasteJoinCodeButton.interactable = false;
        _connectToServerByCodeButton.interactable = false;
        _cancelButton.interactable = false;
    }

    private void UnlockUI()
    {
        _connectToRandomServerButton.interactable = true;
        _joinCodeInputField.readOnly = false;
        _pasteJoinCodeButton.interactable = true;
        _connectToServerByCodeButton.interactable = true;
        _cancelButton.interactable = true;
    }
}
