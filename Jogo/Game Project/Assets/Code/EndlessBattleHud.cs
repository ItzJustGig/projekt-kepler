using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class EndlessBattleHud : MonoBehaviour
{
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

    [SerializeField] private Slider ultSlider;
    [SerializeField] private TooltipButton ultInfo;
    [SerializeField] private Text ultText;

    [SerializeField] private TooltipButton sanityInfo;
    [SerializeField] private Text sanityText;
    [SerializeField] private SpriteRenderer sanityIcon;

    [SerializeField] private Sprite sanity100;
    [SerializeField] private Sprite sanity75;
    [SerializeField] private Sprite sanity50;
    [SerializeField] private Sprite sanity25;

    private FightLang langmang;
    private string language;

    void Awake()
    {
        //langmang = GameObject.Find("GameManager").GetComponent<FightLang>();
        //language = langmang.language;
    }

    public void SetHud(Character charc, EndlessInfo info, int staminaTired)
    {
        //language = langmang.language;

        hpText.text = (charc.stats.hp * info.playerHp).ToString("0") + "/" + charc.stats.hp;
        hpSlider.maxValue = charc.stats.hp;
        hpSlider.value = charc.stats.hp * info.playerHp;
        fillHp.color = gradientHp.Evaluate(1f);
        //hpInfo.text = langmang.languageManager.GetText(language, "stats", "name", "hp");

        manaText.text = (charc.stats.mana * info.playerMn).ToString("0") + "/" + charc.stats.mana;
        manaSlider.maxValue = charc.stats.mana;
        manaSlider.value = charc.stats.mana * info.playerMn;
        fillMana.color = gradientMana.Evaluate(1f);
        //manaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "mana");

        staminaText.text = (charc.stats.stamina * info.playerSta).ToString("0") + "/" + charc.stats.stamina;
        staminaSlider.maxValue = charc.stats.stamina;
        staminaSlider.value = charc.stats.stamina * info.playerSta;
        fillStamina.color = gradientStamina.Evaluate(1f);

        /*staminaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "stamina");
        staminaInfo.text += "\n" + langmang.languageManager.GetText(language, "gui", "text", "staminatired");
        staminaInfo.text = staminaInfo.text.Replace("%v%", staminaTired.ToString());*/

        //ultInfo.text = langmang.languageManager.GetText(language, "gui", "text", "ultimate");
        ultText.text = (info.playerUlt).ToString("0.00")+"%";
        ultSlider.maxValue = 100;
        ultSlider.value = info.playerUlt;

        //sanityInfo.text = langmang.languageManager.GetText(language, "stats", "name", "sanity");
        sanityText.text = (int)(charc.stats.sanity * info.playerSan) + "/" + charc.stats.sanity;
        float sanityPer = info.playerSan*100;

        if (sanityPer > 75)
            sanityIcon.sprite = sanity100;
        else if (sanityPer <= 75 && sanityPer > 50)
            sanityIcon.sprite = sanity75;
        else if (sanityPer <= 50 && sanityPer > 25)
            sanityIcon.sprite = sanity50;
        else if (sanityPer <= 25)
            sanityIcon.sprite = sanity25;
    }
}
