using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TooltipPopUp tooltipPopup;
    public TooltipPopUp tooltipPopupSec;
    public string text;
    public string textSec;
    public bool wantSec = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            wantSec = true;
        else
            wantSec = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            if (wantSec)
                tooltipPopupSec.DisplayInfo(textSec);
            else
                tooltipPopup.DisplayInfo(text);
        }
        catch
        {

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            tooltipPopup.HideInfo();
            tooltipPopupSec.HideInfo();
        }
        catch
        {

        }
        
    }
}