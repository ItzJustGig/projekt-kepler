using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectManager : MonoBehaviour
{
    [SerializeField] private StuffList items;
    [SerializeField] private GameObject itemIconPrefab;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Transform grid;
    [SerializeField] private GameObject item1, item2;
    [SerializeField] private GameObject panel;
    [SerializeField] private SelectManager mainManager;

    private void Start()
    {
        foreach (Items a in items.returnStuff())
        {
            GameObject itemIcon = Instantiate(itemIconPrefab, grid);

            itemIcon.GetComponent<TooltipButton>().tooltipPopup = tooltip.GetComponent<TooltipPopUp>();
            itemIcon.GetComponent<TooltipButton>().text = a.GetTooltipText();
            itemIcon.name = a.name;

            Image icon = itemIcon.transform.Find("Icon").gameObject.GetComponent<Image>();
            icon.sprite = a.icon;
            //itemIcon.GetComponent<ItemBtnPrefabFuncs>().Create();
        }
    }

    public void SelectItem(string slc, Button btn)
    {
        Debug.Log("I CLICK");
        Button btnItem1 = item1.GetComponent<Button>();
        Button btnItem2 = item2.GetComponent<Button>();

        if (!btnItem1.interactable || !btnItem2.interactable)
        {
            foreach(Items a in items.returnStuff())
            {
                if (a.name == slc)
                {
                    Debug.Log("I EXIST");
                    Image imgItem1 = item1.transform.Find("Icon").GetComponent<Image>();
                    Image imgItem2 = item2.transform.Find("Icon").GetComponent<Image>();

                    if (!btnItem1.interactable)
                    {
                        imgItem1.sprite = a.icon;
                        btnItem1.interactable = true;
                        btn.interactable = false;
                        Debug.Log("I DO ITEM 1");
                        btnItem1.gameObject.name = a.name;
                        btnItem1.onClick.RemoveAllListeners();
                        btnItem1.onClick.AddListener(delegate { DeselectItem(btnItem1, btnItem2); });
                    }
                    else if (!btnItem2.interactable)
                    {
                        imgItem2.sprite = a.icon;
                        btnItem2.interactable = true;
                        btn.interactable = false;
                        Debug.Log("I DO ITEM 2");
                        btnItem2.gameObject.name = a.name;
                        btnItem2.onClick.RemoveAllListeners();
                        btnItem2.onClick.AddListener(delegate { DeselectItem(btnItem2); });
                    }
                }
            }
        }
    }

    public void DeselectItem(Button btnSelected, Button btnSec)
    {
        try
        {
            Button btnItem1 = grid.Find(btnSelected.gameObject.name).GetComponent<Button>();
            btnItem1.interactable = true;
            
            if (btnSec.interactable)
            {
                Button btnItem2 = grid.Find(btnSec.gameObject.name).GetComponent<Button>();

                Image imgItemSelec = btnSelected.gameObject.transform.Find("Icon").GetComponent<Image>();
                Image imgItemSec = btnSec.gameObject.transform.Find("Icon").GetComponent<Image>();

                imgItemSelec.sprite = btnItem2.gameObject.transform.Find("Icon").GetComponent<Image>().sprite;
                imgItemSec.sprite = null;

                btnSelected.interactable = true;
                btnSelected.gameObject.name = btnSec.gameObject.name;

                btnSec.interactable = false;
                btnSec.gameObject.name = "Item";
            }
            else
            {
                DeselectItem(btnSelected);
            }
        }
        catch
        {
        };
    }

    public void DeselectItem(Button btnSelected)
    {
        try
        {
            Button btnItem = grid.Find(btnSelected.gameObject.name).GetComponent<Button>();
            btnItem.interactable = true;
            Image imgItem = btnSelected.gameObject.transform.Find("Icon").GetComponent<Image>();
            imgItem.sprite = null;
            btnSelected.interactable = false;
            btnSelected.gameObject.name = "Item";
        }
        catch
        {
        };
    }

    public void SelectItems()
    {
        mainManager.DisablePortraits();
        panel.SetActive(false);
    }

    public void CancelItems()
    {
        DeselectItem(item1.GetComponent<Button>());
        DeselectItem(item2.GetComponent<Button>());
        mainManager.DisablePortraits();
        panel.SetActive(false);
    }

    public void ShowPanel()
    {
        mainManager.DisablePortraits();
        panel.SetActive(true);
    }
}
