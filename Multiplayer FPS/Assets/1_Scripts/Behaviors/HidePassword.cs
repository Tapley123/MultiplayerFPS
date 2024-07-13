using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HidePassword : MonoBehaviour
{
    #region Variables    
    public TMP_InputField input;
    public Sprite hiddenIcon;
    public Sprite visableIcon;

    public bool hidden = true;
    #endregion

    void Start()
    {
        if (!hidden)
        {
            this.GetComponent<Image>().sprite = visableIcon;
            input.contentType = TMP_InputField.ContentType.Standard;
            hidden = false;
        }
        else
        {
            this.GetComponent<Image>().sprite = hiddenIcon;
            input.contentType = TMP_InputField.ContentType.Password;
            hidden = true;
        }
    }

    public void Button_Hide()
    {
        if (hidden)
        {
            this.GetComponent<Image>().sprite = visableIcon;
            input.contentType = TMP_InputField.ContentType.Standard;
            input.ActivateInputField();
            hidden = false;
        }
        else
        {
            this.GetComponent<Image>().sprite = hiddenIcon;
            input.contentType = TMP_InputField.ContentType.Password;
            input.ActivateInputField();
            hidden = true;
        }
    }
}