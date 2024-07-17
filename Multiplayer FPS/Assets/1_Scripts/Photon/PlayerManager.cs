using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    public PhotonView pv;
    [SerializeField] private GameObject prefab_PlayerController;

    GameObject playerController;

    int kills = 0;
    int deaths = 0;
    int assists = 0;

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

    public void GetKill()
    {
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }

    [PunRPC]
    public void RPC_GetKill()
    {
        //incriment kills by 1
        kills++;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "kills", kills }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }
}
