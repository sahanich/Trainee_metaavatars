using Assets.Scripts.Player.WindowsMovement;
using UnityEngine;

public class CompositionRoot : MonoBehaviour
{
    [SerializeField]
    private LocalPlayerRoot LocalPlayerPrefab;
    [SerializeField]
    private NetworkStarter NetworkStarter;
    [SerializeField]
    private PlayerSpawner PlayerSpawner;
    [SerializeField]
    private NonVrMovementSystem NonVrMovementSystem;

    private LocalPlayerRoot _localPlayer;
    private NetworkPlayerEventsMediator _networkPlayerEventsMediator = new();


//    private void Awake()
//    {
//#if UNITY_SERVER
//        return;
//#endif        
//        _networkPlayerEventsMediator = new();
//    }

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
        _localPlayer = PlayerSpawner.SpawnPlayer(LocalPlayerPrefab);
        NonVrMovementSystem.Init(_localPlayer);
        NonVrMovementSystem.StartMovementControl();
    }

    private void OnLocalPlayerAvatarTransmitterSpawned(NetworkAvatarTransmitter networkAvatarTransmitter)
    {
        networkAvatarTransmitter.Init(_localPlayer.LocalAvatarEntity, _localPlayer.AvatarAssetId);
    }

    private void OnLocalPlayerPositionSynchronizerSpawned(NetworkPlayerPositionSynchronizer networkPlayerPositionSynchronizer)
    {
        networkPlayerPositionSynchronizer.Init(_localPlayer.XrOrigin.transform);
    }
}
