using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bonus Gold", menuName = "Encounter/Bonus Gold", order = 5)]
public class BonusGold : ScriptableObject
{
    public Character.Strenght id;
    public int minGold = 0;
    public int maxGold = 0;
}
