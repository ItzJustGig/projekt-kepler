using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using static LanguageManager;
using static Utils;

[CreateAssetMenu (fileName = "New Move", menuName = "Move")]

public class Moves : ScriptableObject
{
    public enum MoveType { PHYSICAL, MAGICAL, RANGED, SUPPORT, DEFFENCIVE, ENCHANT, SUMMON, BASIC }
    public enum Target { ENEMY, SELF, ALLY, ALLYSELF }
    public enum TargetType { SINGLE, AOE }
    public enum HealFromDmg { NONE, PHYSICAL, MAGICAL, TRUE, PHYSICAL_MAGICAL, PHYSICAL_TRUE, MAGICAL_TRUE, ALL }

    public MoveType type;
    public Target target;
    public TargetType targetType;

    public int id = 0;
    public new string name;
    public int hitTime = 1;
    public int priority = 0;
    public int uses = -1;
    public bool isUlt = false;
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
    public int ultEnergy;
    public HealFromDmg healFromDmgType = HealFromDmg.NONE;
    public float healFromDmg = 0;

    public int cooldown;
    public int inCooldown = 0;
    public int manaCost;
    public int staminaCost;
    public int ultCost = 100;
    public bool needFullUlt = true;

    public float critDmgBonus;
    public float critChanceBonus;

    public StatMod statModUser;
    public StatMod statModEnemy;

    public List<StatScale> scale = new List<StatScale>();
    public List<Dotdmg> dot = new List<Dotdmg>();
    public List<EffectsMove> effects = new List<EffectsMove>();

    public Passives grantPassive;
    public Summon summon;

    public string animUser;
    public string animTarget;

