﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Utils;
using static LanguageManager;

[CreateAssetMenu (fileName = "New Stat Scale", menuName = "Stat Scale")]

public class StatScale : ScriptableObject
{
    public DmgType type;
    public bool playerStat = true;
    //only for passive
    public int flatValue;
    //

    public float curHp;
    public float missHp;
    public float maxHp;
    public float hpRegen;

    public float missMana;
    public float curMana;
    public float maxMana;
    public float manaRegen;

    public float maxStamina;
    public float curStamina;
    public float missStamina;
    public float staminaRegen;

    public float curSanity;
    public float missSanity;
    public float maxSanity;

    public float atkDmg;
    public float magicPower;

    public float dmgResis;
    public float magicResis;

    public float timing;
    public float movSpeed;

    public StatScale ReturnScale()
    {
        StatScale scale = CreateInstance<StatScale>();

        scale.type = type;
        scale.playerStat = playerStat;
        scale.flatValue = flatValue;
        scale.curHp = curHp;
        scale.missHp = missHp;
        scale.maxHp = maxHp;
        scale.hpRegen = hpRegen;
        scale.curMana = curMana;
        scale.missMana = missMana;
        scale.maxMana = maxMana;
        scale.manaRegen = manaRegen;
        scale.curStamina = curStamina;
        scale.missStamina = missStamina;
        scale.maxStamina = maxStamina;
        scale.staminaRegen = staminaRegen;
        scale.curSanity = curSanity;
        scale.missSanity = missSanity;
        scale.maxSanity = maxSanity;
        scale.atkDmg = atkDmg;
        scale.magicPower = magicPower;
        scale.dmgResis = dmgResis;
        scale.magicResis = magicResis;
        scale.timing = timing;
        scale.movSpeed = movSpeed;

        return scale;
    }

