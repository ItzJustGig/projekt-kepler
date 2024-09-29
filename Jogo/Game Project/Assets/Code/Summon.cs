using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using static Utils;
using static LanguageManager;

[CreateAssetMenu(fileName = "New Summon", menuName = "Summon/Summon")]
public class Summon : ScriptableObject
{
    public Sprite icon;
    public new string name;
    public StatsSummon stats;
    public SumMove move;
    public int summonTurn = 0;
    [SerializeField] GameObject iconInGame;
    Unit owner;
    public Unit target;

    public Summon ReturnSummon()
    {
        Summon summon = CreateInstance<Summon>();
        summon.name = name;
        summon.icon = icon;
        summon.stats = stats.ReturnStats();
        summon.move = move.ReturnMove();
        summon.summonTurn = summonTurn;

        return summon;
    }

    //Used on AI
    public DMG ReturnSummonDmg(Stats stats, Unit unit)
    {
        DMG dmg = default;
        dmg.Reset();
        float atk = this.stats.atkScale.SetScale(stats, unit);

        switch (move.dmgType)
        {
            case DmgType.PHYSICAL:
                dmg.phyDmg += atk;
                break;
            case DmgType.MAGICAL:
                dmg.magicDmg += atk;
                break;
            case DmgType.HEAL:
                dmg.heal += atk;
                break;
            case DmgType.SHIELD:
                dmg.shield += atk;
                break;
            case DmgType.TRUE:
                dmg.trueDmg += atk;
                break;
        }

        return dmg;
    }

    public string GetMoveTypeLangId()
    {
        string whatis = "";
        switch (move.dmgType)
        {
            case DmgType.PHYSICAL:
                whatis = "physic";
                break;
            case DmgType.MAGICAL:
                whatis = "magic";
                break;
            case DmgType.TRUE:
                whatis = "trued";
                break;
            case DmgType.HEAL:
                whatis = "heal";
                break;
            case DmgType.SHIELD:
                whatis = "shield";
                break;
        }
        return whatis;
    }

    public StringBuilder GetSummonInfo(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();

        StringBuilder temp = stats.hpScale.GetStatScaleInfo();
        if (stats.hpScale.flatValue <= 0 )
        {
            temp.Remove(0, 2);
        }
        text.Append($"<color={GetColor("health")}>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hp")) + ": " + temp + "</color>").AppendLine();

        temp = stats.atkScale.GetStatScaleInfo();
        if (stats.atkScale.flatValue <= 0)
        {
            temp.Remove(0, 2);
        }

        text.Append($"<color={GetColor("attack")}>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "attackpower")) + ": " + temp + "</color>").AppendLine();

        return text;
    }

    public StringBuilder GetSummonInfoSec(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();

        text.Append($"<color={GetColor("health")}>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "hp")) + ": " + (stats.hpScale.SetScale(owner.SetModifiers(), owner) + stats.hpScale.flatValue).ToString("0") + "</color>").AppendLine();
        text.Append($"<color={GetColor("attack")}>" + languageManager.GetText(new ArgumentsFetch(language, "stats", "name", "attackpower")) + ": " + (stats.atkScale.SetScale(owner.SetModifiers(), owner) + stats.atkScale.flatValue).ToString("0.0") + "</color>").AppendLine();

        return text;
    }

    public void SetIconCombat(GameObject icon)
    {
        iconInGame = icon;
    }

    public void SetOwner(Unit unit)
    {
        owner = unit;
    }

    public void UpdateInfoCombat()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        iconInGame.gameObject.GetComponent<TooltipButton>().text = GetSummonInfoCombat(languageManager, language).ToString();
    }

    public bool CheckIfHasIcon()
    {
        if (iconInGame != null)
            return true;

        return false;
    }

    public StringBuilder GetSummonInfoCombat(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();
        text.Append("<size=25><align=center>").Append(languageManager.GetText(new ArgumentsFetch(language, "summon", "name", name))).Append("</align></size>").AppendLine();
        text.Append($"<size=19><align=center><color={GetColor("origin")}>").Append(languageManager.GetText(new ArgumentsFetch(language, "summon", "title", ""))).Append("</color></align></size>").AppendLine().AppendLine();

        text.Append(languageManager.GetText(new ArgumentsFetch(language, "summon", "descsum", "")));

        string colour = "";
        string whatis = "";
        switch (move.dmgType)
        {
            case DmgType.PHYSICAL:
                colour = GetColor("attack");
                whatis = "physic";
                break;
            case DmgType.MAGICAL:
                colour = GetColor("magic");
                whatis = "magic";
                break;
            case DmgType.TRUE:
                colour = GetColor("true");
                whatis = "trued";
                break;
            case DmgType.HEAL:
                colour = GetColor("healthregen");
                whatis = "heal";
                break;
            case DmgType.SHIELD:
                colour = GetColor("shield");
                whatis = "shield";
                break;
        }
        whatis = languageManager.GetText(new ArgumentsFetch(language, "summon", whatis));

        text.Replace("%c%", "<color=" + colour + ">");
        text.Replace("%c/%", "</color>");
        text.Replace("%dmg%", stats.atkPower.ToString("0.0"));

        if (move.inCd <= 0)
            text.Replace("%cd%", "");
        else
        {
            text.Replace("%cd%", languageManager.GetText(new ArgumentsFetch(language, "summon", "cd", "")));
            text.Replace("%num%", move.inCd.ToString());
        }

        text.Replace("%summonaction%", (char.ToUpper(whatis[0]) + whatis.Remove(0, 1)).ToString());
        text.Replace("%summonactioncd%", move.cd.ToString());

        return text;
    }
    
    public void SetupStats(Stats summStats, Unit summoner)
    {
        stats.hp += SetScale(stats.hpScale, summStats, summoner);
        stats.atkPower += SetScale(stats.atkScale, summStats, summoner);
        stats.movSpeed += SetScale(stats.movScale, summStats, summoner);
    }

    float SetScale(StatScale scale, Stats stats, Unit summoner)
    {
        float temp = scale.flatValue;

        temp += (summoner.curHp * scale.curHp);
        temp += ((stats.hp - summoner.curHp) * scale.missHp);
        temp += (stats.hp * scale.maxHp);
        temp += (stats.hpRegen * scale.hpRegen);

        temp += (summoner.curMana * scale.curMana);
        temp += ((stats.mana - summoner.curMana) * scale.missMana);
        temp += (stats.mana * scale.maxMana);
        temp += (stats.manaRegen * scale.manaRegen);

        temp += (summoner.curStamina * scale.curStamina);
        temp += ((stats.stamina - summoner.curStamina) * scale.missStamina);
        temp += (stats.stamina * scale.maxStamina);
        temp += (stats.staminaRegen * scale.staminaRegen);

        temp += (summoner.curSanity * scale.curSanity);
        temp += (stats.sanity - summoner.curSanity) * scale.missSanity;
        temp += (stats.sanity * scale.maxSanity);

        temp += (stats.atkDmg * scale.atkDmg);
        temp += (stats.magicPower * scale.magicPower);

        temp += (stats.dmgResis * scale.dmgResis);
        temp += (stats.magicResis * scale.magicResis);

        temp += (stats.timing * scale.timing);
        temp += (stats.movSpeed * scale.movSpeed);

        return temp;
    }
}
