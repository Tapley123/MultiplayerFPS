using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomUITriggerEvents : EventTrigger
{
    public UITweaker uiTweaker;
    bool canPress = true;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");

        uiTweaker.Highlight();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");

        uiTweaker.Unhighlight();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");

        uiTweaker.Press();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");

        if (!canPress)
            canPress = true;
    }
}
