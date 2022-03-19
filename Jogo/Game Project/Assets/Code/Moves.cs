using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu (fileName = "New Move", menuName = "Move")]

public class Moves : ScriptableObject
{
    public enum MoveType { PHYSICAL, MAGICAL, RANGED, SUPPORT, DEFFENCIVE, STATMOD, BASIC, ULT }
    public enum HealFromDmg { NONE, PHYSICAL, MAGICAL, TRUE, PHYSICAL_MAGICAL, PHYSICAL_TRUE, MAGICAL_TRUE, ALL }

    public MoveType type;

    public new string name;
    public int hitTime = 1;
    public int priority = 0;
    public int uses = -1;
    public bool blocksPhysical = false;
    public bool blocksMagical = false;
    public bool blocksRanged = false;

    public int phyDmg;
    public int magicDmg;
    public int trueDmg;
    public int sanityDmg;
    public int heal;
    public int healMana;
    public int healStamina;
    public int healSanity;
    public int shield;
    public HealFromDmg healFromDmgType = HealFromDmg.NONE;
    public float healFromDmg = 0;

    public int cooldown;
    public int inCooldown = 0;
    public int manaCost;
    public int staminaCost;

    public float critDmgBonus;
    public float critChanceBonus;

    public StatMod statModUser;
    public StatMod statModEnemy;

    public List<StatScale> scale = new List<StatScale>();
    public List<Dotdmg> dot = new List<Dotdmg>();
    public List<EffectsMove> effects = new List<EffectsMove>();

    public Passives grantPassive;

