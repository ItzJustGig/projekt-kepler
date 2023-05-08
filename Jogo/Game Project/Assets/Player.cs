using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static LanguageManager;

public class Player : MonoBehaviour
{
    [SerializeField] public Unit unit1;
    [SerializeField] public Unit unit2;
    [SerializeField] public Unit unit3;
    [SerializeField] public Button unit1Btn;
    [SerializeField] public Button unit2Btn;
    [SerializeField] public Button unit3Btn;

    private float aiManaRecover;
    private float aiGaranteedManaRecover;
    [SerializeField] private bool isEnemy;

    public void SetUpStats(Unit unit, EndlessInfo info, float tired)
    {
        if (PlayerPrefs.GetInt("isEndless") == 1 && !unit.isEnemy)
        {
            unit.curHp = unit.charc.stats.hp * info.playerHp;
            unit.curMana = unit.charc.stats.mana * info.playerMn;
            unit.curStamina = unit.charc.stats.stamina * info.playerSta;
            unit.curSanity = (int)(unit.charc.stats.sanity * info.playerSan);
            unit.ult = info.playerUlt;
        }
        else
        {
            unit.curHp = unit.charc.stats.hp;
            unit.curMana = unit.charc.stats.mana;
            unit.curStamina = unit.charc.stats.stamina;
            unit.curSanity = unit.charc.stats.sanity;
        }

        unit.hud.SetHud(unit, (int)(unit.curStamina * tired));
        unit.curShield = 0;

        unit.statMods.Clear();

        unit.effects.Clear();

        unit.randomItems.Clear();

        //setup preset player items (testing porpuse)
        unit.LoadItems();
    }

