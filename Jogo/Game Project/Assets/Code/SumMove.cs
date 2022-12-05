using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Summon Move", menuName = "Summon/Move")]
public class SumMove : ScriptableObject
{
    public enum DmgType { PHYSICAL, MAGICAL, TRUE, HEAL, SHIELD }

    public int cd = 0;
    public int inCd = 0;

    public DmgType dmgType;
    public int sanityDmg = 0;

    public SumMove ReturnMove()
    {
        SumMove move = CreateInstance<SumMove>();
        move.cd = cd;
        move.dmgType = dmgType;
        move.sanityDmg = sanityDmg;

        return move;
    }

    public float getDmg(StatsSummon sum)
    {
        return sum.atkPower;
    }
}
