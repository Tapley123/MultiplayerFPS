using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class CachedData : MonoBehaviour
{
    #region Singleton
    private static CachedData _instance;
    public static CachedData Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<CachedData>(); } return _instance; } }
    #endregion

    public int testInt;

    private void Awake()
    {
        //Persist through scenes
        DontDestroyOnLoad(this);

        //Makes sure there is only one instance of this class
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
}
