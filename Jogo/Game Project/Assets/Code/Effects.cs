using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effect")]
public class Effects : ScriptableObject
{
    public string id;
    public Sprite sprite;

    public int duration;
    public int timesInc = 0;

    public bool canUsePhysical = true;
    public bool canUseRanged = true;
    public bool canUseMagic = true;
    public bool canUseSupp = true;
    public bool canUseProtec = true;
    public bool canUseStatMod = true;
    //public bool canUseDeploy = true;

    public int phyDmgMin;
    public int phyDmgMax;
    public int phyDmgInc;

    public int magicDmgMin;
    public int magicDmgMax;
    public int magicDmgInc;

    public int trueDmgMin;
    public int trueDmgMax;
    public int trueDmgInc;

    public int sanityDmgMin;
    public int sanityDmgMax;
    public int sanityDmgInc;

    public int sanityHealMin;
    public int sanityHealMax;
    public int sanityHealInc;

    public int healMin;
    public int healMax;
    public int healInc;

    public int healManaMin;
    public int healManaMax;
    public int healManaInc;

    public int healStaminaMin;
    public int healStaminaMax;
    public int healStaminaInc;

    public int shieldMin;
    public int shieldMax;
    public int shieldInc;

    public float cancelAtkChance;
    public int recoil;

    public List<StatScale> scale = new List<StatScale>();
    public List<StatMod> statMods = new List<StatMod>();

    public Effects ReturnEffect()
    {
        Effects effect = CreateInstance<Effects>();

        effect.name = name;
        effect.id = id;
        effect.sprite = sprite;
        effect.timesInc = timesInc;

        effect.duration = duration;

        effect.canUsePhysical = canUsePhysical;
        effect.canUseRanged = canUseRanged;
        effect.canUseMagic = canUseMagic;
        effect.canUseSupp = canUseSupp;
        effect.canUseProtec = canUseProtec;
        effect.canUseStatMod = canUseStatMod;
        //effect.canUseDeploy = canUseDeploy;

        effect.phyDmgMin = phyDmgMin;
        effect.phyDmgMax = phyDmgMax;
        effect.phyDmgInc = phyDmgInc;

        effect.magicDmgMin = magicDmgMin;
        effect.magicDmgMax = magicDmgMax;
        effect.magicDmgInc = magicDmgInc;

        effect.trueDmgMin = trueDmgMin;
        effect.trueDmgMax = trueDmgMax;
        effect.trueDmgInc = trueDmgInc;

        effect.sanityDmgMin = sanityDmgMin;
        effect.sanityDmgMax = sanityDmgMax;
        effect.sanityDmgInc = sanityDmgInc;

        effect.sanityHealMin = sanityHealMin;
        effect.sanityHealMax = sanityHealMax;
        effect.sanityHealInc = sanityHealInc;

        effect.healMin = healMin;
        effect.healMax = healMax;
        effect.healInc = healInc;

        effect.healManaMin = healManaMin;
        effect.healManaMax = healManaMax;
        effect.healManaInc = healManaInc;

        effect.healStaminaMin = healStaminaMin;
        effect.healStaminaMax = healStaminaMax;
        effect.healStaminaInc = healStaminaInc;

        effect.shieldMin = shieldMin;
        effect.shieldMax = shieldMax;
        effect.shieldInc = shieldInc;

        effect.cancelAtkChance = cancelAtkChance;
        effect.recoil = recoil;

        effect.scale = scale;
        effect.statMods = statMods;

        foreach(StatMod a in statMods)
        {
            a.time = duration;
        }

        return effect;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        return builder;
    }

