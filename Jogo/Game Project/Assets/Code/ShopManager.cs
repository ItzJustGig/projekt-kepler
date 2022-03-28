using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopItem item1;
    [SerializeField] private ShopItem item2;
    [SerializeField] private ShopItem item3;

    [SerializeField] private GameObject itemOwnPrefab;
    [SerializeField] private Transform itemOwnList;
    [SerializeField] private GameObject inventoryGO;
    [SerializeField] private GameObject tooltip;

    [SerializeField] private Image charcIcon;
    [SerializeField] private Text gold;

    [SerializeField] private EndlessInfo info;
    private ShopLangManager langmanag;

    public List<Character> characters = new List<Character>();
    public List<Character> monsters = new List<Character>();
    public List<Items> items = new List<Items>();
    public List<ItemEncounter> encounters = new List<ItemEncounter>();
    private Items.ShopRarity rarity;

    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;

    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        langmanag = this.gameObject.GetComponent<ShopLangManager>();
        info.Load();
        gold.text = info.gold.ToString();

        if (info.isPlayerChamp)
            charcIcon.sprite = characters[info.playerId-1].charcIcon;
        else
            charcIcon.sprite = monsters[info.playerId].charcIcon;

        if (info.items.Count > 0)
        {
            RefreshOwnItems();
        }

        Items temp = null;
        int i = 0;

        if (info.generateShop == true)
        {
            item1.SetUpCard(GenItem(), tooltip);

            if (item1.itemName != "")
            {
                do
                {
                    temp = GenItem();
                    i++;

                    if (i == 40 || temp == null)
                    {
                        temp = null;
                        i = 0;
                        break;
                    }

                } while (temp.name == item1.itemName);
            }

            item2.SetUpCard(temp, tooltip);
            temp = null;

            if (item1.itemName != "" && item2.itemName != "")
            {
                do
                {
                    temp = GenItem();
                    i++;

                    if (i == 40 || temp == null)
                    {
                        temp = null;
                        break;
                    }
                } while (temp.name == item2.itemName || temp.name == item1.itemName);
            }

            item3.SetUpCard(temp, tooltip);

            info.itemShop.Add(item1.GetItemString());
            info.itemShop.Add(item2.GetItemString());
            info.itemShop.Add(item3.GetItemString());
            info.generateShop = false;
        } else
        {
            i = 0;
            foreach (string a in info.itemShop)
            {
                i++;
                string name = "";
                string price = "0";

                if (a != "")
                {
                    name = a.Substring(0, a.IndexOf(';'));
                    Debug.Log(name);
                    price = a.Substring(a.IndexOf(';') + 1);
                    Debug.Log(price);
                }
                
                int priceI = Convert.ToInt32(price);

                temp = GetItem(name);
                switch (i)
                {
                    case 1:
                        item1.SetUpCard(temp, priceI, tooltip);
                        break;
                    case 2:
                        item2.SetUpCard(temp, priceI, tooltip);
                        break;
                    case 3:
                        item3.SetUpCard(temp, priceI, tooltip);
                        break;
                    default:
                        Debug.LogWarning("Tried to load many items found in itemShop from the endless data file (" + i + ")");
                        break;
                }
            }
        }

        CheckPrice();
    }
    
    private void RefreshOwnItems()
    {
        foreach (Transform child in itemOwnList)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i< info.items.Count; i++)
        {
            if (info.items[i] != "")
            {
                foreach (Items a in items)
                {
                    if (a.name == info.items[i])
                    {
                        itemOwnPrefab.GetComponent<TooltipButton>().tooltipPopup = tooltip.GetComponent<TooltipPopUp>();
                        itemOwnPrefab.GetComponent<TooltipButton>().text = a.GetTooltipText();
                    }
                }

                itemOwnPrefab.name = GetItem(info.items[i]).name;

                Text text = itemOwnPrefab.transform.Find("Text").gameObject.GetComponent<Text>();
                text.text = langmanag.GetInfo("items", "name", GetItem(info.items[i]).name);

                Image icon = itemOwnPrefab.transform.Find("Icon").gameObject.GetComponent<Image>();
                icon.sprite = GetItem(info.items[i]).icon;

                Instantiate(itemOwnPrefab, itemOwnList);
            }
        }
    }

    public void CheckPrice()
    {
        if (info.gold >= item1.price)
            item1.Lock(false);
        else
            item1.Lock(true);

        if (info.gold >= item2.price)
            item2.Lock(false);
        else
            item2.Lock(true);

        if (info.gold >= item3.price)
            item3.Lock(false);
        else
            item3.Lock(true);
    }

    Items GenItem()
    {
        Items.ShopRarity selectedRarity = Items.ShopRarity.COMMON;

        foreach (ItemEncounter enc in encounters)
        {
            if (info.round >= enc.startRound && info.round <= enc.endRound)
            {
                for (int i = 0; i < enc.rarity.Count; i++)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= enc.rarity[i].chance)
                    {
                        selectedRarity = enc.rarity[i].rarity;
                        break;
                    }
                }
            }
        }

        rarity = selectedRarity;

        List<int> num = new List<int>();
        int d = 0;

        foreach (Items item in items)
        {
            if (item.rarity == selectedRarity)
                num.Add(d);

            d++;
        }

        int itemId = num[UnityEngine.Random.Range(0, num.Count)];
        bool conti = false;
        int tries = 0;

        if (info.items.Count > 0)
        {
            do
            {
                foreach (string a in info.items)
                {
                    if (items[itemId].name != a)
                    {
                        conti = true;
                    }
                    else
                    {
                        conti = false;
                        break;
                    }
                }

                if (!conti)
                {
                    tries++;
                    itemId = num[UnityEngine.Random.Range(0, num.Count)];
                }

                if (tries > 100)
                    break;
            } while (!conti);
        }
        else
            conti = true;

        if (conti)
            return GetItem(items[itemId].name);
        else
            return null;
    }

    Items GetItem(string item)
    {
        if (item != "")
            foreach (Items a in items)
            {
                if (a.name == item)
                    return a;
            }
        return null;
    }

    public void Buy(ShopItem shopItem)
    {
        shopItem.OutOfStock();
        info.items.Add(shopItem.itemName);
        info.gold -= shopItem.price;
        info.itemShop.Remove(shopItem.GetItemString());
        info.itemShop.Add("");
        SaveSystem.Save(info);
        gold.text = info.gold.ToString();
        CheckPrice();
        RefreshOwnItems();
    }

    public void BackBtn()
    {
        SaveSystem.Save(info);
        loader.LoadScene(3, slider, loadPanel);
    }

    public void InvBtn()
    {
        inventoryGO.SetActive(true);
    }

    public void InvCloseBtn()
    {
        inventoryGO.SetActive(false);
    }
}
