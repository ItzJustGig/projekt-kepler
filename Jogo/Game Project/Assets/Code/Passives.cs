using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LanguageManager;
using static Utils;

[CreateAssetMenu(fileName = "New Passive", menuName = "Passive")]
public class Passives : ScriptableObject
{
    public enum Origin { PASSIVE, ITEM, MOVE }

    public new string name;
    public bool showNameOnInfo = true;
    public Origin origin = Origin.PASSIVE;
    public Sprite sprite;
    public float num;
    public float maxNum;
    public int stacks;
    public int maxStacks;
    public int cd;
    public int inCd;

    public string anim;
    public string anim2;

    public StatScale statScale;
    public StatScale statScale2;

    public StatMod statMod;
    public StatMod statMod2;

    public Passives grantPassive;
    public Moves grantMove;
    public EffectsMove grantEffect;

    public StatScale ifConditionTrueScale()
    {
        return statScale;
    }

    public StatScale ifConditionTrueScale2()
    {
        return statScale2;
    }


    public StatMod ifConditionTrueMod()
    {
        return statMod;
    }

    public Passives ReturnPassive()
    {
        Passives passive = CreateInstance<Passives>();

        passive.name = name;
        passive.origin = origin;
        passive.sprite = sprite;
        passive.num = num;
        passive.maxNum = maxNum;
        passive.stacks = stacks;
        passive.maxStacks = maxStacks;
        passive.cd = cd;
        passive.inCd = inCd;
        passive.statScale = statScale;
        passive.statScale2 = statScale2;
        passive.statMod = statMod;
        passive.statMod2 = statMod2;
        passive.grantPassive = grantPassive;
        passive.grantMove = grantMove;
        passive.grantEffect = grantEffect;
        passive.anim = anim;
        passive.anim2 = anim2;

        return passive;
    }

