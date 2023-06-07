using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private Transform SpawnCenter;
    [SerializeField]
    private float SpawnRadius;

    public void SpawnPlayer(LocalPlayerRoot localPlayer)
    {
        if (localPlayer == null)
        {
            return;
        }
        float angle = Random.Range(0f, 359f);
        Vector3 spawnPoint = SpawnCenter.position 
            + new Vector3(SpawnRadius * Mathf.Cos(angle), 0, SpawnRadius * Mathf.Sin(angle));

        localPlayer.SetPosition(spawnPoint);
        localPlayer.SetRotation(Quaternion.LookRotation(SpawnCenter.position - spawnPoint));
        localPlayer.SetActive(true);
    }
}
