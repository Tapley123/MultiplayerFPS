using Photon.Pun;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerController playerContoller;
    public PhotonView pv;

    public static Action shootInput;

    public static Action reloadInput;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    public static Action swapWeapon;
    [SerializeField] private KeyCode swapWeaponKey = KeyCode.Alpha1;


    private void Awake()
    {
        //online and does not belong to me so disable it
        if (PhotonNetwork.IsConnected && !pv.IsMine) { this.enabled = false; }
    }

    void Update()
    {
        //online and does not belong to me so do nothing
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        if(Input.GetMouseButtonDown(0))
        {
            shootInput?.Invoke();
        }

        if(Input.GetKeyDown(reloadKey))
        {
            reloadInput?.Invoke();
        }

        if(Input.GetKeyDown(swapWeaponKey))
        {
            swapWeapon?.Invoke();
        }

        //scroll up
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            playerContoller.NextWeapon();
        }

        //scroll down
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            playerContoller.PreviousWeapon();
        }
    }
}
