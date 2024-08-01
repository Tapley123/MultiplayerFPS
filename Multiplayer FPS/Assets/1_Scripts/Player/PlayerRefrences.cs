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
    public Image image_HealthBar;
    public TMP_Text text_Username;
    public TMP_Text text_AmmoCount;

    [BoxGroup("UI")] public GameObject playerUI;
    [BoxGroup("UI")] public GameObject crossHairGo;
    [BoxGroup("UI")] public List<CrosshairPart> movingCrosshairParts = new List<CrosshairPart>();

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

    private void Start()
    {
        //if there is at leas 1 crosshair part to move
        if(movingCrosshairParts.Count > 0)
        {
            // Initialize start and furthest forward positions
            foreach (CrosshairPart rectTransformMovement in movingCrosshairParts)
            {
                rectTransformMovement.startPosition = rectTransformMovement.rectTransform.localPosition;
            }
        }
        else
            Debug.LogError($"No Crosshair parts to move");
    }
}

[System.Serializable]
public class CrosshairPart
{
    public RectTransform rectTransform;
    public Vector3 startPosition;
}
