using UnityEngine;

public class LocalPlayerRoot : MonoBehaviour
{
    [field: SerializeField]
    public SampleAvatarEntity LocalAvatarEntity { get; private set; }

    [field: SerializeField]
    public int AvatarAssetId { get; private set; }
}
