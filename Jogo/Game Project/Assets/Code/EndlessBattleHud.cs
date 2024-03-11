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
    [SerializeField] private Image sanityIcon;

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

    public void SetHud(EndlessInfo info)
    {
        //language = langmang.language;

        hpText.text = (info.player.hp*100).ToString("0.00") + "%";
        hpSlider.maxValue = 1;
        hpSlider.value = info.player.hp;
        fillHp.color = gradientHp.Evaluate(1f);
        //hpInfo.text = langmang.languageManager.GetText(language, "stats", "name", "hp");

        manaText.text = (info.player.mn*100).ToString("0.00") + "%";
        manaSlider.maxValue = 1;
        manaSlider.value = info.player.mn;
        fillMana.color = gradientMana.Evaluate(1f);
        //manaInfo.text = langmang.languageManager.GetText(language, "stats", "name", "mana");

        staminaText.text = (info.player.sta*100).ToString("0.00") + "%";
        staminaSlider.maxValue = 1;
        staminaSlider.value = info.player.sta;
        fillStamina.color = gradientStamina.Evaluate(1f);

        //ultInfo.text = langmang.languageManager.GetText(language, "gui", "text", "ultimate");
        ultText.text = info.player.ult.ToString("0.00")+"%";
        ultSlider.maxValue = 100;
        ultSlider.value = info.player.ult;

        //sanityInfo.text = langmang.languageManager.GetText(language, "stats", "name", "sanity");
        float sanityPer = info.player.san * 100;
        sanityText.text = sanityPer.ToString("0.00") + "%";
        
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