    public void SetStats(Unit unit, int turnCount, float tiredStart, float tiredGrowth, int tiredStacks, BattleState state, EndlessInfo info = null)
    {
        Stats stats = unit.SetModifiers();

        if (turnCount == 1 && state is BattleState.START)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1)
            {
                unit.curHp = stats.hp * info.playerHp;
                unit.curMana = stats.mana * info.playerMn;
                unit.curStamina = stats.stamina * info.playerSta;
            }
            else
            {
                unit.curHp = stats.hp;
                unit.curMana = stats.mana;
                unit.curStamina = stats.stamina;
            }
        }

        StartCoroutine(unit.hud.SetHp(unit.curHp, stats.hp, unit.SetModifiers().healBonus * 100, unit.curShield));
        StartCoroutine(unit.hud.SetMana(unit.curMana, stats.mana, unit.SetModifiers().manaCost * 100));
        StartCoroutine(unit.hud.SetStamina(unit.curStamina, stats.stamina, (int)(stats.stamina * (tiredStart + (tiredGrowth * tiredStacks))), unit.SetModifiers().staminaCost * 100));
        StartCoroutine(unit.hud.SetShield(unit.curShield, unit.SetModifiers().shieldBonus * 100));
        unit.hud.SetUlt(unit.ult, unit.SetModifiers().ultrate);
        unit.hud.SetBlood(unit.bloodStacks);
    }

    public void SetStart(float aiManaRecover, float aiGaranteedManaRecover, ActionBox moveList1, ActionBox moveList2, ActionBox moveList3, BattleHud hud1,  BattleHud hud2 = null, BattleHud hud3 = null)
    {
        this.aiManaRecover = aiManaRecover;
        this.aiGaranteedManaRecover = aiGaranteedManaRecover;
        unit1.hud = hud1;
        unit2.hud = hud2;
        unit3.hud = hud3;
        unit1.id = 1;
        unit2.id = 2;
        unit3.id = 3;
        unit1.selectBtn = unit1Btn;
        unit2.selectBtn = unit2Btn;
        unit3.selectBtn = unit3Btn;
    }

    public void EnableBtn(Unit unit1 = null, Unit unit2 = null, Unit unit3 = null)
    {
        if (unit1 != null)
        {
            unit1.selectBtn.enabled = true;
        }

        if (unit2 != null)
        {
            unit2.selectBtn.enabled = true;
        }
        
        if (unit3 != null)
        {
            unit3.selectBtn.enabled = true;
        }
    }
    
    public void EnableAllBtn()
    {
        EnableBtn(unit1, unit2, unit3);
    }

    public void DisableBtn(Unit unit1 = null, Unit unit2 = null, Unit unit3 = null)
    {
        if (unit1 != null)
        {
            unit1.selectBtn.enabled = false;
        }

        if (unit2 != null)
        {
            unit2.selectBtn.enabled = false;
        }

        if (unit3 != null)
        {
            unit3.selectBtn.enabled = false;
        }
    }
    
    public void DisableAllBtn()
    {
        DisableBtn(unit1, unit2, unit3);
    }

    public void UpdateStats()
    {
        /*unit1.hud.SetStats(unit1.charc.stats, unit1.charc.stats, unit1.curSanity);
        unit2.hud.SetStats(unit2.charc.stats, unit2.charc.stats, unit2.curSanity);
        unit3.hud.SetStats(unit3.charc.stats, unit3.charc.stats, unit3.curSanity);*/
    }

    public Unit GetLeader()
    {
        if (unit1.isLeader)
            return unit1;
        else if (unit2.isLeader)
            return unit2;
        else return unit3;
    }

    public bool CheckLose()
    {
        if (unit1.isDead && unit2.isDead && unit3.isDead)
            return true;

        return false;
    }

    public Moves AIChooseMove(Unit user, Player target)
    {
        Moves move = null;
        bool skip = false;

        if (user.curMana < (user.SetModifiers().mana * aiManaRecover) && !user.moves.Contains(user.recoverMana))
            user.moves.Add(user.recoverMana);
        else if (user.curMana > (user.SetModifiers().mana * aiManaRecover) && user.moves.Contains(user.recoverMana))
            user.moves.Remove(user.recoverMana);

        if (((user.ult == 100 && user.ultMove.needFullUlt) ||
        (user.ult == user.ultMove.ultCost && !user.ultMove.needFullUlt))
        && !user.moves.Contains(user.ultMove))
            user.moves.Add(user.ultMove);
        else if (((user.ult < 100 && user.ultMove.needFullUlt) ||
        (user.ult < user.ultMove.ultCost && !user.ultMove.needFullUlt))
        && user.moves.Contains(user.ultMove))
            user.moves.Remove(user.ultMove);

        int random = 0;
        int i = 0;

        if (!user.canUseMagic && !user.canUsePhysical && !user.canUseRanged && !user.canUseEnchant && !user.canUseSupp
            && !user.canUseProtec && !user.canUseSummon)
            skip = true;

        do
        {
            if (user.curMana <= (user.SetModifiers().mana * aiGaranteedManaRecover))
            {
                if (user.canUseSupp && user.moves.Contains(user.recoverMana) && user.recoverMana.inCooldown == 0)
                    random = user.moves.Count - 1;
                else
                {
                    Stats statsU = user.SetModifiers();
                    Stats statsT = target.unit1.SetModifiers();

                    random = user.charc.ai.chooseMove(user.moves, user, target.unit1, statsU, statsT);
                }
            }
            else
            {
                Stats statsU = user.SetModifiers();
                Stats statsT = target.unit1.SetModifiers();

                random = user.charc.ai.chooseMove(user.moves, target.unit1, user, statsU, statsT);
            }

            foreach (Moves a in user.moves)
            {
                if (!skip)
                {
                    if (i == random)
                        if (user.curMana >= a.manaCost && user.curStamina >= a.staminaCost)
                        {
                            int inCd = a.inCooldown;

                            if (inCd <= 0)
                            {
                                bool canUse = true;

                                if (!a.isUlt)
                                {
                                    switch (a.type)
                                    {
                                        case Moves.MoveType.PHYSICAL:
                                            if (!user.canUsePhysical)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.MAGICAL:
                                            if (!user.canUseMagic)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.RANGED:
                                            if (!user.canUseRanged)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.DEFFENCIVE:
                                            if (!user.canUseProtec)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.SUPPORT:
                                            if (!user.canUseSupp)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.ENCHANT:
                                            if (!user.canUseEnchant)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.SUMMON:
                                            if (!user.canUseSummon)
                                                canUse = false;
                                            break;
                                    }
                                }
                                else
                                {
                                    user.ult -= user.ultMove.ultCost;
                                }

                                if (canUse)
                                    move = a;
                            }
                            else
                                move = null;
                        }
                        else
                        {
                            move = null;
                        }
                }
                else
                    move = a;

                i++;
            }
            i = 0;
            StartCoroutine(WaitWhile());
        } while (move == null);

        if (skip)
            return null;
        else
            return move;
    }

    IEnumerator WaitWhile()
    {
        yield return null;
    }

    public void PickItemAi(Unit unit, int playerItemCount = 0)
    {
        int itemCount = unit1.items.Count + unit2.items.Count + unit3.items.Count;
        if (PlayerPrefs.GetInt("isEndless") == 0 && playerItemCount > 0 && itemCount > 0)
        {
            List<int> picked = new List<int>();

            for (int i = 0; playerItemCount > i; i++)
            {
                bool isPicked = false;
                do
                {
                    isPicked = false;
                    int num = UnityEngine.Random.Range(0, itemCount);
                    if (picked.Count > 0)
                    {
                        foreach (int a in picked)
                        {
                            if (num == a)
                                isPicked = true;
                        }
                    }

                    if (!isPicked)
                        picked.Add(num);

                } while (isPicked);
            }
            if (picked.Count > 0)
                unit.randomItems = picked;
        }
    }

    public bool HaveLost()
    {
        return !(unit1.isDead || unit2.isDead || unit3.isDead);
    }

    public void Victory()
    {
        unit1.WonLost(true);
        unit2.WonLost(true);
        unit3.WonLost(true);
    }
}