    private StringBuilder GetName(LanguageManager languageManager, string language, string passive)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "passive", "name", passive)));

        return builder;
    }

    private StringBuilder GetDesc(LanguageManager languageManager, string language, string passive)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "passive", "desc", passive)));

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, string dmg, string color)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));
        builder.Replace("%c%", "<color="+color+">");
        builder.Replace("%c/%", "</color>");
        builder.Replace("%val%", dmg);
        builder.Replace("%ao%", languageManager.GetText(new ArgumentsFetch(language, "showdetail", "as_", "")));

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, string user)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));

        builder.Replace("%u%", languageManager.GetText(new ArgumentsFetch(language, "showdetail", user, "")));

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, int turns)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));

        builder.Replace("%val%", turns.ToString());

        return builder;
    }

    public string GetPassiveInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder desc = new StringBuilder();

        if (showNameOnInfo)
        {
            desc.Append("<size=25><align=center>").Append(GetName(languageManager, language, name)).Append("</align></size>").AppendLine();
            
            switch (origin)
            {
                case Origin.PASSIVE:
                    desc.Append($"<size=19><align=center><color={GetColor("origin")}>").Append(languageManager.GetText(new ArgumentsFetch(language, "passive", "title", ""))).Append("</color></align></size>").AppendLine().AppendLine();
                    break;
                case Origin.MOVE:
                    desc.Append($"<size=19><align=center><color={GetColor("origin")}>").Append(languageManager.GetText(new ArgumentsFetch(language, "passive", "titlemove", ""))).Append("</color></align></size>").AppendLine().AppendLine();
                    break;
                case Origin.ITEM:
                    desc.Append($"<size=19><align=center><color={GetColor("origin")}>").Append(languageManager.GetText(new ArgumentsFetch(language, "items", "title", ""))).Append("</color></align></size>").AppendLine().AppendLine();
                    break;
            }
        }
            
        desc.Append(GetDesc(languageManager, language, name)).AppendLine();

        if (num >= 1)
            desc.Replace("%num%", num.ToString());
        else
            desc.Replace("%num%", (num*100).ToString());

        if (maxNum >= 1)
            desc.Replace("%maxNum%", maxNum.ToString());
        else
            desc.Replace("%maxNum%", (maxNum * 100).ToString());

        desc.Replace("%stacks%", stacks.ToString());
        desc.Replace("%maxStacks%", maxStacks.ToString());

        desc.Replace("%cd%", cd.ToString());
        desc.Replace("%incd%", inCd.ToString());

        if (statScale)
        {
            switch (statScale.type)
            {
                case DmgType.PHYSICAL:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "physicdmg", statScale.GetStatScaleInfo().ToString(), GetColor("attack")).ToString());
                    break;
                case DmgType.MAGICAL:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "magicdmg", statScale.GetStatScaleInfo().ToString(), GetColor("magic")).ToString()); 
                    break;
                case DmgType.TRUE:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "truedmg", statScale.GetStatScaleInfo().ToString(), GetColor("true")).ToString());
                    break;
                case DmgType.SANITY:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "sanitydmg", statScale.GetStatScaleInfo().ToString(), GetColor("sanity")).ToString());
                    break;
                case DmgType.SHIELD:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "shielddmg", statScale.GetStatScaleInfo().ToString(), GetColor("shield")).ToString());
                    break;
                case DmgType.HEAL:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "healdmg", statScale.GetStatScaleInfo().ToString(), GetColor("health")).ToString());
                    break;
                case DmgType.HEALMANA:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "healmanadmg", statScale.GetStatScaleInfo().ToString(), GetColor("healmana")).ToString());
                    break;
                case DmgType.HEALSTAMINA:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "healstaminadmg", statScale.GetStatScaleInfo().ToString(), GetColor("healstamina")).ToString());
                    break;
                case DmgType.HEALSANITY:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "healsanitydmg", statScale.GetStatScaleInfo().ToString(), GetColor("healsanity")).ToString());
                    break;
                case DmgType.ULTENEGY:
                    desc.Replace("%scale%", GetInfo(languageManager, language, "ultenergydmg", statScale.GetStatScaleInfo().ToString(), GetColor("ult")).ToString());
                    break;
            }
        }

        if (statScale2)
        {
            switch (statScale2.type)
            {
                case DmgType.PHYSICAL:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "physicdmg", statScale2.GetStatScaleInfo().ToString(), GetColor("attack")).ToString());
                    break;
                case DmgType.MAGICAL:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "magicdmg", statScale2.GetStatScaleInfo().ToString(), GetColor("magic")).ToString());
                    break;
                case DmgType.TRUE:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "truedmg", statScale2.GetStatScaleInfo().ToString(), GetColor("true")).ToString());
                    break;
                case DmgType.SANITY:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "sanitydmg", statScale2.GetStatScaleInfo().ToString(), GetColor("sanity")).ToString());
                    break;
                case DmgType.SHIELD:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "shielddmg", statScale2.GetStatScaleInfo().ToString(), GetColor("shield")).ToString());
                    break;
                case DmgType.HEAL:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "healdmg", statScale2.GetStatScaleInfo().ToString(), GetColor("healthregen")).ToString());
                    break;
                case DmgType.HEALMANA:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "healmanadmg", statScale2.GetStatScaleInfo().ToString(), GetColor("healmana")).ToString());
                    break;
                case DmgType.HEALSTAMINA:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "healstaminadmg", statScale2.GetStatScaleInfo().ToString(), GetColor("healstamina")).ToString());
                    break;
                case DmgType.HEALSANITY:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "healsanitydmg", statScale2.GetStatScaleInfo().ToString(), GetColor("healsanity")).ToString());
                    break;
                case DmgType.ULTENEGY:
                    desc.Replace("%scale2%", GetInfo(languageManager, language, "ultenergydmg", statScale2.GetStatScaleInfo().ToString(), GetColor("ult")).ToString());
                    break;
            }
        }

        if (statMod)
        {
            desc.Replace("%mod%", statMod.GetStatModInfo(true, true).ToString());
            desc.Replace(GetInfo(languageManager, language, "statmodwho", "user").ToString(), "");
            desc.Replace(GetInfo(languageManager, language, "statmodtime", 1).ToString(), "");
        }

        if (statMod2)
        {
            desc.Replace("%mod2%", statMod2.GetStatModInfo(true, true).ToString());
            desc.Replace(GetInfo(languageManager, language, "statmodwho", "user").ToString(), "");
            desc.Replace(GetInfo(languageManager, language, "statmodtime", 1).ToString(), "");
        }

        return desc.ToString();
    }
}