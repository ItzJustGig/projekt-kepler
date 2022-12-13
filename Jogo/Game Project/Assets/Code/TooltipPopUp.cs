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
    [SerializeField] private TooltipButton lastBtn;
    [SerializeField] private TooltipPopUp otherTooltip;
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
        ResetLastBtn();

        lastBtn = btn;
        infoText.text = text;
        popupCanvasObj.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObj);
    }

    public void ResetLastBtn()
    {
        if (lastBtn)
        {
            if (otherTooltip) {
                if (!otherTooltip.gameObject.activeInHierarchy)
                {
                    lastBtn.ResetIsShowing();
                }
            } else
            {
                lastBtn.ResetIsShowing();
            }
        }
    }

    public void ForceResetLastBtn()
    {
        if (lastBtn)
        {
            lastBtn.ResetIsShowing();
            lastBtn = null;
        }
    }

    public void HideInfo()
    {
        popupCanvasObj.SetActive(false);
    }
}
