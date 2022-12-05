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
    bool isShowing = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            wantSec = true;
        else
            wantSec = false;

        if (isShowing)
            ShowTooltip();

        if ((tooltipPopup && !tooltipPopup.gameObject.activeInHierarchy) && (tooltipPopupSec && !tooltipPopupSec.gameObject.activeInHierarchy))
            ResetIsShowing();
    }

    void ShowTooltip()
    {
        try
        {
            tooltipPopup.HideInfo();
            if (tooltipPopupSec)
                tooltipPopupSec.HideInfo();

            if (wantSec && tooltipPopupSec)
                tooltipPopupSec.DisplayInfo(textSec, this);
            else
                tooltipPopup.DisplayInfo(text, this);
        }
        catch
        {

        }
    }

    public void ResetIsShowing()
    {
        isShowing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            ShowTooltip();
            isShowing = true;
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

        isShowing = false;
    }
}