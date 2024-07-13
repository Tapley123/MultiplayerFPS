using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PhotonView pv;

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
    }
}
