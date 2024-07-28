using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerRefrences : MonoBehaviour
{
    [ReadOnly] public PlayerManager playerManager; //only for online
    public PlayerController playerController;
    public PlayerInput playerInput;
    public CursorController cursorController;
    public HandTransitioner handTransitioner;
    public SoundEffectPlayer soundEffectPlayer;
    public PhotonView pv;
    public Camera cam;
    public GameObject cameraHolder;
    public Transform magazineDump;
    public Rigidbody rb;
    public GameObject playerUI;
    public Image image_HealthBar;
    public TMP_Text text_Username;
    public TMP_Text text_AmmoCount;

    private void Awake()
    {
        //online and not mine
        if (PhotonNetwork.IsConnected && !pv.IsMine) { return; }

        //if the pause menu exists
        if (PauseManager.Instance)
        {
            //Connect the player to the pause menu
            PauseManager.Instance.playerRefrences = this;

            //PauseManager.Instance.paused = true;
            //PauseManager.Instance.TogglePause();
        }
        //no pause menu in the scene
        else
        {
            Debug.LogError($"No Pause Manager Found in the scene");
        }
    }
}
