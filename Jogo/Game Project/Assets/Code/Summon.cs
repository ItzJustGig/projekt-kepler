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
        text.Append("<color=#ffaa00>" + languageManager.GetText(language, "stats", "name", "attack") + ": " + stats.atkScale.GetStatScaleInfo().Remove(0, 2) + "</color>").AppendLine();
        text.Append("<color=#1a66ff>" + languageManager.GetText(language, "stats", "name", "magicpower") + ": " + stats.mpScale.GetStatScaleInfo().Remove(0, 2) + "</color>").AppendLine().AppendLine();

        string color = "";
        string whatis = "";
        switch (move.dmgType)
        {
            case SumMove.DmgType.PHYSICAL:
                color = "ffaa00";
                whatis = "physic";
                break;
            case SumMove.DmgType.MAGICAL:
                color = "1a66ff";
                whatis = "magic";
                break;
            case SumMove.DmgType.TRUE:
                color = "a6a6a6";
                whatis = "trued";
                break;
            case SumMove.DmgType.HEAL:
                color = "00ff11";
                whatis = "heal";
                break;
            case SumMove.DmgType.SHIELD:
                color = "787878";
                whatis = "shield";
                break;
        }
        whatis = languageManager.GetText(language, "summon", whatis);

        text.Append("<color=#" + color + ">" + char.ToUpper(whatis[0]) + whatis.Remove(0, 1) + ": ");
        text.Append("<color=#ffaa00>" + (move.atkScale * 100).ToString() + "% " + languageManager.GetText(language, "stats", "name", "attack") + "</color>");
        text.Append(" + <color=#1a66ff>" + (move.mpScale*100).ToString() + "% " + languageManager.GetText(language, "stats", "name", "magicpower") + "</color>");

        return text;
    }
    
    public void SetupStats(Stats summStats, Unit summoner)
    {
        stats.hp += SetScale(stats.hpScale, summStats, summoner);
        stats.atkDmg += SetScale(stats.atkScale, summStats, summoner);
        stats.magicPower += SetScale(stats.mpScale, summStats, summoner);
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
