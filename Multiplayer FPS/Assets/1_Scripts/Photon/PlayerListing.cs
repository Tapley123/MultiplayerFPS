using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text text_Name;

    Player player;

    public void Initialize(Player _player)
    {
        player = _player;
        text_Name.text = _player.NickName;
    }
    
    //called when a player leaves
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //this player has left the room
        if(player == otherPlayer)
        {
            //destroy this game object
            Destroy(this.gameObject);
        }
    }

    //if you leave the room
    public override void OnLeftRoom()
    {
        //destroy this game object
        Destroy(this.gameObject);
    }
}
