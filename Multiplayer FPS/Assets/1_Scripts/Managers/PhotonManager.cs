using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;
using Photon.Realtime;
using System.Text;
using UnityEngine.UI;
using System.Linq;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> allPanels = new List<GameObject>();

    [Foldout("Loading")][SerializeField] private GameObject panel_Loading;
    [Foldout("Loading")][SerializeField] private TMP_Text text_Loading;

    [Foldout("Error")][SerializeField] private TMP_Text text_Error;

    [Foldout("LoggedIn")][SerializeField] private GameObject panel_LoggedIn;

    [Header("Create Room")]
    [Foldout("Create Room")][SerializeField] private GameObject panel_CreateRoom;
    [Header("Name")]
    [Foldout("Create Room")][SerializeField] private TMP_InputField input_RoomName;
    [Header("Private/Password")]
    [Foldout("Create Room")][SerializeField] private CustomToggle toggle_Private;
    [Foldout("Create Room")][SerializeField] private GameObject passwordGo;
    [Foldout("Create Room")][SerializeField] private bool useRandomPassword = false;
    [Foldout("Create Room")][ShowIf("useRandomPassword")][SerializeField] private int randomPasswordLenth = 4;
    [Foldout("Create Room")] private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //used for creating random password
    [Foldout("Create Room")][SerializeField] private string defaultPassword = $"123";
    [Foldout("Create Room")][SerializeField] private TMP_InputField input_Password;
    [Header("Players")]
    [Foldout("Create Room")][SerializeField] private TMP_Text text_AmtOfPlayers;
    [Foldout("Create Room")][SerializeField] private int maxAmtOfPlayers = 4;
    [Foldout("Create Room")][SerializeField] private int minAmtOfPlayers = 2;
    [Foldout("Create Room")][SerializeField] private int amtOfPlayers = 4;

    [Header("Join Room")]
    [Foldout("Join Room")][SerializeField] private GameObject panel_JoinRoom;
    [Header("Room Lising")]
    [Foldout("Join Room")][SerializeField] private Transform roomListHolder;
    [Foldout("Join Room")][SerializeField] private RoomListing roomListing;
    [Foldout("Join Room")] private List<RoomInfo> roomList = new List<RoomInfo>();

    [Foldout("In A Room")][SerializeField] private GameObject panel_InARoom;
    [Foldout("In A Room")][SerializeField] private TMP_Text text_RoomName;
    [Header("Player Listing")]
    [Foldout("In A Room")][SerializeField] private Transform playerListHolder;
    [Foldout("In A Room")][SerializeField] private PlayerListing playerListing;
    [Foldout("In A Room")][SerializeField] private TMP_Text text_playerCount;
    [Header("Host Settings")]
    [Foldout("In A Room")][SerializeField] private List<Button> host_Buttons = new List<Button>();
    [Foldout("In A Room")][SerializeField] private List<Toggle> host_Toggles = new List<Toggle>();
    [Foldout("In A Room")][SerializeField] private List<Slider> host_Sliders = new List<Slider>();

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

        PhotonNetwork.CreateRoom(input_RoomName.text, options, TypedLobby.Default);
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
    public void JoinRoom(RoomInfo info)
    {
        //join the room
        PhotonNetwork.JoinRoom(info.Name);

        //turn on loading screen
        Load($"Joining room...");
    }

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

        // Clear existing UI player list
        foreach (Transform child in playerListHolder)
        {
            Destroy(child.gameObject);
        }
        //get all of the players
        Player[] players = PhotonNetwork.PlayerList;
        //loop through all of the players
        for (int i = 0; i < players.Count(); i++)
        {
            //spawn the player listing item
            PlayerListing playerListItem = Instantiate(playerListing, playerListHolder);
            //initialize the player listing
            playerListItem.Initialize(players[i]);
        }

        ToggleHostSettings();
    }

    void ToggleHostSettings()
    {
        //Buttons
        if (host_Buttons.Count > 0)
        {
            foreach (Button b in host_Buttons)
            {
                b.interactable = PhotonNetwork.IsMasterClient;
            }
        }
        //Toggles
        if (host_Toggles.Count > 0)
        {
            foreach (Toggle t in host_Toggles)
            {
                t.interactable = PhotonNetwork.IsMasterClient;
            }
        }
        //Sliders
        if (host_Sliders.Count > 0)
        {
            foreach (Slider s in host_Sliders)
            {
                s.interactable = PhotonNetwork.IsMasterClient;
            }
        }
    }

    public void Button_StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
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

    public void Button_QuitGame()
    {
        Application.Quit();
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

        XMLManager.Instance.LoadItems(SaveType.Player);

        Debug.LogError($"Setting username here!!");

        //set username
        string username = XMLManager.Instance.playerDB.username;
        //if the saved username is empty
        if (string.IsNullOrEmpty(username))
        {
            //set random username
            username = $"TestUsername{GenerateCode(4)}";
        }
        //if the username is stored locally
        else
        {
#if UNITY_EDITOR
            username = $"{username}(Editor)";
#else
            username = $"{username}(Build){GenerateCode(4)}";
#endif
        }

        PhotonNetwork.NickName = username;
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
        UpdatePlayerCount();
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //spawn the player listing item
        PlayerListing playerListItem = Instantiate(playerListing, playerListHolder);
        //initialize the player listing
        playerListItem.Initialize(newPlayer);

        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
        //the master may have left so make sure if you are the new master that you can use the host settings
        ToggleHostSettings();
    }

    void UpdatePlayerCount()
    {
        //Player Count Text update
        text_playerCount.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    #endregion
}
