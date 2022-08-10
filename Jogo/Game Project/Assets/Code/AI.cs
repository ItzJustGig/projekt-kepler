using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ai", menuName = "Ai/Ai", order = 3)]
public class AI : ScriptableObject
{
    [SerializeField] private AiType aiType;
    [SerializeField] private List<Effects> preferedEffects;
    [SerializeField] private List<string> preferedStats;

    float SetScale(StatScale scale, Stats stats, Unit user)
    {
        float temp = 0;

        temp += (user.curHp * scale.curHp);
        temp += ((stats.hp - user.curHp) * scale.missHp);
        temp += (stats.hp * scale.maxHp);
        temp += (stats.hpRegen * scale.hpRegen);

        temp += (user.curMana * scale.curMana);
        temp += ((stats.mana - user.curMana) * scale.missMana);
        temp += (stats.mana * scale.maxMana);
        temp += (stats.manaRegen * scale.manaRegen);

        temp += (user.curStamina * scale.curStamina);
        temp += ((stats.stamina - user.curStamina) * scale.missStamina);
        temp += (stats.stamina * scale.maxStamina);
        temp += (stats.staminaRegen * scale.staminaRegen);

        temp += (user.curSanity * scale.curSanity);
        temp += (stats.sanity - user.curSanity) * scale.missSanity;
        temp += (stats.sanity * scale.maxSanity);

        temp += (stats.atkDmg * scale.atkDmg);
        temp += (stats.magicPower * scale.magicPower);

        temp += (stats.dmgResis * scale.dmgResis);
        temp += (stats.magicResis * scale.magicResis);

        temp += (stats.timing * scale.timing);
        temp += (stats.movSpeed * scale.movSpeed);

        return temp;
    }


    BattleSystem.DMG SetScaleDmg(StatScale scale, Stats stats, Unit unit)
    {
        BattleSystem.DMG dmg;

        dmg.phyDmg = 0;
        dmg.magicDmg = 0;
        dmg.trueDmg = 0;
        dmg.sanityDmg = 0;
        dmg.heal = 0;
        dmg.healMana = 0;
        dmg.healStamina = 0;
        dmg.healSanity = 0;
        dmg.shield = 0;

        switch (scale.type)
        {
            case StatScale.DmgType.PHYSICAL:
                dmg.phyDmg += scale.flatValue;
                dmg.phyDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.MAGICAL:
                dmg.magicDmg += scale.flatValue;
                dmg.magicDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.TRUE:
                dmg.trueDmg += scale.flatValue;
                dmg.trueDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.SANITY:
                dmg.sanityDmg += scale.flatValue;
                dmg.sanityDmg += (int)SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEAL:
                dmg.heal += scale.flatValue;
                dmg.heal += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALMANA:
                dmg.healMana += scale.flatValue;
                dmg.healMana += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALSTAMINA:
                dmg.healStamina += scale.flatValue;
                dmg.healStamina += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALSANITY:
                dmg.healSanity += scale.flatValue;
                dmg.healSanity += (int)SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.SHIELD:
                dmg.shield += scale.flatValue;
                dmg.shield += (int)SetScale(scale, stats, unit);
                break;
        }

        return dmg;
    }

    public int chooseMove(List<Moves> moves, Unit user, Unit target, Stats statsU, Stats statsT)
    {
        AiType ai = this.aiType;
        List<float> chances = new List<float>();
        foreach (Moves a in moves)
        {
            float chance = 0;
            if (a.type == Moves.MoveType.ULT)
                chance += 250;
            
            if (a.inCooldown <= 0)
            {
                BattleSystem.DMG it = default;
                it.AddBaseDmgHeal(a);
                foreach (StatScale scale in a.scale)
                {
                    Unit unit;
                    Stats stats;
                    if (scale.playerStat)
                    {
                        unit = user;
                        stats = statsU;
                    }
                    else
                    {
                        unit = target;
                        stats = statsT;
                    }

                    it.AddDmg(SetScaleDmg(scale, stats, unit));
                }

                it.Multiply(a.hitTime);
                float totalDmg = it.phyDmg + it.magicDmg + it.trueDmg;

                if (ai.totalPhyDmg > 0)
                    chance = it.phyDmg / ai.totalPhyDmg;

                if (ai.totalMagicDmg > 0)
                    chance += it.magicDmg / ai.totalMagicDmg;

                if (ai.totalDmg > 0)
                    chance += totalDmg / ai.totalDmg;

                if (ai.totalSanityDmg > 0)
                    chance += it.sanityDmg / ai.totalSanityDmg;

                if (ai.totalHealing > 0)
                    chance += it.heal / ai.totalHealing;

                if (ai.totalShielding > 0)
                    chance += it.shield / ai.totalShielding;

                if (ai.applyAnyEffect > 0)
                    if (a.effects.Count > 0)
                        chance += ai.applyAnyEffect;

                if (ai.block > 0)
                    if (a.blocksPhysical || a.blocksMagical || a.blocksRanged)
                        chance += ai.block;

                if (ai.anyStatUp > 0)
                    if (a.statModUser != null && a.statModUser.HowPositive())
                        chance += ai.anyStatUp;

                if (ai.anyStatDown > 0)
                    if (a.statModEnemy != null && a.statModEnemy.HowPositive())
                        chance += ai.anyStatDown;
            }

            chances.Add(chance);
            //Debug.Log(a.name + ": " + chance);
        }

        return checkChances(chances);
    }

    int checkChances(List<float> chances)
    {
        //get totalPoints
        float totalPoints = 0;

        foreach (float chance in chances)
        {
            totalPoints += chance;
        }

        float totalPer = 0;
        //calculate chances
        List<float> values = new List<float>();
        foreach (float chance in chances)
        {
            float value = 0;

            if (totalPoints == 0)
            {
                value = 100 / chances.Count;
            }
            else
            {
                value = (chance * 100) / totalPoints;
            }

            totalPer += value;
            values.Add(value);
            Debug.Log(": " + value);
        }

        for (int i = 0; i < values.Count; i++)
        {
            float valRand = Random.Range(0f, 100f);
            Debug.Log(valRand + " / " + values[i]);
            if (valRand <= values[i] && values[i] != 0)
            {
                return i;
            }
        }

        return -1;
    }
}
