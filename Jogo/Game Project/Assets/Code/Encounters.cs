using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter/Encounter", order = 1)]
public class Encounters : ScriptableObject
{
    public List<CharacterStrenght> strenghts = new List<CharacterStrenght>();
    public int startRound;
    public int endRound;
    public bool isBoss = false;
}