    private StringBuilder GetEffect(LanguageManager languageManager, string language, string whatIs, string effect)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "effect", whatIs, effect));

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs, float val)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));
        builder.Replace("%val%", val.ToString());

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs, string colour, float val)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        builder.Replace("%val%", val.ToString());
        builder.Replace("%chance%", val.ToString());

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs, string colour, float val, string whatIsOp, float valOp)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%val%", val.ToString());
        builder.Replace("%chance%", (val*100).ToString());

        if (valOp > 0)
            builder.Replace("%optional%", GetInfo(languageManager, language, whatIsOp, valOp).ToString());
        else
            builder.Replace("%optional%", "");

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        return builder;
    }

    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs, string colour, float valmin, float valmax, float valinc, string scale)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        if (valmax > valmin)
            builder.Replace("%val%", (valmin + "-" + valmax + "%val%").ToString());
        else
            builder.Replace("%val%", (valmin + "%val%").ToString());

        if (valinc > 0)
            builder.Replace("%val%", (GetInfo(languageManager, language, "increasedmg", colour, valinc) + "%val%").ToString());

        if (scale != "")
            builder.Replace("%val%", scale);
        else
            builder.Replace("%val%", "");

        //builder.Remove(builder.Length-1, 1);
        builder.Append(GetInfo(languageManager, language, "eachturn").ToString()).AppendLine();

        return builder;
    }
    
    private StringBuilder GetInfo(LanguageManager languageManager, string language, string whatIs, string colour)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(languageManager.GetText(language, "showdetail", whatIs));

        builder.Replace("%c%", "<color=#" + colour + ">");
        builder.Replace("%c/%", "</color>");

        return builder;
    }

    private string GetLanguage()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().language;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().language;
        else
            return null;
    }

    private LanguageManager GetLanguageMan()
    {
        if (GameObject.Find("GameManager").GetComponent<CharcSelectLang>())
            return GameObject.Find("GameManager").GetComponent<CharcSelectLang>().languageManager;
        else if (GameObject.Find("GameManager").GetComponent<FightLang>())
            return GameObject.Find("GameManager").GetComponent<FightLang>().languageManager;
        else
            return null;
    }

    public string GetEffectInfo()
    {
        LanguageManager languageManager = GetLanguageMan();
        string language = GetLanguage();
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=25><align=center>").Append(GetEffect(languageManager, language, "name", id.ToLower())).Append("</align></size>").AppendLine();
        builder.Append("<size=19><align=center><color=#B2B2B2>").Append(languageManager.GetText(language, "effect", "title")).Append("</color></align></size>").AppendLine();
        builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
        if (!canUseMagic || !canUsePhysical || !canUseProtec || !canUseRanged || !canUseStatMod || !canUseSupp)
        {
            builder.Append(GetInfo(languageManager, language, "cantuse")).AppendLine();
            if (!canUseMagic)
                builder.Append(GetInfo(languageManager, language, "cantmagic", "1a66ff")).AppendLine();

            if (!canUsePhysical)
                builder.Append(GetInfo(languageManager, language, "cantphysical", "ffaa00")).AppendLine();

            if (!canUseRanged)
                builder.Append(GetInfo(languageManager, language, "cantranged", "f75145")).AppendLine();

            if (!canUseProtec)
                builder.Append(GetInfo(languageManager, language, "cantdefence", "787878")).AppendLine();

            if (!canUseStatMod)
                builder.Append(GetInfo(languageManager, language, "cantstatmod", "f0dd0a")).AppendLine();

            if (!canUseSupp)
                builder.Append(GetInfo(languageManager, language, "cantsupport", "00ff11")).AppendLine();

            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
        }
        bool showDmg = false;

        if (phyDmgMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.PHYSICAL)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "takephysicdmg", "ffaa00", phyDmgMin, phyDmgMax, phyDmgInc, tempB.ToString()));
        }

        if (magicDmgMin > 0)
        {
            showDmg = true;
            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.MAGICAL)
                    tempB.Append(a.GetStatScaleInfo());
            }

            builder.Append(GetInfo(languageManager, language, "takemagicdmg", "1a66ff", magicDmgMin, magicDmgMax, magicDmgInc, tempB.ToString()));
        }

        if (trueDmgMin > 0)
        {
            showDmg = true;
            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.TRUE)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "taketruedmg", "a6a6a6", trueDmgMin, trueDmgMax, trueDmgInc, tempB.ToString()));
        }

        if (sanityDmgMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.SANITY)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "takesanitydmg", "b829ff", sanityDmgMin, sanityDmgMax, sanityDmgInc, tempB.ToString()));
        }

        if (healMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEAL)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "heal", "00ff11", healMin, healMax, healInc, tempB.ToString()));
        }

        if (healManaMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALMANA)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "healmana", "1e68fc", healManaMin, healManaMax, healManaInc, tempB.ToString()));
        }

        if (healStaminaMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALSTAMINA)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "healstamina", "f0dd0a", healStaminaMin, healStaminaMax, healStaminaInc, tempB.ToString()));
        }

        if (sanityHealMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.HEALSANITY)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "healsanity", "b641f0", sanityHealMin, sanityHealMax, sanityHealInc, tempB.ToString()));
        }

        if (shieldMin > 0)
        {
            showDmg = true;

            StringBuilder tempB = new StringBuilder();
            foreach (StatScale a in scale)
            {
                if (a.type is StatScale.DmgType.SHIELD)
                    tempB.Append(a.GetStatScaleInfo());
            }
            builder.Append(GetInfo(languageManager, language, "shield", "787878", shieldMin, shieldMax, shieldInc, tempB.ToString()));
        }

        if (showDmg && (cancelAtkChance > 0 || statMods.Count > 0))
            builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();

        if (cancelAtkChance > 0)
        {
            builder.Append(GetInfo(languageManager, language, "chancetostop", "a6a6a6", cancelAtkChance, "chancetostopoptional", recoil)).AppendLine();
            if (statMods.Count > 0)
                builder.Append("<s><align=center>").Append("|                 |").Append("</align></s>").AppendLine();
        }

        if (statMods.Count > 0)
        {
            foreach (StatMod a in statMods)
            {
                builder.Append(a.GetStatModInfo(true));
            }
        }

        return builder.ToString();
    }
}
