using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

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

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        //buttons
        moveBtnText.text = languageManager.GetText(language, "gui", "button", "moves");
        selectBtnText.text = languageManager.GetText(language, "gui", "button", "select");
        returnBtnText.text = languageManager.GetText(language, "gui", "button", "back");
        hideBtnText.text = languageManager.GetText(language, "gui", "button", "hide");
        ultimateBtnText.text = languageManager.GetText(language, "gui", "button", "ultimate");

        //text boxes
        passiveText.text = languageManager.GetText(language, "gui", "text", "passives");
        movesText.text = languageManager.GetText(language, "gui", "text", "moves");
        cdText.text = languageManager.GetText(language, "gui", "text", "cd");
        staText.text = languageManager.GetText(language, "gui", "text", "sta");
        mnText.text = languageManager.GetText(language, "gui", "text", "mn");

        //stats
        hpTooltip.text = languageManager.GetText(language, "stats", "name", "hp") + "<br>" + languageManager.GetText(language, "stats", "desc", "hp");
        hpRegenTooltip.text = languageManager.GetText(language, "stats", "name", "hpregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "hpregen");
        manaTooltip.text = languageManager.GetText(language, "stats", "name", "mana") + "<br>" + languageManager.GetText(language, "stats", "desc", "mana");
        manaRegenTooltip.text = languageManager.GetText(language, "stats", "name", "manaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "manaregen");
        staminaTooltip.text = languageManager.GetText(language, "stats", "name", "stamina") + "<br>" + languageManager.GetText(language, "stats", "desc", "stamina");
        staminaRegenTooltip.text = languageManager.GetText(language, "stats", "name", "staminaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "staminaregen");
        sanityTooltip.text = languageManager.GetText(language, "stats", "name", "sanity") + "<br>" + languageManager.GetText(language, "stats", "desc", "sanity");
        defTooltip.text = languageManager.GetText(language, "stats", "name", "def") + "<br>" + languageManager.GetText(language, "stats", "desc", "def");
        magicdefTooltip.text = languageManager.GetText(language, "stats", "name", "magicdef") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicdef");
        atkTooltip.text = languageManager.GetText(language, "stats", "name", "attack") + "<br>" + languageManager.GetText(language, "stats", "desc", "attack");
        mpTooltip.text = languageManager.GetText(language, "stats", "name", "magicpower") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicpower");
        critchanceTooltip.text = languageManager.GetText(language, "stats", "name", "critchance") + "<br>" + languageManager.GetText(language, "stats", "desc", "critchance");
        critdmgTooltip.text = languageManager.GetText(language, "stats", "name", "critdmg") + "<br>" + languageManager.GetText(language, "stats", "desc", "critdmg");
        movspeedTooltip.text = languageManager.GetText(language, "stats", "name", "movspeed") + "<br>" + languageManager.GetText(language, "stats", "desc", "movspeed");
        timingTooltip.text = languageManager.GetText(language, "stats", "name", "timing") + "<br>" + languageManager.GetText(language, "stats", "desc", "timing");
        lifestealTooltip.text = languageManager.GetText(language, "stats", "name", "lifesteal") + "<br>" + languageManager.GetText(language, "stats", "desc", "lifesteal");
        evasionTooltip.text = languageManager.GetText(language, "stats", "name", "evasion") + "<br>" + languageManager.GetText(language, "stats", "desc", "evasion");
        accuracyTooltip.text = languageManager.GetText(language, "stats", "name", "accuracy") + "<br>" + languageManager.GetText(language, "stats", "desc", "accuracy");
    }
}
