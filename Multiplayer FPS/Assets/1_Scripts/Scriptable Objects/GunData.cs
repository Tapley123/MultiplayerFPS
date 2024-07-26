using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "GunData", menuName = "Storage/GunData")]
public class GunData : ScriptableObject
{
    //Info
    [BoxGroup("Info")] public string name;

    //Shooting
    [BoxGroup("Shooting")][Tooltip("Damage dealt per shot landing")] public float damage;
    [BoxGroup("Shooting")][Tooltip("Maximum Distance the bullets can reach")] public float maxDistance;
    [BoxGroup("Shooting")][Tooltip("The Layers The gun can hit")] public LayerMask canShootLayers;

    //Adsing
    [BoxGroup("ADS")][Tooltip("Speed of the ADS transition")] public float adsSpeed = 5f; 
    [BoxGroup("ADS")][Tooltip("Adjust this value to change snapping sensitivity for position")] public float positionSnapTolerance = 0.001f;
    [BoxGroup("ADS")][Tooltip("Adjust this value to change snapping sensitivity for rotation")] public float rotationSnapTolerance = 0.01f;

    //Reloading
    [BoxGroup("Reloading")][Tooltip("Ammount of bullets in each magasine")] public int magSize;
    [BoxGroup("Reloading")][Tooltip("Ammount of bullets you have period.")] public int ammoCapacity;
    [BoxGroup("Reloading")][Tooltip("Rounds Per Minute")]public float fireRate;
    [BoxGroup("Reloading")][Tooltip("Time to reload in Seconds")]public float reloadTime;

    //effects
    [BoxGroup("Effects")][Tooltip("Spawned in at the point that was shot")] public GameObject bulletImpactPrefab;
}