    Unit owner;

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
        if (summon)
            summon.SetOwner(this.owner);
    }

    private List<StatScale> ReturnScales()
    {
        List<StatScale> scales = new List<StatScale>();

        foreach(StatScale a in scale)
        {
            scales.Add(a.ReturnScale());
        }

        return scales;
    }


    public Moves ReturnMove()
    {
        Moves move = CreateInstance<Moves>();

        move.owner = owner;
        move.type = type;
        move.target = target;
        move.targetType = targetType;
        move.name = name;
        move.isUlt = isUlt;

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
        move.healSanity = healSanity;
        move.shield = shield;
        move.ultEnergy = ultEnergy;

        move.healFromDmgType = healFromDmgType;
        move.healFromDmg = healFromDmg;

        move.cooldown = cooldown;
        move.inCooldown = cooldown;

        move.manaCost = manaCost;
        move.staminaCost = staminaCost;
        move.ultCost = ultCost;
        move.needFullUlt = needFullUlt;

        move.critChanceBonus = critChanceBonus;
        move.critDmgBonus = critDmgBonus;

        move.statModUser = statModUser;
        move.statModEnemy = statModEnemy;

        move.scale = ReturnScales();
        move.dot = dot;
        move.effects = effects;
        move.summon = summon;

        move.animUser = animUser;
        move.animTarget = animTarget;

        return move;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", val.ToString());
        builder.Replace("%n%", val.ToString());

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));
        builder.Replace("%val%", val.ToString());

        return builder;
    }

    /*private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour, float scale)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");

        if (scale != 0)
            builder.Replace("%val%", (val+scale).ToString());
        else
            builder.Replace("%val%", val.ToString());

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

        return builder;
    }*/

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour, string scale)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");

        if (scale != "")
            builder.Replace("%val%", val.ToString() + scale);
        else
            builder.Replace("%val%", val.ToString());

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, string colour, string scale)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", scale);

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, string colour)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", whatIs, "");
        builder.Append(languageManager.GetText(fetch));

        builder.Replace("%c%", "<color=" + colour + ">");
        builder.Replace("%c/%", "</color>");

        return builder;
    }

    private StringBuilder GetDetail(LanguageManager languageManager, string language, string whatIsMain, string whatIsSec)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, whatIsMain, whatIsSec, "");
        builder.Append(languageManager.GetText(fetch));

        return builder;
    }

    private StringBuilder GetHealFromDmg(LanguageManager languageManager, string language, float val, HealFromDmg type)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", "healfromdmg", "");
        builder.Append(languageManager.GetText(fetch));

        switch (type)
        {
            case HealFromDmg.PHYSICAL:
                fetch = new ArgumentsFetch(language, "showdetail", "physicdmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("attack")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.MAGICAL:
                fetch = new ArgumentsFetch(language, "showdetail", "magicdmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("magic")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.TRUE:
                fetch = new ArgumentsFetch(language, "showdetail", "truedmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("true")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.PHYSICAL_MAGICAL:
                fetch = new ArgumentsFetch(language, "showdetail", "physicdmg", "");
                builder.Replace("%type%", (languageManager.GetText(fetch) + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("attack")}>");
                builder.Replace("%c/%", "</color>");

                fetch = new ArgumentsFetch(language, "showdetail", "and", "");
                builder.Replace("%and%", (languageManager.GetText(fetch) + "%type%").ToString());

                fetch = new ArgumentsFetch(language, "showdetail", "magicdmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", "");
                builder.Replace("%ao%", "");
                builder.Replace("%c%", $"<color={GetColor("magic")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.PHYSICAL_TRUE:
                fetch = new ArgumentsFetch(language, "showdetail", "physicdmg", "");
                builder.Replace("%type%", (languageManager.GetText(fetch) + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("attack")}>");
                builder.Replace("%c/%", "</color>");

                fetch = new ArgumentsFetch(language, "showdetail", "and", "");
                builder.Replace("%and%", (languageManager.GetText(fetch) + "%type%").ToString());

                fetch = new ArgumentsFetch(language, "showdetail", "truedmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("true")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.MAGICAL_TRUE:
                fetch = new ArgumentsFetch(language, "showdetail", "magicdmg", "");
                builder.Replace("%type%", (languageManager.GetText(fetch) + "%and%"));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("magic")}>");
                builder.Replace("%c/%", "</color>");

                fetch = new ArgumentsFetch(language, "showdetail", "and", "");
                builder.Replace("%and%", (languageManager.GetText(fetch) + "%type%").ToString());

                fetch = new ArgumentsFetch(language, "showdetail", "truedmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", "");
                builder.Replace("%ao%", "");
                builder.Replace("%c%", $"<color={GetColor("true")}>");
                builder.Replace("%c/%", "</color>");
                break;
            case HealFromDmg.ALL:
                fetch = new ArgumentsFetch(language, "showdetail", "alldmg", "");
                builder.Replace("%type%", languageManager.GetText(fetch));
                builder.Replace("%val%", val.ToString("0%"));
                fetch = new ArgumentsFetch(language, "showdetail", "of", "");
                builder.Replace("%ao%", languageManager.GetText(fetch));
                builder.Replace("%c%", $"<color={GetColor("crit")}>");
                builder.Replace("%c/%", "</color>");
                break;
        }

        return builder;
    }

    private StringBuilder GetDmgMoveValue()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();
        bool hasText = false;
        try
        {
            if (phyDmg > 0 || HasScale(DmgType.PHYSICAL))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = phyDmg;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.PHYSICAL)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());

                }
                builder.Append(GetDmg(languageManager, language, "dealphysicdmg", GetColor("attack"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (magicDmg > 0 || HasScale(DmgType.MAGICAL))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = magicDmg;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.MAGICAL)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "dealmagicdmg", GetColor("magic"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (trueDmg > 0 || HasScale(DmgType.TRUE))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = trueDmg;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.TRUE)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "dealtruedmg", GetColor("true"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (sanityDmg > 0 || HasScale(DmgType.SANITY))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = sanityDmg;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.SANITY)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "dealsanitydmg", GetColor("sanity"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (heal > 0 || (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0) || HasScale(DmgType.HEAL))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = heal;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.HEAL)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                if (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0)
                {
                    temp.Append(GetHealFromDmg(languageManager, language, healFromDmg, healFromDmgType));
                }

                builder.Append(GetDmg(languageManager, language, "heal", GetColor("healthregen"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (healMana > 0 || HasScale(DmgType.HEALMANA))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = healMana;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.HEALMANA)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "healmana", GetColor("healmana"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (healStamina > 0 || HasScale(DmgType.HEALSTAMINA))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = healStamina;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.HEALSTAMINA)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "healstamina", GetColor("healstamina"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (healSanity > 0 || HasScale(DmgType.HEALSANITY))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = healSanity;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.HEALSANITY)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "healsanity", GetColor("healsanity"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }

            if (shield > 0 || HasScale(DmgType.SHIELD))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = shield;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.SHIELD)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "shield", GetColor("shield"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }
            
            if (ultEnergy > 0 || HasScale(DmgType.ULTENEGY))
            {
                hasText = true;
                StringBuilder temp = new StringBuilder();
                float val = ultEnergy;
                foreach (StatScale a in scale)
                {
                    if (a.type is DmgType.ULTENEGY)
                        if (a.playerStat)
                            val += a.SetScale(owner.SetModifiers(), owner);
                        else
                            temp.Append(a.GetStatScaleInfo());
                }
                builder.Append(GetDmg(languageManager, language, "ultenergy", GetColor("ult"), val.ToString("0.0")+ temp.ToString())).AppendLine();
            }
        } catch
        {

        }

        if (!hasText)
            builder.Append("NULL");

        return builder;
    }

    private bool HasScale(DmgType type)
    {
        foreach (StatScale a in scale)
        {
            if (a.type == type)
                return true;
        }

        return false;
    }

    private StringBuilder GetDmgMove(bool isActive = false)
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();
        bool hasText = false;

        if (phyDmg > 0 || HasScale(DmgType.PHYSICAL))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.PHYSICAL)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealphysicdmg", phyDmg, GetColor("attack"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (magicDmg > 0 || HasScale(DmgType.MAGICAL))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.MAGICAL)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealmagicdmg", magicDmg, GetColor("magic"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (trueDmg > 0 || HasScale(DmgType.TRUE))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.TRUE)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealtruedmg", trueDmg, GetColor("true"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (sanityDmg > 0 || HasScale(DmgType.SANITY))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.SANITY)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "dealsanitydmg", sanityDmg, GetColor("sanity"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (heal > 0 || (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0) || HasScale(DmgType.HEAL))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.HEAL)
                    temp.Append(a.GetStatScaleInfo());
            }

            if (!(healFromDmgType is HealFromDmg.NONE) && healFromDmg > 0)
            {
                temp.Append(GetHealFromDmg(languageManager, language, healFromDmg, healFromDmgType));
            }

            builder.Append(GetDmg(languageManager, language, "heal", heal, GetColor("healthregen"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (healMana > 0 || HasScale(DmgType.HEALMANA))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.HEALMANA)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healmana", healMana, GetColor("healmana"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (healStamina > 0 || HasScale(DmgType.HEALSTAMINA))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.HEALSTAMINA)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healstamina", healStamina, GetColor("healstamina"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (healSanity > 0 || HasScale(DmgType.HEALSANITY))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.HEALSANITY)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "healsanity", healSanity, GetColor("healsanity"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (shield > 0 || HasScale(DmgType.SHIELD))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.SHIELD)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "shield", shield, GetColor("shield"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (ultEnergy > 0 || HasScale(DmgType.ULTENEGY))
        {
            hasText = true;
            StringBuilder temp = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is DmgType.ULTENEGY)
                    temp.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetDmg(languageManager, language, "ultenergy", ultEnergy, GetColor("ult"), temp.ToString()));
            if (!isActive)
                builder.AppendLine();
            else
                builder.Append(" ");
        }

        if (!hasText)
            builder.Append("NULL");

        return builder;
    }

    private StringBuilder GetMoveType(LanguageManager languageManager, string language)
    {
        StringBuilder builder = new StringBuilder();
        string color = "#00000";
        switch (type)
        {
            case MoveType.BASIC:
                color = GetColor("attack");
                ArgumentsFetch fetch = new ArgumentsFetch(language, "moves", "type", "basic");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.PHYSICAL:
                color = GetColor("attack");
                fetch = new ArgumentsFetch(language, "moves", "type", "physical");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.MAGICAL:
                color = GetColor("magic");
                fetch = new ArgumentsFetch(language, "moves", "type", "magic");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.RANGED:
                color = GetColor("crit");
                fetch = new ArgumentsFetch(language, "moves", "type", "ranged");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.ENCHANT:
                color = GetColor("enchant");
                fetch = new ArgumentsFetch(language, "moves", "type", "enchant");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.DEFFENCIVE:
                color = GetColor("shield");
                fetch = new ArgumentsFetch(language, "moves", "type", "defence");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.SUPPORT:
                color = GetColor("health");
                fetch = new ArgumentsFetch(language, "moves", "type", "support");
                builder.Append(languageManager.GetText(fetch));
                break;
            case MoveType.SUMMON:
                color = GetColor("summon");
                fetch = new ArgumentsFetch(language, "moves", "type", "summon");
                builder.Append(languageManager.GetText(fetch));
                break;
        }

        builder.Replace("%c%", "<color="+color+">");
        builder.Replace("%c/%", "</color>");
        return builder;
    }

    private StringBuilder GetMoveTarget(LanguageManager languageManager, string language)
    {
        var builder = new StringBuilder();
        switch (targetType)
        {
            case TargetType.SINGLE:
                ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", "targettype", "single");
                builder.Append(languageManager.GetText(fetch));
                break;
            case TargetType.AOE:
                fetch = new ArgumentsFetch(language, "showdetail", "targettype", "aoe");
                builder.Append(languageManager.GetText(fetch));
                break;
        }

        builder.Append(" | ");

        switch (target) {
            case Target.ENEMY:
                ArgumentsFetch fetch = new ArgumentsFetch(language, "showdetail", "target", "enemy");
                builder.Append(languageManager.GetText(fetch));
                break;
            case Target.ALLY:
                fetch = new ArgumentsFetch(language, "showdetail", "target", "ally");
                builder.Append(languageManager.GetText(fetch));
                break;
            case Target.SELF:
                fetch = new ArgumentsFetch(language, "showdetail", "target", "self");
                builder.Append(languageManager.GetText(fetch));
                break;
            case Target.ALLYSELF:
                fetch = new ArgumentsFetch(language, "showdetail", "target", "selfally");
                builder.Append(languageManager.GetText(fetch));
                break;
        }
        return builder;
    }

    public string GetTooltipText(bool showVal, float manaCostPer=1, float staminaCostPer=1)
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=24><align=center>").Append(GetDetail(languageManager, language, "moves", name)).Append("</align></size>").AppendLine();
        builder.Append("<size=20><align=center>").Append(GetMoveType(languageManager, language)).Append("</align></size>").AppendLine();
        builder.Append("<size=15><align=center>").Append(GetMoveTarget(languageManager, language)).Append("</align></size>").AppendLine();

        if (!isUlt)
        {
            builder.Append(GetDmg(languageManager, language, "mana", manaCost*manaCostPer, GetColor("mana"))).AppendLine();
            builder.Append(GetDmg(languageManager, language, "stamina", staminaCost*staminaCostPer, GetColor("stamina"))).AppendLine();
        }

        if (cooldown > 0)
        {
            builder.Append(GetDmg(languageManager, language, "cooldown", cooldown));
            if (inCooldown > 0)
                builder.Append(" (" + inCooldown + ")");

            builder.AppendLine();
        }
            

        if (hitTime > 1)
            builder.Append(GetDmg(languageManager, language, "hitimes", hitTime, GetColor())).AppendLine();

        builder.Append(GetDmg(languageManager, language, "priority", priority, GetColor())).AppendLine();

        if (critChanceBonus != 0)
            builder.Append(GetDmg(languageManager, language, "bonuscritchance", (critChanceBonus * 100), GetColor("crit"))).AppendLine();

        if (critDmgBonus != 0)
            builder.Append(GetDmg(languageManager, language, "bonuscritdmg", (critDmgBonus * 100), GetColor("crit"))).AppendLine();

        if (uses > -1)
            builder.Append(GetDmg(languageManager, language, "uses", uses, GetColor())).AppendLine();

        if (blocksPhysical || blocksMagical || blocksRanged)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

            if (blocksPhysical)
                builder.Append(GetDmg(languageManager, language, "blockphysic", GetColor("attack"))).AppendLine();

            if (blocksMagical)
                builder.Append(GetDmg(languageManager, language, "blockmagic", GetColor("magic"))).AppendLine();

            if (blocksRanged)
                builder.Append(GetDmg(languageManager, language, "blockranged", GetColor("crit"))).AppendLine();
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

        {
            StringBuilder temp = new StringBuilder();
            if (!showVal)
                temp.Append(GetDmgMoveValue());
            else
                temp.Append(GetDmgMove());

            if (temp.ToString() != "NULL")
            {
                builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
                builder.Append(temp);
            }
        }

        if (grantPassive)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
            builder.Append(grantPassive.GetPassiveInfo());
        }

        if (summon)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
            {
                StringBuilder temp = new StringBuilder();
                temp = GetMoveInfoSummon(languageManager, language, showVal);
                builder.Append(char.ToUpper(temp[0])).Append(temp.Remove(0, 1));
            }
        }

        return builder.ToString();
    }

    //vv ITEMS vv
    public StringBuilder GetMoveInfoSummon(LanguageManager languageManager, string language, bool showvalue)
    {
        StringBuilder builder = new StringBuilder();
        ArgumentsFetch fetch = new ArgumentsFetch(language, "summon", "desc", "");
        builder.Append(languageManager.GetText(fetch));
        fetch = new ArgumentsFetch(language, "summon", "name", summon.name.ToString());
        builder.Replace("%summonname%", languageManager.GetText(fetch));
        builder.Replace("%summonactioncd%", summon.move.cd.ToString());
        StringBuilder temp = new StringBuilder();

        switch (summon.move.dmgType)
        {
            case DmgType.PHYSICAL:
                temp.Append(GetSummonInfo(languageManager, language, GetColor("attack"), "physic"));
                break;
            case DmgType.MAGICAL:
                temp.Append(GetSummonInfo(languageManager, language, GetColor("magic"), "magic"));
                break;
            case DmgType.TRUE:
                temp.Append(GetSummonInfo(languageManager, language, GetColor("true"), "trued"));
                break;
            case DmgType.SHIELD:
                temp.Append(GetSummonInfo(languageManager, language, GetColor("shield"), "shield"));
                break;
            case DmgType.HEAL:
                temp.Append(GetSummonInfo(languageManager, language, GetColor("healthregen"), "heal"));
                break;
        }

        builder.Replace("%summonaction%", temp.ToString());
        if (showvalue)
            builder.AppendLine().Append(summon.GetSummonInfo(languageManager, language));
        else
            builder.AppendLine().Append(summon.GetSummonInfoSec(languageManager, language));

        return builder;
    }

    public StringBuilder GetSummonInfo(LanguageManager languageManager, string language, string colour, string whatis)
    {
        StringBuilder summonaction = new StringBuilder();
        summonaction.Append("<color=" + colour + ">");
        ArgumentsFetch fetch = new ArgumentsFetch(language, "summon", whatis, "");
        summonaction.Append(languageManager.GetText(fetch));
        summonaction.Append("</color>");

        return summonaction;
    }

    public string GetMoveInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "items", "move", name)));

        builder.Replace("%dmg%", GetDmgMove(true).ToString());
        builder.Replace("%mcost%", manaCost.ToString());
        builder.Replace("%scost%", staminaCost.ToString());
        builder.Replace("%critdmg%", (critDmgBonus*100).ToString());
        builder.Replace("%critchance%", (critChanceBonus*100).ToString());
        builder.Replace("%prio%", priority.ToString());
        builder.Replace("%hit%", hitTime.ToString());
        
        if (summon != null)
        {
            builder.Replace("%summondesc%", GetMoveInfoSummon(languageManager, language, true).ToString());
        }

        {
            StringBuilder temp = new StringBuilder();

            if (blocksPhysical)
                temp.Append(GetDmg(languageManager, language, "blockphysic", GetColor("attack")));

            if (blocksMagical)
                temp.Append(GetDmg(languageManager, language, "blockmagic", GetColor("magic")));

            if (blocksRanged)
                temp.Append(GetDmg(languageManager, language, "blockranged", GetColor("crit")));

            builder.Replace("%block%", temp.ToString());

            temp.Clear();

            switch (type)
            {
                case MoveType.PHYSICAL:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "physical")));
                    temp.Replace("%c%", $"<color={GetColor("attack")}>");
                    break;
                case MoveType.MAGICAL:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "magic")));
                    temp.Replace("%c%", $"<color={GetColor("magic")}>");
                    break;
                case MoveType.RANGED:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "ranged")));
                    temp.Replace("%c%", $"<color={GetColor("crit")}>");
                    break;
                case MoveType.ENCHANT:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "enchant")));
                    temp.Replace("%c%", $"<color={GetColor("sanity")}>");
                    break;
                case MoveType.SUPPORT:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "support")));
                    temp.Replace("%c%", $"<color={GetColor("health")}>");
                    break;
                case MoveType.DEFFENCIVE:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "defence")));
                    temp.Replace("%c%", $"<color={GetColor("shield")}>");
                    break;
                case MoveType.SUMMON:
                    temp.Append(languageManager.GetText(new ArgumentsFetch(language, "moves", "type", "summon")));
                    temp.Replace("%c%", $"<color={GetColor("summon")}>");
                    break;
            }

            temp.Replace("%c/%", "</color>");

            builder.Replace("%type%", temp.ToString());
        }

        if (statModUser || statModEnemy)
        {
            if (statModUser)
                builder.Replace("%modu%", statModUser.GetStatModInfo(true).ToString());

            if (statModEnemy)
                builder.Replace("%mode%", statModEnemy.GetStatModInfo(false).ToString());
        }

        if (effects.Count > 0)
        {
            foreach (EffectsMove a in effects)
            {
                builder.Replace("%effects%", a.GetEffectMoveInfo().ToString());
            }
        }

        if (grantPassive)
        {
            builder.Replace("%gpassive%", grantPassive.GetPassiveInfo());
        }

        return builder.ToString();
    }
}
