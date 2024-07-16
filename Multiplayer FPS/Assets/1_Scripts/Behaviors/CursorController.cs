using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class CursorController : MonoBehaviour
{
    public PhotonView pv;
    [ReadOnly] public bool locked = false;

    void Start()
    {
        //if online and not mine
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        // Lock and hide the cursor when the game starts
        LockAndHideCursor();
    }

    void Update()
    {
        //if online and not mine
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        // Press Escape to unlock and show the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockAndShowCursor();
        }
        // Press the left mouse button to lock and hide the cursor
        else if (Input.GetMouseButtonDown(0))
        {
            LockAndHideCursor();
        }
    }

    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        locked = true;
    }

    private void UnlockAndShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        locked = false;
    }
}
