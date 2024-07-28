using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    #region Singleton
    private static PauseManager _instance;
    public static PauseManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<PauseManager>(); } return _instance; } }
    #endregion

    public bool paused = false;
    [ReadOnly] public PlayerRefrences playerRefrences;
    [SerializeField] private GameObject pauseMenuGo;

    void Awake()
    {
        //Make sure you unpause on start
        paused = true;
        TogglePause();


    }

    void Update()
    {
        
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

        //Unlock the players cursor so they can use the pause menu
        playerRefrences.cursorController.UnlockAndShowCursor();
    }

    private void UnPause()
    {
        Debug.Log($"UnPause");

        //Lock and hide the players cursor
        playerRefrences.cursorController.LockAndHideCursor();
    }
}
