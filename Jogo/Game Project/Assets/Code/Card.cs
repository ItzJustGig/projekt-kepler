using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Character charc;
    public int level;

    [SerializeField] private Color colourBaby;
    [SerializeField] private Color colourWeak;
    [SerializeField] private Color colourNormal;
    [SerializeField] private Color colourStrong;
    [SerializeField] private Color colourSStrong;
    [SerializeField] private Color colourLegend;
    [SerializeField] private Color colourChamp;

    [SerializeField] private Image strenghtColour;
    [SerializeField] private Image charcSprite;
    [SerializeField] private Image class1;
    [SerializeField] private Image class2;
    [SerializeField] private Text cardName;
    [SerializeField] private Text cardLevel;

    [SerializeField] private GameObject bossIcon;

    [SerializeField] private Sprite marksmanIcon, sourcererIcon, assassinIcon, tankIcon, duelistIcon, supportIcon;
    [SerializeField] private EndlessLanguageManager langmanag;

    void Start()
    {
        switch (charc.strenght)
        {
            case Character.Strenght.BABY:
                strenghtColour.color = colourBaby;
                break;
            case Character.Strenght.WEAK:
                strenghtColour.color = colourWeak;
                break;
            case Character.Strenght.NORMAL:
                strenghtColour.color = colourNormal;
                break;
            case Character.Strenght.STRONG:
                strenghtColour.color = colourStrong;
                break;
            case Character.Strenght.SUPERSTRONG:
                strenghtColour.color = colourSStrong;
                break;
            case Character.Strenght.LEGENDARY:
                strenghtColour.color = colourLegend;
                break;
            case Character.Strenght.CHAMPION:
                strenghtColour.color = colourChamp;
                break;
        }

        switch (charc.classe)
        {
            case Character.Class.Tank:
                class1.sprite = tankIcon;
                break;
            case Character.Class.Assassin:
                class1.sprite = assassinIcon;
                break;
            case Character.Class.Sourcerer:
                class1.sprite = sourcererIcon;
                break;
            case Character.Class.Marksman:
                class1.sprite = marksmanIcon;
                break;
            case Character.Class.Support:
                class1.sprite = supportIcon;
                break;
            case Character.Class.Duelist:
                class1.sprite = duelistIcon;
                break;
        }

        Destroy(class2.gameObject);
        cardName.text = langmanag.GetInfo("charc", "name", charc.name);
        cardLevel.text = langmanag.GetInfo("gui", "text", "level")+level;
        charcSprite.sprite = charc.sprite.GetComponent<SpriteRenderer>().sprite;
    }

    public void Boss(){
        bossIcon.SetActive(true);
    }
}
