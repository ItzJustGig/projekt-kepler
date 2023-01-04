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
    public float manaCost = 1;

    public float stamina;
    public float staminaRegen;
    public float staminaCost = 1;

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
    public float healBonus = 0;
    public float shieldBonus = 0;
    public float accuracy = 1;

    public float evasion;
    public float armourPen;
    public float ultrate = 1;
    public float sizeMod = 0;

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
        stats.ultrate = ultrate;
        stats.healBonus = healBonus;
        stats.shieldBonus = shieldBonus;
        stats.manaCost = manaCost;
        stats.staminaCost = staminaCost;
        stats.sizeMod = sizeMod;

        return stats;
    }

    public Stats ReturnStatsLevel(Stats level, Stats growth)
    {
        //level is Stats with level per stat
        //maxLevelStat is max level a stat can have
        //growth is how much a stat increases per level
        Stats stats = CreateInstance<Stats>();
        stats.hp = hp + (growth.hp*level.hp);
        stats.hpRegen = hpRegen + (growth.hpRegen * level.hpRegen);
        stats.mana = mana + (growth.mana * level.mana);
        stats.manaRegen = manaRegen + (growth.manaRegen * level.manaRegen);
        stats.stamina = stamina + (growth.stamina * level.stamina);
        stats.staminaRegen = staminaRegen + (growth.staminaRegen * level.staminaRegen);
        stats.sanity = sanity + (growth.sanity *  level.sanity);
        stats.atkDmg = atkDmg + (growth.atkDmg * level.atkDmg);
        stats.magicPower = magicPower + (growth.magicPower *  level.magicPower);
        stats.critChance = critChance + (growth.critChance *level.critChance);
        stats.critDmg = critDmg + (growth.critDmg * level.critDmg);
        stats.dmgResis = dmgResis + (growth.dmgResis * level.dmgResis);
        stats.magicResis = magicResis + (growth.magicResis * level.magicResis);
        stats.timing = timing + (growth.timing * level.timing);
        stats.movSpeed = movSpeed + (growth.movSpeed * level.movSpeed);

        stats.lifesteal = lifesteal;
        stats.evasion = evasion;
        stats.accuracy = accuracy;
        stats.armourPen = armourPen;
        stats.ultrate = ultrate;

        return stats;
    }
}
