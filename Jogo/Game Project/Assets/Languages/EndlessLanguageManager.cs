using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;
using System;
using static LanguageManager;

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

        goldText.text = GetInfo(new ArgumentsFetch("gui", "text", "gold"));
        roundText.text = GetInfo(new ArgumentsFetch("gui", "text", "round"));
        leaveBtnText.text = GetInfo(new ArgumentsFetch("gui", "button", "leave"));
        startBtnText.text = GetInfo(new ArgumentsFetch("gui", "button", "start"));
        delSaveBtnText.text = GetInfo(new ArgumentsFetch("gui", "button", "delsave"));
        shopBtnText.text = GetInfo(new ArgumentsFetch("gui", "button", "shop"));
        levelText.text = GetInfo(new ArgumentsFetch("gui", "text", "level")) + this.gameObject.GetComponent<EndlessManager>().GetLevel();
        dropTitle.text = GetInfo(new ArgumentsFetch("gui", "text", "loottitle"));
        confirmDeleteTitle.text = GetInfo(new ArgumentsFetch("gui", "text", "confirmdeletetitle"));
        confirmDeleteText.text = GetInfo(new ArgumentsFetch("gui", "text", "confirmdeletetext"));

        okBtn.text = GetInfo(new ArgumentsFetch("gui", "button", "ok"));
        confirmBtn.text = GetInfo(new ArgumentsFetch("gui", "button", "confirm"));
        cancelBtn.text = GetInfo(new ArgumentsFetch("gui", "button", "cancel"));
        passBtn.text = GetInfo(new ArgumentsFetch("gui", "button", "pass"));
    }

    public string GetInfo(ArgumentsFetch fetch)
    {
        StringBuilder builder = new StringBuilder();
        fetch.langId = language;
        builder.Append(languageManager.GetText(fetch));

        return builder.ToString();
    }
}
