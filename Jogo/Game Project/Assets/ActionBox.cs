using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static LanguageManager;

public class ActionBox : MonoBehaviour
{
    [SerializeField] private FightLang fightLang;
    [SerializeField] private TooltipPopUp tooltipMain;
    [SerializeField] private TooltipPopUp tooltipSec;
    [SerializeField] private Transform moveList;
    [SerializeField] private Transform movePanel;
    [SerializeField] private GameObject basicBtn;
    [SerializeField] private Text basicBtnText;
    [SerializeField] private GameObject movesBtn;
    [SerializeField] private Text movesBtnText;
    [SerializeField] private GameObject healManaBtn;
    [SerializeField] private Text healManaBtnText;
    [SerializeField] private GameObject ultBtn;
    [SerializeField] private Text ultBtnText;
    [SerializeField] private Text cancelBtnText;
    [SerializeField] private Text chooseMoveText;
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
            movesBtnText.text = fightLang.GetInfo(new ArgumentsFetch("gui", "button", "moves"));
            basicBtnText.text = fightLang.GetInfo(new ArgumentsFetch("moves", "basicattack"));
            ultBtnText.text = fightLang.GetInfo(new ArgumentsFetch("gui", "button", "ultimate"));
            healManaBtnText.text = fightLang.GetInfo(new ArgumentsFetch("moves", "recovmana"));
            cancelBtnText.text = fightLang.GetInfo(new ArgumentsFetch("gui", "button", "cancel"));
            chooseMoveText.text = fightLang.GetInfo(new ArgumentsFetch("gui", "text", "choosemove"));

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
        healManaBtn.GetComponent<TooltipButton>().text = unit.recoverMana.GetTooltipText(false, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);
        healManaBtn.GetComponent<TooltipButton>().textSec = unit.recoverMana.GetTooltipText(true, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);

        basicBtn.GetComponent<TooltipButton>().text = unit.basicAttack.GetTooltipText(false, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);
        basicBtn.GetComponent<TooltipButton>().textSec = unit.basicAttack.GetTooltipText(true, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);

        ultBtn.GetComponent<TooltipButton>().text = unit.ultMove.GetTooltipText(false, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);
        ultBtn.GetComponent<TooltipButton>().textSec = unit.ultMove.GetTooltipText(true, unit.SetModifiers().manaCost, unit.SetModifiers().staminaCost);
    }

    public void ManageActions()
    {
        Unit temp = battleSystem.player.GetAttacker();

        if (temp == null)
        {
            battleSystem.player.SetAttacker(SelectCharacter());
            unit.SetAnimHud("isSelected", true);
            battleSystem.DoneSelecting();
        } else if (temp.chosenMove.target == null)
        {
            temp.chosenMove.target = unit;
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
            case Moves.Target.ALLYSELF:
                battleSystem.player.EnableAllBtn();
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
        unit.SetCC();
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

                int unitNum = 1;

                if (battleSystem.player.unit2 && unit == battleSystem.player.unit2)
                    unitNum = 2;
                else if (battleSystem.player.unit3 && unit == battleSystem.player.unit3)
                    unitNum = 3;

                if (move.target is Moves.Target.ALLY)
                {
                    switch (unitNum)
                    {
                        case 1:
                            if (battleSystem.player.unit2 && battleSystem.player.unit3)
                            {
                                if (battleSystem.player.unit2.isDead && battleSystem.player.unit3.isDead)
                                    canUse = false;
                            }
                            else if(!battleSystem.player.unit2 && !battleSystem.player.unit3)
                                canUse = false;

                            break;
                        case 2:
                            if (battleSystem.player.unit1 && battleSystem.player.unit3)
                            {
                                if (battleSystem.player.unit1.isDead && battleSystem.player.unit3.isDead)
                                    canUse = false;
                            }
                            else if(!battleSystem.player.unit1 && !battleSystem.player.unit3)
                                canUse = false;
                                
                            break;
                        case 3:
                            if (battleSystem.player.unit1 && battleSystem.player.unit2)
                            {
                                if (battleSystem.player.unit1.isDead && battleSystem.player.unit2.isDead)
                                    canUse = false;
                            }
                            else if(!battleSystem.player.unit1 && !battleSystem.player.unit2) 
                                canUse = false;
                               
                            break;
                    }
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

        if (move.isUlt && (unit.ult < move.ultCost || (unit.ult < 100 && move.needFullUlt)))
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
