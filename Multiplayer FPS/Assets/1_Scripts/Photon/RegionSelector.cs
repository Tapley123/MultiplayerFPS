using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using Photon.Pun;

public class RegionSelector : MonoBehaviourPunCallbacks
{
    public TMP_Dropdown regionDropdown;
    public TMP_Text currentConnectedRegionText;
    [ReadOnly][SerializeField] private int currentRegionIndex;

    void SpeedRates()
    {
        Debug.Log("Upped the network speed rates");
        PhotonNetwork.SendRate = 40; //usually 20
        PhotonNetwork.SerializationRate = 30; //usually 10
    }

    void DisplayRegionInDropdown()
    {
        switch (PhotonNetwork.CloudRegion)
        {
            //Asia (asia)
            case "asia":
                currentRegionIndex = 0;
                break;

            //Australia (au)
            case "au":
                currentRegionIndex = 1;
                break;

            //Canada, East (cae)
            case "cae":
                currentRegionIndex = 2;
                break;

            //Europe (eu)
            case "eu":
                currentRegionIndex = 3;
                break;

            //Hong Kong (hk)
            case "hk":
                currentRegionIndex = 4;
                break;

            //India (in)
            case "in":
                currentRegionIndex = 5;
                break;

            //Japan (jp)
            case "jp":
                currentRegionIndex = 6;
                break;

            //South Africa (za)
            case "za":
                currentRegionIndex = 7;
                break;

            //South America (sa)
            case "sa":
                currentRegionIndex = 8;
                break;

            //South Korea (kr)
            case "kr":
                currentRegionIndex = 9;
                break;

            //Turkey (tr)
            case "tr":
                currentRegionIndex = 10;
                break;

            //United Arab Emirates (uae)
            case "uae":
                currentRegionIndex = 11;
                break;

            //USA, East (us)
            case "us":
                currentRegionIndex = 12;
                break;

            //USA, West (usw)
            case "usw":
                currentRegionIndex = 13;
                break;

            //USA, South Central (ussc)
            case "ussc":
                currentRegionIndex = 14;
                break;
        }

        regionDropdown.value = currentRegionIndex;
    }

    private void DisplayCurrentRegion()
    {
        string regionName = PhotonNetwork.CloudRegion;

        Debug.Log($"Displaying current region ({regionName})");

        if (regionName == "")
        {
            currentConnectedRegionText.text = regionName;
            regionDropdown.captionText.text = "Select Region";
        }
        else
        {
            currentConnectedRegionText.text = "Connected to (" + regionName + ")";
            DisplayRegionInDropdown();
        }
    }

    public void HandleInput(int val)
    {
        val = regionDropdown.value;

        switch (val)
        {
            //Asia (asia)
            case 0:
                ConnectToRegion("asia");
                break;

            //Australia (au)
            case 1:
                ConnectToRegion("au");
                break;

            //Canada, East (cae)
            case 2:
                ConnectToRegion("cae");
                break;

            //Europe (eu)
            case 3:
                ConnectToRegion("eu");
                break;

            //Hong Kong (hk)
            case 4:
                ConnectToRegion("hk");
                break;

            //India (in)
            case 5:
                ConnectToRegion("in");
                break;

            //Japan (jp)
            case 6:
                ConnectToRegion("jp");
                break;

            //South Africa (za)
            case 7:
                ConnectToRegion("za");
                break;

            //South America (sa)
            case 8:
                ConnectToRegion("sa");
                break;

            //South Korea (kr)
            case 9:
                ConnectToRegion("kr");
                break;

            //Turkey (tr)
            case 10:
                ConnectToRegion("tr");
                break;

            //United Arab Emirates (uae)
            case 11:
                ConnectToRegion("uae");
                break;

            //USA, East (us)
            case 12:
                ConnectToRegion("us");
                break;

            //USA, West (usw)
            case 13:
                ConnectToRegion("usw");
                break;

            //USA, South Central (ussc)
            case 14:
                ConnectToRegion("ussc");
                break;
        }
    }

    void ConnectToRegion(string regionName)
    {
        Debug.Log($"Connecting to ({regionName}) region");
        currentConnectedRegionText.text = $"Connecting...";

        PhotonNetwork.Disconnect(); //disconnect from the current region 
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionName; //select the region you want to join
        //SpeedRates();
        PhotonNetwork.ConnectUsingSettings(); //connect to that region with all the other settings
    }

    public override void OnConnectedToMaster()
    {
        //if (!PhotonNetwork.InLobby)
            //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        DisplayCurrentRegion();
    }
}
