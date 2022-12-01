using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipPopUp : MonoBehaviour
{
    public GameObject popupCanvasObj;
    public RectTransform popupObj;
    public TextMeshProUGUI infoText;
    private TooltipButton lastBtn;
    public Vector3 offset;
    public float padding;
    public bool isMain = true;

    private Canvas popupCanvas;

    private void Awake()
    {
        popupCanvas = popupCanvasObj.GetComponent<Canvas>();
    }

    private void Update()
    {
        FollowCursor();
    }

    private void FollowCursor()
    {
        if (!popupCanvasObj.activeSelf) { return; }

        Vector3 newpos = Input.mousePosition + offset;
        newpos.z = 0f;

        float rightEdgeToScreenDistance = Screen.width - (newpos.x + popupObj.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenDistance < 0)
        {
            newpos.x += rightEdgeToScreenDistance;
        }
        float leftEdgeToScreenDistance = 0 - (newpos.x - popupObj.rect.width * popupCanvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenDistance > 0)
        {
            newpos.x += leftEdgeToScreenDistance;
        }
        float topEdgeToScreenDistance = Screen.height - (newpos.y + popupObj.rect.height * popupCanvas.scaleFactor / 2) - padding;
        if (topEdgeToScreenDistance < 0)
        {
            newpos.y += topEdgeToScreenDistance;
        }
        float botEdgeToScreenDistance = 0 - (newpos.y - popupObj.rect.height * popupCanvas.scaleFactor / 2) + padding;
        if (botEdgeToScreenDistance > 0)
        {
            newpos.y += botEdgeToScreenDistance;
        }

        popupObj.transform.position = newpos;
    }

    public void DisplayInfo(string text, TooltipButton btn)
    {
        lastBtn = btn;
        infoText.text = text;
        popupCanvasObj.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObj);
    }

    public void ForceHideInfo()
    {
        popupCanvasObj.SetActive(false);
        if (lastBtn)
            lastBtn.ResetIsShowing();
    }

    public void HideInfo()
    {
        popupCanvasObj.SetActive(false);
    }
}
