using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rarity", menuName = "Encounter/ItemRarity", order = 4)]
public class ItemRarity : ScriptableObject
{
    public Items.ShopRarity rarity;
    public float chance;
}
