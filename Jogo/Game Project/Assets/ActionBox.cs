using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBox : MonoBehaviour
{
    [SerializeField] private TooltipPopUp tooltipMain;
    [SerializeField] private TooltipPopUp tooltipSec;
    [SerializeField] private Transform moveList;
    [SerializeField] private Transform movePanel;
    [SerializeField] private Transform actionPanel;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private int levelToConsiderWeak;
    [SerializeField] private int manaRecoverCdReducWeak;
    public Unit unit;

   public void OnMoveBtn(int moveId)
    {
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();

        Moves move = null;
        int i = 0;

        foreach (Moves a in unit.moves)
        {
            i++;
            if (i == moveId)
                move = a;
        }

        StartCoroutine(Combat(move));
    }

    public void OnAtkBtn()
    {
        int i = 0;
        foreach (Moves move in unit.moves)
        {
            if (move.name != "basicattack")
            {
                string name = move.name + "(Clone)";
                i++;
                GameObject moveBtnGO = moveList.Find(name).gameObject;
                Button moveBtn = moveBtnGO.GetComponent<Button>();

                Text cd = moveBtnGO.transform.Find("Cooldown").gameObject.GetComponent<Text>();
                int inCd = move.inCooldown;

                Text sta = moveBtnGO.transform.Find("Stamina").gameObject.GetComponent<Text>();
                Text mn = moveBtnGO.transform.Find("Mana").gameObject.GetComponent<Text>();

                mn.text = ((int)(move.manaCost * unit.SetModifiers().manaCost)).ToString();
                if (unit.curMana < (move.manaCost * unit.SetModifiers().manaCost))
                    mn.color = Color.red;
                else
                    mn.color = Color.black;

                sta.text = ((int)(move.staminaCost * unit.SetModifiers().staminaCost)).ToString();
                if (unit.curStamina < (move.staminaCost * unit.SetModifiers().staminaCost))
                    sta.color = Color.red;
                else
                    sta.color = Color.black;

                if (inCd <= 0)
                    cd.text = move.cooldown.ToString();
                else
                    cd.text = move.cooldown.ToString() + " (" + inCd.ToString() + ")";

                bool canUse = true;

                switch (move.type)
                {
                    case Moves.MoveType.PHYSICAL:
                        if (!unit.canUsePhysical)
                            canUse = false;
                        break;
                    case Moves.MoveType.MAGICAL:
                        if (!unit.canUseMagic)
                            canUse = false;
                        break;
                    case Moves.MoveType.RANGED:
                        if (!unit.canUseRanged)
                            canUse = false;
                        break;
                    case Moves.MoveType.DEFFENCIVE:
                        if (!unit.canUseProtec)
                            canUse = false;
                        break;
                    case Moves.MoveType.SUPPORT:
                        if (!unit.canUseSupp)
                            canUse = false;
                        break;
                    case Moves.MoveType.ENCHANT:
                        if (!unit.canUseEnchant)
                            canUse = false;
                        break;
                    case Moves.MoveType.SUMMON:
                        if (!unit.canUseSummon)
                            canUse = false;
                        break;
                }

                if (unit.curMana < (move.manaCost * unit.SetModifiers().manaCost)
                    || unit.curStamina < (move.staminaCost * unit.SetModifiers().staminaCost) || inCd > 0)
                    canUse = false;

                if (canUse)
                    moveBtn.interactable = true;
                else
                    moveBtn.interactable = false;
            }
        }

        movePanel.gameObject.SetActive(true);
        scrollbar.value = 1;
    }

    public void OnHealBtn()
    {
        unit.recoverMana.inCooldown = unit.recoverMana.cooldown;
        if (unit.level <= levelToConsiderWeak)
            unit.recoverMana.inCooldown -= manaRecoverCdReducWeak;
        StartCoroutine(Combat(unit.recoverMana.ReturnMove()));
    }

    public void OnUltBtn()
    {
        StartCoroutine(Combat(unit.ultMove));
        unit.ult -= unit.ultMove.ultCost;
    }

    public void OnBasicBtn()
    {
        StartCoroutine(Combat(unit.basicAttack.ReturnMove()));
    }

    public void CancelBtn()
    {
        movePanel.gameObject.SetActive(false);
    }
}
