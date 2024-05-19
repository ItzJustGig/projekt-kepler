using System;
using System.Collections.Generic;

public static class Utils
{
    public enum DmgType { PHYSICAL, MAGICAL, TRUE, SANITY, HEAL, HEALMANA, HEALSTAMINA, HEALSANITY, SHIELD, ULTENEGY };

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
        public float ultenergy;

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
            ultenergy = 0;
        }

        public void ResetOffencive()
        {
            phyDmg = 0;
            magicDmg = 0;
            trueDmg = 0;
            sanityDmg = 0;
        }

        public void ResetHeals()
        {
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
            ultenergy += dmg.ultenergy;
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

        public void ApplyBonusDmg(float bonusPhy, float bonusMag, float bonusHeal, float bonusShield)
        {
            if (phyDmg > 0)
                phyDmg += phyDmg * bonusPhy;

            if (magicDmg > 0)
                magicDmg += magicDmg * bonusMag;

            if (heal > 0)
                heal += heal * bonusHeal;

            if (shield > 0)
                shield += shield * bonusShield;
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

        public void ApplyBonusShield(float bonusShield)
        {
            if (shield > 0)
                shield += shield * bonusShield;
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
            ultenergy += dmg.ultenergy;
            dmg.ultenergy = 0;

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
            ultenergy += move.ultEnergy;
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
            ultenergy *= num;
        }

        public int GetOffenciveNumbers()
        {
            int i = 0;
            if (phyDmg > 0)
                i++;

            if (magicDmg > 0)
                i++;

            if (trueDmg > 0)
                i++;

            if (sanityDmg > 0)
                i++;

            return i;
        }
    };

    private static readonly Dictionary<string, string> Colors = new Dictionary<string, string>
    {
        {"physical", "#ffaa00"},
        {"magical", "#1a66ff"},
        {"true", "#a6a6a6"},
        {"heal", "#00ff11"},
        {"shield", "#787878"},
        {"sanity", "#ffaa00"},
        {"crit", "#f75145"},
        {"enchant", "#CC66FF"},
        {"summon", "#B266FF"},
    }; 
    
    public static string GetColor(string key)
    {
        if (Colors.TryGetValue(key, out var color))
        {
            return color;
        }

        return "#ffffff";
    }
}