    private StringBuilder GetStat(LanguageManager languageManager, string language, string statName,float statVal, string colour, string onWho)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statscale", "")));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%u%", onWho);
        builder.Replace("%n%", (statVal * 100).ToString());
        builder.Replace("%s%", languageManager.GetText(new ArgumentsFetch(language, "stats", "name", statName)));

        return builder;
    }

    public StringBuilder GetStatScaleInfo()
    {
        Debug.Log(flatValue);
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();
        string onWho = "";

        if (playerStat)
            onWho = languageManager.GetText(new ArgumentsFetch(language, "showdetail", "user", ""));
        else
            onWho = languageManager.GetText(new ArgumentsFetch(language, "showdetail", "enemy", ""));

        if (flatValue > 0)
            builder.Append(flatValue);

        if (curHp > 0)
            builder.Append(GetStat(languageManager, language, "curhp", curHp, GetColor("health"), onWho));

        if (missHp > 0)
            builder.Append(GetStat(languageManager, language, "misshp", missHp, GetColor("health"), onWho));

        if (maxHp > 0)
            builder.Append(GetStat(languageManager, language, "maxhp", maxHp, GetColor("health"), onWho));

        if (hpRegen > 0)
            builder.Append(GetStat(languageManager, language, "hpregen", hpRegen, GetColor("healthregen"), onWho));

        if (curMana > 0)
            builder.Append(GetStat(languageManager, language, "curmana", curMana, GetColor("mana"), onWho));

        if (missMana > 0)
            builder.Append(GetStat(languageManager, language, "missmana", missMana, GetColor("mana"), onWho));

        if (maxMana > 0)
            builder.Append(GetStat(languageManager, language, "maxmana", maxMana, GetColor("mana"), onWho));

        if (manaRegen > 0)
            builder.Append(GetStat(languageManager, language, "manaregen", manaRegen, GetColor("healmana"), onWho));

        if (curStamina > 0)
            builder.Append(GetStat(languageManager, language, "curstamina", curStamina, GetColor("stamina"), onWho));

        if (missStamina > 0)
            builder.Append(GetStat(languageManager, language, "missstamina", missStamina, GetColor("stamina"), onWho));

        if (maxStamina > 0)
            builder.Append(GetStat(languageManager, language, "maxstamina", maxStamina, GetColor("stamina"), onWho));

        if (staminaRegen > 0)
            builder.Append(GetStat(languageManager, language, "staminaregen", staminaRegen, GetColor("healstamina"), onWho));

        if (curSanity > 0)
            builder.Append(GetStat(languageManager, language, "cursanity", curSanity, GetColor("sanity"), onWho));

        if (missSanity > 0)
            builder.Append(GetStat(languageManager, language, "misssanity", missSanity, GetColor("sanity"), onWho));

        if (maxSanity > 0)
            builder.Append(GetStat(languageManager, language, "maxsanity", maxSanity, GetColor("sanity"), onWho));

        if (atkDmg > 0)
            builder.Append(GetStat(languageManager, language, "attack", atkDmg, GetColor("attack"), onWho));

        if (magicPower > 0)
            builder.Append(GetStat(languageManager, language, "magicpower", magicPower, GetColor("magic"), onWho));

        if (dmgResis > 0)
            builder.Append(GetStat(languageManager, language, "def", dmgResis, GetColor("def"), onWho));

        if (magicResis > 0)
            builder.Append(GetStat(languageManager, language, "magicdef", magicResis, GetColor("magicdef"), onWho));

        if (timing > 0)
            builder.Append(GetStat(languageManager, language, "timing", timing, GetColor("timing"), onWho));

        if (movSpeed > 0)
            builder.Append(GetStat(languageManager, language, "movspeed", movSpeed, GetColor("speed"), onWho));


        return builder;
    }

    public float SetScale(Stats stats, Unit user)
    {
        float temp = 0;

        temp += (user.curHp * curHp);
        temp += ((stats.hp - user.curHp) * missHp);
        temp += (stats.hp * maxHp);
        temp += (stats.hpRegen * hpRegen);

        temp += (user.curMana * curMana);
        temp += ((stats.mana - user.curMana) * missMana);
        temp += (stats.mana * maxMana);
        temp += (stats.manaRegen * manaRegen);

        temp += (user.curStamina * curStamina);
        temp += ((stats.stamina - user.curStamina) * missStamina);
        temp += (stats.stamina * maxStamina);
        temp += (stats.staminaRegen * staminaRegen);

        temp += (user.curSanity * curSanity);
        temp += (stats.sanity - user.curSanity) * missSanity;
        temp += (stats.sanity * maxSanity);

        temp += (stats.atkDmg * atkDmg);
        temp += (stats.magicPower * magicPower);

        temp += (stats.dmgResis * dmgResis);
        temp += (stats.magicResis * magicResis);

        temp += (stats.timing * timing);
        temp += (stats.movSpeed * movSpeed);

        return temp;
    }

    public float SetScaleFlat(Stats stats, Unit user)
    {
        float temp = flatValue;

        temp += (user.curHp * curHp);
        temp += ((stats.hp - user.curHp) * missHp);
        temp += (stats.hp * maxHp);
        temp += (stats.hpRegen * hpRegen);

        temp += (user.curMana * curMana);
        temp += ((stats.mana - user.curMana) * missMana);
        temp += (stats.mana * maxMana);
        temp += (stats.manaRegen * manaRegen);

        temp += (user.curStamina * curStamina);
        temp += ((stats.stamina - user.curStamina) * missStamina);
        temp += (stats.stamina * maxStamina);
        temp += (stats.staminaRegen * staminaRegen);

        temp += (user.curSanity * curSanity);
        temp += (stats.sanity - user.curSanity) * missSanity;
        temp += (stats.sanity * maxSanity);

        temp += (stats.atkDmg * atkDmg);
        temp += (stats.magicPower * magicPower);

        temp += (stats.dmgResis * dmgResis);
        temp += (stats.magicResis * magicResis);

        temp += (stats.timing * timing);
        temp += (stats.movSpeed * movSpeed);

        return temp;
    }

    public DMG SetScaleDmg(Stats stats, Unit unit)
    {
        DMG dmg;

        dmg.phyDmg = 0;
        dmg.magicDmg = 0;
        dmg.trueDmg = 0;
        dmg.sanityDmg = 0;
        dmg.heal = 0;
        dmg.healMana = 0;
        dmg.healStamina = 0;
        dmg.healSanity = 0;
        dmg.shield = 0;
        dmg.ultenergy = 0;

        switch (type)
        {
            case DmgType.PHYSICAL:
                dmg.phyDmg += flatValue;
                dmg.phyDmg += SetScale(stats, unit);
                break;
            case DmgType.MAGICAL:
                dmg.magicDmg += flatValue;
                dmg.magicDmg += SetScale(stats, unit);
                break;
            case DmgType.TRUE:
                dmg.trueDmg += flatValue;
                dmg.trueDmg += SetScale(stats, unit);
                break;
            case DmgType.SANITY:
                dmg.sanityDmg += flatValue;
                dmg.sanityDmg += (int)SetScale(stats, unit);
                break;
            case DmgType.HEAL:
                dmg.heal += flatValue;
                dmg.heal += SetScale(stats, unit);
                break;
            case DmgType.HEALMANA:
                dmg.healMana += flatValue;
                dmg.healMana += SetScale(stats, unit);
                break;
            case DmgType.HEALSTAMINA:
                dmg.healStamina += flatValue;
                dmg.healStamina += SetScale(stats, unit);
                break;
            case DmgType.HEALSANITY:
                dmg.healSanity += flatValue;
                dmg.healSanity += (int)SetScale(stats, unit);
                break;
            case DmgType.SHIELD:
                dmg.shield += flatValue;
                dmg.shield += SetScale(stats, unit);
                break;
            case DmgType.ULTENEGY:
                dmg.ultenergy += flatValue;
                dmg.ultenergy += SetScale(stats, unit);
                break;
        }

        return dmg;
    }
}
