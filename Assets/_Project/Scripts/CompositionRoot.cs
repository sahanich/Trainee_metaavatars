using Assets.Scripts.Player.WindowsMovement;
using UnityEngine;

public class CompositionRoot : MonoBehaviour
{
    [SerializeField]
    private LocalPlayerRoot LocalPlayer;
    [SerializeField]
    private NetworkStarter NetworkStarter;
    [SerializeField]
    private PlayerSpawner PlayerSpawner;
    [SerializeField]
    private NonVrMovementSystem NonVrMovementSystem;

    private NetworkPlayerEventsMediator _networkPlayerEventsMediator = new();

    private void OnEnable()
    {
#if UNITY_SERVER
        return;
#endif
        NetworkStarter.ClientStarted += OnClientStarted;
        _networkPlayerEventsMediator.RegisterListeners();
        _networkPlayerEventsMediator.LocalPlayerAvatarTransmitterSpawned += OnLocalPlayerAvatarTransmitterSpawned;
        _networkPlayerEventsMediator.LocalPlayerPositionSynchronizerSpawned += OnLocalPlayerPositionSynchronizerSpawned;
    }

    private void OnDisable()
    {
#if UNITY_SERVER
        return;
#endif        
        NetworkStarter.ClientStarted -= OnClientStarted;
        _networkPlayerEventsMediator.UnregisterListeners();
        _networkPlayerEventsMediator.LocalPlayerAvatarTransmitterSpawned -= OnLocalPlayerAvatarTransmitterSpawned;
        _networkPlayerEventsMediator.LocalPlayerPositionSynchronizerSpawned -= OnLocalPlayerPositionSynchronizerSpawned;
    }

    private void Start()
    {
        NetworkStarter.InitConnection();
    }

    private void OnClientStarted()
    {
        PlayerSpawner.SpawnPlayer(LocalPlayer);
        NonVrMovementSystem.Init(LocalPlayer);
        NonVrMovementSystem.StartMovementControl();
    }

    private void OnLocalPlayerAvatarTransmitterSpawned(NetworkAvatarTransmitter networkAvatarTransmitter)
    {
        networkAvatarTransmitter.Init(LocalPlayer.LocalAvatarEntity);
    }

    private void OnLocalPlayerPositionSynchronizerSpawned(NetworkPlayerPositionSynchronizer networkPlayerPositionSynchronizer)
    {
        networkPlayerPositionSynchronizer.Init(LocalPlayer.XrOrigin.transform);
    }
}
