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
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        CreateController();
    }

    void CreateController()
    {
        Debug.Log($"Instansiate the Player controller here");

        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();

        if(PhotonNetwork.IsConnected)
        {
            playerController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"{prefab_PlayerController.name}"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
        }
        else
        {
            playerController = Instantiate(prefab_PlayerController, spawnPoint.position, spawnPoint.rotation);
            //PlayerController script = playerController.GetComponent<PlayerController>();
        }
    }

    public void Die()
    {
        //online
        if(PhotonNetwork.IsConnected)
        {
            //Destroy the player
            PhotonNetwork.Destroy(playerController);
        }
        else
        {
            //Destroy the player
            Destroy(playerController);  
        }

        //Respawn
        CreateController();
    }
}
