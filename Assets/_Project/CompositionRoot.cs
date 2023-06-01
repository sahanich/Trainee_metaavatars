using UnityEngine;

public class CompositionRoot : MonoBehaviour
{
    public LocalPlayerRoot LocalPlayerRoot;

    private void Start()
    {
        NetworkAvatarInitializer networkAvatarInitializer = new();
        networkAvatarInitializer.Init(LocalPlayerRoot);
    }
}
