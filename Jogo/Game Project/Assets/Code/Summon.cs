using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Summon", menuName = "Summon/Summon")]
public class Summon : ScriptableObject
{
    public Sprite icon;
    public new string name;
    public StatsSummon stats;
    public SumMove move;
    public int summonTurn = 0;

    public Summon ReturnSummon()
    {
        Summon summon = CreateInstance<Summon>();
        summon.name = name;
        summon.icon = icon;
        summon.stats = stats.ReturnStats();
        summon.move = move.ReturnMove();
        summon.summonTurn = summonTurn;

        return summon;
    }
    
    public void SetupStats(Stats summStats, Unit summoner)
    {
        stats.hp += SetScale(stats.hpScale, summStats, summoner);
        stats.atkDmg += SetScale(stats.atkScale, summStats, summoner);
        stats.magicPower += SetScale(stats.mpScale, summStats, summoner);
        stats.movSpeed += SetScale(stats.movScale, summStats, summoner);
    }

    float SetScale(StatScale scale, Stats stats, Unit summoner)
    {
        float temp = 0;

        temp += (summoner.curHp * scale.curHp);
        temp += ((stats.hp - summoner.curHp) * scale.missHp);
        temp += (stats.hp * scale.maxHp);
        temp += (stats.hpRegen * scale.hpRegen);

        temp += (summoner.curMana * scale.curMana);
        temp += ((stats.mana - summoner.curMana) * scale.missMana);
        temp += (stats.mana * scale.maxMana);
        temp += (stats.manaRegen * scale.manaRegen);

        temp += (summoner.curStamina * scale.curStamina);
        temp += ((stats.stamina - summoner.curStamina) * scale.missStamina);
        temp += (stats.stamina * scale.maxStamina);
        temp += (stats.staminaRegen * scale.staminaRegen);

        temp += (summoner.curSanity * scale.curSanity);
        temp += (stats.sanity - summoner.curSanity) * scale.missSanity;
        temp += (stats.sanity * scale.maxSanity);

        temp += (stats.atkDmg * scale.atkDmg);
        temp += (stats.magicPower * scale.magicPower);

        temp += (stats.dmgResis * scale.dmgResis);
        temp += (stats.magicResis * scale.magicResis);

        temp += (stats.timing * scale.timing);
        temp += (stats.movSpeed * scale.movSpeed);

        return temp;
    }
}
