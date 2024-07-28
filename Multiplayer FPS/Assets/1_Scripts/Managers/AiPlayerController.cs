using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayerController : MonoBehaviour, IDamageable
{
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        Debug.LogError($"Dealt {damage} to {this.transform.name}");
    }
}
