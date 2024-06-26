using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;
using static LanguageManager;

public class SummaryHud : MonoBehaviour
{
    [SerializeField] private FightLang langmanag;
    [SerializeField] private TooltipPopUp tooltipMain;
    [SerializeField] private TooltipPopUp tooltipSec;

    [SerializeField] private Transform moveLogList;
    [SerializeField] private GameObject moveLogGO;
    [SerializeField] private Scrollbar moveLogScroll;

    [SerializeField] private Text physicDmgDealt;
    [SerializeField] private Text magicDmgDealt;
    [SerializeField] private Text trueDmgDealt;
    [SerializeField] private Text sanityDmgDealt;
    [SerializeField] private Text totalDmgDealt;
    [SerializeField] private Text physicDmgTaken;
    [SerializeField] private Text magicDmgTaken;
    [SerializeField] private Text trueDmgTaken;
    [SerializeField] private Text sanityDmgTaken;
    [SerializeField] private Text totalDmgTaken;
    [SerializeField] private Text physicDmgMitigated;
    [SerializeField] private Text magicDmgMitigated;
    [SerializeField] private Text totalDmgMitigated;
    [SerializeField] private Text healDone;
    [SerializeField] private Text manaRecovered;
    [SerializeField] private Text staminaRecovered;
    [SerializeField] private Text sanityRecovered;
    [SerializeField] private Text shieldDone;

    public void SetupSum()
    {
        physicDmgDealt.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "physicdmgdealt"));
        magicDmgDealt.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "magicdmgdealt"));
        trueDmgDealt.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "truedmgdealt"));
        sanityDmgDealt.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "sanitydmgdealt"));
        totalDmgDealt.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "totaldmgdealt"));
        physicDmgTaken.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "physicdmgtaken"));
        magicDmgTaken.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "magicdmgtaken"));
        trueDmgTaken.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "truedmgtaken"));
        sanityDmgTaken.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "sanitydmgtaken"));
        totalDmgTaken.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "totaldmgtaken"));
        physicDmgMitigated.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "physicdmgmiti"));
        magicDmgMitigated.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "magicdmgmiti"));
        totalDmgMitigated.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "totaldmgmiti"));
        healDone.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "healdone"));
        manaRecovered.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "manarecov"));
        staminaRecovered.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "staminarecov"));
        sanityRecovered.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "sanityrecov"));
        shieldDone.text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "shielddone"));
    }

    public void AddMoveLog(Unit user, Moves move)
    {
        string txtenemy = "";
        if (user.isEnemy)
            txtenemy = langmanag.GetInfo(new ArgumentsFetch("showdetail", "target", "enemy"));
        else
            txtenemy = langmanag.GetInfo(new ArgumentsFetch("showdetail", "target", "ally"));
        moveLogGO.GetComponent<Text>().text = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "usedmove"), langmanag.GetInfo(new ArgumentsFetch("charc", "name", user.charc.name)), langmanag.GetInfo(new ArgumentsFetch("moves", move.name)), txtenemy);
        moveLogGO.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        moveLogGO.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();
        moveLogGO.GetComponent<TooltipButton>().text = move.GetTooltipText(false);
        moveLogGO.GetComponent<TooltipButton>().textSec = move.GetTooltipText(true);
        moveLogGO.GetComponent<TooltipButton>().wantSec = true;
        Instantiate(moveLogGO, moveLogList);
        moveLogScroll.value = 0;
    }
}
