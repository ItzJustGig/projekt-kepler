using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats", order=2)]
public class Stats : ScriptableObject
{
    public float hp;
    public float hpRegen;

    public float mana;
    public float manaRegen;

    public float stamina;
    public float staminaRegen;

    public int sanity;

    public float atkDmg;
    public float magicPower;
    public float critChance;
    public float critDmg;

    public float dmgResis;
    public float magicResis;

    public float timing;
    public float movSpeed;
    public float lifesteal;
    public float accuracy = 1;

    public float evasion;
    public float armourPen;

    public Stats ReturnStats()
    {
        Stats stats = CreateInstance<Stats>();
        stats.hp = hp;
        stats.hpRegen = hpRegen;
        stats.mana = mana;
        stats.manaRegen = manaRegen;
        stats.stamina = stamina;
        stats.staminaRegen = staminaRegen;
        stats.sanity = sanity;
        stats.atkDmg = atkDmg;
        stats.magicPower = magicPower;
        stats.critChance = critChance;
        stats.critDmg = critDmg;
        stats.dmgResis = dmgResis;
        stats.magicResis = magicResis;
        stats.timing = timing;
        stats.movSpeed = movSpeed;
        stats.lifesteal = lifesteal;
        stats.evasion = evasion;
        stats.accuracy = accuracy;
        stats.armourPen = armourPen;

        return stats;
    }
}
