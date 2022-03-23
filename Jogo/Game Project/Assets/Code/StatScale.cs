using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu (fileName = "New Stat Scale", menuName = "Stat Scale")]

public class StatScale : ScriptableObject
{
    public enum DmgType { PHYSICAL, MAGICAL, TRUE, SANITY, HEAL, HEALMANA, HEALSTAMINA, HEALSANITY, SHIELD }

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

    private StringBuilder GetStat(LanguageManager languageManager, string language, string statName,float statVal, string colour, string onWho)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", "statscale"));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%u%", onWho);
        builder.Replace("%n%", (statVal * 100).ToString());
        builder.Replace("%s%", languageManager.GetText(language, "stats", "name", statName));

        return builder;
    }

    private string GetLanguage()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().language;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().language;
        else if (GameObject.Find("GameManager").GetComponent<ShopLangManager>())
            return GameObject.Find("GameManager").GetComponent<ShopLangManager>().language;
        else
            return null;
    }

    private LanguageManager GetLanguageMan()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().languageManager;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().languageManager;
        else if (GameObject.Find("GameManager").GetComponent<ShopLangManager>())
            return GameObject.Find("GameManager").GetComponent<ShopLangManager>().languageManager;
        else
            return null;
    }

    public StringBuilder GetStatScaleInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();
        string onWho = "";

        if (playerStat)
            onWho = languageManager.GetText(language, "showdetail", "user");
        else
            onWho = languageManager.GetText(language, "showdetail", "enemy");

        if (flatValue > 0)
            builder.Append(flatValue);

        if (curHp > 0)
            builder.Append(GetStat(languageManager, language, "curhp", curHp, "00ff11", onWho));

        if (missHp > 0)
            builder.Append(GetStat(languageManager, language, "misshp", missHp, "00ff11", onWho));

        if (maxHp > 0)
            builder.Append(GetStat(languageManager, language, "maxhp", maxHp, "00ff11", onWho));

        if (hpRegen > 0)
            builder.Append(GetStat(languageManager, language, "hpregen", hpRegen, "2ffa3d", onWho));

        if (curMana > 0)
            builder.Append(GetStat(languageManager, language, "curmana", curMana, "3366ff", onWho));

        if (missMana > 0)
            builder.Append(GetStat(languageManager, language, "missmana", missMana, "3366ff", onWho));

        if (maxMana > 0)
            builder.Append(GetStat(languageManager, language, "maxmana", maxMana, "3366ff", onWho));

        if (manaRegen > 0)
            builder.Append(GetStat(languageManager, language, "manaregen", manaRegen, "2d71fa", onWho));

        if (curStamina > 0)
            builder.Append(GetStat(languageManager, language, "curstamina", curStamina, "f0dd0a", onWho));

        if (missStamina > 0)
            builder.Append(GetStat(languageManager, language, "missstamina", missStamina, "f0dd0a", onWho));

        if (maxStamina > 0)
            builder.Append(GetStat(languageManager, language, "maxstamina", maxStamina, "f0dd0a", onWho));

        if (staminaRegen > 0)
            builder.Append(GetStat(languageManager, language, "staminaregen", staminaRegen, "ebdb28", onWho));

        if (curSanity > 0)
            builder.Append(GetStat(languageManager, language, "cursanity", curSanity, "b641f0", onWho));

        if (missSanity > 0)
            builder.Append(GetStat(languageManager, language, "misssanity", missSanity, "b641f0", onWho));

        if (maxSanity > 0)
            builder.Append(GetStat(languageManager, language, "maxsanity", maxSanity, "b641f0", onWho));

        if (atkDmg > 0)
            builder.Append(GetStat(languageManager, language, "attack", atkDmg, "ffaa00", onWho));

        if (magicPower > 0)
            builder.Append(GetStat(languageManager, language, "magicpower", magicPower, "1a66ff", onWho));

        if (dmgResis > 0)
            builder.Append(GetStat(languageManager, language, "def", dmgResis, "937264", onWho));

        if (magicResis > 0)
            builder.Append(GetStat(languageManager, language, "magicdef", magicResis, "946ACD", onWho));

        if (timing > 0)
            builder.Append(GetStat(languageManager, language, "timing", timing, "0984db", onWho));

        if (movSpeed > 0)
            builder.Append(GetStat(languageManager, language, "movspeed", movSpeed, "0095ff", onWho));

        return builder;
    }
}
