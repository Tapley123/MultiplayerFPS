using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using NaughtyAttributes;
using ExitGames.Client.Photon.StructWrapping;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    Player player;
    [ReadOnly] public Scoreboard scoreboardScript;
    [ReadOnly] public Transform myT;

    [Tooltip("Only activate for myself so I can easily see who I am on the leaderboard")]
    public GameObject highlightMeGo;

    public TMP_Text text_Rank;
    public TMP_Text text_Username;
    [ReadOnly] public float score;
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
        //get the transform
        myT = transform;
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
        //if the score value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("score", out object score))
        {
            //set the score text to the amount of kills the player has
            text_Score.text = score.ToString();
        }

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

        //if the assists value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("assists", out object assists))
        {
            //set the assists text to the amount of deaths the player has
            text_Assists.text = assists.ToString();
        }

        //if the k/d value exists in the players custom properties
        if (player.CustomProperties.TryGetValue("kd", out object kd))
        {
            //set the k/d text to the amount of deaths the player has
            text_KDRatio.text = kd.ToString();
        }

        scoreboardScript.OrganizeScoreboardItems();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            if (changedProps.ContainsKey("score"))
            {
                score = (float)changedProps["score"];
            }

            //if the kills or deaths were updated
            if (changedProps.ContainsKey("score") || changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths") || changedProps.ContainsKey("kd") || changedProps.ContainsKey("assists"))
            {
                UpdateStats();
            }
        }
    }
}
