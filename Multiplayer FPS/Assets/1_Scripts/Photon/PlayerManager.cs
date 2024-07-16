using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [SerializeField] private GameObject prefab_PlayerController;

    GameObject playerController;

    void Start()
    {
        if (!PhotonNetwork.IsConnected) { return; }

        if(pv.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

        Debug.Log($"Instansiate the Player controller here");
        playerController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"{prefab_PlayerController.name}"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID});
    }

    public void Die()
    {
        //Destroy the player
        PhotonNetwork.Destroy(playerController);
        //Respawn
        CreateController();
    }
}
