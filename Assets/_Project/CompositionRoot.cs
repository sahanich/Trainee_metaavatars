using Assets.Scripts.Player.WindowsMovement;
using UnityEngine;

public class CompositionRoot : MonoBehaviour
{
    [SerializeField]
    private LocalPlayerRoot LocalPlayerRoot;
    [SerializeField]
    private PlayerSpawner PlayerSpawner;
    [SerializeField]
    private NonVrMovementSystem NonVrMovementSystem;

    private NetworkPlayerEventsMediator _networkPlayerEventsMediator;

    private void Awake()
    {
        _networkPlayerEventsMediator = new();
        PlayerSpawner.Init(LocalPlayerRoot);
        PlayerSpawner.SetWaitingForSpawnMode();

        NonVrMovementSystem.Init(LocalPlayerRoot);
    }

    private void OnEnable()
    {
        _networkPlayerEventsMediator.RegisterListeners();
        _networkPlayerEventsMediator.LocalPlayerAvatarTransmitterSpawned += OnLocalPlayerAvatarTransmitterSpawned;
        _networkPlayerEventsMediator.LocalPlayerPositionSynchronizerSpawned += OnLocalPlayerPositionSynchronizerSpawned;
    }

    private void OnDisable()
    {
        _networkPlayerEventsMediator.UnregisterListeners();
        _networkPlayerEventsMediator.LocalPlayerAvatarTransmitterSpawned -= OnLocalPlayerAvatarTransmitterSpawned;
        _networkPlayerEventsMediator.LocalPlayerPositionSynchronizerSpawned -= OnLocalPlayerPositionSynchronizerSpawned;
    }

    private void OnLocalPlayerAvatarTransmitterSpawned(NetworkAvatarTransmitter networkAvatarTransmitter)
    {
        networkAvatarTransmitter.Init(LocalPlayerRoot.LocalAvatarEntity, LocalPlayerRoot.AvatarAssetId);
    }

    private void OnLocalPlayerPositionSynchronizerSpawned(NetworkPlayerPositionSynchronizer networkPlayerPositionSynchronizer)
    {
        networkPlayerPositionSynchronizer.Init(LocalPlayerRoot.XrOrigin.transform);

        PlayerSpawner.SpawnPlayer();

        NonVrMovementSystem.StartMovementControl();
    }
}
