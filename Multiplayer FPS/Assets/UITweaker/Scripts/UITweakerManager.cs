using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweakerManager : MonoBehaviour
{
    #region Singleton
    private static UITweakerManager _instance;
    public static UITweakerManager Instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<UITweakerManager>(); } return _instance; } }
    #endregion

    public AudioSource audioSource;
}
