using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using NaughtyAttributes.Test;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static RoomManager _instance;
    public static RoomManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<RoomManager>(); } return _instance; } }
    #endregion

    public GameObject prefab_PlayerManager;

    void Awake()
    {
        //Persist through scenes
        DontDestroyOnLoad(this);

        //Makes sure there is only one instance of this class
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable(); //DO NOT DELETE
     
        //Subscribe
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable(); //DO NOT DELETE

        //Un-Subscribe
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //Game Scene
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"{prefab_PlayerManager.name}"), Vector3.zero, Quaternion.identity);
        }
    }
}
