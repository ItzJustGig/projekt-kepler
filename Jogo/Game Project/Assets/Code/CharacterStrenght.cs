using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Strenght", menuName = "Encounter/Strenght", order=2)]
public class CharacterStrenght : ScriptableObject
{
    public Character.Strenght strenght;
    public float chance;
}
