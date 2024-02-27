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
    public float manaCost;

    public float stamina;
    public float staminaRegen;
    public float staminaCost;

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
    public float healBonus;
    public float shieldBonus;
    public float accuracy;

    public float evasion;
    public float armourPen;
    public float magicPen;
    public float ultrate;
    public float size;

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
        stats.magicPen = magicPen;
        stats.ultrate = ultrate;
        stats.manaCost = manaCost;
        stats.staminaCost = staminaCost;
        stats.healBonus = healBonus;
        stats.shieldBonus = shieldBonus;
        stats.size = size;

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
        stats.magicPen += magicPen * times;
        stats.ultrate += ultrate * times;
        stats.size += size * times;

        return stats;
    }

    public int GetStatNumber()
    {
        int i = 0;

        if (hp != 0)
            i++;

        if (hpRegen != 0)
            i++;

        if (mana != 0)
            i++;

        if (manaRegen != 0)
            i++;

        if (stamina != 0)
            i++;

        if (staminaRegen != 0)
            i++;

        if (sanity != 0)
            i++;

        if (atkDmg != 0)
            i++;

        if (magicPower != 0)
            i++;

        if (dmgResis != 0)
            i++;

        if (magicResis != 0)
            i++;

        if (movSpeed != 0)
            i++;

        if (timing != 0)
            i++;

        if (critChance != 0)
            i++;

        if (critDmg != 0)
            i++;

        if (lifesteal != 0)
            i++;

        if (armourPen != 0)
            i++;

        if (magicPen != 0)
            i++;

        if (ultrate != 0)
            i++;

        if (accuracy != 0)
            i++;

        if (evasion != 0)
            i++;

        if (manaCost != 0)
            i++;

        if (staminaCost != 0)
            i++;

        if (healBonus != 0)
            i++;

        if (shieldBonus != 0)
            i++;

        if (size != 0)
            i++;

        return i;
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

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string colour, float val, string stat, bool statNeedPerc, int i, bool isItem)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", "statmodstat"));

        builder.Replace("%c%", "<color=#" + colour + ">");

        builder.Replace("%c/%", "</color>");

        if (!isItem)
        {
            if (!flat || statNeedPerc)
                builder.Replace("%val%", (val * 100).ToString());
            else
                builder.Replace("%val%%", val.ToString());
        } else
        {
            if (!flat || statNeedPerc)
                if (val > 0)
                    builder.Replace("%val%", "+" + (val * 100).ToString());
                else
                    builder.Replace("%val%", (val * 100).ToString());
            else
                if (val > 0)
                    builder.Replace("%val%%", "+" + val.ToString());
                else
                    builder.Replace("%val%%", val.ToString());
        }

        builder.Replace("%stat%", stat);

        if (!isItem)
        {
            if (i > 1)
                builder.Append(", ");
            else if (i == 1)
                builder.Append(languageManager.GetText(language, "showdetail", "and") + " ");
        } else
        {
            builder.AppendLine();
        }

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

    public StringBuilder GetStatModInfo(bool isUser, bool isPassive=false)
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder main = new StringBuilder();

        string onWho = "";

        if (isUser)
            onWho = languageManager.GetText(language, "showdetail", "user");
        else
            onWho = languageManager.GetText(language, "showdetail", "enemy");

        main.Append(languageManager.GetText(language, "showdetail", "statmod"));
        main.Replace("%chance%", GetChance(languageManager, language).ToString() + " ");

        int i = GetStatNumber();

        StringBuilder builder = new StringBuilder();
        if (hp != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "00ff11", hp, languageManager.GetText(language, "stats", "name", "hp"), false, i, false));
        }

        if (hpRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "2ffa3d", hpRegen, languageManager.GetText(language, "stats", "name", "hpregen"), false, i, false));
        }

        if (mana != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "2d71fa", mana, languageManager.GetText(language, "stats", "name", "mana"), false, i, false));
        }

        if (manaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "2d71fa", manaRegen, languageManager.GetText(language, "stats", "name", "manaregen"), false, i, false));
        }

        if (stamina != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f0dd0a", stamina, languageManager.GetText(language, "stats", "name", "stamina"), false, i, false));
        }

        if (staminaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "ebdb28", staminaRegen, languageManager.GetText(language, "stats", "name", "staminaregen"), false, i, false));
        }

        if (sanity != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "b641f0", sanity, languageManager.GetText(language, "stats", "name", "sanity"), false, i, false));
        }

        if (atkDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "ffaa00", atkDmg, languageManager.GetText(language, "stats", "name", "attack"), false, i, false));
        }

        if (magicPower != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "1a66ff", magicPower, languageManager.GetText(language, "stats", "name", "magicpower"), false, i, false));
        }

        if (critChance != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f75145", critChance, languageManager.GetText(language, "stats", "name", "critchance"), true, i, false));
        }

        if (critDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f75145", critDmg, languageManager.GetText(language, "stats", "name", "critdmg"), true, i, false));
        }

        if (dmgResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "937264", dmgResis, languageManager.GetText(language, "stats", "name", "def"), false, i, false));
        }

        if (magicResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "946ACD", magicResis, languageManager.GetText(language, "stats", "name", "magicdef"), false, i, false));
        }

        if (timing != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "0984db", timing, languageManager.GetText(language, "stats", "name", "timing"), false, i, false));
        }

        if (movSpeed != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "0095ff", movSpeed, languageManager.GetText(language, "stats", "name", "movspeed"), false, i, false));
        }

        if (lifesteal != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "078f10", lifesteal, languageManager.GetText(language, "stats", "name", "lifesteal"), true, i, false));
        }

        if (evasion != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "4FA5A0", evasion, languageManager.GetText(language, "stats", "name", "evasion"), true, i, false));
        }

        if (accuracy != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "e04419", accuracy, languageManager.GetText(language, "stats", "name", "accuracy"), true, i, false));
        }

        if (armourPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "c87c32", armourPen, languageManager.GetText(language, "stats", "name", "armourpen"), true, i, false));
        }

        if (magicPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "8360B3", magicPen, languageManager.GetText(language, "stats", "name", "magicpen"), true, i, false));
        }

        if (ultrate != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "d0d0d0", ultrate, languageManager.GetText(language, "stats", "name", "ultrate"), true, i, false));
        }

        if (manaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "3366ff", manaCost, languageManager.GetText(language, "stats", "name", "manacost"), true, i, false));
        }

        if (staminaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f0dd0a", staminaCost, languageManager.GetText(language, "stats", "name", "staminacost"), true, i, false));
        }

        if (healBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "00ff11", healBonus, languageManager.GetText(language, "stats", "name", "healbonus"), true, i, false));
        }

        if (shieldBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "787878", shieldBonus, languageManager.GetText(language, "stats", "name", "shieldbonus"), true, i, false));
        }

        if (size != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "ffffff", size, languageManager.GetText(language, "stats", "name", "size"), false, i, false));
        }

        main.Replace("%stat%", builder.ToString());
        main.Replace("%who%", languageManager.GetText(language, "showdetail", "statmodwho"));
        main.Replace("%u%", onWho);
        main.Replace("%time%", GetTime(languageManager, language).ToString());

        if (!isPassive)
            main.AppendLine();

        return main;
    }

    public StringBuilder GetStatModInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();

        int i = GetStatNumber();

        StringBuilder main = new StringBuilder();
        main.Append(languageManager.GetText(language, "showdetail", "statmod"));
        main.Replace("%chance%", "");

        if (hp != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "00ff11", hp, languageManager.GetText(language, "stats", "name", "maxhp"), false, i, true));
        }

        if (hpRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "2ffa3d", hpRegen, languageManager.GetText(language, "stats", "name", "hpregen"), false, i, true));
        }

        if (mana != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "3366ff", mana, languageManager.GetText(language, "stats", "name", "maxmana"), false, i, true));
        }

        if (manaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "2d71fa", manaRegen, languageManager.GetText(language, "stats", "name", "manaregen"), false, i, true));
        }

        if (stamina != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f0dd0a", stamina, languageManager.GetText(language, "stats", "name", "maxstamina"), false, i, true));
        }

        if (staminaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "ebdb28", staminaRegen, languageManager.GetText(language, "stats", "name", "staminaregen"), false, i, true));
        }

        if (sanity != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "b641f0", sanity, languageManager.GetText(language, "stats", "name", "maxsanity"), false, i, true));
        }

        if (atkDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "ffaa00", atkDmg, languageManager.GetText(language, "stats", "name", "attack"), false, i, true));
        }

        if (magicPower != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "1a66ff", magicPower, languageManager.GetText(language, "stats", "name", "magicpower"), false, i, true));
        }

        if (critChance != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f75145", critChance, languageManager.GetText(language, "stats", "name", "critchance"), true, i, true));
        }

        if (critDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f75145", critDmg, languageManager.GetText(language, "stats", "name", "critdmg"), true, i, true));
        }

        if (dmgResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "937264", dmgResis, languageManager.GetText(language, "stats", "name", "def"), false, i, true));
        }

        if (magicResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "946ACD", magicResis, languageManager.GetText(language, "stats", "name", "magicdef"), false, i, true));
        }

        if (timing != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "0984db", timing, languageManager.GetText(language, "stats", "name", "timing"), false, i, true));
        }

        if (movSpeed != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "0095ff", movSpeed, languageManager.GetText(language, "stats", "name", "movspeed"), false, i, true));
        }

        if (lifesteal != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "078f10", lifesteal, languageManager.GetText(language, "stats", "name", "lifesteal"), true, i, true));
        }

        if (evasion != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "227da1", evasion, languageManager.GetText(language, "stats", "name", "evasion"), true, i, true));
        }

        if (accuracy != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "e04419", accuracy, languageManager.GetText(language, "stats", "name", "accuracy"), true, i, true));
        }

        if (armourPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "c87c32", armourPen, languageManager.GetText(language, "stats", "name", "armourpen"), true, i, true));
        }

        if (magicPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "8360B3", magicPen, languageManager.GetText(language, "stats", "name", "magicpen"), true, i, true));
        }

        if (ultrate != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "E6E6E6", ultrate, languageManager.GetText(language, "stats", "name", "ultrate"), true, i, true));
        }

        if (manaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "3366ff", manaCost, languageManager.GetText(language, "stats", "name", "manacost"), true, i, true));
        }

        if (staminaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "f0dd0a", staminaCost, languageManager.GetText(language, "stats", "name", "staminacost"), true, i, true));
        }

        if (healBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "00ff11", healBonus, languageManager.GetText(language, "stats", "name", "healbonus"), true, i, true));
        }

        if (shieldBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "787878", shieldBonus, languageManager.GetText(language, "stats", "name", "shieldbonus"), true, i, true));
        }

        if (size != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, "000000", size, languageManager.GetText(language, "stats", "name", "size"), false, i, true));
        }

        main.Replace("%stat%", builder.ToString());
        main.Replace("%who%", "");
        main.Replace("%u%", "");
        main.Replace("%time%", "");

        return main;
    }

    public bool HowPositive()
    {
        bool change = false;

        if (atkDmg < 0)
            change = true;

        if (magicPower < 0)
            change = true;

        if (hp < 0)
            change = true;

        if (hpRegen < 0)
            change = true;

        if (mana < 0)
            change = true;

        if (manaRegen < 0)
            change = true;

        if (stamina < 0)
            change = true;

        if (staminaRegen < 0)
            change = true;

        if (sanity < 0)
            change = true;

        if (dmgResis < 0)
            change = true;

        if (magicResis < 0)
            change = true;

        if (critChance < 0)
            change = true;

        if (critDmg < 0)
            change = true;

        if (evasion < 0)
            change = true;

        if (movSpeed < 0)
            change = true;

        if (timing < 0)
            change = true;

        if (accuracy < 0)
            change = true;

        if (armourPen < 0)
            change = true;

        if (magicPen < 0)
            change = true;

        if (lifesteal < 0)
            change = true;

        if (ultrate < 0)
            change = true;

        return change;
    }
}
