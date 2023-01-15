using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Character", menuName = "Character/Character", order=1)]
public class Character : ScriptableObject
{
    public enum Class { None, Tank, Assassin, Sourcerer, Marksman, Support, Duelist }
    public enum Strenght { BABY, WEAK, NORMAL, STRONG, SUPERSTRONG, LEGENDARY, CHAMPION, MYTHIC, None }

    public Strenght strenght;
    public Class classe;
    public int size = 2;
    public new string name;

    public GameObject sprite;
    public Sprite charcIcon;
    public AudioClip audio;

    public Stats stats;
    public Stats growth;
    public List<Passives> passives = new List<Passives>();

    public List<Moves> moves = new List<Moves>();
    public List<Items> recItems = new List<Items>();

    public Moves ultimate;

    public AI ai;

    public Character GetCharcInfo()
    {
        Character charc = CreateInstance<Character>();

        charc.classe = classe;
        charc.size = size;
        charc.name = name;
        charc.strenght = strenght;
        charc.charcIcon = charcIcon;
        charc.sprite = sprite;
        charc.audio = audio;
        if (growth)
            charc.growth = growth;
        charc.stats = stats;
        charc.passives = passives;
        charc.moves = moves;
        charc.ultimate = ultimate;
        charc.recItems = recItems;
        charc.ai = ai;

        return charc;
    }

    public Stats GetStatLevel(int level)
    {
        Stats temp = stats.ReturnStats();
        temp.hp += growth.hp * level;
        temp.hpRegen += growth.hpRegen * level;
        temp.mana += growth.mana*level;
        temp.manaRegen += growth.manaRegen * level;
        temp.manaCost += growth.manaCost * level;
        temp.stamina += growth.stamina * level;
        temp.staminaRegen += growth.staminaRegen * level;
        temp.staminaCost += growth.staminaCost * level;
        temp.sanity += growth.sanity * level;
        temp.atkDmg += growth.atkDmg * level;
        temp.magicPower += growth.magicPower * level;
        temp.critChance += growth.critChance * level;
        temp.critDmg += growth.critDmg * level;
        temp.dmgResis += growth.dmgResis * level;
        temp.magicResis += growth.magicResis * level;
        temp.timing += growth.timing * level;
        temp.movSpeed += growth.movSpeed * level;
        temp.lifesteal += growth.lifesteal * level;
        temp.armourPen += growth.armourPen * level;
        temp.ultrate += growth.ultrate * level;
        return temp;
    }
}
