using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [SerializeField] private GameObject prefab_PlayerController;

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
        Debug.Log($"Instansiate the Player controller here");
        GameObject playerController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"{prefab_PlayerController.name}"), Vector3.zero, Quaternion.identity);
    }
}
