using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Character", menuName = "Character/Character", order=1)]
public class Character : ScriptableObject
{
    public enum Class { None, Tank, Assassin, Sourcerer, Marksman, Support, Duelist, Brawler, Vanguard, Summoner, Enchanter }
    public enum Strenght { BABY, WEAK, NORMAL, STRONG, SUPERSTRONG, LEGENDARY, CHAMPION, MYTHIC }

    public Strenght strenght;
    public Class classe;
    public int size = 2;
    public new string name;

    public GameObject sprite;
    public Sprite charcIcon;
    public AudioClip audio;

    public Stats stats;
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
        charc.stats = stats;
        charc.passives = passives;
        charc.moves = moves;
        charc.ultimate = ultimate;
        charc.recItems = recItems;
        charc.ai = ai;

        return charc;
    }
}
