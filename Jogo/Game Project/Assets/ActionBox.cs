using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static LanguageManager;

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
        this.unit.actionBoxPanel = actionPanel;

        if (!unit.isEnemy)
        {
            this.unit.moveListPanel = moveList;

            basicBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
            basicBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

            healManaBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
            healManaBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

            ultBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
            ultBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();
        }
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

    public void ManageActions()
    {
        Unit temp = battleSystem.player.GetAttacker();

        if (temp == null)
        {
            battleSystem.player.SetAttacker(SelectCharacter());
            battleSystem.DoneSelecting();
        } else if (temp.chosenMove.target == null)
        {
            temp.chosenMove.target = unit;
            //FIX LATER
            temp.summonTarget.target = unit;
            temp.summonTarget.ally = temp;
            battleSystem.DoneChoosingMove();
        }
    }

    public Unit SelectCharacter()
    {
        return unit;
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

    public void OnMoveBtn(int moveId)
    {
        Moves move = null;
        int i = 0;

        foreach (Moves a in unit.moves)
        {
            i++;
            if (i == moveId)
                move = a;
        }

        unit.chosenMove.move = move;
        EnableTarget();
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        battleSystem.HideMoveHud();
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
                GameObject moveBtnGO = moveList.GetChild(0).Find(name).gameObject;
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

        SetupSpecialMoveBtn(1);
        SetupSpecialMoveBtn(2);

        movePanel.gameObject.SetActive(true);
        scrollbar.value = 1;
    }

    public void SetupSpecialMoveBtn(int id)
    {
        Moves move = unit.basicAttack;
        Button moveBtn = basicBtn.GetComponent<Button>();
        switch (id)
        {
            case 1:
                move = unit.recoverMana;
                moveBtn = healManaBtn.GetComponent<Button>();
            break;
            case 2:
                move = unit.ultMove;
                moveBtn = ultBtn.GetComponent<Button>();
            break;
        }

        int inCd = move.inCooldown;
            
        bool canUse = true;

        if (move.isUlt && unit.ult < move.ultCost)
        {
            canUse = false;
        }

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

    public void OnHealBtn()
    {
        battleSystem.HideMoveHud();
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        unit.actionBoxPanel.gameObject.SetActive(false);
        unit.recoverMana.inCooldown = unit.recoverMana.cooldown;
        if (unit.level <= levelToConsiderWeak)
            unit.recoverMana.inCooldown -= manaRecoverCdReducWeak;

        unit.chosenMove.move = unit.recoverMana;
        EnableTarget();
    }

    public void OnUltBtn()
    {
        battleSystem.HideMoveHud();
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        unit.actionBoxPanel.gameObject.SetActive(false);
        unit.ult -= unit.ultMove.ultCost;
        unit.chosenMove.move = unit.ultMove;
        EnableTarget();
    }

    public void OnBasicBtn()
    {
        battleSystem.HideMoveHud();
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        unit.actionBoxPanel.gameObject.SetActive(false);
        unit.chosenMove.move = unit.basicAttack;
        EnableTarget();
    }

    public void CancelBtn()
    {
        SetupSpecialMoveBtn(1);
        SetupSpecialMoveBtn(2);
        movePanel.gameObject.SetActive(false);
    }
}
