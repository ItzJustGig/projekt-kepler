public enum DmgType { PHYSICAL, MAGICAL, TRUE, SANITY, HEAL, HEALMANA, HEALSTAMINA, HEALSANITY, SHIELD };

public struct DMG
{
    public float phyDmg;
    public float magicDmg;
    public float trueDmg;
    public int sanityDmg;
    public float heal;
    public float healMana;
    public float healStamina;
    public int healSanity;
    public float shield;

    public void Reset()
    {
        phyDmg = 0;
        magicDmg = 0;
        trueDmg = 0;
        sanityDmg = 0;
        heal = 0;
        healMana = 0;
        healStamina = 0;
        healSanity = 0;
        shield = 0;
    }

    public void AddDmg(DMG dmg)
    {
        phyDmg += dmg.phyDmg;
        magicDmg += dmg.magicDmg;
        trueDmg += dmg.trueDmg;
        sanityDmg += dmg.sanityDmg;
        heal += dmg.heal;
        healMana += dmg.healMana;
        healStamina += dmg.healStamina;
        healSanity += dmg.healSanity;
        shield += dmg.shield;
    }

    public void AddBaseDmg(Moves move)
    {
        phyDmg += move.phyDmg;
        magicDmg += move.magicDmg;
        trueDmg += move.trueDmg;
        sanityDmg += move.sanityDmg;
    }

    public void ApplyCrit(bool magicCanCrit, float critDmg)
    {
        if (phyDmg > 0)
            phyDmg += phyDmg * critDmg;

        if (magicCanCrit && magicDmg > 0)
            magicDmg += magicDmg * critDmg;
    }

    public void ApplyLifesteal(bool magicCanSteal, float lifesteal)
    {
        if (phyDmg > 0)
        heal += phyDmg * lifesteal;

        if (magicCanSteal && magicDmg > 0)
            heal += magicDmg * lifesteal;
    }

    public void ApplyBonusDmg(float bonusPhy, float bonusMag, float bonusHeal)
    {
        if (phyDmg > 0)
            phyDmg += phyDmg * bonusPhy;

        if (magicDmg > 0)
            magicDmg += magicDmg * bonusMag;

        if (heal > 0)
            heal += heal * bonusHeal;
    }

    public void ApplyBonusPhyDmg(float bonusPhy)
    {
        if (phyDmg > 0)
            phyDmg += phyDmg * bonusPhy;
    }

    public void ApplyBonusMagicDmg(float bonusMagic)
    {
        if (magicDmg > 0)
            magicDmg += magicDmg * bonusMagic;
    }

    public void ApplyBonusHeal(float bonusHeal)
    {
        if (heal > 0)
            heal += heal * bonusHeal;
    }

    public DMG TransferHeals(DMG dmg)
    {
        heal += dmg.heal;
        dmg.heal = 0;
        healMana += dmg.healMana;
        dmg.healMana = 0;
        healStamina += dmg.healStamina;
        dmg.healStamina = 0;
        healSanity += dmg.healSanity;
        dmg.healSanity = 0;
        shield += dmg.shield;
        dmg.shield = 0;

        return dmg;
    }

    //used on AI
    public void AddBaseDmgHeal(Moves move)
    {
        phyDmg += move.phyDmg;
        magicDmg += move.magicDmg;
        trueDmg += move.trueDmg;
        sanityDmg += move.sanityDmg;

        heal += move.heal;
        healMana += move.healMana;
        healStamina += move.healStamina;
        healSanity += move.healSanity;

        shield += move.shield;
    }

    //used on AI
    public void Multiply(float num)
    {
        phyDmg *= num;
        magicDmg *= num;
        trueDmg *= num;
        sanityDmg *= (int)num;

        heal *= num;
        healMana *= num;
        healStamina *= num;
        healSanity *= (int)num;

        shield *= num;
    }
};