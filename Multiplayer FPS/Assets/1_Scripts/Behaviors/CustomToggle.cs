using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    public Toggle toggle;

    public Image image;
    public Sprite image_On;
    public Sprite image_Off;

    void Start()
    {
        ToggleImage();
    }

    public void ToggleImage()
    {
        //On
        if (toggle.isOn)
        {
            //set the toggles image
            image.sprite = image_On;
        }
        //Off
        else
        {
            //set the toggles image
            image.sprite = image_Off;
        }
    }
}
