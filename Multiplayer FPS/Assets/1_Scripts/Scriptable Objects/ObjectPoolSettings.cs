using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolSettings", menuName = "Storage/ObjectPoolSettings")]
public class ObjectPoolSettings : ScriptableObject
{
    public GameObject enemyImpactPrefab;
    public GameObject bulletTrailPrefab;
}
