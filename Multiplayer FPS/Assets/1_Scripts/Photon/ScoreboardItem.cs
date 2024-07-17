using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
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
    }
}
