using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public abstract class Item : MonoBehaviour
{
    [Expandable] public ItemInfo itemInfo;
    public GameObject itemGameobject;

    public abstract void Use();
}
