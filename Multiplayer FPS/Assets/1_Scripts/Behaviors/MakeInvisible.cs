using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MakeInvisible : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();
    [SerializeField] private bool makeInvisibleOnAwake = true;

    void Awake()
    {
        if(makeInvisibleOnAwake)
        {
            SetInvisible();
        }
    }

    [Button]
    void GetRenderers()
    {
        //clear the current list
        renderers.Clear();

        //find all of the renderers
        Renderer[] foundRenderers = GetComponentsInChildren<Renderer>();

        //loop through the found renderers
        foreach (Renderer renderer in foundRenderers)
        {
            //add them to the cached list
            renderers.Add(renderer);
        }
    }

    [Button]
    void SetVisible()
    {
        //no renderers cached
        if (renderers.Count < 1) { GetRenderers(); }

        //no renderers fouhd
        if (renderers.Count < 1) { Debug.LogError($"Couldnt find any renderers!"); return; }

        //loop through all of the renderers
        foreach(Renderer r in renderers)
        {
            //turn each one on
            r.enabled = true;
        }
    }

    [Button]
    void SetInvisible()
    {
        //no renderers cached
        if (renderers.Count < 1) { GetRenderers(); }

        //no renderers fouhd
        if (renderers.Count < 1) { Debug.LogError($"Couldnt find any renderers!"); return; }

        //loop through all of the renderers
        foreach (Renderer r in renderers)
        {
            //turn each one off
            r.enabled = false;
        }
    }
}
