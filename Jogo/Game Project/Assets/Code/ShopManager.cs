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
    [SerializeField] private Toggle discountToggle;

    [SerializeField] private Image charcIcon;
    [SerializeField] private Text gold;
    
    [SerializeField] private Text CommonTxt;
    [SerializeField] private Text UncommonTxt;
    [SerializeField] private Text RareTxt;
    [SerializeField] private Text EpicTxt;

    [SerializeField] private EndlessInfo info;
    private ShopLangManager langmanag;

    [SerializeField] private StuffList champions;
    [SerializeField] private StuffList monsters;
    [SerializeField] private StuffList items;
    [SerializeField] private StuffList shopEncounters;

    private Items.ShopRarity rarity;

    [SerializeField] Button rerollBtn;
    [SerializeField] Button restBtn;
    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;

    [SerializeField] public float itemDiscount = .2f;
    [SerializeField] int rerolls = 2;
    [SerializeField] public int restPrice = 15;
    [SerializeField] float nonCombatPriceIncrease;
    [SerializeField] float nonCombatChanceDecrease;
    [SerializeField] int commonMin;
    [SerializeField] int commonMax;
    [SerializeField] int uncommonMin;
    [SerializeField] int uncommonMax;
    [SerializeField] int rareMin;
    [SerializeField] int rareMax;
    [SerializeField] int epicMin;
    [SerializeField] int epicMax;
    [SerializeField] int legendaryMin;
    [SerializeField] int legendaryMax;

    [SerializeField] public float hpRecover;
    [SerializeField] public float costsRecover;
    [SerializeField] public float sanityRecover;


    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        langmanag = this.gameObject.GetComponent<ShopLangManager>();
        info.Load();
        gold.text = info.gold.ToString();

        if (info.shopcoupon <= 0)
            discountToggle.interactable = false;

        List<Character> champs = new List<Character>();
        foreach (Character t in champions.returnStuff())
        {
            champs.Add(t.GetCharcInfo());
        }

        List<Character> mons = new List<Character>();
        foreach (Character t in monsters.returnStuff())
        {
            mons.Add(t.GetCharcInfo());
        }

        if (info.player.isChamp)
            charcIcon.sprite = champs[info.player.id-1].charcIcon;
        else
            charcIcon.sprite = mons[info.player.id].charcIcon;

        if (info.items.Count > 0)
        {
            RefreshOwnItems();
        }

        if (info.generateShop)
            info.shoprerolls = rerolls;

        if (info.shoprerolls <= 0)
            rerollBtn.interactable = false;

        GenerateItems();
        GetNextChance();
    }

    public string GetRarityGold(int i)
    {
        switch (i)
        {
            case 0:
                return commonMin + "-" + commonMax;
            case 1:
                return uncommonMin + "-" + uncommonMax;
            case 2:
                return rareMin + "-" + rareMax;
            case 3:
                return epicMin + "-" + epicMax;
            default:
                return "NULL";
        }
    }

    private void SetItem(ShopItem shopItem, Items genItem)
    {
        int min = 0;
        int max = 0;
        if (genItem != null)
        {
            switch (genItem.rarity)
            {
                case Items.ShopRarity.COMMON:
                case Items.ShopRarity.COMMONPLUS:
                    min = commonMin;
                    max = commonMax;
                    break;
                case Items.ShopRarity.UNCOMMON:
                case Items.ShopRarity.UNCOMMONPLUS:
                    min = uncommonMin;
                    max = uncommonMax;
                    break;
                case Items.ShopRarity.RARE:
                case Items.ShopRarity.RAREPLUS:
                    min = rareMin;
                    max = rareMax;
                    break;
                case Items.ShopRarity.EPIC:
                case Items.ShopRarity.EPICPLUS:
                    min = epicMin;
                    max = epicMax;
                    break;
                case Items.ShopRarity.LEGENDARY:
                    min = legendaryMin;
                    max = legendaryMax;
                    break;
            }
        }

        if (shopItem.nonCombatItem)
        {
            min += (int)(min * nonCombatPriceIncrease);
            max += (int)(max * nonCombatPriceIncrease);
        }

        if (discountToggle.isOn)
            shopItem.SetUpCard(genItem, tooltip, min, max, itemDiscount);
        else
            shopItem.SetUpCard(genItem, tooltip, min, max, 0);
    }

    public void ToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            item1.CheckDiscount(itemDiscount);
            item2.CheckDiscount(itemDiscount);
            item3.CheckDiscount(itemDiscount);
        } else
        {
            item1.CheckDiscount(0);
            item2.CheckDiscount(0);
            item3.CheckDiscount(0);
        }
        CheckPrice();
    }

    private void RefreshOwnItems()
    {
        foreach (Transform child in itemOwnList)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i< info.items.Count; i++)
        {
            if (info.items[i] != "")
            {
                foreach (Items a in items.returnStuff())
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

        if (info.gold >= restPrice && !info.hasRested && !info.wasPassUsed)
            restBtn.interactable = true;
        else
            restBtn.interactable = false;
    }

    void GetNextChance()
    {
        float common = 0;
        float uncommon = 0;
        float rare = 0;
        float epic = 0;

        foreach (ItemEncounter enc in shopEncounters.returnStuff())
        {
            if (info.round+1 >= enc.startRound && info.round+1 <= enc.endRound)
            {
                foreach (var rarity in enc.rarity)
                {
                    switch (rarity.rarity)
                    {
                        case Items.ShopRarity.COMMON:
                            common += rarity.chance;
                            break;
                        case Items.ShopRarity.UNCOMMON:
                            uncommon += rarity.chance;
                            break;
                        case Items.ShopRarity.RARE:
                            rare += rarity.chance;
                            break;
                        case Items.ShopRarity.EPIC:
                            epic += rarity.chance;
                            break;
                    }
                }
            }
        }
        CommonTxt.text = common.ToString("0%");
        UncommonTxt.text = uncommon.ToString("0%");
        RareTxt.text = rare.ToString("0%");
        EpicTxt.text = epic.ToString("0%");
    }

    void GenerateItems()
    {
        Items temp = null;
        int i = 0;

        if (info.generateShop == true)
        {
            SetItem(item1, GenItem());

            if (item1.itemName != "")
            {
                do
                {
                    temp = GenItem();
                    if (temp != null)
                        temp = temp.returnItem();

                    i++;

                    if (i == 40 || temp == null)
                    {
                        temp = null;
                        i = 0;
                        break;
                    }

                } while (temp.name == item1.itemName);
            }

            SetItem(item2, temp);
            temp = null;

            if (item1.itemName != "" && item2.itemName != "")
            {
                do
                {
                    temp = GenItem();
                    if (temp != null)
                        temp = temp.returnItem();
                    i++;

                    if (i == 40 || temp == null)
                    {
                        temp = null;
                        break;
                    }
                } while (temp.name == item2.itemName || temp.name == item1.itemName);
            }

            SetItem(item3, temp);

            info.itemShop.Add(item1.GetItemString(false));
            info.itemShop.Add(item2.GetItemString(false));
            info.itemShop.Add(item3.GetItemString(false));
            info.generateShop = false;
            info.hasRested = false;
        }
        else
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
                        Debug.LogWarning("Tried to load too many items from the data file (" + i + " items)");
                        break;
                }
            }
        }
        CheckPrice();
    }

    Items GenItem()
    {
        Items.ShopRarity selectedRarity = Items.ShopRarity.NONE;

        foreach (ItemEncounter enc in shopEncounters.returnStuff())
        {
            if (info.round >= enc.startRound && info.round <= enc.endRound)
            {
                while (selectedRarity == Items.ShopRarity.NONE)
                {
                    float rng = UnityEngine.Random.Range(0f, 1f);
                    float counter = 0;
                    for (int i = 0; i < enc.rarity.Count; i++)
                    {
                        counter += enc.rarity[i].chance;
                        if (rng <= counter)
                        {
                            selectedRarity = enc.rarity[i].rarity;
                            break;
                        }
                    }
                }
            }
        }

        rarity = selectedRarity;

        List<int> num = new List<int>();
        int d = 0;

        List<Items> ite = new List<Items>();
        foreach (Items item in items.returnStuff())
        {
            ite.Add(item.returnItem());

            switch (selectedRarity)
            {
                case Items.ShopRarity.COMMON:
                    if (item.rarity is Items.ShopRarity.COMMON || item.rarity is Items.ShopRarity.COMMONPLUS)
                        num.Add(d);
                    break;
                case Items.ShopRarity.UNCOMMON:
                    if (item.rarity is Items.ShopRarity.UNCOMMON || item.rarity is Items.ShopRarity.UNCOMMONPLUS)
                        num.Add(d);
                    else if (item.rarity is Items.ShopRarity.COMMONPLUS)
                        num.Add(d);
                    break;
                case Items.ShopRarity.RARE:
                    if (item.rarity is Items.ShopRarity.RARE || item.rarity is Items.ShopRarity.RAREPLUS)
                        num.Add(d);
                    else if (item.rarity is Items.ShopRarity.COMMONPLUS || item.rarity is Items.ShopRarity.UNCOMMONPLUS)
                        num.Add(d);
                    break;
                case Items.ShopRarity.EPIC:
                    if (item.rarity is Items.ShopRarity.EPIC || item.rarity is Items.ShopRarity.EPICPLUS)
                        num.Add(d);
                    else if (item.rarity is Items.ShopRarity.COMMONPLUS || item.rarity is Items.ShopRarity.UNCOMMONPLUS || item.rarity is Items.ShopRarity.RAREPLUS)
                        num.Add(d);
                    break;
                default:
                    if (item.rarity == selectedRarity)
                        num.Add(d);
                    break;
            }

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
                    if (ite[itemId].name != a)
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
            return GetItem(ite[itemId].name).returnItem();
        else
            return null;
    }

    public void Reroll()
    {
        info.shoprerolls--;
        info.generateShop = true;
        info.itemShop.Clear();
        info.Save();
        item1.BackInStock();
        item2.BackInStock();
        item3.BackInStock();
        GenerateItems();
        if (info.shoprerolls <= 0)
            rerollBtn.interactable = false;
    }

    Items GetItem(string item)
    {
        if (item != "")
            foreach (Items a in items.returnStuff())
            {
                if (a.name == item)
                    return a.returnItem();
            }
        return null;
    }

    public void Rest()
    {
        info.player.hp += hpRecover;
        if (info.player.hp > 1)
            info.player.hp = 1;

        info.player.mn += costsRecover;
        if (info.player.mn > 1)
            info.player.mn = 1;

        info.player.sta += costsRecover;
        if (info.player.sta > 1)
            info.player.sta = 1;

        info.player.san += sanityRecover;
        if (info.player.san > 1)
            info.player.san = 1;

        info.gold -= restPrice;
        info.hasRested = true;
        restBtn.interactable = false;
        CheckPrice();
        gold.text = info.gold.ToString();

        info.Save();
    }

    public void Buy(ShopItem shopItem)
    {
        shopItem.OutOfStock();
        info.gold -= shopItem.price;
        info.itemShop.Remove(shopItem.GetItemString(discountToggle.isOn));
        info.itemShop.Add("");

        if (discountToggle.isOn)
        {
            if (info.shopcoupon > 0)
            {
                info.shopcoupon--;
                if (info.shopcoupon <= 0)
                    discountToggle.interactable = false;
            } 
            discountToggle.isOn = false;
        }

        if (!shopItem.nonCombatItem)
            info.items.Add(shopItem.itemName);
        else
        {
            switch (shopItem.itemName)
            {
                case "xpflask":
                    info.player.level += 1;
                    break;
                case "xpbottle":
                    info.player.level += 2;
                    break;
                case "xppot":
                    info.player.level += 3;
                    break;
                case "shoppass":
                    info.shoppass += 1;
                    discountToggle.interactable = true;
                    break;
                case "shopcoupon":
                    info.shopcoupon += 1;
                    break;
            }
        }
        info.Save();
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
