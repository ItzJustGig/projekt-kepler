using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New DOT", menuName = "DOT")]
public class Dotdmg : ScriptableObject
{
    public enum DmgType { PHYSICAL, MAGICAL, TRUE, SANITY, HEAL, HEALMANA, HEALSTAMINA, HEALSANITY, SHIELD }
    public DmgType type;
    public float dmg;
    public int time;
    public int inTime;
    public bool isCrit=false;

    public void Setup(float dmgT, bool crit)
    {
        dmg = dmgT / time;
        inTime = time;
        isCrit = crit;
    }

    public void Setup(float dmgT)
    {
        dmg = dmgT / time;
        inTime = time;
    }

    public Dotdmg ReturnDOT()
    {
        Dotdmg dot = CreateInstance<Dotdmg>();

        dot.dmg = dmg;
        dot.time = time;
        dot.inTime = inTime;
        dot.type = type;
        dot.isCrit = isCrit;

        return dot;
    }
    /*
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
            builder.Replace("%val%", (val * 100).ToString());
        else
        {
            builder.Replace("%val%%", val.ToString());
        }
        builder.Replace("%stat%", stat);
        builder.Replace("%who%", languageManager.GetText(language, "showdetail", "statmodwho"));
        builder.Replace("%u%", user);
        builder.Replace("%time%", GetTime(languageManager, language).ToString());

        return builder;
    }

    private string GetLanguage()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().language;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().language;
        else
            return null;
    }

    private LanguageManager GetLanguageMan()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().languageManager;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().languageManager;
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
    }*/
}
