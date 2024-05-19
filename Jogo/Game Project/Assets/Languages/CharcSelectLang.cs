using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static LanguageManager;

public class CharcSelectLang : MonoBehaviour
{
    [SerializeField] public string language;
    public LanguageManager languageManager;

    [SerializeField] private Text moveBtnText;
    [SerializeField] private Text selectBtnText;
    [SerializeField] private Text returnBtnText;
    [SerializeField] private Text hideBtnText;
    [SerializeField] private Text ultimateBtnText;
    [SerializeField] private Text passiveText;
    [SerializeField] private Text movesText;

    [SerializeField] private Text cdText;
    [SerializeField] private Text staText;
    [SerializeField] private Text mnText;

    [SerializeField] private TooltipButton hpTooltip;
    [SerializeField] private TooltipButton hpRegenTooltip;
    [SerializeField] private TooltipButton manaTooltip;
    [SerializeField] private TooltipButton manaRegenTooltip;
    [SerializeField] private TooltipButton staminaTooltip;
    [SerializeField] private TooltipButton staminaRegenTooltip;
    [SerializeField] private TooltipButton sanityTooltip;
    [SerializeField] private TooltipButton defTooltip;
    [SerializeField] private TooltipButton magicdefTooltip;
    [SerializeField] private TooltipButton atkTooltip;
    [SerializeField] private TooltipButton mpTooltip;
    [SerializeField] private TooltipButton critchanceTooltip;
    [SerializeField] private TooltipButton critdmgTooltip;
    [SerializeField] private TooltipButton movspeedTooltip;
    [SerializeField] private TooltipButton timingTooltip;
    [SerializeField] private TooltipButton lifestealTooltip;
    [SerializeField] private TooltipButton evasionTooltip;
    [SerializeField] private TooltipButton accuracyTooltip;

    [SerializeField] private Text itemBtnText;
    [SerializeField] private Text slcItemBtnText;
    [SerializeField] private Text cancelItemBtnText;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        //buttons
        //main menu
        moveBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "moves"));
        selectBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "select"));
        returnBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "back"));
        hideBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "hide"));
        ultimateBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "ultimate"));
        //items
        itemBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "items"));
        slcItemBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "select"));
        cancelItemBtnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "button", "cancel"));

        //text boxes
        passiveText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "passives"));
        movesText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "moves"));
        cdText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "cd"));
        staText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "sta"));
        mnText.text = languageManager.GetText(new ArgumentsFetch(language, "gui", "text", "mn"));

        //stats
        hpTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hp")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "hp"));
        hpRegenTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hpregen")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "hpregen"));
        manaTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "mana")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "mana"));
        manaRegenTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "manaregen")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "manaregen"));
        staminaTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "stamina")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "stamina"));
        staminaRegenTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "staminaregen")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "staminaregen"));
        sanityTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "sanity")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "sanity"));
        defTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "def")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "def"));
        magicdefTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicdef")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "magicdef"));
        atkTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "attack")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "attack"));
        mpTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicpower")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "magicpower"));
        critchanceTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critchance")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "critchance"));
        critdmgTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critdmg")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "critdmg"));
        movspeedTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "movspeed")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "movspeed"));
        timingTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "timing")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "timing"));
        lifestealTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "lifesteal")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "lifesteal"));
        evasionTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "evasion")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "evasion"));
        accuracyTooltip.text = languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "accuracy")) + "<br>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "desc", "accuracy"));
    }
}
