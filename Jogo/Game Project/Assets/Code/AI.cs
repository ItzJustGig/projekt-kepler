using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ai", menuName = "Ai/Ai", order = 3)]
public class AI : ScriptableObject
{
    [SerializeField] private AiType aiType;
    [SerializeField] private List<Effects> preferedEffects;
    [SerializeField] private List<string> preferedStats;

    public int chooseMove(List<Moves> moves, Unit user, Unit target, Stats statsU, Stats statsT)
    {
        AiType ai = this.aiType;
        List<float> chances = new List<float>();
        foreach (Moves a in moves)
        {
            float chance = 0;
            if (a.isUlt)
                chance += 80;

            if (a.inCooldown <= 0)
            {
                DMG it = default;
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

                    it.AddDmg(scale.SetScaleDmg(stats, unit));
                }

                it.Multiply(a.hitTime);

                if (a.summon)
                {
                    Unit unit = user;
                    Stats stats = statsU;

                    it.AddDmg(a.summon.ReturnSummonDmg(stats, unit));
                }

                float totalDmg = it.phyDmg + it.magicDmg + it.trueDmg;

                if (ai.totalPhyDmg > 0)
                    chance += it.phyDmg / ai.totalPhyDmg;

                if (ai.totalMagicDmg > 0)
                    chance += it.magicDmg / ai.totalMagicDmg;

                if (ai.totalDmg > 0)
                    chance += totalDmg / ai.totalDmg;

                if (ai.totalSanityDmg > 0)
                    chance += it.sanityDmg / ai.totalSanityDmg;

                if (ai.totalHealing > 0)
                {
                    chance += it.heal / ai.totalHealing;
                    chance += it.healMana / (ai.totalHealing / 2);
                    chance += it.healStamina / (ai.totalHealing / 2);
                    chance += it.healSanity / (ai.totalHealing / 2);
                }

                if (ai.totalShielding > 0)
                    chance += it.shield / ai.totalShielding;

                if (ai.applyAnyEffect > 0)
                    if (a.effects.Count > 0)
                        chance += ai.applyAnyEffect;

                if (ai.block > 0)
                    if (a.blocksPhysical || a.blocksMagical || a.blocksRanged)
                        chance += ai.block;

                if (ai.anyStatUp > 0)
                    if (a.statModUser && !a.statModUser.HowPositive())
                        chance += ai.anyStatUp;

                if (ai.anyStatDown > 0)
                    if (a.statModEnemy && a.statModEnemy.HowPositive())
                        chance += ai.anyStatDown;

                if (ai.hasSummon > 0)
                    if (a.summon != null)
                        chance += ai.hasSummon;
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
            //Debug.Log(": " + value);
        }

        float counter = 0;
        float valRand = Random.Range(0f, 100f);
        for (int i = 0; i < values.Count; i++)
        {
            counter += values[i];
            //Debug.Log(valRand + " / " + values[i]);
            if (valRand <= counter && values[i] != 0)
            {
                return i;
            }
        }

        return -1;
    }
}
