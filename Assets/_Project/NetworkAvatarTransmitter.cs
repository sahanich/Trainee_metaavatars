using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;

public class NetworkAvatarInitializer
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

public class NetworkAvatarTransmitter : NetworkBehaviour
{
    [SerializeField]
    private SampleAvatarEntity _remoteAvatar;

    private SampleAvatarEntity _localAvatar;
    private PoseSerializableAvatarData _poseAvatarData;
    private BaseSerializableAvatarData _baseAvatarData;

    public static event Action<NetworkAvatarTransmitter> LocalPlayerAvatarTransmitterSpawned;

    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            return;
        }

        if (IsOwner)
        {
            LocalPlayerAvatarTransmitterSpawned?.Invoke(this);
            //SendBaseAvatarDataServerRpc();
            //StartCoroutine(SendingAvatarPoseData);
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
        LoadDefaultRemoteAvatar(assetId);

        _baseAvatarData = new()
        {
            assetId = assetId,
            position = localAvatar.transform.position,
            rotation = localAvatar.transform.rotation,
        };

        StartCoroutine(WaitingForTrackingPoseValid());
        StartCoroutine(AvatarDataTransmissionRoutine());
    }

    private IEnumerator AvatarDataTransmissionRoutine()
    {
        while (_remoteAvatar.SkeletonJointCount == 0)
        {
            yield return null;
        }

        if (NetworkManager == null)
        {
            yield break;
        }

        WaitForSeconds sendingDelay = new(2f / NetworkManager.NetworkConfig.TickRate);
        WaitForEndOfFrame endOfFrameDelay = new();

        PoseSerializableAvatarData poseData = new();
        poseData.PacketData = new();

        while (true)
        {
            yield return sendingDelay;
            yield return endOfFrameDelay;

            if (_remoteAvatar.SkeletonJointCount == 0)
            {
                continue;
            }

            if (NetworkManager == null)
            {
                yield break;
            }

            poseData.PacketData.lod = StreamLOD.Low;
            poseData.PacketData.data = _localAvatar.RecordStreamData(StreamLOD.Low);

            SendPoseDataServerRPC(poseData);
        }
    }

    private void SetAvatarPositionAndForward(Vector3 position, Vector3 forward)
    {
        _localAvatar.transform.localPosition = position;
        _localAvatar.transform.forward = forward;
        _remoteAvatar.transform.SetPositionAndRotation(_localAvatar.transform.position, _localAvatar.transform.rotation);

        _baseAvatarData.position = _remoteAvatar.transform.localPosition;
        _baseAvatarData.rotation = _remoteAvatar.transform.localRotation;
        SendBaseAvatarDataServerRpc(_baseAvatarData);
    }

    private IEnumerator WaitingForTrackingPoseValid()
    {
        // Avatar will have default pose, rotation and position until HMD mounted (or in PC mode)
        SetAvatarPositionAndForward(_localAvatar.transform.localPosition, -_localAvatar.transform.forward);

        while (!_localAvatar.TrackingPoseValid)
        {
            yield return null;
            if (_localAvatar == null)
            {
                yield break;
            }
        }

        SetAvatarPositionAndForward(Vector3.zero, -_localAvatar.transform.forward);
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
        _remoteAvatar.transform.localPosition = _baseAvatarData.position;
        _remoteAvatar.transform.localRotation = _baseAvatarData.rotation;
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
