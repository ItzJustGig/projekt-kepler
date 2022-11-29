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
    public int shield;

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

    public void Mitigate(float dmgResisPer, float magicResisPer, Unit user)
    {
        if (phyDmg > 0)
        {
            float dmgMitigated = (float)(user.SetModifiers().dmgResis * dmgResisPer);
            if (dmgMitigated < phyDmg)
            {
                phyDmg -= dmgMitigated;
                user.phyDmgMitigated += dmgMitigated;
                user.phyDmgTaken += phyDmg - dmgMitigated;
            }
            else
            {
                dmgMitigated = phyDmg;
                phyDmg = 0;
                user.phyDmgMitigated += dmgMitigated;
                user.phyDmgTaken += 0;
            }
        }

        if (magicDmg > 0)
        {
            float dmgMitigated = (float)(user.SetModifiers().magicResis * magicResisPer);

            if (dmgMitigated < magicDmg)
            {
                magicDmg -= dmgMitigated;
                user.magicDmgMitigated += dmgMitigated;
                user.magicDmgTaken += magicDmg - dmgMitigated;
            }
            else
            {
                dmgMitigated = magicDmg;
                magicDmg = 0;
                user.magicDmgMitigated += dmgMitigated;
            }
        }
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
    public void Multiply(int num)
    {
        phyDmg *= num;
        magicDmg *= num;
        trueDmg *= num;
        sanityDmg *= num;

        heal *= num;
        healMana *= num;
        healStamina *= num;
        healSanity *= num;

        shield *= num;
    }
};
