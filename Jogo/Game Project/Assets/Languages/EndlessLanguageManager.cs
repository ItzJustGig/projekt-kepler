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
    [SerializeField] private Text dropTitle;
    [SerializeField] private Text confirmDeleteTitle;
    [SerializeField] private Text confirmDeleteText;

    [SerializeField] private Text okBtn;
    [SerializeField] private Text confirmBtn;
    [SerializeField] private Text cancelBtn;
    [SerializeField] private TooltipButton passBtn;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        goldText.text = GetInfo("gui", "text", "gold");
        roundText.text = GetInfo("gui", "text", "round");
        leaveBtnText.text = GetInfo("gui", "button", "leave");
        startBtnText.text = GetInfo("gui", "button", "start");
        delSaveBtnText.text = GetInfo("gui", "button", "delsave");
        shopBtnText.text = GetInfo("gui", "button", "shop");
        levelText.text = GetInfo("gui", "text", "level") + this.gameObject.GetComponent<EndlessManager>().GetLevel();
        dropTitle.text = GetInfo("gui", "text", "loottitle");
        confirmDeleteTitle.text = GetInfo("gui", "text", "confirmdeletetitle");
        confirmDeleteText.text = GetInfo("gui", "text", "confirmdeletetext");

        okBtn.text = GetInfo("gui", "button", "ok");
        confirmBtn.text = GetInfo("gui", "button", "confirm");
        cancelBtn.text = GetInfo("gui", "button", "cancel");
        passBtn.text = GetInfo("gui", "button", "pass");
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
