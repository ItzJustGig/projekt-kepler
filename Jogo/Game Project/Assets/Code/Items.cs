using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Items : ScriptableObject
{
    public enum ShopRarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, CHAMPION, NONE }

    public new string name;
    public Sprite icon;
    public List<Passives> passives = new List<Passives>();
    public List<StatMod> statmod = new List<StatMod>();
    public List<Moves> moves = new List<Moves>();
    public ShopRarity rarity;

    public Items returnItem()
    {
        Items item = CreateInstance<Items>();
        item.name = name;
        item.icon = icon;
        item.passives = passives;
        item.statmod = statmod;
        item.moves = moves;
        item.rarity = rarity;

        return item;
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

    private StringBuilder GetName(LanguageManager languageManager, string language, string whatIs, string whatIsIt, string whatIsComp)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, whatIs, whatIsIt, whatIsComp));

        return builder;
    }

    public StringBuilder GetActive(LanguageManager languageManager, string language, int uses, int cd)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "items", "active"));

        builder.Replace("%uses%", languageManager.GetText(language, "items", "uses"));
        builder.Replace("%val%", uses.ToString());

        builder.Replace("%cd%", languageManager.GetText(language, "items", "cd"));
        builder.Replace("%val%", cd.ToString());

        return builder;
    }

    public StringBuilder GetActive(LanguageManager languageManager, string language, int cd)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(languageManager.GetText(language, "items", "active"));

        builder.Replace("%uses%", "");

        builder.Replace("%cd%", languageManager.GetText(language, "items", "cd"));
        builder.Replace("%val%", cd.ToString());

        return builder;
    }

    public string GetTooltipText()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=24><align=center>").Append(GetName(languageManager, language, "items", "name", name)).Append("</align></size>").AppendLine();

        if (statmod.Count > 0 || moves.Count > 0 || passives.Count > 0)
        {
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
            if (statmod.Count > 0)
            {
                int i = statmod.Count;
                foreach (StatMod s in statmod)
                {
                    i--;
                    builder.Append(s.GetStatModInfo());
                }

                if (i == 0)
                    builder.AppendLine();
            }
                

            if (passives.Count > 0) 
                foreach(Passives p in passives)
                {
                    builder.Append(languageManager.GetText(language, "items", "passive")).AppendLine();

                    builder.Replace("%txt%", p.GetPassiveInfo());
                }

            if (moves.Count > 0)
                foreach (Moves m in moves)
                {
                    if (m.uses > 0)
                        builder.Append(GetActive(languageManager, language, m.uses, m.cooldown)).AppendLine();
                    else
                        builder.Append(GetActive(languageManager, language, m.cooldown)).AppendLine();

                    builder.Replace("%txt%", m.GetMoveInfo());
                }
        }

        return builder.ToString();
    }
}
