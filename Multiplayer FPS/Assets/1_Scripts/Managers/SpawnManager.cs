using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SpawnManager : MonoBehaviour
{
    #region Singleton
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<SpawnManager>(); } return _instance; } }
    #endregion

    [SerializeField] private SpawnPoint[] spawnPoints;

    void Start()
    {
        if(spawnPoints.Length != GetComponentsInChildren<SpawnPoint>().Length)
            GetSpawnPoints();
    }

    void Update()
    {
        
    }

    [Button]
    void GetSpawnPoints()
    {
        //get the spawnpoints
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        Transform t = null;

        //set t to a random spawnpoint in the array
        t = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;

        return t;
    }
}