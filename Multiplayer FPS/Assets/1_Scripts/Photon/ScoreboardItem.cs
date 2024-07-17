using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    Player player;

    [Tooltip("Only activate for myself so I can easily see who I am on the leaderboard")]
    public GameObject highlightMeGo;

    public TMP_Text text_Rank;
    public TMP_Text text_Username;
    public TMP_Text text_Score;
    public TMP_Text text_Kills;
    public TMP_Text text_Deaths;
    public TMP_Text text_KDRatio;
    public TMP_Text text_Assists;
    public TMP_Text text_Ping;

    [Tooltip("Set active when player is talking")]
    public GameObject micGo;
    [Tooltip("Set active when player is dead")]
    public GameObject deadGo;

    [Tooltip("The picture emblem for your current rank")]
    public Image rankIcon;

    public void Initialize(Player player)
    {
        //set the username
        text_Username.text = player.NickName;
        //set the player
        this.player = player;
        //check if local player
        if(player.IsLocal)
        {
            //highlight this for you as you are this player
            highlightMeGo.SetActive(true);
        }
        //you are not the local player
        else
        {
            //Un-Highlight this for you as you are NOT this player
            highlightMeGo.SetActive(false);
        }
        //update the stats
        UpdateStats(); //called here if you join room late the stats will be updated for every player
    }

    void UpdateStats()
    {
        //if the kills value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            //set the kills text to the amount of kills the player has
            text_Kills.text = kills.ToString();
        }

        //if the deaths value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            //set the deaths text to the amount of deaths the player has
            text_Deaths.text = deaths.ToString();
        }

        //if the k/d value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("kd", out object kd))
        {
            //set the k/d text to the amount of deaths the player has
            text_KDRatio.text = kd.ToString();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            //if the kills or deaths were updated
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
            {
                UpdateStats();
            }
        }
    }
}
