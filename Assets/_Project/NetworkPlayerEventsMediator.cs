using System;

public class NetworkPlayerEventsMediator
{
    public event Action<NetworkAvatarTransmitter> LocalPlayerAvatarTransmitterSpawned;
    public event Action<NetworkPlayerPositionSynchronizer> LocalPlayerPositionSynchronizerSpawned;

    public void RegisterListeners()
    {
        NetworkAvatarTransmitter.LocalPlayerAvatarTransmitterSpawned += OnCurrentUserNetworkAvatarSpawned;
        NetworkPlayerPositionSynchronizer.LocalPlayerPositionSynchronizerSpawned += OnLocalPlayerPositionSynchronizerSpawned;
    }

    public void UnregisterListeners()
    {
        NetworkAvatarTransmitter.LocalPlayerAvatarTransmitterSpawned += OnCurrentUserNetworkAvatarSpawned;
        NetworkPlayerPositionSynchronizer.LocalPlayerPositionSynchronizerSpawned += OnLocalPlayerPositionSynchronizerSpawned;
    }

    private void OnCurrentUserNetworkAvatarSpawned(NetworkAvatarTransmitter avatarTransmitter)
    {
        LocalPlayerAvatarTransmitterSpawned?.Invoke(avatarTransmitter);
    }

    private void OnLocalPlayerPositionSynchronizerSpawned(NetworkPlayerPositionSynchronizer playerPositionSynchronizer)
    {
        LocalPlayerPositionSynchronizerSpawned?.Invoke(playerPositionSynchronizer);
    }
}
