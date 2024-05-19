using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LanguageManager;

[CreateAssetMenu(fileName = "New Effect On Move", menuName = "Effect Move")]

public class EffectsMove : ScriptableObject
{
    public int durationMin;
    public int durationMax;
    public float chance;
    public bool targetPlayer = false;
    private bool applied = false;

    public Effects effect;

    public bool WasApplied()
    {
        return applied;
    }

    public void SetApply(bool a)
    {
        applied = a;
    }

    /*public EffectsMove GetEffectsMove()
    {
        EffectsMove effect = CreateInstance<EffectsMove>();
        effect.durationMax = durationMax;
        effect.durationMin = durationMin;
        effect.chance = chance;
        effect.targetPlayer = targetPlayer;
        effect.effect = this.effect;

        return effect;
    }*/

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, string chance, string effect, string user, string time)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));

        builder.Replace("%chance%", chance);
        builder.Replace("%e%", effect);
        builder.Replace("%u%", languageManager.GetText(new ArgumentsFetch(language, "showdetail", user, "")));
        builder.Replace("%time%", time);

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, int timeMax, int timeMin)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));

        if (timeMax > timeMin)
            builder.Replace("%val%", timeMin.ToString() + "-" + timeMax.ToString());
        else
            builder.Replace("%val%", timeMin.ToString());

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string detail, float chance)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "showdetail", detail, "")));

        builder.Replace("%val%", chance.ToString());

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string effect)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(new ArgumentsFetch(language, "effect", "name", effect)));

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

    public StringBuilder GetEffectMoveInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        string onWho = "";

        if (targetPlayer)
            onWho = "user";
        else
            onWho = "enemy";

        string time = GetInfo(languageManager, language, "statmodtime", durationMax, durationMin).ToString();
        string effecttxt = GetInfo(languageManager, language, effect.id.ToLower()).ToString();
        string chancetxt = GetInfo(languageManager, language, "chancetostatmod", chance*100).ToString();

        builder.Append(GetInfo(languageManager, language, "inflicteffect", chancetxt, effecttxt, onWho, time)).AppendLine();
        return builder;
    }
}
