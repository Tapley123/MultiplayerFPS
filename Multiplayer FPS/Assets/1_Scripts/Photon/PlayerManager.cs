using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using NaughtyAttributes;

public class PlayerManager : MonoBehaviour
{
    public PhotonView pv;
    [SerializeField] private GameObject prefab_PlayerController;
    [ReadOnly] public CachedData cachedData;

    GameObject playerController;

    float score = 0;
    int kills = 0;
    int deaths = 0;
    int assists = 0;

    //score calculations
    [Header("Score")]
    [SerializeField] private float scorePerKill = 100;
    [SerializeField] private float scorePerAssist = 50;

    void Start()
    {
        //*** Dont do anything if online and not mine ***\\
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //Get cached Data
        if(cachedData == null && CachedData.Instance)
        {
            cachedData = CachedData.Instance;
            pv.RPC(nameof(RPC_SendCachedData), RpcTarget.OthersBuffered, cachedData.testInt);
        }

        CreateController();
    }

    [PunRPC]
    void RPC_SendCachedData(int testInt)
    {
        Debug.Log($"Player {pv.Owner.NickName}:{pv.Owner.ActorNumber}'s testInt = {testInt}");
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

    public void GetAssist()
    {
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }

    [PunRPC]
    public void RPC_GetAssist()
    {
        //incriment kills by 1
        assists++;

        //increase the score
        score += scorePerAssist;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "score", score },
            { "assists", assists }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
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

        //increase the score
        score += scorePerKill;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "score", score },
            { "kills", kills },
            { "kd", CalculateKDRatio(kills, deaths)}
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

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
