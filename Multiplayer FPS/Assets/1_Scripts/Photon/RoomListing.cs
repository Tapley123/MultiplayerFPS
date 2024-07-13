using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomListing : MonoBehaviour
{
    [ReadOnly] public PhotonManager photonManager;

    [SerializeField] private TMP_Text text_RoomName;
    [SerializeField] private TMP_Text text_PlayerCount;
    [SerializeField] private TMP_InputField inputField_Password;
    [SerializeField] private RoomInfo roomInfo;

    [SerializeField] private GameObject privateRoom;
    [SerializeField] private GameObject publicRoom;

    [SerializeField] private List<UITweaker> uiTweakers = new List<UITweaker>();

    private void Awake()
    {
        //if there are ui tweakers
        if(uiTweakers.Count > 0)
        {
            //loop through all the ui tweakers
            foreach(UITweaker t in uiTweakers)
            {
                //if the audio source is not set
                if(t.audioSource == null)
                {
                    //find their audio source
                    t.audioSource = UITweakerManager.Instance.audioSource;
                }
            }
        }
    }

    public void SetRoomInfo(RoomInfo info)
    {
        roomInfo = info;
        text_RoomName.text = info.Name;
        text_PlayerCount.text = $"{info.PlayerCount}/{info.MaxPlayers}";

        // Access room custom properties
        ExitGames.Client.Photon.Hashtable customProperties = info.CustomProperties;

        //private
        if (customProperties.ContainsKey("password"))
        {
            string roomPassword = customProperties["password"].ToString();
            Debug.Log("Room " + roomInfo.Name + " has password: " + roomPassword);

            privateRoom.SetActive(true);
            publicRoom.SetActive(false);
        }
        //public
        else
        {
            publicRoom.SetActive(true);
            privateRoom.SetActive(false);
        }

        //map
        if (customProperties.ContainsKey("map"))
        {
            string map = customProperties["map"].ToString();
            Debug.Log($"map: {map}");
        }
        //No Map
        else
        {
            Debug.LogError($"no map!");
        }
    }

    public void Button_JoinRoom()
    {
        // Access room custom properties
        ExitGames.Client.Photon.Hashtable customProperties = roomInfo.CustomProperties;

        //private
        if (customProperties.ContainsKey("password"))
        {
            Debug.Log($"** Room Has Password **");

            string roomPassword = customProperties["password"].ToString();

            //wrong password
            if (inputField_Password.text != roomPassword)
            {
                if(photonManager != null)
                    photonManager.Error("Wrong Password! Try again");
                else
                    Debug.LogError($"Wrong Password! Try again");
                
                return;
            }
        }

        //has access to the photon manager
        if (photonManager != null)
        {
            photonManager.ClearError();
            photonManager.JoinRoom(roomInfo);
        }
        else
        {
            Debug.Log($"Trying to join '{text_RoomName.text}' room...");
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
    }
}
