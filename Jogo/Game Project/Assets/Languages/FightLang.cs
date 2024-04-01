using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static LanguageManager;

public class FightLang : MonoBehaviour
{
    [SerializeField] public string language;
    public LanguageManager languageManager;

    [SerializeField] private Text statsPBtnText;
    [SerializeField] private Text statsEBtnText;
    [SerializeField] private Text overviewBtnText;
    [SerializeField] private Text sumHideBtnText;
    [SerializeField] private Text pHideBtnText;
    [SerializeField] private Text eHideBtnText;
    [SerializeField] private Text leaveBtnText;
    [SerializeField] private Text chooseMoveText;
    [SerializeField] private Text cancelMoveBtnText;
    [SerializeField] private Text turnsText;
    [SerializeField] private Text ffBtnText;
    [SerializeField] private Text ffConfirmBtnText;
    [SerializeField] private Text cancelConfirmBtnText;
    [SerializeField] private Text leaveTitleText;
    [SerializeField] private Text leaveTextText;

    //stats player
    [SerializeField] private TooltipButton hpTooltipP;
    [SerializeField] private TooltipButton hpRegenTooltipP;
    [SerializeField] private TooltipButton manaTooltipP;
    [SerializeField] private TooltipButton manaRegenTooltipP;
    [SerializeField] private TooltipButton staminaTooltipP;
    [SerializeField] private TooltipButton staminaRegenTooltipP;
    [SerializeField] private TooltipButton sanityTooltipP;
    [SerializeField] private TooltipButton defTooltipP;
    [SerializeField] private TooltipButton magicdefTooltipP;
    [SerializeField] private TooltipButton atkTooltipP;
    [SerializeField] private TooltipButton mpTooltipP;
    [SerializeField] private TooltipButton critchanceTooltipP;
    [SerializeField] private TooltipButton critdmgTooltipP;
    [SerializeField] private TooltipButton movspeedTooltipP;
    [SerializeField] private TooltipButton timingTooltipP;
    [SerializeField] private TooltipButton lifestealTooltipP;
    [SerializeField] private TooltipButton evasionTooltipP;
    [SerializeField] private TooltipButton accuracyTooltipP;
    [SerializeField] private TooltipButton armourpenTooltipP;

    //stats enemy
    [SerializeField] private TooltipButton hpTooltipE;
    [SerializeField] private TooltipButton hpRegenTooltipE;
    [SerializeField] private TooltipButton manaTooltipE;
    [SerializeField] private TooltipButton manaRegenTooltipE;
    [SerializeField] private TooltipButton staminaTooltipE;
    [SerializeField] private TooltipButton staminaRegenTooltipE;
    [SerializeField] private TooltipButton sanityTooltipE;
    [SerializeField] private TooltipButton defTooltipE;
    [SerializeField] private TooltipButton magicdefTooltipE;
    [SerializeField] private TooltipButton atkTooltipE;
    [SerializeField] private TooltipButton mpTooltipE;
    [SerializeField] private TooltipButton critchanceTooltipE;
    [SerializeField] private TooltipButton critdmgTooltipE;
    [SerializeField] private TooltipButton movspeedTooltipE;
    [SerializeField] private TooltipButton timingTooltipE;
    [SerializeField] private TooltipButton lifestealTooltipE;
    [SerializeField] private TooltipButton evasionTooltipE;
    [SerializeField] private TooltipButton accuracyTooltipE;
    [SerializeField] private TooltipButton armourpenTooltipE;

    [SerializeField] private Text overviewText;

