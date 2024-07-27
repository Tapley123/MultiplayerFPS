using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "GunData", menuName = "Storage/GunData")]
public class GunData : ScriptableObject
{
    //Info
    [Header("Info")]
    [BoxGroup("Info")] public string name;

    //Shooting
    [Header("Shooting")]
    [BoxGroup("Shooting")][Tooltip("Damage dealt per shot landing")] public bool automatic;
    [BoxGroup("Shooting")][Tooltip("Damage dealt per shot landing")] public float damage;
    [BoxGroup("Shooting")][Tooltip("Rounds Per Minute")] public float fireRate;
    [BoxGroup("Shooting")][Tooltip("Maximum Distance the bullets can reach")] public float maxDistance;
    [BoxGroup("Shooting")][Tooltip("The Layers The gun can hit")] public LayerMask canShootLayers;

    //Adsing
    [Header("ADS")]
    [BoxGroup("ADS")][Tooltip("Speed of the ADS transition")] public float adsSpeed = 5f; 
    [BoxGroup("ADS")][Tooltip("Adjust this value to change snapping sensitivity for position")] public float positionSnapTolerance = 0.001f;
    [BoxGroup("ADS")][Tooltip("Adjust this value to change snapping sensitivity for rotation")] public float rotationSnapTolerance = 0.01f;

    //Sway
    [Header("Sway")]
    [BoxGroup("Sway")] public float smooth = 8;
    [BoxGroup("Sway")] public float swayMultiplier = 2;

    //Recoil
    [Header("Recoil")]
    [BoxGroup("Recoil")] public float recoilAmount = 5f;
    [BoxGroup("Recoil")] public float recoilSmooth = 5f;
    [BoxGroup("Recoil")] public float recoilReturnSpeed = 10f;

    //Reloading
    [Header("Reloading")]
    [BoxGroup("Reloading")][Tooltip("Ammount of bullets in each magasine")] public int magSize;
    [BoxGroup("Reloading")][Tooltip("Ammount of bullets you have period.")] public int ammoCapacity;
    [BoxGroup("Reloading")][Tooltip("Time to reload in Seconds")]public float reloadTime;

    //effects
    [Header("Effects")]
    [BoxGroup("Effects")][Tooltip("Spawned in at the point that was shot")] public GameObject bulletImpactPrefab;

    //Audio
    [Header("Audio")]
    [BoxGroup("Audio")][Tooltip("the audio clip that is played when the gun shoots a bullet")] public AudioClip shotSound;
    [BoxGroup("Audio")][Tooltip("the audio clip that is played if you try shoot with no bullets")] public AudioClip emptySound;
    [BoxGroup("Audio")][Tooltip("the audio clip that is played if you try shoot with no bullets")] public AudioClip removeMagSound;
    [BoxGroup("Audio")][Tooltip("the audio clip that is played if you try shoot with no bullets")] public AudioClip insertMagSound;
}
