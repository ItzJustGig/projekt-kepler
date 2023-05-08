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
    [SerializeField] private GameObject basicBtn;
    [SerializeField] private GameObject movesBtn;
    [SerializeField] private GameObject healManaBtn;
    [SerializeField] private GameObject ultBtn;
    [SerializeField] private Transform actionPanel;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private int levelToConsiderWeak;
    [SerializeField] private int manaRecoverCdReducWeak;
    public Unit unit;
    [SerializeField] private BattleSystem battleSystem;

    public void Setup(int levelWeak, int recovCdReduc, Unit unit)
    {
        levelToConsiderWeak = levelWeak;
        manaRecoverCdReducWeak = recovCdReduc;
        this.unit = unit;
        this.unit.moveListHud = moveList;

        basicBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        basicBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

        healManaBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        healManaBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

        ultBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        ultBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();
    }

    public void UpdateTooltips()
    {
        healManaBtn.GetComponent<TooltipButton>().text = unit.recoverMana.GetTooltipText(false);
        healManaBtn.GetComponent<TooltipButton>().textSec = unit.recoverMana.GetTooltipText(true);

        basicBtn.GetComponent<TooltipButton>().text = unit.basicAttack.GetTooltipText(false);
        basicBtn.GetComponent<TooltipButton>().textSec = unit.basicAttack.GetTooltipText(true);

        ultBtn.GetComponent<TooltipButton>().text = unit.ultMove.GetTooltipText(false);
        ultBtn.GetComponent<TooltipButton>().textSec = unit.ultMove.GetTooltipText(true);
    }

    public void EnableTarget()
    {
        switch (unit.chosenMove.move.target)
        {
            case Moves.Target.SELF:
                battleSystem.player.EnableBtn(unit);
                break;
            case Moves.Target.ALLY:
                battleSystem.player.EnableAllBtn();
                battleSystem.player.DisableBtn(unit);
                break;
            case Moves.Target.ENEMY:
                battleSystem.enemy.EnableAllBtn();
                break;
        }
    }

    public void SelectTarget(string target)
    {
        Unit charc;
        switch (target)
        {
            case "P1":
                charc = battleSystem.player.unit1;
                break;
            case "P2":
                charc = battleSystem.player.unit2;
                break;
            case "P3":
                charc = battleSystem.player.unit3;
                break;
            case "E1":
                charc = battleSystem.enemy.unit1;
                break;
            case "E2":
                charc = battleSystem.enemy.unit2;
                break;
            case "E3":
                charc = battleSystem.enemy.unit3;
                break;
            default:
                charc = unit;
                break;
        }

        unit.chosenMove.target = charc;
        unit.turnReady = true;
    }

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

        unit.chosenMove.move = move.ReturnMove();
        EnableTarget();
    }

    public void SetupMoveListBtn()
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
                int inCd = move.inCooldown;
                /*Text cd = moveBtnGO.transform.Find("Cooldown").gameObject.GetComponent<Text>();

                Text sta = moveBtnGO.transform.Find("Stamina").gameObject.GetComponent<Text>();
                Text mn = moveBtnGO.transform.Find("Mana").gameObject.GetComponent<Text>();*/

                //mn.text = ((int)(move.manaCost * unit.SetModifiers().manaCost)).ToString();
                //if (unit.curMana < (move.manaCost * unit.SetModifiers().manaCost))
                //    mn.color = Color.red;
                //else
                //    mn.color = Color.black;

                //sta.text = ((int)(move.staminaCost * unit.SetModifiers().staminaCost)).ToString();
                //if (unit.curStamina < (move.staminaCost * unit.SetModifiers().staminaCost))
                //    sta.color = Color.red;
                //else
                //    sta.color = Color.black;

                //if (inCd <= 0)
                //    cd.text = move.cooldown.ToString();
                //else
                //    cd.text = move.cooldown.ToString() + " (" + inCd.ToString() + ")";

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

        unit.chosenMove.move = unit.recoverMana.ReturnMove();
        EnableTarget();
    }

    public void OnUltBtn()
    {
        unit.ult -= unit.ultMove.ultCost;
        unit.chosenMove.move = unit.ultMove;
        EnableTarget();
    }

    public void OnBasicBtn()
    {
        unit.chosenMove.move = unit.basicAttack.ReturnMove();
        EnableTarget();
    }

    public void CancelBtn()
    {
        movePanel.gameObject.SetActive(false);
    }
}
