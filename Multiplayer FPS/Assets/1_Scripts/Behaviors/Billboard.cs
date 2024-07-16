using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    void Update()
    {
        //if there is no camera look for one
        if (cam == null) { cam = FindObjectOfType<Camera>(); }

        //if you didnt find one then stop looking
        if (cam == null) { return; }

        //look at the active camera
        transform.LookAt(cam.transform);
    }
}
