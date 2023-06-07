using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalPlayerRoot : MonoBehaviour
{
    [field: SerializeField]
    public XROrigin XrOrigin { get; private set; }

    [field: SerializeField]
    public OVRCameraRig OVRCameraRig { get; private set; }

    [field: SerializeField]
    public CharacterController CharacterController { get; private set; }

    [field: SerializeField]
    public SampleAvatarEntity LocalAvatarEntity { get; private set; }


    private void Awake()
    {
        StartCoroutine(WaitingForTrackingPoseValid());
    }

    public void SetPosition(Vector3 position)
    {
        XrOrigin.transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        XrOrigin.transform.rotation = rotation;
    }

    public void SetActive(bool active)
    {
        OVRCameraRig.gameObject.SetActive(active);
    }

    private IEnumerator WaitingForTrackingPoseValid()
    {
        // Avatar will have default pose, rotation and position until HMD mounted (or in PC mode)

        while (!LocalAvatarEntity.TrackingPoseValid)
        {
            yield return null;
            if (LocalAvatarEntity == null)
            {
                yield break;
            }
        }

        LocalAvatarEntity.transform.localPosition = Vector3.zero;
        LocalAvatarEntity.transform.forward = -LocalAvatarEntity.transform.forward;
    }
}
