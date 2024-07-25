using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static Action shootInput;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            shootInput?.Invoke();
        }
    }
}
