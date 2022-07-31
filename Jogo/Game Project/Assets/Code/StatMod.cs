using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Stat Modifier")]
public class StatMod : ScriptableObject
{
    public bool flat = false;
    public int time;
    public int inTime;
    public float chance;

    public float hp;
    public float hpRegen;

    public float mana;
    public float manaRegen;

    public float stamina;
    public float staminaRegen;

    public float sanity;

    public float atkDmg;
    public float magicPower;
    public float critChance;
    public float critDmg;

    public float dmgResis;
    public float magicResis;

    public float timing;
    public float movSpeed;
    public float lifesteal;
    public float accuracy;

    public float evasion;
    public float armourPen;

    public StatMod ReturnStats()
    {
        StatMod stats = CreateInstance<StatMod>();

        stats.flat = flat;
        stats.time = time;
        stats.inTime = inTime;

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

    public StatMod ReturnStatsTimes(int times)
    {
        StatMod stats = CreateInstance<StatMod>();

        stats.flat = flat;
        stats.time = time;
        stats.inTime = inTime;

        stats.hp += hp * times;
        stats.hpRegen += hpRegen * times;
        stats.mana += mana * times;
        stats.manaRegen += manaRegen * times;
        stats.stamina += stamina * times;
        stats.staminaRegen += staminaRegen * times;
        stats.sanity += sanity * times;
        stats.atkDmg += atkDmg * times;
        stats.magicPower += magicPower * times;
        stats.critChance += critChance * times;
        stats.critDmg += critDmg * times;
        stats.dmgResis += dmgResis * times;
        stats.magicResis += magicResis * times;
        stats.timing += timing * times;
        stats.movSpeed += movSpeed * times;
        stats.lifesteal += lifesteal * times;
        stats.evasion += evasion * times;
        stats.accuracy += accuracy * times;
        stats.armourPen += armourPen * times;

        return stats;
    }

    public StringBuilder GetChance(LanguageManager languageManager, string language)
    {
        StringBuilder builder = new StringBuilder();
        if (chance > 0)
        {
            builder.Append(languageManager.GetText(language, "showdetail", "chancetostatmod"));

            builder.Replace("%val%", (chance*100).ToString());

            return builder;
        }
        else
            return builder;
    }

    public StringBuilder GetTime(LanguageManager languageManager, string language)
    {
        StringBuilder builder = new StringBuilder();
        if (time > 0)
        {
            builder.Append(languageManager.GetText(language, "showdetail", "statmodtime"));

            builder.Replace("%val%", time.ToString());

            return builder;
        }   
        else
            return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string colour, float val, string stat, string user, bool statNeedPerc)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", "statmod"));

        builder.Replace("%chance%", GetChance(languageManager, language).ToString());
        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        if (!flat || statNeedPerc)
            builder.Replace("%val%", (val*100).ToString());
        else
            builder.Replace("%val%%", val.ToString());
        
        builder.Replace("%stat%", stat);
        builder.Replace("%who%", languageManager.GetText(language, "showdetail", "statmodwho"));
        builder.Replace("%u%", user);
        builder.Replace("%time%", GetTime(languageManager, language).ToString());

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string colour, float val, string stat, bool statNeedPerc)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "items", "stat"));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        if (!flat || statNeedPerc)
            builder.Replace("%val%", (val * 100).ToString());
        else
            builder.Replace("%val%%", val.ToString());

        if (val < 0)
            builder.Replace("+", "");

        builder.Replace("%stat%", stat);

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

    public StringBuilder GetStatModInfo(bool isUser)
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();

        string onWho = "";

        if (isUser)
            onWho = languageManager.GetText(language, "showdetail", "user");
        else
            onWho = languageManager.GetText(language, "showdetail", "enemy");

        if (hp != 0)
            builder.Append(GetInfo(languageManager, language, "00ff11", hp, languageManager.GetText(language, "stats", "name", "hp"), onWho, false)).AppendLine();

        if (hpRegen != 0)
            builder.Append(GetInfo(languageManager, language, "2ffa3d", hpRegen, languageManager.GetText(language, "stats", "name", "hpregen"), onWho, false)).AppendLine();

        if (mana != 0)
            builder.Append(GetInfo(languageManager, language, "3366ff", mana, languageManager.GetText(language, "stats", "name", "mana"), onWho, false)).AppendLine();

        if (manaRegen != 0)
            builder.Append(GetInfo(languageManager, language, "2d71fa", manaRegen, languageManager.GetText(language, "stats", "name", "manaregen"), onWho, false)).AppendLine();

        if (stamina != 0)
            builder.Append(GetInfo(languageManager, language, "f0dd0a", stamina, languageManager.GetText(language, "stats", "name", "stamina"), onWho, false)).AppendLine();

        if (staminaRegen != 0)
            builder.Append(GetInfo(languageManager, language, "ebdb28", staminaRegen, languageManager.GetText(language, "stats", "name", "staminaregen"), onWho, false)).AppendLine();

        if (sanity != 0)
            builder.Append(GetInfo(languageManager, language, "b641f0", sanity, languageManager.GetText(language, "stats", "name", "sanity"), onWho, false)).AppendLine();

        if (atkDmg != 0)
            builder.Append(GetInfo(languageManager, language, "ffaa00", atkDmg, languageManager.GetText(language, "stats", "name", "attack"), onWho, false)).AppendLine();

        if (magicPower != 0)
            builder.Append(GetInfo(languageManager, language, "1a66ff", magicPower, languageManager.GetText(language, "stats", "name", "magicpower"), onWho, false)).AppendLine();

        if (critChance != 0)
            builder.Append(GetInfo(languageManager, language, "f75145", critChance, languageManager.GetText(language, "stats", "name", "critchance"), onWho, true)).AppendLine();

        if (critDmg != 0)
            builder.Append(GetInfo(languageManager, language, "f75145", critDmg, languageManager.GetText(language, "stats", "name", "critdmg"), onWho, true)).AppendLine();

        if (dmgResis != 0)
            builder.Append(GetInfo(languageManager, language, "937264", dmgResis, languageManager.GetText(language, "stats", "name", "def"), onWho, false)).AppendLine();

        if (magicResis != 0)
            builder.Append(GetInfo(languageManager, language, "946ACD", magicResis, languageManager.GetText(language, "stats", "name", "magicdef"), onWho, false)).AppendLine();

        if (timing != 0)
            builder.Append(GetInfo(languageManager, language, "0984db", timing, languageManager.GetText(language, "stats", "name", "timing"), onWho, false)).AppendLine();

        if (movSpeed != 0)
            builder.Append(GetInfo(languageManager, language, "0095ff", movSpeed, languageManager.GetText(language, "stats", "name", "movspeed"), onWho, false)).AppendLine();

        if (lifesteal != 0)
            builder.Append(GetInfo(languageManager, language, "078f10", lifesteal, languageManager.GetText(language, "stats", "name", "lifesteal"), onWho, true)).AppendLine();

        if (evasion != 0)
            builder.Append(GetInfo(languageManager, language, "227da1", evasion, languageManager.GetText(language, "stats", "name", "evasion"), onWho, true)).AppendLine();

        if (accuracy != 0)
            builder.Append(GetInfo(languageManager, language, "e04419", accuracy, languageManager.GetText(language, "stats", "name", "accuracy"), onWho, true)).AppendLine();

        if (armourPen != 0)
            builder.Append(GetInfo(languageManager, language, "c87c32", armourPen, languageManager.GetText(language, "stats", "name", "armourpen"), onWho, true)).AppendLine();

        return builder;
    }

    public StringBuilder GetStatModInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();

        if (hp != 0)
            builder.Append(GetInfo(languageManager, language, "00ff11", hp, languageManager.GetText(language, "stats", "name", "maxhp"), false)).AppendLine();

        if (hpRegen != 0)
            builder.Append(GetInfo(languageManager, language, "2ffa3d", hpRegen, languageManager.GetText(language, "stats", "name", "hpregen"), false)).AppendLine();

        if (mana != 0)
            builder.Append(GetInfo(languageManager, language, "3366ff", mana, languageManager.GetText(language, "stats", "name", "maxmana"), false)).AppendLine();

        if (manaRegen != 0)
            builder.Append(GetInfo(languageManager, language, "2d71fa", manaRegen, languageManager.GetText(language, "stats", "name", "manaregen"), false)).AppendLine();

        if (stamina != 0)
            builder.Append(GetInfo(languageManager, language, "f0dd0a", stamina, languageManager.GetText(language, "stats", "name", "maxstamina"), false)).AppendLine();

        if (staminaRegen != 0)
            builder.Append(GetInfo(languageManager, language, "ebdb28", staminaRegen, languageManager.GetText(language, "stats", "name", "staminaregen"), false)).AppendLine();

        if (sanity != 0)
            builder.Append(GetInfo(languageManager, language, "b641f0", sanity, languageManager.GetText(language, "stats", "name", "maxsanity"), false)).AppendLine();

        if (atkDmg != 0)
            builder.Append(GetInfo(languageManager, language, "ffaa00", atkDmg, languageManager.GetText(language, "stats", "name", "attack"), false)).AppendLine();

        if (magicPower != 0)
            builder.Append(GetInfo(languageManager, language, "1a66ff", magicPower, languageManager.GetText(language, "stats", "name", "magicpower"), false)).AppendLine();

        if (critChance != 0)
            builder.Append(GetInfo(languageManager, language, "f75145", critChance, languageManager.GetText(language, "stats", "name", "critchance"), true)).AppendLine();

        if (critDmg != 0)
            builder.Append(GetInfo(languageManager, language, "f75145", critDmg, languageManager.GetText(language, "stats", "name", "critdmg"), true)).AppendLine();

        if (dmgResis != 0)
            builder.Append(GetInfo(languageManager, language, "937264", dmgResis, languageManager.GetText(language, "stats", "name", "def"), false)).AppendLine();

        if (magicResis != 0)
            builder.Append(GetInfo(languageManager, language, "946ACD", magicResis, languageManager.GetText(language, "stats", "name", "magicdef"), false)).AppendLine();

        if (timing != 0)
            builder.Append(GetInfo(languageManager, language, "0984db", timing, languageManager.GetText(language, "stats", "name", "timing"), false)).AppendLine();

        if (movSpeed != 0)
            builder.Append(GetInfo(languageManager, language, "0095ff", movSpeed, languageManager.GetText(language, "stats", "name", "movspeed"), false)).AppendLine();

        if (lifesteal != 0)
            builder.Append(GetInfo(languageManager, language, "078f10", lifesteal, languageManager.GetText(language, "stats", "name", "lifesteal"), true)).AppendLine();

        if (evasion != 0)
            builder.Append(GetInfo(languageManager, language, "227da1", evasion, languageManager.GetText(language, "stats", "name", "evasion"), true)).AppendLine();

        if (accuracy != 0)
            builder.Append(GetInfo(languageManager, language, "e04419", accuracy, languageManager.GetText(language, "stats", "name", "accuracy"), true)).AppendLine();

        if (armourPen != 0)
            builder.Append(GetInfo(languageManager, language, "c87c32", armourPen, languageManager.GetText(language, "stats", "name", "armourpen"), true)).AppendLine();

        return builder;
    }

    public bool HowPositive()
    {
        bool change = false;

        if (atkDmg < 0)
            change  = true;

        if (magicPower < 0)
            change  = true;

        if (hp < 0)
            change  = true;

        if (hpRegen < 0)
            change  = true;

        if (mana < 0)
            change  = true;

        if (manaRegen < 0)
            change  = true;

        if (stamina < 0)
            change  = true;

        if (staminaRegen < 0)
            change  = true;

        if (sanity < 0)
            change  = true;

        if (dmgResis < 0)
            change  = true;

        if (magicResis < 0)
            change  = true;

        if (critChance < 0)
            change  = true;

        if (critDmg < 0)
            change  = true;

        if (evasion < 0)
            change  = true;

        if (movSpeed < 0)
            change  = true;

        if (timing < 0)
            change  = true;

        if (accuracy < 0)
            change  = true;

        if (armourPen < 0)
            change  = true;

        if (lifesteal < 0)
            change  = true;
 

        return change;
    }
}
