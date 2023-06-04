public class NetworkPlayerInitializer
{
    private LocalPlayerRoot _localPlayerRoot;

    public void Init(LocalPlayerRoot localPlayerRoot)
    {
        _localPlayerRoot = localPlayerRoot;
        NetworkAvatarTransmitter.LocalPlayerAvatarTransmitterSpawned += NetworkAvatarTransmitter_CurrentUserNetworkAvatarSpawned;
    }

    public void DeInit()
    {
        NetworkAvatarTransmitter.LocalPlayerAvatarTransmitterSpawned -= NetworkAvatarTransmitter_CurrentUserNetworkAvatarSpawned;
    }

    private void NetworkAvatarTransmitter_CurrentUserNetworkAvatarSpawned(NetworkAvatarTransmitter obj)
    {
        obj.Init(_localPlayerRoot.LocalAvatarEntity, _localPlayerRoot.AvatarAssetId);
    }
}
