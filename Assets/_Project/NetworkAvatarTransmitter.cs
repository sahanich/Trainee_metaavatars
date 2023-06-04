using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;

public class NetworkAvatarTransmitter : NetworkBehaviour
{
    [SerializeField]
    private SampleAvatarEntity _remoteAvatar;

    private SampleAvatarEntity _localAvatar;
    private PoseSerializableAvatarData _poseAvatarData;
    private BaseSerializableAvatarData _baseAvatarData;

    public static event Action<NetworkAvatarTransmitter> LocalPlayerAvatarTransmitterSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsClient)
        {
            return;
        }

        if (IsOwner)
        {
            LocalPlayerAvatarTransmitterSpawned?.Invoke(this);
        }
        else
        {
            RequestBaseAvatarDataServerRpc(NetworkManager.LocalClientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();
    }

    public void Init(SampleAvatarEntity localAvatar, int assetId)
    {
        _localAvatar = localAvatar;
        _localAvatar.ReloadAvatarManually(assetId.ToString(), SampleAvatarEntity.AssetSource.Zip);
        _remoteAvatar.Hidden = true;
        LoadDefaultRemoteAvatar(assetId);

        _baseAvatarData = new()
        {
            assetId = assetId,
            position = localAvatar.transform.position,
            rotation = localAvatar.transform.rotation,
        };

        SendBaseAvatarDataServerRpc(_baseAvatarData);
        StartCoroutine(AvatarDataTransmissionRoutine());
    }

    private IEnumerator AvatarDataTransmissionRoutine()
    {
        while (!_remoteAvatar.isActiveAndEnabled || _remoteAvatar.SkeletonJointCount == 0)
        {
            yield return null;
        }

        if (NetworkManager == null)
        {
            yield break;
        }
        
        WaitForSeconds sendingDelay = new(1f / NetworkManager.NetworkConfig.TickRate);
        WaitForEndOfFrame endOfFrameDelay = new();

        PoseSerializableAvatarData poseData = new();
        poseData.PacketData = new();

        while (true)
        {
            yield return sendingDelay;
            yield return endOfFrameDelay;

            if (_localAvatar.SkeletonJointCount == 0 || !_localAvatar.HasJoints)
            {
                continue;
            }

            if (NetworkManager == null)
            {
                yield break;
            }

            poseData.PacketData.lod = StreamLOD.Low;
            poseData.PacketData.data = _localAvatar.RecordStreamData(StreamLOD.Low);
            poseData.position = _localAvatar.transform.position;
            poseData.rotation = _localAvatar.transform.rotation;
            _remoteAvatar.ApplyStreamData(poseData.PacketData.data);
            _remoteAvatar.transform.SetPositionAndRotation(poseData.position, poseData.rotation);

            SendPoseDataServerRPC(poseData);
        }
    }

    [ServerRpc]
    private void SendBaseAvatarDataServerRpc(BaseSerializableAvatarData baseAvatarData)
    {
        _baseAvatarData = baseAvatarData;
        SendBaseAvatarDataClientRpc(_baseAvatarData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBaseAvatarDataServerRpc(ulong clientId)
    {
        if (_baseAvatarData == null)
        {
            return;
        }

        ClientRpcParams clientRpcParams = new()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        SendBaseAvatarDataClientRpc(_baseAvatarData, clientRpcParams);
    }


    [ClientRpc]
    private void SendBaseAvatarDataClientRpc(BaseSerializableAvatarData baseAvatarData, 
        ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner)
        {
            return;
        }

        _baseAvatarData = baseAvatarData;
        LoadDefaultRemoteAvatar(_baseAvatarData.assetId);
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SendPoseDataServerRPC(PoseSerializableAvatarData poseData)
    {
        _poseAvatarData = poseData;
        SendPoseDataClientRpc(_poseAvatarData);
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    private void SendPoseDataClientRpc(PoseSerializableAvatarData poseData,
        ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner)
        {
            return;
        }

        _poseAvatarData = poseData;

        _remoteAvatar.transform.SetPositionAndRotation(poseData.position, poseData.rotation);
        if (_remoteAvatar.SkeletonJointCount == 0)
        {
            return;
        }

        _remoteAvatar.ApplyStreamData(poseData.PacketData.data);
    }

    private void LoadDefaultRemoteAvatar(int assetId)
    {
        _remoteAvatar.ReloadAvatarManually(assetId.ToString(), SampleAvatarEntity.AssetSource.Zip);
    }
}
