using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class DestroyMe : MonoBehaviour
{
    public enum DestroyType { Mine, NotMine, Everyone };

    [Tooltip("Mine = Only destroy if it belongs to me, NotMine = Only destroy if it does not belong to me, Everyone = Destroy for everyone")]
    public DestroyType destroyType;

    [SerializeField] private PhotonView pv;

    public bool destroyOnAwake = true;

    void Awake()
    {
        //if there is a photon view
        if(transform.root.GetComponent<PhotonView>())
        {
            //set the photon view
            pv = transform.root.GetComponent<PhotonView>();
        }

        //destroy on awake
        if(destroyOnAwake)
        {
            DestroyThis();
        }
    }

    [Button]
    void DestroyThis()
    {
        switch (destroyType)
        {
            case DestroyType.Mine:
                //online
                if (PhotonNetwork.IsConnected && pv.IsMine)
                    Destroy(this.gameObject);
                //offline
                if(!PhotonNetwork.IsConnected)
                    Destroy(this.gameObject);
                break;

            case DestroyType.NotMine:
                //online
                if (PhotonNetwork.IsConnected && !pv.IsMine)
                    Destroy(this.gameObject);
                break;

            case DestroyType.Everyone:
                Destroy(this.gameObject);
                break;
        }
    }
}
