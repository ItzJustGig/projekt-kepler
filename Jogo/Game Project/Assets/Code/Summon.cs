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
    public Moves move;
    public int summonTurn = 0;

    public Summon ReturnSummon()
    {
        Summon summon = CreateInstance<Summon>();
        summon.name = name;
        summon.icon = icon;
        summon.stats = stats.ReturnStats();
        summon.move = move;
        summon.summonTurn = summonTurn;

        return summon;
    }
    
    public void SetupStats(Stats summoner)
    {
        stats.hp = summoner.hp * stats.hp;
        stats.atkDmg = summoner.atkDmg * stats.atkDmg;
        stats.magicPower = summoner.magicPower * stats.magicPower;
        stats.movSpeed = summoner.movSpeed * stats.movSpeed;
    }
}
