using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TooltipPopUp tooltipPopup;
    public string text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
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
        }
        catch
        {

        }
        
    }
}