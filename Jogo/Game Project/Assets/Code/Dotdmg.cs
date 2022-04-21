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
