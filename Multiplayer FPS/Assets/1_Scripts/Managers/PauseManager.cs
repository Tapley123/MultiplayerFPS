using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PauseManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static PauseManager _instance;
    public static PauseManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<PauseManager>(); } return _instance; } }
    #endregion

    #region Refrences
    public bool paused = false;
    [ReadOnly] public PlayerRefrences playerRefrences;
    [SerializeField] private GameObject pauseMenuGo;

    [SerializeField] private List<GameObject> allPanels;

    #region Main Panel
    [SerializeField] private GameObject panel_Main;
    #endregion

    #region Audio

    #endregion
    #endregion

    void Start()
    {
        //make sure it is on the main panel
        SwapPanel(panel_Main);

        //Make sure you unpause on start
        paused = true;
        TogglePause();
    }

    public void TogglePause()
    {
        Debug.Log($"Toggle Pause");

        //Swap the paused state
        paused = !paused;

        //toggle the game object
        pauseMenuGo.SetActive(paused);

        //Pause
        if (paused)
        {
            Pause();
        }
        //UnPause
        else
        {
            UnPause();
        }
    }

    private void Pause()
    {
        Debug.Log($"Pause");

        if(playerRefrences != null)
        {
            //Unlock the players cursor so they can use the pause menu
            playerRefrences.cursorController.UnlockAndShowCursor();
        }
        else
        {
            Debug.LogError($"Player Refrences {playerRefrences} is not assigned");
        }
    }

    private void UnPause()
    {
        Debug.Log($"UnPause");

        
        if (playerRefrences != null)
        {
            //Lock and hide the players cursor
            playerRefrences.cursorController.LockAndHideCursor();
        }
        else
        {
            Debug.LogError($"Player Refrences {playerRefrences} is not assigned");
        }
    }

    #region Behaviors
    public void SwapPanel(GameObject panelToActivate)
    {
        //Loop through all of the panels
        foreach(GameObject go in allPanels)
        {
            //turn them all off
            go.SetActive(false);
        }

        //Turn on current panel
        panelToActivate.SetActive(true);
    }
    #endregion

    #region Main Panel
    public void Button_ReturnToMenu()
    {
        //Online
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        //Offline
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    #endregion

    #region Audio Panel

    #endregion

    #region Photon Overrrides
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
