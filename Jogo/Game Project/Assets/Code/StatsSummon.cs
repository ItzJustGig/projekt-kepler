﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summon Stats", menuName = "Summon/Stats", order=2)]
public class StatsSummon : ScriptableObject
{
    public float hp;

    public float atkDmg;
    public float magicPower;

    public float movSpeed;

    public StatsSummon ReturnStats()
    {
        StatsSummon stats = CreateInstance<StatsSummon>();
        stats.hp = hp;
        stats.atkDmg = atkDmg;
        stats.magicPower = magicPower;
        stats.movSpeed = movSpeed;

        return stats;
    }
}