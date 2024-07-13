using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;
using UnityEditor.PackageManager;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Foldout("UI")] public List<GameObject> allPanels = new List<GameObject>();

    [Header("Loading")]
    [Foldout("UI")] public GameObject panel_Loading;
    [Foldout("UI")] public TMP_Text text_Loading;

    [Header("Error")]
    [Foldout("UI")] public TMP_Text text_Error;

    [Header("LoggedIn")]
    [Foldout("UI")] public GameObject panel_LoggedIn;

    [Header("Create Room")]
    [Foldout("UI")] public GameObject panel_CreateRoom;
    [Foldout("UI")] public TMP_InputField input_RoomName;

    [Header("Join Room")]
    [Foldout("UI")] public GameObject panel_JoinRoom;

    [Header("In A Room")]
    [Foldout("UI")] public GameObject panel_InARoom;
    [Foldout("UI")] public TMP_Text text_RoomName;

    void Start()
    {

    }

    void Update()
    {

    }

    #region Create Room
    public void Button_CreateRoom()
    {
        //checks
        if(string.IsNullOrEmpty(input_RoomName.text))
        {
            Error($"Room Name is empty");
            return;
        }

        //clear the errors
        ClearError();
        Load($"Creating Room...");
        PhotonNetwork.CreateRoom(input_RoomName.text);
    }
    #endregion

    #region Join Room

    #endregion

    #region In A Room
    void EnteredRoom()
    {
        //leave the loading screen and go back to the create room screen
        SwapPanel(panel_InARoom);

        //set the room name
        text_RoomName.text = $"{PhotonNetwork.CurrentRoom.Name}'s Room";
    }

    public void Button_LeaveRoom()
    {
        Load($"Leaving Room...");
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Behaviors
    public void SwapPanel(GameObject panelToActivate)
    {
        foreach (GameObject go in allPanels)
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

    void ClearError()
    {
        text_Error.text = string.Empty;
    }

    void Error(string errorMsg)
    {
        text_Error.text = errorMsg;
        Debug.LogError(errorMsg);
    }
    #endregion

    #region Connect To Photon
    //Called when login to playfab is done
    public void ConnectToPhoton()
    {
        Load($"Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    //called when connected to photon lobby
    void ConnectedToPhoton()
    {
        //Go to the logged in panel
        SwapPanel(panel_LoggedIn);

        input_RoomName.text = $"{XMLManager.Instance.playerDB.username}'s Room";
    }
    #endregion

    #region Ovverride Functions
    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to Photon");
        ConnectedToPhoton();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined Lobby");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //display error
        Error($"Create Room Failed! Return Code: {returnCode}, Msg: {message}");
        //leave the loading screen and go back to the create room screen
        SwapPanel(panel_CreateRoom);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Created Room");
        
        EnteredRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room");
    }

    public override void OnLeftRoom()
    {
        //go back to the create/join room screen
        SwapPanel(panel_LoggedIn);
    }
    #endregion
}
