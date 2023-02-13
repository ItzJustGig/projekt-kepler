using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private GameObject card;
    [SerializeField] private Button btnBuy;
    [SerializeField] private Color colourCommon;
    [SerializeField] private Color colourUncommon;
    [SerializeField] private Color colourRare;
    [SerializeField] private Color colourEpic;
    [SerializeField] private Color colourLegendary;
    [SerializeField] private Color colourChampionItem;

    [SerializeField] private Image rarityColour;
    [SerializeField] private Image itemSprite;
    [SerializeField] private GameObject discountGO;
    [SerializeField] private Text cardName;
    [SerializeField] private Text goldText;
    [SerializeField] private Text discountGoldText;
    [SerializeField] public string itemName;
    [SerializeField] public int price;
    [SerializeField] public int priceTemp;
    [SerializeField] public bool nonCombatItem;

    [SerializeField] private ShopLangManager langmanag;

    public void SetUpCard(Items item, GameObject tooltip, int minPrice, int maxPrice, float discount = 0)
    {
        if (item != null)
        {
            item = item.returnItem();
            cardName.text = langmanag.GetInfo("items", "name", item.name);
            Debug.Log(item.name);
            itemName = item.name;
            price = Random.Range(minPrice, maxPrice);
            priceTemp = 0;
            goldText.text = price + langmanag.GetInfo("gui", "text", "goldinicial");
            itemSprite.sprite = item.icon;

            nonCombatItem = item.NonCombatItem;

            btnBuy.gameObject.GetComponent<TooltipButton>().tooltipPopup = tooltip.GetComponent<TooltipPopUp>();
            btnBuy.gameObject.GetComponent<TooltipButton>().text = item.GetTooltipText();

            switch (item.rarity)
            {
                case Items.ShopRarity.COMMON:
                case Items.ShopRarity.COMMONPLUS:
                    rarityColour.color = colourCommon;
                    break;
                case Items.ShopRarity.UNCOMMON:
                case Items.ShopRarity.UNCOMMONPLUS:
                    rarityColour.color = colourUncommon;
                    break;
                case Items.ShopRarity.RARE:
                case Items.ShopRarity.RAREPLUS:
                    rarityColour.color = colourRare;
                    break;
                case Items.ShopRarity.EPIC:
                case Items.ShopRarity.EPICPLUS:
                    rarityColour.color = colourEpic;
                    break;
                case Items.ShopRarity.LEGENDARY:
                    rarityColour.color = colourLegendary;
                    break;
                case Items.ShopRarity.CHAMPION:
                    rarityColour.color = colourChampionItem;
                    break;
            }
        }
        else
        {
            OutOfStock();
        }
    }
    
    public void SetUpCard(Items item, int price, GameObject tooltip)
    {
        if (item != null)
        {
            cardName.text = langmanag.GetInfo("items", "name", item.name);
            itemName = item.name;
            this.price = price;
            goldText.text = price + langmanag.GetInfo("gui", "text", "goldinicial");
            itemSprite.sprite = item.icon;

            btnBuy.gameObject.GetComponent<TooltipButton>().tooltipPopup = tooltip.GetComponent<TooltipPopUp>();
            btnBuy.gameObject.GetComponent<TooltipButton>().text = item.GetTooltipText();

            switch (item.rarity)
            {
                case Items.ShopRarity.COMMON:
                case Items.ShopRarity.COMMONPLUS:
                    rarityColour.color = colourCommon;
                    break;
                case Items.ShopRarity.UNCOMMON:
                case Items.ShopRarity.UNCOMMONPLUS:
                    rarityColour.color = colourUncommon;
                    break;
                case Items.ShopRarity.RARE:
                case Items.ShopRarity.RAREPLUS:
                    rarityColour.color = colourRare;
                    break;
                case Items.ShopRarity.EPIC:
                case Items.ShopRarity.EPICPLUS:
                    rarityColour.color = colourEpic;
                    break;
                case Items.ShopRarity.LEGENDARY:
                    rarityColour.color = colourLegendary;
                    break;
                case Items.ShopRarity.CHAMPION:
                    rarityColour.color = colourChampionItem;
                    break;
            }
        } else
        {
            OutOfStock();
        }
        
    }

    public void CheckDiscount(float discount)
    {
        if (discount > 0)
        {
            if (priceTemp == 0)
            {
                priceTemp = price;
                price = price - (int)(price * discount);
            } else
            {
                int temp = price;
                price = priceTemp;
                priceTemp = temp;
            }
            
            discountGO.SetActive(true);
            discountGoldText.text = price + langmanag.GetInfo("gui", "text", "goldinicial") + "!";
        } else
        {
            int temp = price;
            price = priceTemp;
            priceTemp = temp;
            
            discountGO.SetActive(false);
        }

    }

    public void Lock(bool locks)
    {
        if (locks)
            btnBuy.interactable = false;
        else
            btnBuy.interactable = true;
    }

    public void OutOfStock()
    {
        btnBuy.gameObject.SetActive(false);
        card.SetActive(false);
    }

    public void BackInStock()
    {
        btnBuy.gameObject.SetActive(true);
        card.SetActive(true);
    }

    public string GetItemString(bool isDiscountOn)
    {
        if (isDiscountOn)
            return itemName + ";" + priceTemp;
        else
            return itemName + ";" + price;
    }
}
