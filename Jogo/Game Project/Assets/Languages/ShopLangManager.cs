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

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        goldText.text = languageManager.GetText(language, "gui", "text", "gold");
        shopText.text = languageManager.GetText(language, "gui", "text", "shop");
        inventoryBtnText.text = languageManager.GetText(language, "gui", "button", "inventory");
        leaveBtnText.text = languageManager.GetText(language, "gui", "button", "leave");
        closeBtnText.text = languageManager.GetText(language, "gui", "button", "hide");
        chanceShopText.text = languageManager.GetText(language, "gui", "text", "chanceshop");
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
