using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summon Stats", menuName = "Summon/Stats", order=2)]
public class StatsSummon : ScriptableObject
{
    public float hp;
    public float atkPower;
    public float movSpeed;

    public StatScale hpScale;
    public StatScale atkScale;
    public StatScale movScale;

    public StatsSummon ReturnStats()
    {
        StatsSummon stats = CreateInstance<StatsSummon>();
        stats.hpScale = hpScale.ReturnScale();
        stats.atkScale = atkScale.ReturnScale();
        stats.movScale = movScale.ReturnScale();

        return stats;
    }
}
