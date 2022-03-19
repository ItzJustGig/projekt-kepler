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
    [SerializeField] private Text cardName;
    [SerializeField] private Text goldText;
    [SerializeField] public string itemName;
    [SerializeField] public int price;

    [SerializeField] private ShopLangManager langmanag;

    public void SetUpCard(Items item)
    {
        if (item != null)
        {
            cardName.text = langmanag.GetInfo("items", item.name);
            itemName = item.name;
            price = Random.Range(item.minPrice, item.maxPrice);
            goldText.text = price + langmanag.GetInfo("gui", "text", "goldinicial");
            itemSprite.sprite = item.icon;

            switch (item.rarity)
            {
                case Items.ShopRarity.COMMON:
                    rarityColour.color = colourCommon;
                    break;
                case Items.ShopRarity.UNCOMMON:
                    rarityColour.color = colourUncommon;
                    break;
                case Items.ShopRarity.RARE:
                    rarityColour.color = colourRare;
                    break;
                case Items.ShopRarity.EPIC:
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
    
    public void SetUpCard(Items item, int price)
    {
        if (item != null)
        {
            cardName.text = langmanag.GetInfo("items", item.name);
            itemName = item.name;
            this.price = price;
            goldText.text = price + langmanag.GetInfo("gui", "text", "goldinicial");
            itemSprite.sprite = item.icon;

            switch (item.rarity)
            {
                case Items.ShopRarity.COMMON:
                    rarityColour.color = colourCommon;
                    break;
                case Items.ShopRarity.UNCOMMON:
                    rarityColour.color = colourUncommon;
                    break;
                case Items.ShopRarity.RARE:
                    rarityColour.color = colourRare;
                    break;
                case Items.ShopRarity.EPIC:
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

    public string GetItemString()
    {
        return itemName + ";" + price;
    }
}
