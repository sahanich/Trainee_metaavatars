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

    //private LocalPlayerRoot _localPlayerPrefab;

    //public void Init(LocalPlayerRoot localPlayerPrefab)
    //{
    //    _localPlayerPrefab = localPlayerPrefab;
    //}

    public LocalPlayerRoot SpawnPlayer(LocalPlayerRoot localPlayerPrefab)
    {
        if (localPlayerPrefab == null)
        {
            return null;
        }
        float angle = Random.Range(0f, 359f);
        Vector3 spawnPoint = SpawnCenter.position 
            + new Vector3(SpawnRadius * Mathf.Cos(angle), 0, SpawnRadius * Mathf.Sin(angle));

        return Instantiate(localPlayerPrefab, spawnPoint, Quaternion.LookRotation(SpawnCenter.position - spawnPoint));
    }
}
