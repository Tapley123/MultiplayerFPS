using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Rotator : MonoBehaviour
{
    //X
    [BoxGroup("X")][SerializeField] private bool rotateX = false;
    [BoxGroup("X")][ShowIf("rotateX")] [SerializeField] private float xSpeed = 10f;

    //Y
    [BoxGroup("Y")][SerializeField] private bool rotateY = false;
    [BoxGroup("Y")][ShowIf("rotateY")][SerializeField] private float ySpeed = 10f;

    //Z
    [BoxGroup("Z")][SerializeField] private bool rotateZ = true;
    [BoxGroup("Z")][ShowIf("rotateZ")][SerializeField] private float zSpeed = 10f;

    void Update()
    {
        //if none are true dont do anthing
        if (!rotateX && !rotateY && !rotateZ) { return; }

        // Create a vector for the rotation
        Vector3 rotation = Vector3.zero;

        // Add rotation speed for each enabled axis
        if (rotateX) rotation.x = xSpeed * Time.deltaTime;
        if (rotateY) rotation.y = ySpeed * Time.deltaTime;
        if (rotateZ) rotation.z = zSpeed * Time.deltaTime;

        // Apply the rotation
        transform.Rotate(rotation);
    }
}
