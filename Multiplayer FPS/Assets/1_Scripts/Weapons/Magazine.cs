using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Magazine : MonoBehaviour
{
    public GameObject bulletHolder;

    public void ToggleBullets(bool full)
    {
        bulletHolder.SetActive(full);
    }
}
