using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;
using Utp;

/// <summary>
/// An <see cref="INetworkHandler"/> for Mirror.
/// </summary>
class MirrorNetworkHandler : INetworkHandler
{
    const string _enclosingType = nameof(MirrorNetworkHandler);

    private bool _isNetworkRunning = false;
    private NetworkRole _networkRole;

    /// <summary>
    /// Starts the Mirror session.
    /// </summary>
    /// <param name="configuration">
    /// The <see cref="NetworkConfiguration"/> used to configure this session.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that can be awaited. Resolves once the <see cref="LobbyNetworkManager"/> has finished connecting.
    /// </returns>
    /// <exception cref="CustomSessionException">
    /// Thrown when the <see cref="LobbyNetworkManager"/> fails to start.
    /// </exception>
    public Task StartAsync(NetworkConfiguration configuration)
    {
        var networkManager = LobbyNetworkManager.Instance;

        if (networkManager == null)
        {
            throw new CustomSessionException($"Cannot start when {nameof(LobbyNetworkManager.Instance)} is not set.",
                SessionError.NetworkManagerNotInitialized);
        }

        // If we have a current session, nothing needs to be started.
        if (_isNetworkRunning)
        {
            Debug.LogWarning($"{_enclosingType}: Session already started.");
            return Task.CompletedTask;
        }

        switch (configuration.Type)
        {
            case NetworkType.Direct:
                StartDirect(networkManager, configuration);
                break;
            case NetworkType.Relay:
                StartRelay(networkManager, configuration);
                break;
            case NetworkType.DistributedAuthority:
                throw new CustomSessionException("Distributed Authority requires Netcode For GameObjects 2.0", SessionError.NetworkManagerStartFailed);
        }

        _isNetworkRunning = true;
        _networkRole = configuration.Role;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the Mirror session.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that can be awaited. Resolves once the <see cref="LobbyNetworkManager"/> has finished shutting down.
    /// </returns>
    public Task StopAsync()
    {
        if (!_isNetworkRunning)
        {
            Debug.LogWarning($"{_enclosingType}: Failed to stop session: session was never started.");
            return Task.CompletedTask;
        }

        var networkManager = LobbyNetworkManager.Instance;

        if (networkManager == null)
        {
            Debug.LogWarning($"{_enclosingType}: Failed to stop session: session apparently was started " +
                $"but the {nameof(LobbyNetworkManager.Instance)} was not set.");
        }
        else
        {
            switch (_networkRole)
            {
                case NetworkRole.Server:
                    networkManager.StopServer();
                    break;
                case NetworkRole.Host:
                    networkManager.StopHost();
                    break;
                case NetworkRole.Client:
                    networkManager.StopClient();
                    break;
            }
        }

        _isNetworkRunning = false;
        _networkRole = default;

        return Task.CompletedTask;
    }

    private void StartDirect(LobbyNetworkManager networkManager, NetworkConfiguration configuration)
    {
        Debug.Log($"{_enclosingType}: Publish Address: {configuration.DirectNetworkPublishAddress} - Listen Address: {configuration.DirectNetworkListenAddress}");

        switch (configuration.Role)
        {
            case NetworkRole.Server:
                networkManager.StartStandardServer();
                break;
            case NetworkRole.Host:
                networkManager.StartStandardHost();
                break;
            case NetworkRole.Client:
                networkManager.JoinStandardServer(configuration.DirectNetworkPublishAddress.Address /* or DirectNetworkListenAddress? */);
                break;
        }

        // When binding to port 0, a random available port is chosen by the OS.
        // Ensure that we update our configuration with the chosen port.
        if (configuration.Role != NetworkRole.Client && configuration.DirectNetworkListenAddress.Port == 0)
            UpdateDirectConnectionPortBinding(networkManager, configuration);
    }

    private void UpdateDirectConnectionPortBinding(LobbyNetworkManager networkManager, NetworkConfiguration configuration)
    {
        if (!networkManager.TryGetComponent<UtpTransport>(out var transport))
        {
            throw new CustomSessionException($"{nameof(LobbyNetworkManager)} must have a {nameof(UtpTransport)} component.",
                SessionError.TransportComponentMissing);
        }

        var localEndpoint = transport.GetLocalEndpoint();

        Debug.Log($"{_enclosingType}: LocalEndpoint {localEndpoint}.");

        configuration.UpdatePublishPort(localEndpoint.Port);
    }

    private void StartRelay(LobbyNetworkManager networkManager, NetworkConfiguration configuration)
    {
        switch (configuration.Role)
        {
            case NetworkRole.Server:
            // Always start Relay sessions with the user as a Host.
            // TODO: This seems like a bug - Dedicated game servers
            case NetworkRole.Host:
                networkManager.StartRelayHost(configuration.RelayServerData);
                break;
            case NetworkRole.Client:
                networkManager.JoinRelayServer(configuration.RelayServerData);
                break;
        }
    }
}
