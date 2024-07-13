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
    [SerializeField] private TMP_Text text_RoomName;
    [SerializeField] private TMP_Text text_PlayerCount;
    [SerializeField] private TMP_InputField inputField_Password;
    [SerializeField] private RoomInfo roomInfo;

    [SerializeField] private GameObject privateRoom;
    [SerializeField] private GameObject publicRoom;

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
            string roomPassword = customProperties["password"].ToString();

            //wrong password
            if (inputField_Password.text != roomPassword)
            {
                Debug.LogError($"Wrong Password! Try again");
                return;
            }
            else
            {
                Debug.LogError($"Correct Password");
            }
        }
        //public
        else
        {
            Debug.LogError($"No Password needed");
        }

        Debug.Log($"Trying to join '{text_RoomName.text}' room...");
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }
}
