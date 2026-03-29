using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public static UnityEvent OnSignedIn = new();
    public bool IsSignedIn { get; private set; }
    public ISession ActiveSession { get; private set; }

    [SerializeField] private bool _useRelay = true;
    [SerializeField] private RelayProtocol _relayProtocol = RelayProtocol.Default;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private async void Start()
    {
        IsSignedIn = false;

        try
        {
            await UnityServices.InitializeAsync();

#if !UNITY_EDITOR
            string argWithClientNumber = Environment.GetCommandLineArgs().FirstOrDefault(arg => Regex.IsMatch(arg, "^-client[1-5]$"));
            string profile = string.IsNullOrEmpty(argWithClientNumber) ? "Player0" : $"Player{argWithClientNumber.Last()}";

            AuthenticationService.Instance.SwitchProfile(profile);
#endif

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");

            IsSignedIn = true;
            OnSignedIn.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #region Host

    public async Task<StartSessionStatus> StartPublicSessionAsHost()
    {
        var options = new SessionOptions()
        {
            IsLocked = false,
            IsPrivate = false,
            MaxPlayers = LobbyNetworkManager.Instance.maxConnections
        };

        options = _useRelay ?
            options.WithRelayNetwork().WithNetworkOptions(new NetworkOptions { RelayProtocol = _relayProtocol }) :
            options.WithDirectNetwork(new DirectNetworkOptions(LobbyNetworkManager.Instance.Port));

        options = options.WithNetworkHandler(new MirrorNetworkHandler());

        try
        {
            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log($"Public session {ActiveSession.Id} created! Join code: {ActiveSession.Code}");
            return StartSessionStatus.Created;
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StartSessionStatus.Failed;
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StartSessionStatus.Failed;
        }
    }

    public async Task<StartSessionStatus> StartPrivateSessionAsHost()
    {
        var options = new SessionOptions()
        {
            IsLocked = false,
            IsPrivate = true,
            MaxPlayers = LobbyNetworkManager.Instance.maxConnections
        };

        options = _useRelay ?
            options.WithRelayNetwork().WithNetworkOptions(new NetworkOptions { RelayProtocol = _relayProtocol}) :
            options.WithDirectNetwork(new DirectNetworkOptions(LobbyNetworkManager.Instance.Port));

        options = options.WithNetworkHandler(new MirrorNetworkHandler());

        try
        {
            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            Debug.Log($"Private session {ActiveSession.Id} created! Join code: {ActiveSession.Code}");
            return StartSessionStatus.Created;
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StartSessionStatus.Failed;
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StartSessionStatus.Failed;
        }
    }

    public async Task<KickPlayerStatus> KickPlayer(string playerId)
    {
        if (!ActiveSession.IsHost)
            return KickPlayerStatus.IsNotHost;

        try
        {
            await ActiveSession.AsHost().RemovePlayerAsync(playerId);

            Debug.Log($"Kicked player {playerId} from the session");
            return KickPlayerStatus.Kicked;
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return KickPlayerStatus.Failed;
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return KickPlayerStatus.Failed;
        }
    }

    public async Task<StopSessionStatus> StopSession()
    {
        try
        {
            string id = ActiveSession.Id;
            await ActiveSession.AsHost().DeleteAsync();

            Debug.Log($"Session {id} stopped");
            return StopSessionStatus.Stopped;
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StopSessionStatus.Failed;
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return StopSessionStatus.Failed;
        }
    }

    #endregion Host

    #region Client

    public async Task<(FindSessionsStatus Status, IList<ISessionInfo> Sessions)> FindSessions()
    {
        var options = new QuerySessionsOptions()
        {
            SortOptions = new List<SortOption> { new(SortOrder.Ascending, SortField.CreationTime) },
            FilterOptions = new List<FilterOption> { new(FilterField.AvailableSlots, "0", FilterOperation.Greater) }
        };

        try
        {
            var results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return (FindSessionsStatus.Found, results.Sessions);
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return (FindSessionsStatus.Failed, null);
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return (FindSessionsStatus.Failed, null);
        }
    }

    public async Task<JoinSessionStatus> JoinSession(ISessionInfo session)
    {
        var options = new JoinSessionOptions()
        {
            PlayerProperties = new Dictionary<string, PlayerProperty>()
            {
                // TODO: post the real username
                { "_name", new PlayerProperty("Unknown Player", VisibilityPropertyOptions.Public) }
            }
        }
        .WithNetworkHandler(new MirrorNetworkHandler());

        try
        {
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(session.Id, options);

            Debug.Log($"Joined session {ActiveSession.Id}!");
            return JoinSessionStatus.Joined;
        }
        catch (SessionException ex)
        {
            return HandleJoinSessionException(ex);
        }
        catch (CustomSessionException ex)
        {
            return HandleJoinSessionException(ex);
        }
    }

    public async Task<JoinSessionStatus> JoinSession(string code)
    {
        var options = new JoinSessionOptions()
        {
            PlayerProperties = new Dictionary<string, PlayerProperty>()
            {
                // TODO: post the real username
                { "_name", new PlayerProperty("Unknown Player", VisibilityPropertyOptions.Public) }
            }
        }
        .WithNetworkHandler(new MirrorNetworkHandler());

        try
        {
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code, options);

            Debug.Log($"Joined session {ActiveSession.Id} by code!");
            return JoinSessionStatus.Joined;
        }
        catch (SessionException ex)
        {
            return HandleJoinSessionException(ex);
        }
        catch (CustomSessionException ex)
        {
            return HandleJoinSessionException(ex);
        }
    }

    public async Task<LeaveSessionStatus> LeaveSession()
    {
        try
        {
            string id = ActiveSession.Id;
            await ActiveSession.LeaveAsync();
            ActiveSession = null;

            Debug.Log($"Left session {id}");
            return LeaveSessionStatus.Left;
        }
        catch (SessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return LeaveSessionStatus.Failed;
        }
        catch (CustomSessionException ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex.Error.ToString());
            return LeaveSessionStatus.Failed;
        }
    }

    private JoinSessionStatus HandleJoinSessionException(Exception ex)
    {
        Debug.LogException(ex);

        if (ex is SessionException sessionException)
        {
            Debug.LogError(sessionException.Error.ToString());
        }
        else if (ex is CustomSessionException customSessionException)
        {
            Debug.LogError(customSessionException.Error.ToString());
        }

        return ex.Message switch
        {
            "lobby is full" => JoinSessionStatus.SessionIsFull,
            "lobby not found" => JoinSessionStatus.NotFound,
            _ => JoinSessionStatus.Failed
        };
    }

    #endregion Client
}
