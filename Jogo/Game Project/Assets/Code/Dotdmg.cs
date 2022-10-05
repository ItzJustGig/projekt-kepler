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

    public enum SrcType { MOVE, PASSIVE }
    public string srcId;
    public SrcType srcType;

    public void Setup(float dmgT, bool crit, string srcId, SrcType srcType)
    {
        dmg = dmgT / time;
        inTime = time;
        isCrit = crit;
        this.srcId = srcId;
        this.srcType = srcType;
    }

    public void Setup(float dmgT, string srcId, SrcType srcType)
    {
        dmg = dmgT / time;
        inTime = time;
        this.srcId = srcId;
        this.srcType = srcType;
    }

    public void Setup(float dmgT, float time, string srcId, SrcType srcType, DmgType dmgType)
    {
        dmg = dmgT / (int)time;
        inTime = (int)time;
        this.srcId = srcId;
        this.srcType = srcType;
        this.type = dmgType;
    }

    public Dotdmg ReturnDOT()
    {
        Dotdmg dot = CreateInstance<Dotdmg>();

        dot.dmg = dmg;
        dot.time = time;
        dot.inTime = inTime;
        dot.type = type;
        dot.srcId = srcId;
        dot.srcType = srcType;

        return dot;
    }

    private StringBuilder GetDOT(LanguageManager languageManager, string language, int time)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", "overtime"));

        builder.Replace("%n%", time.ToString());

        return builder;
    }

    public static StringBuilder ReplaceDot(StringBuilder builder, string whatIs, List<Dotdmg> dot)
    {
        StringBuilder temp = new StringBuilder();
        foreach (Dotdmg a in dot)
        {
            if (whatIs == "dealphysicdmg" && a.type is Dotdmg.DmgType.PHYSICAL)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "dealmagicdmg" && a.type is Dotdmg.DmgType.MAGICAL)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "dealtruedmg" && a.type is Dotdmg.DmgType.TRUE)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "dealsanitydmg" && a.type is Dotdmg.DmgType.SANITY)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "heal" && a.type is Dotdmg.DmgType.HEAL)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "healmana" && a.type is Dotdmg.DmgType.HEALMANA)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "healstamina" && a.type is Dotdmg.DmgType.HEALSTAMINA)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "healsanity" && a.type is Dotdmg.DmgType.HEALSANITY)
            {
                temp = a.GetDOTInfo();
            }
            else if (whatIs == "shield" && a.type is Dotdmg.DmgType.SHIELD)
            {
                temp = a.GetDOTInfo();
            }
            else
            {
                temp.Append("");
            }
        }

        builder.Replace("%dot%", temp.ToString());

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

    public StringBuilder GetDOTInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();

        StringBuilder builder = new StringBuilder();

        builder.Append(GetDOT(languageManager, language, time));

        return builder;
    }
}
