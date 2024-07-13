using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;
using Photon.Realtime;
using System.Text;
using UnityEngine.UIElements;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Foldout("UI")] public List<GameObject> allPanels = new List<GameObject>();

    [Header("Loading")]
    [Foldout("UI")][SerializeField] private GameObject panel_Loading;
    [Foldout("UI")][SerializeField] private TMP_Text text_Loading;

    [Header("Error")]
    [Foldout("UI")][SerializeField] private TMP_Text text_Error;

    [Header("LoggedIn")]
    [Foldout("UI")][SerializeField] private GameObject panel_LoggedIn;

    [Header("Create Room")]
    [Foldout("UI")][SerializeField] private GameObject panel_CreateRoom;
    //Name
    [Foldout("UI")][SerializeField] private TMP_InputField input_RoomName;
    //Private/Password
    [Foldout("UI")][SerializeField] private CustomToggle toggle_Private;
    [Foldout("UI")][SerializeField] private GameObject passwordGo;
    [Foldout("UI")][SerializeField] private bool useRandomPassword = false;
    [Foldout("UI")][ShowIf("useRandomPassword")][SerializeField] private int randomPasswordLenth = 4;
    [Foldout("UI")] private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    [Foldout("UI")][SerializeField] private string defaultPassword = $"123";
    [Foldout("UI")][SerializeField] private TMP_InputField input_Password;
    //Players
    [Foldout("UI")][SerializeField] private TMP_Text text_AmtOfPlayers;
    [Foldout("UI")][SerializeField] private int maxAmtOfPlayers = 4;
    [Foldout("UI")][SerializeField] private int minAmtOfPlayers = 2;
    [Foldout("UI")][SerializeField] private int amtOfPlayers = 4;

    [Header("Join Room")]
    [Foldout("UI")][SerializeField] private GameObject panel_JoinRoom;
    [Foldout("UI")][SerializeField] private Transform roomListHolder;
    [Foldout("UI")][SerializeField] private RoomListing roomListing;
    [Foldout("UI")] private List<RoomInfo> roomList = new List<RoomInfo>();

    [Header("In A Room")]
    [Foldout("UI")][SerializeField] private GameObject panel_InARoom;
    [Foldout("UI")][SerializeField] private TMP_Text text_RoomName;

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


        //CREATE ROOM
        RoomOptions options = new RoomOptions();
        ExitGames.Client.Photon.Hashtable customRoomProperties;

        //private
        if (toggle_Private.toggle.isOn)
        {
            //make sure password is not empty
            if(string.IsNullOrEmpty(input_Password.text))
            {
                Error($"Password is empty...");
                return;
            }

            // Define custom room properties
            customRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "password", input_Password.text },
                { "map", $"MapName" },
            };
        }
        //public
        else
        {
            // Define custom room properties
            customRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "map", $"MapName" },
            };
        }

        // Assign the custom properties to the room options
        options.CustomRoomProperties = customRoomProperties;

        // Define which custom properties should be visible in the lobby
        options.CustomRoomPropertiesForLobby = new string[] { "password", "map" };

        options.MaxPlayers = amtOfPlayers;
        //options.IsPrivate = toggle_Private.toggle.isOn;
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.CreateRoom(input_RoomName.text);
    }

    public void Button_OpenCreateRoom()
    {
        SwapPanel(panel_CreateRoom);

        //toggle private/public room stuff
        Toggle_Private();

        //if the amount of players deosnt match the text
        if(int.Parse(text_AmtOfPlayers.text) != amtOfPlayers)
        {
            text_AmtOfPlayers.text = $"{amtOfPlayers}";
        }
    }

    public void Toggle_Private()
    {
        passwordGo.SetActive(toggle_Private.toggle.isOn);

        //private
        if (toggle_Private.toggle.isOn)
        {
            //random password
            if (useRandomPassword)
            {
                //set random password
                input_Password.text = GenerateCode(randomPasswordLenth);
            }
            //default password
            else
            {
                //if there is no password typed then use the default password
                if (string.IsNullOrEmpty(input_Password.text))
                    input_Password.text = defaultPassword;
            }
            
        }
        //public
        else
        {

        }
    }

    public string GenerateCode(int length = 4)
    {
        StringBuilder result = new StringBuilder(length);
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(characters.Length);
            result.Append(characters[index]);
        }

        return result.ToString();
    }

    public void Button_MorePlayers()
    {
        if (amtOfPlayers < maxAmtOfPlayers)
        {
            amtOfPlayers++;
        }

        text_AmtOfPlayers.text = $"{amtOfPlayers}";
    }

    public void Button_LessPlayers()
    {
        //2 is the minimum amount of players
        if (amtOfPlayers > minAmtOfPlayers)
        {
            amtOfPlayers--;
        }

        text_AmtOfPlayers.text = $"{amtOfPlayers}";
    }
    #endregion

    #region Join Room
    public void UpdateRoomListUI()
    {
        // Clear existing UI elements
        foreach (Transform child in roomListHolder)
        {
            Destroy(child.gameObject);
        }

        // Populate the UI with the updated room list
        foreach (RoomInfo room in roomList)
        {
            RoomListing roomListItem = Instantiate(roomListing, roomListHolder);
            roomListItem.SetRoomInfo(room);
            roomListItem.photonManager = this;
        }
    }
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
        if(!panel_Loading.activeInHierarchy)
            SwapPanel(panel_Loading);
        //changes the loading screen text to display what is happening
        text_Loading.text = loadingMsg;
    }

    public void ClearError()
    {
        text_Error.text = string.Empty;
    }

    public void Error(string errorMsg)
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

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room");

        EnteredRoom();
    }

    public override void OnLeftRoom()
    {
        //go back to the create/join room screen
        SwapPanel(panel_LoggedIn);
    }

    public override void OnRoomListUpdate(List<RoomInfo> updatedRoomList)
    {
        // Loop through the updated room list
        foreach (RoomInfo room in updatedRoomList)
        {
            if (room.RemovedFromList)
            {
                // Remove the room from our list if it has been removed
                roomList.Remove(room);
            }
            else
            {
                // Add or update the room in our list
                int index = roomList.FindIndex(r => r.Name == room.Name);
                if (index == -1)
                {
                    // Add the new room
                    roomList.Add(room);
                }
                else
                {
                    // Update the existing room
                    roomList[index] = room;
                }
            }
        }

        UpdateRoomListUI();
    }
    #endregion
}
