using System;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

public class NetworkStarter : NetworkBehaviour
{
    private const string SenseTowerSettingsPortKey = "Port";

    [SerializeField]
    private ushort DefaultConnectionPort = 7790;
    [SerializeField]
    private string DefaultConnectionIp = "127.0.0.1";

    public event Action ClientStarted;

    private void Awake()
    {
        NetworkStarter[] oldStarters = FindObjectsOfType<NetworkStarter>();
        foreach (var item in oldStarters)
        {
            if (item != this)
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void InitConnection()
    {
        NetworkManager.Singleton.NetworkConfig.EnableNetworkLogs = true;

        var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.ConnectionData.Port = Port;
        transport.ConnectionData.Address = UnityServerIp;

        if (IsUnityServer)
        {
            StartServer(transport);
        }
        else
        {
            StartClient();
        }
    }

    private void StartClient()
    {
        Debug.Log($"Start client {UnityServerIp}:{Port}");

        var startResult = NetworkManager.Singleton.StartClient();
        
        Debug.Log(startResult ? "Client opened successfully" : "Client opening failed");

        ClientStarted?.Invoke();
    }

    private void StartServer(NetworkTransport transport)
    {
        Debug.Log($"[{DateTime.Now:HH:mm:ss}] Start server {UnityServerIp}:{Port}");
        NetworkManager.Singleton.OnServerStarted += () => Debug.Log($"Server started");
        NetworkManager.Singleton.OnClientConnectedCallback += clientId => Debug.Log($"Client connected {clientId}");
        NetworkManager.Singleton.OnClientDisconnectCallback += clientId => Debug.Log($"Client disconnected {clientId}");

        transport.OnTransportEvent += (eventType, clientId, payload, receiveTime) =>
        {
            if (eventType != NetworkEvent.Data)
            {
                Debug.Log($"[{DateTime.Now:HH:mm:ss}] Transport event {eventType}: ClientID={clientId}");
            }
        };

        var startResult = NetworkManager.Singleton.StartServer();
        Debug.Log(startResult ? "Server opened successfully" : "Server opening failed");
    }

    private string UnityServerIp
    {
        get
        {
            return IsUnityServer ? "0.0.0.0" : DefaultConnectionIp;
        }
    }

    private ushort Port
    {
        get
        {
            if (IsUnityServer)
            {
                var port = Environment.GetEnvironmentVariable(SenseTowerSettingsPortKey);
                if (ushort.TryParse(port, out var parsedPort))
                {
                    return parsedPort;
                }
            }

            return DefaultConnectionPort;
        }
    }

    private static bool IsUnityServer
    {
        get
        {
#if UNITY_SERVER
            return true;
#else
            return false;
#endif
        }
    }
}
