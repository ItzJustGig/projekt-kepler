using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summon Stats", menuName = "Summon/Stats", order=2)]
public class StatsSummon : ScriptableObject
{
    public float hp;

    public float atkDmg;
    public float magicPower;

    public float movSpeed;

    public StatScale hpScale;
    public StatScale atkScale;
    public StatScale mpScale;
    public StatScale movScale;

    public StatsSummon ReturnStats()
    {
        StatsSummon stats = CreateInstance<StatsSummon>();
        stats.hpScale = hpScale;
        stats.atkScale = atkScale;
        stats.mpScale = mpScale;
        stats.movScale = movScale;

        return stats;
    }
}
