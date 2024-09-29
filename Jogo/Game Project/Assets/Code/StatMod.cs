using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LanguageManager;
using static Utils;

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
            builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "chancetostatmod", "")));

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
            builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statmodtime", "")));

            builder.Replace("%val%", time.ToString());

            return builder;
        }   
        else
            return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string colour, float val, string stat, bool statNeedPerc, int i, bool isItem)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statmodstat", "")));

        builder.Replace("%c%", "<color=" + colour + ">");
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
                builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "and", "")) + " ");
        } else
        {
            builder.AppendLine();
        }

        return builder;
    }

    public StringBuilder GetStatModInfo(bool isUser, bool isPassive=false)
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder main = new StringBuilder();

        string onWho = "";

        if (isUser)
            onWho = languageManager.GetText(new ArgumentsFetch(language, "showdetail", "user", ""));
        else
            onWho = languageManager.GetText(new ArgumentsFetch(language, "showdetail", "enemy", ""));

        main.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statmod", "")));
        main.Replace("%chance%", GetChance(languageManager, language).ToString() + " ");

        int i = GetStatNumber();

        StringBuilder builder = new StringBuilder();
        if (hp != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("health"), hp, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hp")), false, i, false));
        }

        if (hpRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healthregen"), hpRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hpregen")), false, i, false));
        }

        if (mana != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("mana"), mana, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "mana")), false, i, false));
        }

        if (manaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healmana"), manaRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "manaregen")), false, i, false));
        }

        if (stamina != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("stamina"), stamina, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "stamina")), false, i, false));
        }

        if (staminaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healstamina"), staminaRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "staminaregen")), false, i, false));
        }

        if (sanity != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("sanity"), sanity, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "sanity")), false, i, false));
        }

        if (atkDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("attack"), atkDmg, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "attack")), false, i, false));
        }

        if (magicPower != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magic"), magicPower, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicpower")), false, i, false));
        }

        if (critChance != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("crit"), critChance, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critchance")), true, i, false));
        }

        if (critDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("crit"), critDmg, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critdmg")), true, i, false));
        }

        if (dmgResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("def"), dmgResis, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "def")), false, i, false));
        }

        if (magicResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magicdef"), magicResis, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicdef")), false, i, false));
        }

        if (timing != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("timing"), timing, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "timing")), false, i, false));
        }

        if (movSpeed != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("speed"), movSpeed, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "movspeed")), false, i, false));
        }

        if (lifesteal != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("lifesteal"), lifesteal, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "lifesteal")), true, i, false));
        }

        if (evasion != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("evasion"), evasion, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "evasion")), true, i, false));
        }

        if (accuracy != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("accuracy"), accuracy, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "accuracy")), true, i, false));
        }

        if (armourPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("defpen"), armourPen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "armourpen")), true, i, false));
        }

        if (magicPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magicpen"), magicPen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicpen")), true, i, false));
        }

        if (ultrate != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("ult"), ultrate, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "ultrate")), true, i, false));
        }

        if (manaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("mana"), manaCost, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "manacost")), true, i, false));
        }

        if (staminaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("stamina"), staminaCost, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "staminacost")), true, i, false));
        }

        if (healBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healthregen"), healBonus, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "healbonus")), true, i, false));
        }

        if (shieldBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("shield"), shieldBonus, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "shieldbonus")), true, i, false));
        }

        if (size != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor(), size, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "size")), false, i, false));
        }

        main.Replace("%stat%", builder.ToString());
        main.Replace("%who%", languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statmodwho", "")));
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
        main.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", "statmod", "")));
        main.Replace("%chance%", "");

        if (hp != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("health"), hp, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "maxhp")), false, i, true));
        }

        if (hpRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healthregen"), hpRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hpregen")), false, i, true));
        }

        if (mana != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("mana"), mana, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "maxmana")), false, i, true));
        }

        if (manaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healmana"), manaRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "manaregen")), false, i, true));
        }

        if (stamina != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("stamina"), stamina, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "maxstamina")), false, i, true));
        }

        if (staminaRegen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healstamina"), staminaRegen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "staminaregen")), false, i, true));
        }

        if (sanity != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("sanity"), sanity, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "maxsanity")), false, i, true));
        }

        if (atkDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("attack"), atkDmg, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "attack")), false, i, true));
        }

        if (magicPower != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magic"), magicPower, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicpower")), false, i, true));
        }

        if (critChance != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("crit"), critChance, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critchance")), true, i, true));
        }

        if (critDmg != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("crit"), critDmg, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "critdmg")), true, i, true));
        }

        if (dmgResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("def"), dmgResis, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "def")), false, i, true));
        }

        if (magicResis != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magicdef"), magicResis, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicdef")), false, i, true));
        }

        if (timing != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("timing"), timing, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "timing")), false, i, true));
        }

        if (movSpeed != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("speed"), movSpeed, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "movspeed")), false, i, true));
        }

        if (lifesteal != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("lifesteal"), lifesteal, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "lifesteal")), true, i, true));
        }

        if (evasion != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("evasion"), evasion, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "evasion")), true, i, true));
        }

        if (accuracy != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("accuracy"), accuracy, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "accuracy")), true, i, true));
        }

        if (armourPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("defpen"), armourPen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "armourpen")), true, i, true));
        }

        if (magicPen != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("magicpen"), magicPen, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "magicpen")), true, i, true));
        }

        if (ultrate != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("ult"), ultrate, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "ultrate")), true, i, true));
        }

        if (manaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("mana"), manaCost, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "manacost")), true, i, true));
        }

        if (staminaCost != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("stamina"), staminaCost, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "staminacost")), true, i, true));
        }

        if (healBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("healthregen"), healBonus, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "healbonus")), true, i, true));
        }

        if (shieldBonus != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor("shield"), shieldBonus, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "shieldbonus")), true, i, true));
        }

        if (size != 0)
        {
            i--;
            builder.Append(GetInfo(languageManager, language, GetColor(), size, languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "size")), false, i, true));
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
