using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Foldout("UI")] public List<GameObject> allPanels = new List<GameObject>();

    [Header("Loading")]
    [Foldout("UI")] public GameObject panel_Loading;
    [Foldout("UI")] public TMP_Text text_Loading;

    [Header("LoggedIn")]
    [Foldout("UI")] public GameObject panel_LoggedIn;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SwapPanel(GameObject panelToActivate)
    {
        foreach(GameObject go in allPanels)
        {
            go.SetActive(false);
        }

        panelToActivate.SetActive(true);
    }

    void Load(string loadingMsg)
    {
        //opens the loading screen panel
        SwapPanel(panel_Loading);
        //changes the loading screen text to display what is happening
        text_Loading.text = loadingMsg;
    }

    //Called when login to playfab is done
    public void ConnectToPhoton()
    {
        Load($"Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Photon");
        Load($"Connected to Photon, connecting to lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined Lobby");
        SwapPanel(panel_LoggedIn);
    }
}
