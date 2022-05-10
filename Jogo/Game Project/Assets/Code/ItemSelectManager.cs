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

    private void Start()
    {
        foreach (Items a in items.returnStuff())
        {
            itemIconPrefab.GetComponent<TooltipButton>().tooltipPopup = tooltip.GetComponent<TooltipPopUp>();
            itemIconPrefab.GetComponent<TooltipButton>().text = a.GetTooltipText();

            itemIconPrefab.name = a.name;

            Image icon = itemIconPrefab.transform.Find("Icon").gameObject.GetComponent<Image>();
            icon.sprite = a.icon;

            Instantiate(itemIconPrefab, grid);
        }
    }
}
