using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Text;

public class SummaryHud : MonoBehaviour
{
    [SerializeField] private FightLang langmanag;
    [SerializeField] private TooltipPopUp tooltipMain;
    [SerializeField] private TooltipPopUp tooltipSec;

    [SerializeField] private Text nameText;
    [SerializeField] private Image charcIcon;

    [SerializeField] private Text phyDmgDealt;
    [SerializeField] private Text magicDmgDealt;
    [SerializeField] private Text trueDmgDealt;
    [SerializeField] private Text sanityDmgDealt;
    [SerializeField] private Text totalDmgDealt;

    [SerializeField] private Text phyDmgTaken;
    [SerializeField] private Text magicDmgTaken;
    [SerializeField] private Text trueDmgTaken;
    [SerializeField] private Text sanityDmgTaken;
    [SerializeField] private Text totalDmgTaken;

    [SerializeField] private Text phyDmgMitigated;
    [SerializeField] private Text magicDmgMitigated;
    [SerializeField] private Text totalDmgMitigated;

    [SerializeField] private Text healDone;
    [SerializeField] private Text healManaDone;
    [SerializeField] private Text healStaminaDone;
    [SerializeField] private Text healSanityDone;
    [SerializeField] private Text shieldDone;

    [SerializeField] private Scrollbar scrollBar;

    [SerializeField] private Transform moveLogList;
    [SerializeField] private GameObject moveLogGO;
    [SerializeField] private Scrollbar moveLogScroll;

    void Awake()
    {
        scrollBar.value = 1;
    }

    public void AddMoveLog(Unit user, Moves move)
    {
        moveLogGO.GetComponent<Text>().text = langmanag.GetInfo("gui", "text", "usedmove", langmanag.GetInfo("charc", "name", user.charc.name), langmanag.GetInfo("moves", move.name));
        moveLogGO.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        moveLogGO.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();
        moveLogGO.GetComponent<TooltipButton>().text = move.GetTooltipText(false);
        moveLogGO.GetComponent<TooltipButton>().textSec = move.GetTooltipText(true);
        moveLogGO.GetComponent<TooltipButton>().wantSec = true;
        Instantiate(moveLogGO, moveLogList);
        moveLogScroll.value = 0;
    }

    public void UpdateValues(Unit unit, string name)
    {
        nameText.text = name;
        charcIcon.sprite = unit.charc.charcIcon;

        phyDmgDealt.text = unit.phyDmgDealt.ToString();
        magicDmgDealt.text = unit.magicDmgDealt.ToString();
        trueDmgDealt.text = unit.trueDmgDealt.ToString();
        sanityDmgDealt.text = unit.sanityDmgDealt.ToString();
        totalDmgDealt.text = (unit.phyDmgDealt + unit.magicDmgDealt + unit.trueDmgDealt).ToString();

        phyDmgTaken.text = unit.phyDmgTaken.ToString();
        magicDmgTaken.text = unit.magicDmgTaken.ToString();
        trueDmgTaken.text = unit.trueDmgTaken.ToString();
        sanityDmgTaken.text = unit.sanityDmgTaken.ToString();
        totalDmgTaken.text = (unit.phyDmgTaken + unit.magicDmgTaken + unit.trueDmgTaken).ToString();

        phyDmgMitigated.text = unit.phyDmgMitigated.ToString();
        magicDmgMitigated.text = unit.magicDmgMitigated.ToString();
        totalDmgMitigated.text = (unit.phyDmgMitigated + unit.magicDmgMitigated).ToString();

        healDone.text = unit.healDone.ToString();
        healManaDone.text = unit.manaHealDone.ToString();
        healStaminaDone.text = unit.staminaHealDone.ToString();
        healSanityDone.text = unit.sanityHealDone.ToString();
        shieldDone.text = unit.shieldDone.ToString();
    }

}
