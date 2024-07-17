using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool offOnStart = true;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private ScoreboardItem scoreboardItem;
    [SerializeField] private Transform team1PlayerHolder;
    [SerializeField] private Transform team2PlayerHolder;

    //store the players ref with its corosponding scoreboard item
    Dictionary<Player, ScoreboardItem> scoreboardItemsDict = new Dictionary<Player, ScoreboardItem>();

    void Start()
    {
        //turn off on start
        if (offOnStart && canvasGroup.alpha != 0)
            canvasGroup.alpha = 0;

        //dont do anything below this if offline
        if (!PhotonNetwork.IsConnected) { return; }

        Initialize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //If it is hidden show it
            if (offOnStart && canvasGroup.alpha == 0)
                canvasGroup.alpha = 1;
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            //if it is on display hide it
            if (offOnStart && canvasGroup.alpha != 0)
                canvasGroup.alpha = 0;
        }
    }

    private void Initialize()
    {
        //loop through all current scoreboard items for team 1
        foreach(Transform child in team1PlayerHolder)
        {
            //destroy them
            Destroy(child.gameObject);
        }

        //loop through all current scoreboard items for team 2
        foreach (Transform child in team2PlayerHolder)
        {
            //destroy them
            Destroy(child.gameObject);
        }

        //loop through all of the players
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            //add the player to the scoreboard
            AddScoreboardItem(p);
        }
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItem, team1PlayerHolder);
        item.Initialize(player);

        //add the player refrence to the dictionary
        scoreboardItemsDict[player] = item;
    }

    void RemoveScoreboardItem(Player player)
    {
        //destroy the player who lefts scoreboard item
        Destroy(scoreboardItemsDict[player].gameObject);
        //remove that players refrence from the dictionary
        scoreboardItemsDict.Remove(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
}
