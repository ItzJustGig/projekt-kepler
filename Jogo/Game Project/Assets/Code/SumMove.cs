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
    public float atkScale = 0;
    public float mpScale = 0;
    public int sanityDmg = 0;

    public SumMove ReturnMove()
    {
        SumMove move = CreateInstance<SumMove>();
        move.cd = cd;
        move.dmgType = dmgType;
        move.atkScale = atkScale;
        move.mpScale = mpScale;
        move.sanityDmg = sanityDmg;

        return move;
    }

    public float getDmg(StatsSummon sum)
    {
        float dmg = 0;

        dmg += sum.atkDmg * atkScale;
        dmg += sum.magicPower * mpScale;

        return dmg;
    }
}