    public Moves ReturnMove()
    {
        Moves move = CreateInstance<Moves>();

        move.type = type;
        move.name = name;

        move.hitTime = hitTime;
        move.priority = priority;
        move.uses = uses;

        move.blocksPhysical = blocksPhysical;
        move.blocksMagical = blocksMagical;
        move.blocksRanged = blocksRanged;

        move.grantPassive = grantPassive;

        move.phyDmg = phyDmg;
        move.magicDmg = magicDmg;
        move.trueDmg = trueDmg;
        move.sanityDmg = sanityDmg;
        move.heal = heal;
        move.healMana = healMana;
        move.healStamina = healStamina;
        move.shield = shield;

        move.healFromDmgType = healFromDmgType;
        move.healFromDmg = healFromDmg;

        move.cooldown = cooldown;
        move.inCooldown = cooldown;

        move.manaCost = manaCost;
        move.staminaCost = staminaCost;

        move.critChanceBonus = critChanceBonus;
        move.critDmgBonus = critDmgBonus;

        move.statModUser = statModUser;
        move.statModEnemy = statModEnemy;

        move.scale = scale;
        move.dot = dot;
        move.effects = effects;

        return move;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", val.ToString());
        builder.Replace("%n%", val.ToString());

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour, string scale)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        if (scale != "")
            builder.Replace("%val%", val.ToString() + scale);
        else
            builder.Replace("%val%", val.ToString() );

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, string colour)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        return builder;
    }

    private StringBuilder GetDetail(LanguageManager languageManager, string language, string whatIs, string whatIsComp)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, whatIs, whatIsComp));

        return builder;
    }

    private StringBuilder GetHealFromDmg(LanguageManager languageManager, string language, float val, HealFromDmg type)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", "healfromdmg"));

        switch (type)
        {
            case HealFromDmg.PHYSICAL:
                
                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "physicdmg"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#ffaa00>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.MAGICAL:
                
                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "magicdmg"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#1a66ff>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.TRUE:
                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "truedmg"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#a6a6a6>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.PHYSICAL_MAGICAL:
                builder.Replace("%type%", (languageManager.GetText(language, "showdetail", "physicdmg") + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#ffaa00>");
                builder.Replace("%c/%", "</color>");

                builder.Replace("%and%", (languageManager.GetText(language, "showdetail", "and") + "%type%").ToString());

                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "magicdmg"));
                builder.Replace("%val%", "");
                builder.Replace("%ao%", "");
                builder.Replace("%c%", "<color=#1a66ff>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.PHYSICAL_TRUE:
                builder.Replace("%type%", (languageManager.GetText(language, "showdetail", "physicdmg") + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#ffaa00>");
                builder.Replace("%c/%", "</color>");

                builder.Replace("%and%", (languageManager.GetText(language, "showdetail", "and") + "%type%").ToString());

                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "truedmg"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#a6a6a6>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.MAGICAL_TRUE:
                builder.Replace("%type%", (languageManager.GetText(language, "showdetail", "magicdmg") + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#1a66ff>");
                builder.Replace("%c/%", "</color>");

                builder.Replace("%and%", (languageManager.GetText(language, "showdetail", "and") + "%type%").ToString());

                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "truedmg"));
                builder.Replace("%val%", "");
                builder.Replace("%ao%", "");
                builder.Replace("%c%", "<color=#a6a6a6>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.ALL:
                builder.Replace("%type%", languageManager.GetText(language, "showdetail", "alldmg"));
                builder.Replace("%val%", val.ToString("0%"));
                builder.Replace("%ao%", languageManager.GetText(language, "showdetail", "of"));
                builder.Replace("%c%", "<color=#f75145>");
                builder.Replace("%c/%", "</color>");
                break;
        }

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

    public string GetTooltipText()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=24><align=center>").Append(GetDetail(languageManager, language, "moves", name)).Append("</align></size>").AppendLine();
        //builder.Append(description).AppendLine();
        if (hitTime > 1)
            builder.Append(GetDmg(languageManager, language, "hitimes", hitTime, "FFFFFF")).AppendLine();

        builder.Append(GetDmg(languageManager, language, "priority", priority, "FFFFFF")).AppendLine();

        if (critChanceBonus != 0)
            builder.Append(GetDmg(languageManager, language, "bonuscritchance", (critChanceBonus * 100), "f75145")).AppendLine();

        if (critDmgBonus != 0)
            builder.Append(GetDmg(languageManager, language, "bonuscritdmg", (critDmgBonus * 100), "f75145")).AppendLine();

        if (uses > -1)
            builder.Append(GetDmg(languageManager, language, "uses", uses, "FFFFFF")).AppendLine();

        if (blocksPhysical || blocksMagical || blocksRanged)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

            if (blocksPhysical)
                builder.Append(GetDmg(languageManager, language, "blockphysic", "ffaa00")).AppendLine();

            if (blocksMagical)
                builder.Append(GetDmg(languageManager, language, "blockmagic", "1a66ff")).AppendLine();

            if (blocksRanged)
                builder.Append(GetDmg(languageManager, language, "blockranged", "f75145")).AppendLine();
        }   

        if (statModUser || statModEnemy)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

            if (statModUser)
                builder.Append(statModUser.GetStatModInfo(true));

            if (statModEnemy)
                builder.Append(statModEnemy.GetStatModInfo(false));
        }

        if (effects.Count > 0)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

            foreach (EffectsMove a in effects)
            {
                builder.Append(a.GetEffectMoveInfo());
            }
        }
        builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

        if (phyDmg > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.PHYSICAL)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealphysicdmg", phyDmg, "ffaa00", temp.ToString())).AppendLine();
        }

        if (magicDmg > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.MAGICAL)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealmagicdmg", magicDmg, "1a66ff", temp.ToString())).AppendLine();
        }

        if (trueDmg > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.TRUE)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealtruedmg", trueDmg, "a6a6a6", temp.ToString())).AppendLine();
        }

        if (sanityDmg > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.SANITY)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealsanitydmg", sanityDmg, "b829ff", temp.ToString())).AppendLine();
        }

        if (heal > 0 || (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0))
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEAL)
                    temp.Append(a.GetStatScaleInfo());
            }

            if (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0)
            {
                temp.Append(GetHealFromDmg(languageManager, language, healFromDmg, healFromDmgType));
            }

            builder.Append(GetDmg(languageManager, language, "heal", heal, "00ff11", temp.ToString())).AppendLine();
        }

        if (healMana > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALMANA)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healmana", healMana, "1e68fc", temp.ToString())).AppendLine();
        }

        if (healStamina > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALSTAMINA)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healstamina", healStamina, "f0dd0a", temp.ToString())).AppendLine();
        }

        if (healSanity > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALSANITY)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healsanity", healSanity, "b641f0", temp.ToString())).AppendLine();
        }

        if (shield > 0)
        {
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.SHIELD)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "shield", shield, "787878", temp.ToString())).AppendLine();
        }

        if (grantPassive)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
            builder.Append(grantPassive.GetPassiveInfo());
        }

        return builder.ToString();
    }
}
