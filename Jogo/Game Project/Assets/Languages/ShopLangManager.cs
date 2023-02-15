using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;

public class ShopLangManager : MonoBehaviour
{
    [SerializeField] public string language;
    public LanguageManager languageManager;

    [SerializeField] private Text goldText;
    [SerializeField] private Text shopText;
    [SerializeField] private Text inventoryBtnText;
    [SerializeField] private Text leaveBtnText;
    [SerializeField] private Text closeBtnText;
    [SerializeField] private Text chanceShopText;
    [SerializeField] private TooltipButton commonTt;
    [SerializeField] private TooltipButton uncommonTt;
    [SerializeField] private TooltipButton rareTt;
    [SerializeField] private TooltipButton epicTt;
    [SerializeField] private TooltipButton coupon;
    [SerializeField] private TooltipButton reroll;
    [SerializeField] private TooltipButton rest;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        goldText.text = GetInfo("gui", "text", "gold");
        shopText.text = GetInfo("gui", "text", "shop");
        inventoryBtnText.text = GetInfo("gui", "button", "inventory");
        leaveBtnText.text = GetInfo("gui", "button", "leave");
        closeBtnText.text = GetInfo("gui", "button", "hide");
        chanceShopText.text = GetInfo("gui", "text", "chanceshop");

        commonTt.text = GetInfoRarity("items", "rarity", "common", 0);
        uncommonTt.text = GetInfoRarity("items", "rarity", "uncommon", 1);
        rareTt.text = GetInfoRarity("items", "rarity", "rare", 2);
        epicTt.text = GetInfoRarity("items", "rarity", "epic", 3);

        coupon.text = GetInfo("gui", "button", "coupon");
        coupon.text = coupon.text.Replace("%off%", (this.gameObject.GetComponent<ShopManager>().itemDiscount*100).ToString());
        reroll.text = GetInfo("gui", "button", "reroll");
        rest.text = GetInfo("gui", "button", "rest");
        rest.text = rest.text.Replace("%val1%", (this.gameObject.GetComponent<ShopManager>().hpRecover*100).ToString());
        rest.text = rest.text.Replace("%val2%", (this.gameObject.GetComponent<ShopManager>().costsRecover*100).ToString());
        rest.text = rest.text.Replace("%val3%", (this.gameObject.GetComponent<ShopManager>().sanityRecover * 100).ToString());
        rest.text = rest.text.Replace("%gold%", this.gameObject.GetComponent<ShopManager>().restPrice.ToString());
    }

    public string GetInfoRarity(string arg1, string arg2, string arg3, int rarity)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=25><align=center>").Append(GetInfo(arg1, arg2, arg3)).Append("</align></size>").AppendLine().AppendLine();
        ShopManager manag = this.gameObject.GetComponent<ShopManager>();
        builder.Append("<size=19><align=center><color=#B2B2B2>").Append(manag.GetRarityGold(rarity) + GetInfo("gui", "text", "goldinicial")).Append("</color></align></size>").AppendLine();

        return builder.ToString();
    }

    public string GetInfo(string arg1, string arg2)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, arg1, arg2));

        return builder.ToString();
    }

    public string GetInfo(string arg1, string arg2, string arg3)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, arg1, arg2, arg3));

        return builder.ToString();
    }
}
