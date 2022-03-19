using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Items : ScriptableObject
{
    public enum ShopRarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, CHAMPION }

    public new string name;
    public Sprite icon;
    public List<Passives> passives = new List<Passives>();
    public List<StatMod> statmod = new List<StatMod>();
    public List<Moves> moves = new List<Moves>();
    public ShopRarity rarity;
    public int minPrice;
    public int maxPrice;
}
