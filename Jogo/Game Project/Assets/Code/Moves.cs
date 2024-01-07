using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
        move.shield = shield;

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
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", val.ToString());
        builder.Replace("%n%", val.ToString());

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));
        builder.Replace("%val%", val.ToString());

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, float val, string colour, float scale)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        if (scale != 0)
            builder.Replace("%val%", (val+scale).ToString());
        else
            builder.Replace("%val%", val.ToString());

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

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
            builder.Replace("%val%", val.ToString());

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

        return builder;
    }

    private StringBuilder GetDmg(LanguageManager languageManager, string language, string whatIs, string colour, string scale)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", scale);

        builder = Dotdmg.ReplaceDot(builder, whatIs, dot);

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

    private StringBuilder GetDetail(LanguageManager languageManager, string language, string whatIsMain, string whatIsSec)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, whatIsMain, whatIsSec));

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
                builder.Append(GetDmg(languageManager, language, "dealphysicdmg", "ffaa00", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "dealmagicdmg", "1a66ff", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "dealtruedmg", "a6a6a6", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "dealsanitydmg", "b829ff", val.ToString("0.0")+ temp.ToString())).AppendLine();
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

                builder.Append(GetDmg(languageManager, language, "heal", "00ff11", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "healmana", "1e68fc", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "healstamina", "f0dd0a", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "healsanity", "b641f0", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
                builder.Append(GetDmg(languageManager, language, "shield", "787878", val.ToString("0.0")+ temp.ToString())).AppendLine();
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
            builder.Append(GetDmg(languageManager, language, "dealphysicdmg", phyDmg, "ffaa00", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "dealmagicdmg", magicDmg, "1a66ff", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "dealtruedmg", trueDmg, "a6a6a6", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "dealsanitydmg", sanityDmg, "b829ff", temp.ToString()));
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

            builder.Append(GetDmg(languageManager, language, "heal", heal, "00ff11", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "healmana", healMana, "1e68fc", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "healstamina", healStamina, "f0dd0a", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "healsanity", healSanity, "b641f0", temp.ToString()));
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
            builder.Append(GetDmg(languageManager, language, "shield", shield, "787878", temp.ToString()));
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
                color = "#ffaa00";
                builder.Append(languageManager.GetText(language, "moves", "type", "basic"));
                break;
            case MoveType.PHYSICAL:
                color = "#ffaa00";
                builder.Append(languageManager.GetText(language, "moves", "type", "physical"));
                break;
            case MoveType.MAGICAL:
                color = "#1a66ff";
                builder.Append(languageManager.GetText(language, "moves", "type", "magic"));
                break;
            case MoveType.RANGED:
                color = "#f75145";
                builder.Append(languageManager.GetText(language, "moves", "type", "ranged"));
                break;
            case MoveType.ENCHANT:
                color = "#CC66FF";
                builder.Append(languageManager.GetText(language, "moves", "type", "enchant"));
                break;
            case MoveType.DEFFENCIVE:
                color = "#787878";
                builder.Append(languageManager.GetText(language, "moves", "type", "defence"));
                break;
            case MoveType.SUPPORT:
                color = "#00ff11";
                builder.Append(languageManager.GetText(language, "moves", "type", "support"));
                break;
            case MoveType.SUMMON:
                color = "#B266FF";
                builder.Append(languageManager.GetText(language, "moves", "type", "summon"));
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
                builder.Append(languageManager.GetText(language, "showdetail", "targettype", "single"));
                break;
            case TargetType.AOE:
                builder.Append(languageManager.GetText(language, "showdetail", "targettype", "aoe"));
                break;
        }

        builder.Append(" | ");

        switch (target) {
            case Target.ENEMY:
                builder.Append(languageManager.GetText(language, "showdetail", "target", "enemy"));
                break;
            case Target.ALLY:
                builder.Append(languageManager.GetText(language, "showdetail", "target", "ally"));
                break;
            case Target.SELF:
                builder.Append(languageManager.GetText(language, "showdetail", "target", "self"));
                break;
            case Target.ALLYSELF:
                builder.Append(languageManager.GetText(language, "showdetail", "target", "selfally"));
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
            builder.Append(GetDmg(languageManager, language, "mana", manaCost*manaCostPer, "2d71fa")).AppendLine();
            builder.Append(GetDmg(languageManager, language, "stamina", staminaCost*staminaCostPer, "ebdb28")).AppendLine();
        }

        if (cooldown > 0)
        {
            builder.Append(GetDmg(languageManager, language, "cooldown", cooldown));
            if (inCooldown > 0)
                builder.Append(" (" + inCooldown + ")");

            builder.AppendLine();
        }
            

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
        builder.Append(languageManager.GetText(language, "summon", "desc"));
        builder.Replace("%summonname%", languageManager.GetText(language, "summon", "name", summon.name.ToString()));
        builder.Replace("%summonactioncd%", summon.move.cd.ToString());
        StringBuilder temp = new StringBuilder();

        switch (summon.move.dmgType)
        {
            case DmgType.PHYSICAL:
                temp.Append(GetSummonInfo(languageManager, language, "ffaa00", "physic"));
                break;
            case DmgType.MAGICAL:
                temp.Append(GetSummonInfo(languageManager, language, "1a66ff", "magic"));
                break;
            case DmgType.TRUE:
                temp.Append(GetSummonInfo(languageManager, language, "a6a6a6", "trued"));
                break;
            case DmgType.SHIELD:
                temp.Append(GetSummonInfo(languageManager, language, "787878", "shield"));
                break;
            case DmgType.HEAL:
                temp.Append(GetSummonInfo(languageManager, language, "00ff11", "heal"));
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
        summonaction.Append("<color=#" + colour + ">");
        summonaction.Append(languageManager.GetText(language, "summon", whatis));
        summonaction.Append("</color>");

        return summonaction;
    }

    public string GetMoveInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append(languageManager.GetText(language, "items", "move", name));

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
                temp.Append(GetDmg(languageManager, language, "blockphysic", "ffaa00"));

            if (blocksMagical)
                temp.Append(GetDmg(languageManager, language, "blockmagic", "1a66ff"));

            if (blocksRanged)
                temp.Append(GetDmg(languageManager, language, "blockranged", "f75145"));

            builder.Replace("%block%", temp.ToString());

            temp.Clear();

            switch (type)
            {
                case MoveType.PHYSICAL:
                    temp.Append(languageManager.GetText(language, "moves", "type", "physical"));
                    temp.Replace("%c%", "<color=#ffaa00>");
                    break;
                case MoveType.MAGICAL:
                    temp.Append(languageManager.GetText(language, "moves", "type", "magic"));
                    temp.Replace("%c%", "<color=#1a66ff>");
                    break;
                case MoveType.RANGED:
                    temp.Append(languageManager.GetText(language, "moves", "type", "ranged"));
                    temp.Replace("%c%", "<color=#f75145>");
                    break;
                case MoveType.ENCHANT:
                    temp.Append(languageManager.GetText(language, "moves", "type", "enchant"));
                    temp.Replace("%c%", "<color=#CC66FF>");
                    break;
                case MoveType.SUPPORT:
                    temp.Append(languageManager.GetText(language, "moves", "type", "support"));
                    temp.Replace("%c%", "<color=#00ff11>");
                    break;
                case MoveType.DEFFENCIVE:
                    temp.Append(languageManager.GetText(language, "moves", "type", "defence"));
                    temp.Replace("%c%", "<color=#787878>");
                    break;
                case MoveType.SUMMON:
                    temp.Append(languageManager.GetText(language, "moves", "type", "summon"));
                    temp.Replace("%c%", "<color=#B266FF>");
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
