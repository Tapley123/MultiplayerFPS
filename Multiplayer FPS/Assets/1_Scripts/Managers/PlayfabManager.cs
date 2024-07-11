using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [Header("Login/Create")]
    [Foldout("UI")][SerializeField] private TMP_InputField inputField_Email;
    [Foldout("UI")][SerializeField] private TMP_InputField inputField_Password;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
