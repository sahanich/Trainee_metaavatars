using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerPositionSynchronizer : NetworkBehaviour
{
    public static event Action<NetworkPlayerPositionSynchronizer> LocalPlayerPositionSynchronizerSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            LocalPlayerPositionSynchronizerSpawned?.Invoke(this);
        }
    }

    public void Init(Transform target)
    {
        StartCoroutine(SyncPositionAndRotationRoutine(target));
    }

    private IEnumerator SyncPositionAndRotationRoutine(Transform target)
    {
        WaitForSeconds sendingDelay = new WaitForSeconds(0.1f);
        while (target != null)
        {
            SendNewPositionServerRpc(target.position, target.rotation);
            yield return sendingDelay;
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SendNewPositionServerRpc(Vector3 position, Quaternion rotation)
    {
        SendNewPositionClientRpc(position, rotation);
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    private void SendNewPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }
}
