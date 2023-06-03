using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    //[SerializeField]
    //private Transform[] SpawnPoints;

    [SerializeField]
    private Transform SpawnCenter;
    [SerializeField]
    private float SpawnRadius;

    private LocalPlayerRoot _localPlayer;
    //private void OnEnable()
    //{
    //    NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    //}

    //private void OnDisable()
    //{
    //    NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    //}

    //private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    //{
    //    throw new System.NotImplementedException();
    //}

    public void Init(LocalPlayerRoot localPlayer)
    {
        _localPlayer = localPlayer;
    }

    public void SetWaitingForSpawnMode()
    {
        //_localPlayer.gameObject.SetActive(false);
    }

    public void SpawnPlayer()
    {
        if (_localPlayer == null)
        {
            return;
        }
        float angle = Random.Range(0f, 359f);
        Vector3 spawnPoint = SpawnCenter.position 
            + new Vector3(SpawnRadius * Mathf.Cos(angle), 0, SpawnRadius * Mathf.Sin(angle));
        _localPlayer.SetPosition(spawnPoint);
        _localPlayer.SetForward(SpawnCenter.position - spawnPoint);

        //_localPlayer.gameObject.SetActive(true);
    }
}
