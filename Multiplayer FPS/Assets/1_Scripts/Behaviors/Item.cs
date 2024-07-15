using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Item : MonoBehaviour
{
    [Expandable] public ItemInfo item;
    public GameObject itemGameobject;
}
