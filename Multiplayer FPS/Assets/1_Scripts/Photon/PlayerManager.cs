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

            //incriment kills by 1
            deaths++;

            //add deaths to hashtable
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
            {
                { "deaths", deaths },
                { "kd", CalculateKDRatio(kills, deaths)}
            };

            //update your deaths across the network
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        //offline
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
            { "kills", kills },
            { "kd", CalculateKDRatio(kills, deaths)}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    /*
    float CalculateKDRatio(int kills, int deaths)
    {
        if (deaths == 0)
        {
            if (kills == 0)
            {
                return 0f; // or return float.PositiveInfinity; if you want to show infinity
            }
            return kills;
        }
        return (float)kills / deaths;
    }
    */

    string CalculateKDRatio(int kills, int deaths)
    {
        if (kills == 0 && deaths == 0)
        {
            return "0.00"; // COD BO2 behavior for 0 kills and 0 deaths
        }
        if (deaths == 0)
        {
            return kills.ToString("F2"); // Infinite K/D ratio if no deaths
        }
        return ((float)kills / deaths).ToString("F2");
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }
}
