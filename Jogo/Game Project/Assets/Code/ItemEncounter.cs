using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Encounter", menuName = "Encounter/Item", order = 3)]
public class ItemEncounter : ScriptableObject
{
    public List<ItemRarity> rarity = new List<ItemRarity>();
    public int startRound;
    public int endRound;
}
