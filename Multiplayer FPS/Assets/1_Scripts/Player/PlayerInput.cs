using NaughtyAttributes;
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

    //Pause
    //public static Action pauseInput;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode altPauseKey = KeyCode.P;

    //shooting
    public static Action shootInput;
    [ReadOnly] public bool holdingShootButton = false;
    
    //reloading
    public static Action reloadInput;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    //aiming
    [ReadOnly] public bool isAiming = false;

    //weapon swapping
    public static Action swapWeaponInput;
    [SerializeField] private KeyCode swapWeaponKey = KeyCode.Alpha1;

    //crouch
    [Tooltip("When true you will uncrouch when you let go of the crouch button")] public bool holdCrouch = false;
    public static Action toggleCrouchInput;
    [SerializeField] private KeyCode toggleCrouchKey = KeyCode.LeftControl;

    //get mouse input
    [ReadOnly] public float mouseX;
    [ReadOnly] public float mouseY;


    private void Awake()
    {
        //online and does not belong to me so disable it
        if (PhotonNetwork.IsConnected && !pv.IsMine) { this.enabled = false; }
    }

    void Update()
    {
        //online and does not belong to me so do nothing
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //PAUSE
        if(Input.GetKeyDown(pauseKey) || Input.GetKeyDown(altPauseKey))
        {
            //pauseInput?.Invoke();
            PauseManager.Instance.TogglePause();
        }

        //if you are paused then dont read inputs below this
        if (PauseManager.Instance && PauseManager.Instance.paused) { return; }

        //MousePositions
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        //SHOOTING
        if (Input.GetMouseButtonDown(0))
        {
            shootInput?.Invoke();
            holdingShootButton = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            holdingShootButton = false;
        }

        //RELOADING
        if (Input.GetKeyDown(reloadKey))
        {
            reloadInput?.Invoke();
        }

        //AIMING
        // Check if right mouse button is held
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        //WEAPON SWAPPING
        //Button
        if (Input.GetKeyDown(swapWeaponKey))
        {
            swapWeaponInput?.Invoke();
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

        //crouch
        if(Input.GetKeyDown(toggleCrouchKey))
        {
            toggleCrouchInput?.Invoke();
        }
        if(holdCrouch && Input.GetKeyUp(toggleCrouchKey))
        {
            toggleCrouchInput?.Invoke();
        }
    }
}
