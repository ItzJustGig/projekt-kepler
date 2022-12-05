using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Summon", menuName = "Summon/Summon")]
public class Summon : ScriptableObject
{
    public Sprite icon;
    public new string name;
    public StatsSummon stats;
    public SumMove move;
    public int summonTurn = 0;
    GameObject iconInGame;
    Unit owner;

    public string GetLanguage()
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

    public LanguageManager GetLanguageMan()
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

    public string GetMoveTypeLangId()
    {
        string whatis = "";
        switch (move.dmgType)
        {
            case SumMove.DmgType.PHYSICAL:
                whatis = "physic";
                break;
            case SumMove.DmgType.MAGICAL:
                whatis = "magic";
                break;
            case SumMove.DmgType.TRUE:
                whatis = "trued";
                break;
            case SumMove.DmgType.HEAL:
                whatis = "heal";
                break;
            case SumMove.DmgType.SHIELD:
                whatis = "shield";
                break;
        }
        return whatis;
    }

    public StringBuilder GetSummonInfo(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();

        text.Append("<color=#00ff11>" + languageManager.GetText(language, "stats", "name", "hp") + ": " + stats.hpScale.GetStatScaleInfo().Remove(0, 2) + "</color>").AppendLine();
        text.Append("<color=#ffaa00>" + languageManager.GetText(language, "stats", "name", "attackpower") + ": " + stats.atkScale.GetStatScaleInfo().Remove(0, 2) + "</color>").AppendLine();

        return text;
    }

    public StringBuilder GetSummonInfoSec(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();

        text.Append("<color=#00ff11>" + languageManager.GetText(language, "stats", "name", "hp") + ": " + stats.hpScale.SetScale(owner.SetModifiers(), owner).ToString("0") + "</color>").AppendLine();
        text.Append("<color=#ffaa00>" + languageManager.GetText(language, "stats", "name", "attackpower") + ": " + stats.atkScale.SetScale(owner.SetModifiers(), owner).ToString("0.0") + "</color>").AppendLine();

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

    public StringBuilder GetSummonInfoCombat(LanguageManager languageManager, string language)
    {
        StringBuilder text = new StringBuilder();
        text.Append("<size=25><align=center>").Append(languageManager.GetText(language, "summon", "name", name)).Append("</align></size>").AppendLine();
        text.Append("<size=19><align=center><color=#B2B2B2>").Append(languageManager.GetText(language, "summon", "title")).Append("</color></align></size>").AppendLine().AppendLine();

        text.Append(languageManager.GetText(language, "summon", "descsum"));

        string colour = "";
        string whatis = "";
        switch (move.dmgType)
        {
            case SumMove.DmgType.PHYSICAL:
                colour = "ffaa00";
                whatis = "physic";
                break;
            case SumMove.DmgType.MAGICAL:
                colour = "1a66ff";
                whatis = "magic";
                break;
            case SumMove.DmgType.TRUE:
                colour = "a6a6a6";
                whatis = "trued";
                break;
            case SumMove.DmgType.HEAL:
                colour = "00ff11";
                whatis = "heal";
                break;
            case SumMove.DmgType.SHIELD:
                colour = "787878";
                whatis = "shield";
                break;
        }
        whatis = languageManager.GetText(language, "summon", whatis);

        text.Replace("%c%", "<color=#" + colour + ">");
        text.Replace("%c/%", "</color>");
        text.Replace("%dmg%", stats.atkPower.ToString("0.0"));

        if (move.inCd <= 0)
            text.Replace("%cd%", "");
        else
        {
            text.Replace("%cd%", languageManager.GetText(language, "summon", "cd"));
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
        float temp = 0;

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
