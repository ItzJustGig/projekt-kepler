using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AiType", menuName = "Ai/AiType", order = 3)]
public class AiType : ScriptableObject
{
    public int totalPhyDmg;
    public int totalMagicDmg;
    public int totalSanityDmg;
    public int totalDmg;
    public int totalHealing;
    public int totalShielding;
    public int applySelectedEffects;
    public int applyAnyEffect;
    public int block;
    public int selectedStatUp;
    public int anyStatUp;
    public int anyStatDown;
    public AiType(int totalPhyDmg, int totalMagicDmg, int totalSanityDmg, int totalDmg, int totalHealing, int totalShielding, int applySelectedEffects, int applyAnyEffect, int block, int selectedStatUp, int anyStatUp, int anyStatDown)
    {
        this.totalPhyDmg = totalPhyDmg;
        this.totalMagicDmg = totalMagicDmg;
        this.totalSanityDmg = totalSanityDmg;
        this.totalDmg = totalDmg;
        this.totalHealing = totalHealing;
        this.totalShielding = totalShielding;
        this.block = block;
        this.applySelectedEffects = applySelectedEffects;
        this.applyAnyEffect = applyAnyEffect;
        this.selectedStatUp = selectedStatUp;
        this.anyStatUp = anyStatUp;
        this.anyStatDown = anyStatDown;
    }
    
}
