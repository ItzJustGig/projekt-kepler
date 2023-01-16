using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;

public class EndlessLanguageManager : MonoBehaviour
{
    [SerializeField] public string language;
    public LanguageManager languageManager;

    [SerializeField] private Text goldText;
    [SerializeField] private Text roundText;
    [SerializeField] private Text leaveBtnText;
    [SerializeField] private Text startBtnText;
    [SerializeField] private Text delSaveBtnText;
    [SerializeField] private Text shopBtnText;
    [SerializeField] private Text levelText;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        goldText.text = languageManager.GetText(language, "gui", "text", "gold");
        roundText.text = languageManager.GetText(language, "gui", "text", "round");
        leaveBtnText.text = languageManager.GetText(language, "gui", "button", "leave");
        startBtnText.text = languageManager.GetText(language, "gui", "button", "start");
        delSaveBtnText.text = languageManager.GetText(language, "gui", "button", "delsave");
        shopBtnText.text = languageManager.GetText(language, "gui", "button", "shop");
        levelText.text = languageManager.GetText(language, "gui", "text", "level") + this.gameObject.GetComponent<EndlessManager>().GetLevel();
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
