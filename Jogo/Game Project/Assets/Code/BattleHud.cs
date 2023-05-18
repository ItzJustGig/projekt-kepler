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

    public void SetHud (Unit unit, int staminaTired)
    {
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
        //SetStats(stats.ReturnStats(), user.charc.stats.ReturnStats(), user.curSanity);
    }

    public void SetStats(Stats statsTemp, Stats originalTemp, int curSanity)
    {
        Text hp = statsGO.transform.Find("hp").GetChild(0).GetComponent<Text>();
        Text hpRegen = statsGO.transform.Find("hpregen").GetChild(0).GetComponent<Text>();
        Text mana = statsGO.transform.Find("mana").GetChild(0).GetComponent<Text>();
        Text manaRegen = statsGO.transform.Find("manaregen").GetChild(0).GetComponent<Text>();
        Text stamina = statsGO.transform.Find("stamina").GetChild(0).GetComponent<Text>();
        Text staminaRegen = statsGO.transform.Find("staminaregen").GetChild(0).GetComponent<Text>();
        Text sanity = statsGO.transform.Find("sanity").GetChild(0).GetComponent<Text>();
        Text dmgResis = statsGO.transform.Find("dmgresis").GetChild(0).GetComponent<Text>();
        Text magicResis = statsGO.transform.Find("magicresis").GetChild(0).GetComponent<Text>();
        Text atkDmg = statsGO.transform.Find("atkdmg").GetChild(0).GetComponent<Text>();
        Text mp = statsGO.transform.Find("mp").GetChild(0).GetComponent<Text>();
        Text critChance = statsGO.transform.Find("critchance").GetChild(0).GetComponent<Text>();
        Text critDmg = statsGO.transform.Find("critdmg").GetChild(0).GetComponent<Text>();
        Text timing = statsGO.transform.Find("timing").GetChild(0).GetComponent<Text>();
        Text movSpeed = statsGO.transform.Find("movspeed").GetChild(0).GetComponent<Text>();
        Text evasion = statsGO.transform.Find("evasion").GetChild(0).GetComponent<Text>();
        Text lifesteal = statsGO.transform.Find("lifesteal").GetChild(0).GetComponent<Text>();
        Text accuracy = statsGO.transform.Find("accuracy").GetChild(0).GetComponent<Text>();
        Text armourpen = statsGO.transform.Find("armourpen").GetChild(0).GetComponent<Text>();

        Image sanityIcon = statsGO.transform.Find("sanity").GetComponent<Image>();

        Stats stats = statsTemp.ReturnStats();
        Stats original = originalTemp.ReturnStats();

        hp.text = stats.hp.ToString("0");

        if (stats.hp > original.hp)
            hp.color = Color.green;
        else if (stats.hp < original.hp)
            hp.color = Color.red;
        else
            hp.color = Color.black;

        hpRegen.text = stats.hpRegen.ToString("0");

        if (stats.hpRegen > original.hpRegen)
            hpRegen.color = Color.green;
        else if (stats.hpRegen < original.hpRegen)
            hpRegen.color = Color.red;
        else
            hpRegen.color = Color.black;

        mana.text = stats.mana.ToString("0");

        if (stats.mana > original.mana)
            mana.color = Color.green;
        else if (stats.mana < original.mana)
            mana.color = Color.red;
        else
            mana.color = Color.black;

        manaRegen.text = stats.manaRegen.ToString("0.00");

        if (stats.manaRegen > original.manaRegen)
            manaRegen.color = Color.green;
        else if (stats.manaRegen < original.manaRegen)
            manaRegen.color = Color.red;
        else
            manaRegen.color = Color.black;

        stamina.text = stats.stamina.ToString("0");

        if (stats.stamina > original.stamina)
            stamina.color = Color.green;
        else if (stats.stamina < original.stamina)
            stamina.color = Color.red;
        else
            stamina.color = Color.black;

        staminaRegen.text = stats.staminaRegen.ToString("0.00");

        if (stats.staminaRegen > original.staminaRegen)
            staminaRegen.color = Color.green;
        else if (stats.staminaRegen < original.staminaRegen)
            staminaRegen.color = Color.red;
        else
            staminaRegen.color = Color.black;

        sanity.text = curSanity.ToString("0") + "/" + stats.sanity.ToString("0");

        if (stats.sanity > original.sanity)
            sanity.color = Color.green;
        else if (stats.sanity < original.sanity)
            sanity.color = Color.red;
        else
            sanity.color = Color.black;

        dmgResis.text = stats.dmgResis.ToString("0");

        if (stats.dmgResis > original.dmgResis)
            dmgResis.color = Color.green;
        else if (stats.dmgResis < original.dmgResis)
            dmgResis.color = Color.red;
        else
            dmgResis.color = Color.black;

        magicResis.text = stats.magicResis.ToString("0");

        if (stats.magicResis > original.magicResis)
            magicResis.color = Color.green;
        else if (stats.magicResis < original.magicResis)
            magicResis.color = Color.red;
        else
            magicResis.color = Color.black;

        atkDmg.text = stats.atkDmg.ToString("0");

        if (stats.atkDmg > original.atkDmg)
            atkDmg.color = Color.green;
        else if (stats.atkDmg < original.atkDmg)
            atkDmg.color = Color.red;
        else
            atkDmg.color = Color.black;

        mp.text = stats.magicPower.ToString("0");

        if (stats.magicPower > original.magicPower)
            mp.color = Color.green;
        else if (stats.magicPower < original.magicPower)
            mp.color = Color.red;
        else
            mp.color = Color.black;

        if (stats.critChance > 0)
            critChance.text = stats.critChance.ToString("0.0%");
        else
            critChance.text = 0 + "%"; 

        if (stats.critChance > original.critChance)
            critChance.color = Color.green;
        else if (stats.critChance < original.critChance)
            critChance.color = Color.red;
        else
            critChance.color = Color.black;

        critDmg.text = stats.critDmg.ToString("0.0%");

        if (stats.critDmg > original.critDmg)
            critDmg.color = Color.green;
        else if (stats.critDmg < original.critDmg)
            critDmg.color = Color.red;
        else
            critDmg.color = Color.black;

        float timingVal = stats.timing;

        if (stats.timing > 5)
            timingVal = 5;

        timing.text = timingVal.ToString("0.00");

        if (timingVal > original.timing)
            timing.color = Color.green;
        else if (timingVal < original.timing && original.timing <= 5)
            timing.color = Color.red;
        else
            timing.color = Color.black;

        movSpeed.text = stats.movSpeed.ToString("0");

        if (stats.movSpeed > original.movSpeed)
            movSpeed.color = Color.green;
        else if (stats.movSpeed < original.movSpeed)
            movSpeed.color = Color.red;
        else
            movSpeed.color = Color.black;

        if ((stats.movSpeed * 0.035) + (timingVal * 0.5) + (curSanity * 0.01) + stats.evasion > 0)
            evasion.text = ((stats.movSpeed * 0.035) + (timingVal * 0.5) + (curSanity * 0.01) + stats.evasion).ToString("0.00") + "%";
        else
            evasion.text =  0 + "%";

        if (stats.evasion > 0)
            evasion.color = Color.green;
        else if (stats.evasion < 0)
            evasion.color = Color.red;
        else
            evasion.color = Color.black;

        lifesteal.text = stats.lifesteal.ToString("0.00%");

        if (stats.lifesteal > original.lifesteal)
            lifesteal.color = Color.green;
        else if (stats.lifesteal < original.lifesteal)
            lifesteal.color = Color.red;
        else
            lifesteal.color = Color.black;

        accuracy.text = stats.accuracy.ToString("0.0%");

        if (stats.accuracy > original.accuracy)
            accuracy.color = Color.green;
        else if (stats.accuracy < original.accuracy)
            accuracy.color = Color.red;
        else
            accuracy.color = Color.black;

        float sanityPer = (curSanity * 100) / stats.sanity;

        if (sanityPer > 75)
            sanityIcon.sprite = sanity100;
        else if (sanityPer <= 75 && sanityPer > 50)
            sanityIcon.sprite = sanity75;
        else if (sanityPer <= 50 && sanityPer > 25)
            sanityIcon.sprite = sanity50;
        else if (sanityPer <= 25)
            sanityIcon.sprite = sanity25;

        armourpen.text = stats.armourPen.ToString("0.0%") + " | " + stats.magicPen.ToString("0.0%");
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
        hpSlider.maxValue = maxHp+shield;
        hpSlider.value = hp+shield;
        hpText.text = hpSlider.value.ToString("0") + "/" + maxHp;
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