    private void Awake()
    {
        language = PlayerPrefs.GetString("language", language);

        //buttons
        //statsPBtnText.text = languageManager.GetText(language, "gui", "button", "stats");
        //statsEBtnText.text = languageManager.GetText(language, "gui", "button", "stats");
        overviewBtnText.text = languageManager.GetText(language, "gui", "button", "overview");
        sumHideBtnText.text = languageManager.GetText(language, "gui", "button", "hide");
        //pHideBtnText.text = languageManager.GetText(language, "gui", "button", "hide");
        //eHideBtnText.text = languageManager.GetText(language, "gui", "button", "hide");
        leaveBtnText.text = languageManager.GetText(language, "gui", "button", "leave");
        ffBtnText.text = languageManager.GetText(language, "gui", "button", "forfeit");
        ffConfirmBtnText.text = languageManager.GetText(language, "gui", "button", "forfeit");
        cancelConfirmBtnText.text = languageManager.GetText(language, "gui", "button", "cancelforfeit");

        //text
        overviewText.text = languageManager.GetText(language, "gui", "text", "overview");
        turnsText.text = languageManager.GetText(language, "gui", "text", "turn");
        turnsText.text = turnsText.text.Replace("%n%", "0");
        leaveTextText.text = languageManager.GetText(language, "gui", "text", "leavetext");
        leaveTitleText.text = languageManager.GetText(language, "gui", "text", "leavetitle");

        //stats player
        /*hpTooltipP.text = languageManager.GetText(language, "stats", "name", "hp") + "<br>" + languageManager.GetText(language, "stats", "desc", "hp");
        hpRegenTooltipP.text = languageManager.GetText(language, "stats", "name", "hpregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "hpregen");
        manaTooltipP.text = languageManager.GetText(language, "stats", "name", "mana") + "<br>" + languageManager.GetText(language, "stats", "desc", "mana");
        manaRegenTooltipP.text = languageManager.GetText(language, "stats", "name", "manaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "manaregen");
        staminaTooltipP.text = languageManager.GetText(language, "stats", "name", "stamina") + "<br>" + languageManager.GetText(language, "stats", "desc", "stamina");
        staminaRegenTooltipP.text = languageManager.GetText(language, "stats", "name", "staminaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "staminaregen");
        sanityTooltipP.text = languageManager.GetText(language, "stats", "name", "sanity") + "<br>" + languageManager.GetText(language, "stats", "desc", "sanity");
        defTooltipP.text = languageManager.GetText(language, "stats", "name", "def") + "<br>" + languageManager.GetText(language, "stats", "desc", "def");
        magicdefTooltipP.text = languageManager.GetText(language, "stats", "name", "magicdef") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicdef");
        atkTooltipP.text = languageManager.GetText(language, "stats", "name", "attack") + "<br>" + languageManager.GetText(language, "stats", "desc", "attack");
        mpTooltipP.text = languageManager.GetText(language, "stats", "name", "magicpower") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicpower");
        critchanceTooltipP.text = languageManager.GetText(language, "stats", "name", "critchance") + "<br>" + languageManager.GetText(language, "stats", "desc", "critchance");
        critdmgTooltipP.text = languageManager.GetText(language, "stats", "name", "critdmg") + "<br>" + languageManager.GetText(language, "stats", "desc", "critdmg");
        movspeedTooltipP.text = languageManager.GetText(language, "stats", "name", "movspeed") + "<br>" + languageManager.GetText(language, "stats", "desc", "movspeed");
        timingTooltipP.text = languageManager.GetText(language, "stats", "name", "timing") + "<br>" + languageManager.GetText(language, "stats", "desc", "timing");
        lifestealTooltipP.text = languageManager.GetText(language, "stats", "name", "lifesteal") + "<br>" + languageManager.GetText(language, "stats", "desc", "lifesteal");
        evasionTooltipP.text = languageManager.GetText(language, "stats", "name", "evasion") + "<br>" + languageManager.GetText(language, "stats", "desc", "evasion");
        accuracyTooltipP.text = languageManager.GetText(language, "stats", "name", "accuracy") + "<br>" + languageManager.GetText(language, "stats", "desc", "accuracy");
        armourpenTooltipP.text = languageManager.GetText(language, "stats", "name", "armourpen") + " | " + languageManager.GetText(language, "stats", "name", "magicpen");
        armourpenTooltipP.text += "<br>" + languageManager.GetText(language, "stats", "desc", "armourpen");

        //stats enemy
        hpTooltipE.text = languageManager.GetText(language, "stats", "name", "hp") + "<br>" + languageManager.GetText(language, "stats", "desc", "hp");
        hpRegenTooltipE.text = languageManager.GetText(language, "stats", "name", "hpregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "hpregen");
        manaTooltipE.text = languageManager.GetText(language, "stats", "name", "mana") + "<br>" + languageManager.GetText(language, "stats", "desc", "mana");
        manaRegenTooltipE.text = languageManager.GetText(language, "stats", "name", "manaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "manaregen");
        staminaTooltipE.text = languageManager.GetText(language, "stats", "name", "stamina") + "<br>" + languageManager.GetText(language, "stats", "desc", "stamina");
        staminaRegenTooltipE.text = languageManager.GetText(language, "stats", "name", "staminaregen") + "<br>" + languageManager.GetText(language, "stats", "desc", "staminaregen");
        sanityTooltipE.text = languageManager.GetText(language, "stats", "name", "sanity") + "<br>" + languageManager.GetText(language, "stats", "desc", "sanity");
        defTooltipE.text = languageManager.GetText(language, "stats", "name", "def") + "<br>" + languageManager.GetText(language, "stats", "desc", "def");
        magicdefTooltipE.text = languageManager.GetText(language, "stats", "name", "magicdef") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicdef");
        atkTooltipE.text = languageManager.GetText(language, "stats", "name", "attack") + "<br>" + languageManager.GetText(language, "stats", "desc", "attack");
        mpTooltipE.text = languageManager.GetText(language, "stats", "name", "magicpower") + "<br>" + languageManager.GetText(language, "stats", "desc", "magicpower");
        critchanceTooltipE.text = languageManager.GetText(language, "stats", "name", "critchance") + "<br>" + languageManager.GetText(language, "stats", "desc", "critchance");
        critdmgTooltipE.text = languageManager.GetText(language, "stats", "name", "critdmg") + "<br>" + languageManager.GetText(language, "stats", "desc", "critdmg");
        movspeedTooltipE.text = languageManager.GetText(language, "stats", "name", "movspeed") + "<br>" + languageManager.GetText(language, "stats", "desc", "movspeed");
        timingTooltipE.text = languageManager.GetText(language, "stats", "name", "timing") + "<br>" + languageManager.GetText(language, "stats", "desc", "timing");
        lifestealTooltipE.text = languageManager.GetText(language, "stats", "name", "lifesteal") + "<br>" + languageManager.GetText(language, "stats", "desc", "lifesteal");
        evasionTooltipE.text = languageManager.GetText(language, "stats", "name", "evasion") + "<br>" + languageManager.GetText(language, "stats", "desc", "evasion");
        accuracyTooltipE.text = languageManager.GetText(language, "stats", "name", "accuracy") + "<br>" + languageManager.GetText(language, "stats", "desc", "accuracy"); 
        armourpenTooltipE.text = languageManager.GetText(language, "stats", "name", "armourpen") + " | " + languageManager.GetText(language, "stats", "name", "magicpen");
        armourpenTooltipE.text += "<br>" + languageManager.GetText(language, "stats", "desc", "armourpen");
        */
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

    public string GetInfo(string arg1, string arg2, string arg3, string thing)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, arg1, arg2, arg3));
        builder.Replace("%p%", thing);

        return builder.ToString();
    }
    
    public string GetInfo(string arg1, string arg2, string arg3, int turns)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, arg1, arg2, arg3));
        builder.Replace("%n%", turns.ToString());

        return builder.ToString();
    }

    public string GetInfo(string arg1, string arg2, string arg3, string name, string move, string target)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, arg1, arg2, arg3));
        builder.Replace("%p%", name);
        builder.Replace("%m%", move);
        builder.Replace("%t%", target);

        return builder.ToString();
    }
}
