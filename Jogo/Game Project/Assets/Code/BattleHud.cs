using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;

    [SerializeField] private Text hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillHp;
    [SerializeField] private Gradient gradientHp;
    [SerializeField] private TooltipButton hpInfo;

    [SerializeField] private Text manaText;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Image fillMana;
    [SerializeField] private Gradient gradientMana;
    [SerializeField] private TooltipButton manaInfo;

    [SerializeField] private Text staminaText;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillStamina;
    [SerializeField] private Gradient gradientStamina;
    [SerializeField] private TooltipButton staminaInfo;

    [SerializeField] private Text shieldText;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Image fillShield;
    [SerializeField] private TooltipButton shieldInfo;

    [SerializeField] private Slider ultSlider;
    [SerializeField] private TooltipButton ultInfo;

    [SerializeField] private Slider bloodSlider;
    [SerializeField] private Gradient gradientBlood;
    [SerializeField] private Image fillBlood;

    [SerializeField] private GameObject statsGO;

    [SerializeField] private Sprite sanity100;
    [SerializeField] private Sprite sanity75;
    [SerializeField] private Sprite sanity50;
    [SerializeField] private Sprite sanity25;

    private FightLang langmang;
    private string language;

    void Awake()
    {
        langmang = GameObject.Find("GameManager").GetComponent<FightLang>();
        language = langmang.language;

        shieldSlider.maxValue = GameObject.Find("GameManager").GetComponent<BattleSystem>().maxShield;
        bloodSlider.value = 0;
        ultSlider.value = 0;
        shieldSlider.value = 0;
    }

    public void SetHud (Unit unit, int staminaTired, GameObject statsGO)
    {
        this.statsGO = statsGO;
        language = langmang.language;
        icon.sprite = unit.charc.charcIcon;
        nameText.text = langmang.languageManager.GetText(language, "charc", "name", unit.charc.name);
        levelText.text = langmang.languageManager.GetText(language, "gui", "text", "level")+unit.level;

        hpText.text = unit.curHp + "/" + unit.charc.stats.hp;
        hpSlider.maxValue = unit.SetModifiers().hp;
        hpSlider.value = unit.curHp;
        fillHp.color = gradientHp.Evaluate(1f);
        hpInfo.text = langmang.languageManager.GetText(language, "stats", "name", "hp");
        hpInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "healbonus") + ": " + unit.SetModifiers().healBonus.ToString("0.00") + "%";

        manaText.text = unit.curMana + "/" + unit.charc.stats.mana;
        manaSlider.maxValue = unit.SetModifiers().mana;
        manaSlider.value = unit.curMana;
        fillMana.color = gradientMana.Evaluate(1f);
        manaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "mana");
        manaInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "manacost") + ": " + unit.SetModifiers().manaCost.ToString("0.00") + "%";

        staminaText.text = unit.curStamina + "/" + unit.charc.stats.stamina;
        staminaSlider.maxValue = unit.SetModifiers().stamina;
        staminaSlider.value = unit.curStamina;
        fillStamina.color = gradientStamina.Evaluate(1f);

        staminaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "stamina");
        staminaInfo.text += "\n" + langmang.languageManager.GetText(language, "gui", "text", "staminatired");
        staminaInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "staminacost") + ": " + unit.SetModifiers().staminaCost.ToString("0.00") + "%";
        staminaInfo.text = staminaInfo.text.Replace("%v%", staminaTired.ToString());

        shieldText.text = unit.curShield.ToString();
        shieldSlider.value = unit.curShield;
        shieldInfo.text = langmang.languageManager.GetText(language, "gui", "text", "shield");
        shieldInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "shieldbonus") + ": " + unit.SetModifiers().shieldBonus.ToString("0.00") + "%";

        ultInfo.text = langmang.languageManager.GetText(language, "gui", "text", "ultimate");
        ultInfo.text = ultInfo.text.Replace("%v%", ultSlider.value.ToString("0.00") +"%");
        ultInfo.text = ultInfo.text.Replace("%r%", unit.SetModifiers().ultrate.ToString("0.00") +"%");

        bloodSlider.value = unit.bloodStacks;
    }

    public void SetStatsHud(Unit user)
    {
        Stats stats = user.SetModifiers();
        user.LoadSize(user.size + stats.sizeMod);
        SetStats(stats.ReturnStats(), user.charc.stats.ReturnStats(), user.curSanity);
    }

    public void SetStats(Stats statsTemp, Stats originalTemp, int curSanity)
    {
        Text hp = statsGO.transform.Find("HP").GetComponent<Text>();
        Text hpRegen = statsGO.transform.Find("HP REGEN").GetComponent<Text>();
        Text mana = statsGO.transform.Find("MN").GetComponent<Text>();
        Text manaRegen = statsGO.transform.Find("MN REGEN").GetComponent<Text>();
        Text stamina = statsGO.transform.Find("STA").GetComponent<Text>();
        Text staminaRegen = statsGO.transform.Find("STA REGEN").GetComponent<Text>();
        Text sanity = statsGO.transform.Find("SANITY").GetComponent<Text>();
        Text dmgResis = statsGO.transform.Find("DEF").GetComponent<Text>();
        Text magicResis = statsGO.transform.Find("MR").GetComponent<Text>();
        Text atkDmg = statsGO.transform.Find("ATK").GetComponent<Text>();
        Text mp = statsGO.transform.Find("MP").GetComponent<Text>();
        Text critChance = statsGO.transform.Find("CRIT CHANCE").GetComponent<Text>();
        Text critDmg = statsGO.transform.Find("CRIT DMG").GetComponent<Text>();
        Text timing = statsGO.transform.Find("TIMING").GetComponent<Text>();
        Text movSpeed = statsGO.transform.Find("MOV").GetComponent<Text>();
        Text evasion = statsGO.transform.Find("EVA").GetComponent<Text>();
        Text lifesteal = statsGO.transform.Find("LIFESTEAL").GetComponent<Text>();
        Text accuracy = statsGO.transform.Find("ACC").GetComponent<Text>();
        Text armourpen = statsGO.transform.Find("ARM PEN").GetComponent<Text>();
        Text magicpen = statsGO.transform.Find("MAGIC PEN").GetComponent<Text>();
        Text healbonus = statsGO.transform.Find("HEALBONUS").GetComponent<Text>();
        Text shieldbonus = statsGO.transform.Find("SHIELD BONUS").GetComponent<Text>();
        Text manacost = statsGO.transform.Find("MN COST").GetComponent<Text>();
        Text staminacost = statsGO.transform.Find("STA COST").GetComponent<Text>();

        Image sanityIcon = statsGO.transform.Find("SANITY").GetChild(0).GetComponent<Image>();

        Stats stats = statsTemp.ReturnStats();
        Stats original = originalTemp.ReturnStats();

        hp.text = original.hp.ToString("0") + "+(" + (stats.hp - original.hp).ToString("0") + ")";

        hpRegen.text = original.hpRegen.ToString("0.0") + "+(" + (stats.hpRegen - original.hpRegen).ToString("0.0") + ")";

        mana.text = original.mana.ToString("0") + "+(" + (stats.mana - original.mana).ToString("0") + ")";

        manaRegen.text = original.manaRegen.ToString("0.0") + "+(" + (stats.manaRegen - original.manaRegen).ToString("0.0") + ")";

        stamina.text = original.stamina.ToString("0") + "+(" + (stats.stamina - original.stamina).ToString("0") + ")";

        staminaRegen.text = original.staminaRegen.ToString("0.0") + "+(" + (stats.staminaRegen - original.staminaRegen).ToString("0.0") + ")";

        sanity.text = curSanity.ToString("0") + "/" + stats.sanity.ToString("0");

        dmgResis.text = original.dmgResis.ToString("0") + "+(" + (stats.dmgResis - original.dmgResis).ToString("0") + ")";

        magicResis.text = original.magicResis.ToString("0") + "+(" + (stats.magicResis - original.magicResis).ToString("0") + ")";

        atkDmg.text = original.atkDmg.ToString("0") + "+(" + (stats.atkDmg - original.atkDmg).ToString("0") + ")";


        mp.text = original.magicPower.ToString("0") + "+(" + (stats.magicPower - original.magicPower).ToString("0") + ")";


        if (stats.critChance > 0)
            critChance.text = original.critChance.ToString("0.0%") + "+(" + (stats.critChance - original.critChance).ToString("0.0%") + ")";
        else
            critChance.text = 0 + "%"; 

        critDmg.text = original.critDmg.ToString("0.0%") + "+(" + (stats.critDmg - original.critDmg).ToString("0.0%") + ")";

        float timingVal = stats.timing;

        if (stats.timing > 5)
            timingVal = 5;

        timing.text = original.timing.ToString("0.00") + "+(" + (timingVal - original.timing).ToString("0.00") + ")";

        movSpeed.text = original.movSpeed.ToString("0") + "+(" + (stats.movSpeed - original.movSpeed).ToString("0") + ")";

        if ((stats.movSpeed * 0.035) + (timingVal * 0.5) + (curSanity * 0.01) + stats.evasion > 0)
            evasion.text = ((stats.movSpeed * 0.035) + (timingVal * 0.5) + (curSanity * 0.01)).ToString("0.00")+"%" + "+(" + stats.evasion.ToString("0.00") + "%)";
        else
            evasion.text =  0 + "%";

        lifesteal.text = original.lifesteal.ToString("0.0%") + "+(" + (stats.lifesteal - original.lifesteal).ToString("0.0%") + ")";

        accuracy.text = original.accuracy.ToString("0.0%") + "+(" + (stats.accuracy - original.accuracy).ToString("0.0%") + ")";

        if (stats.sanity > 0)
        {
            float sanityPer = (curSanity * 100) / stats.sanity;

            if (sanityPer > 75)
                sanityIcon.sprite = sanity100;
            else if (sanityPer <= 75 && sanityPer > 50)
                sanityIcon.sprite = sanity75;
            else if (sanityPer <= 50 && sanityPer > 25)
                sanityIcon.sprite = sanity50;
            else if (sanityPer <= 25)
                sanityIcon.sprite = sanity25;
        } else
        {
            sanityIcon.sprite = sanity100;
        }

        armourpen.text = original.armourPen.ToString("0.0%") + "+(" + (stats.armourPen - original.armourPen).ToString("0.0%") + ")";

        healbonus.text = original.healBonus.ToString("0.0%") + "+(" + (stats.healBonus - original.healBonus).ToString("0.0%") + ")";
        
        shieldbonus.text = original.shieldBonus.ToString("0.0%") + "+(" + (stats.shieldBonus - original.shieldBonus).ToString("0.0%") + ")";
        
        magicpen.text = original.magicPen.ToString("0.0%") + "+(" + (stats.magicPen - original.magicPen).ToString("0.0%") + ")";

        manacost.text = original.manaCost.ToString("0.0%") + "+(" + (stats.manaCost - original.manaCost).ToString("0.0%") + ")";

        staminacost.text = original.staminaCost.ToString("0.0%") + "+(" + (stats.staminaCost - original.staminaCost).ToString("0.0%") + ")";
    }

    public void OpenStatsMenu()
    {
        statsGO.SetActive(true);
    }

    public void CloseStatsMenu()
    {
        statsGO.SetActive(false);
    }

    public IEnumerator SetHp (float hp, float maxHp, float healBonus, float shield)
    {
        if (hp < 0)
            hp = 0;

        /*lerpTimer = 0f;
        while (lerpTimer < chipSpeed)
        {
            lerpTimer += Time.deltaTime;
            float curHp = hpSlider.value;

            if (curHp > hp)
            {
                float completedPercent = lerpTimer / chipSpeed;
                hpSlider.value = Mathf.Lerp(curHp, hp, completedPercent);
                hpText.text = hpSlider.value.ToString("0") + "/" + maxHp;
            }
            
            if (curHp < hp)
            {
                float completedPercent = lerpTimer / chipSpeed;
                hpSlider.value = Mathf.Lerp(hp, curHp, completedPercent);
                hpText.text = hpSlider.value.ToString("0") + "/" + maxHp;
            }
        }*/

        yield return null;
        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;
        hpText.text = (hpSlider.value+shield).ToString("0") + "/" + maxHp;
        hpInfo.text = langmang.languageManager.GetText(language, "stats", "name", "hp");
        hpInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "healbonus") + ": " + healBonus.ToString("0.00") + "%";
        fillHp.color = gradientHp.Evaluate(hpSlider.normalizedValue);
    }

    public IEnumerator SetMana(float mana, float maxMana, float manaCost)
    {
        if (mana < 0)
            mana = 0;

        /*lerpTimer = 0f;
        while (lerpTimer < chipSpeed)
        {
            lerpTimer += Time.deltaTime;
            float curMana = manaSlider.value;

            if (manaSlider.value > mana)
            {
                float completedPercent = lerpTimer / chipSpeed;
                manaSlider.value = Mathf.Lerp(curMana, mana, completedPercent);
                manaText.text = manaSlider.value.ToString("0") + "/" + maxMana;
            }

            if (manaSlider.value < mana)
            {
                float completedPercent = lerpTimer / chipSpeed;
                manaSlider.value = Mathf.Lerp(mana, curMana, completedPercent);
                manaText.text = manaSlider.value.ToString("0") + "/" + maxMana;
            }
        }*/

        yield return null;
        manaSlider.maxValue = maxMana;
        manaSlider.value = mana;
        manaText.text = manaSlider.value.ToString("0") + "/" + maxMana;
        manaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "mana");
        manaInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "manacost") + ": " + manaCost.ToString("0.00") + "%";
        fillMana.color = gradientMana.Evaluate(manaSlider.normalizedValue);
    }

    public IEnumerator SetStamina(float stamina, float maxStamina, int staminaTired, float staminaCost)
    {
        if (stamina < 0)
            stamina = 0;

        /*lerpTimer = 0f;
        while (lerpTimer < chipSpeed)
        {
            lerpTimer += Time.deltaTime;
            float curStamina = staminaSlider.value;

            if (staminaSlider.value > stamina)
            {
                float completedPercent = lerpTimer / chipSpeed;
                staminaSlider.value = Mathf.Lerp(curStamina, stamina, completedPercent);
                staminaText.text = staminaSlider.value.ToString("0") + "/" + maxStamina;
            }

            if (staminaSlider.value < stamina)
            {
                float completedPercent = lerpTimer / chipSpeed;
                staminaSlider.value = Mathf.Lerp(stamina, curStamina, completedPercent);
                staminaText.text = staminaSlider.value.ToString("0") + "/" + maxStamina;
            }
        }*/

        yield return null;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        staminaText.text = staminaSlider.value.ToString("0") + "/" + maxStamina; 
        staminaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "stamina");
        staminaInfo.text += "\n" + langmang.languageManager.GetText(language, "gui", "text", "staminatired");
        staminaInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "staminacost") + ": " + staminaCost.ToString("0.00") + "%";
        staminaInfo.text = staminaInfo.text.Replace("%v%", staminaTired.ToString());
        fillStamina.color = gradientStamina.Evaluate(staminaSlider.normalizedValue);
    }

    public IEnumerator SetShield(float shield, float shieldBonus)
    {
        if (shield < 0)
            shield = 0;

        /*lerpTimer = 0f;
        while (lerpTimer < chipSpeed)
        {
            lerpTimer += Time.deltaTime;
            float curShield = shieldSlider.value;

            if (shieldSlider.value > shield)
            {
                float completedPercent = lerpTimer / chipSpeed;
                shieldSlider.value = Mathf.Lerp(curShield, shield, completedPercent);
                shieldText.text = shieldSlider.value.ToString("0");
            }

            if (shieldSlider.value < shield)
            {
                float completedPercent = lerpTimer / chipSpeed;
                shieldSlider.value = Mathf.Lerp(shield, curShield, completedPercent);
                shieldText.text = shieldSlider.value.ToString("0");
            }
        }*/

        yield return null;
        shieldSlider.value = shield;
        shieldText.text = shieldSlider.value.ToString("0");
        shieldInfo.text = langmang.languageManager.GetText(language, "gui", "text", "shield");
        shieldInfo.text += "\n" + langmang.languageManager.GetText(language, "stats", "name", "shieldbonus") + ": " + shieldBonus.ToString("0.00") + "%";
    }

    public void SetUlt(float value, float ultrate)
    {
        ultSlider.value = value;
        ultInfo.text = langmang.languageManager.GetText(language, "gui", "text", "ultimate");
        ultInfo.text = ultInfo.text.Replace("%v%", ultSlider.value.ToString("0.00") + "%");
        ultInfo.text = ultInfo.text.Replace("%r%", (ultrate*100).ToString("0.00") + "%");
    }

    public void SetBlood(float value)
    {
        if (value > 0)
            bloodSlider.value = value+1;
        else
            bloodSlider.value = value;

        fillBlood.color = gradientBlood.Evaluate(bloodSlider.normalizedValue);
    }
}