using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtnPrefabFuncs : MonoBehaviour
{
    ItemSelectManager manager;

    public void Start()
    {
        manager = FindObjectOfType<ItemSelectManager>();

        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate { manager.SelectItem(this.gameObject.name, this.gameObject.GetComponent<Button>()); });
    }
}
