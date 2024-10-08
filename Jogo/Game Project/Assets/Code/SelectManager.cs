﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LanguageManager;

public class SelectManager : MonoBehaviour
{
    public int charcLevel = 30;
    [SerializeField] private GameObject alex;
    [SerializeField] private GameObject leowind;
    [SerializeField] private GameObject bonsour;
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject sandew;
    [SerializeField] private GameObject isadoe;
    [SerializeField] private GameObject william;
    [SerializeField] private GameObject hestia;
    [SerializeField] private GameObject icer;
    [SerializeField] private GameObject enya;
    [SerializeField] private GameObject arts;

    [SerializeField] private Text nameDisplay;
    [SerializeField] private Text titleDisplay;
    [SerializeField] private GameObject passiveList;
    [SerializeField] private GameObject passivePrefab;

    [SerializeField] private GameObject moveHud;
    [SerializeField] private GameObject moveList;
    [SerializeField] private GameObject movePrefab;
    [SerializeField] private GameObject ultInfo;

    [SerializeField] private TooltipPopUp tooltipPopup;

    private SpriteRenderer alexRender, leowindRender, bonsourRender, shineRender, sandewRender, isadoeRender, williamRender, hestiaRender, icerRender, enyaRender;
    [SerializeField] private Sprite marksmanIcon, sourcererIcon, assassinIcon, tankIcon, duelistIcon, supportIcon;
    [SerializeField] private GameObject statsDisplay;
    [SerializeField] private Image classIcon;
    [SerializeField] private Image charcIcon;

    [SerializeField] private Sprite phyAtk;
    [SerializeField] private Sprite magiAtk;
    [SerializeField] private Sprite rangeAtk;
    [SerializeField] private Sprite suppAtk;
    [SerializeField] private Sprite defAtk;
    [SerializeField] private Sprite statAtk;
    [SerializeField] private Sprite summonAtk;

    private CharcSelectLang thisLangManager;
    [SerializeField] Text curCharacterTxt;

    private int i = 1;
    private int max = 0;
    private int characterId = 1;
    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string isPlayerChamp = "isPlayerChamp";
    private readonly string isEnemyChamp = "isEnemyChamp";
    private readonly string selectedItem1 = "selectedItem1";
    private readonly string selectedItem2 = "selectedItem2";
    private readonly string selectedLevel = "selectedLevel";

    [SerializeField] private GameObject itemSelect;
    [SerializeField] private GameObject item1;
    [SerializeField] private GameObject item2;

    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;

    private void Awake()
    {
        thisLangManager = this.gameObject.GetComponent<CharcSelectLang>();

        alexRender = alex.GetComponent<SpriteRenderer>();
        max++;
        leowindRender = leowind.GetComponent<SpriteRenderer>();
        max++;
        bonsourRender = bonsour.GetComponent<SpriteRenderer>();
        max++;
        shineRender = shine.GetComponent<SpriteRenderer>();
        max++;
        sandewRender = sandew.GetComponent<SpriteRenderer>();
        max++;
        isadoeRender = isadoe.GetComponent<SpriteRenderer>();
        max++;
        williamRender = william.GetComponent<SpriteRenderer>();
        max++;
        hestiaRender = hestia.GetComponent<SpriteRenderer>();
        max++;
        icerRender = icer.GetComponent<SpriteRenderer>();
        max++;
        enyaRender = enya.GetComponent<SpriteRenderer>();
        max++;

        alexRender.enabled = true;
        SetCharacterNumber();
    }

    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        if (PlayerPrefs.GetInt("isEndless") != 0)
        {
            itemSelect.SetActive(false);
            charcLevel = 0;
        }

        SetText(i);
        SetPassives(i);
        SetMoves(i);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextCharacter();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousCharacter();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ManageMoveInfoHud();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ChangeScene();
        }
    }

    public void SetPassives(int n)
    {
        foreach (Transform child in passiveList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        switch (n)
        {
            case 1:
                foreach (Passives a in alex.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 2:
                foreach (Passives a in leowind.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 3:
                foreach (Passives a in bonsour.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 4:
                foreach (Passives a in shine.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 5:
                foreach (Passives a in sandew.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 6:
                foreach (Passives a in isadoe.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 7:
                foreach (Passives a in william.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 8:
                foreach (Passives a in hestia.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 9:
                foreach (Passives a in icer.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
            case 10:
                foreach (Passives a in enya.GetComponent<CharacterInfo>().character.passives)
                {
                    Image icon = passivePrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                    icon.sprite = a.sprite;

                    TooltipButton tooltipButton = passivePrefab.transform.GetComponent<TooltipButton>();
                    tooltipButton.tooltipPopup = tooltipPopup;
                    tooltipButton.text = a.GetPassiveInfo();

                    Instantiate(passivePrefab, passiveList.transform);
                }
                break;
        }
    }

    private void SetupMoves(GameObject charac)
    {
        int i = 1;
        foreach (Moves a in charac.GetComponent<CharacterInfo>().character.moves)
        {
            if (i > 1)
            {
                if (a.name != "Recover Mana")
                    ShowMove(a);
                else
                    charac.GetComponent<CharacterInfo>().character.moves.Remove(a);
            }
            i++;
        }
        ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
        ultInfo.GetComponent<TooltipButton>().text = charac.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText(true);
    }

    public void SetMoves(int n)
    {
        foreach (Transform child in moveList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        try
        {
            switch (n)
            {
                case 1:
                    SetupMoves(alex);
                    break;
                case 2:
                    SetupMoves(leowind);
                    break;
                case 3:
                    SetupMoves(bonsour);
                    break;
                case 4:
                    SetupMoves(shine);
                    break;
                case 5:
                    SetupMoves(sandew);
                    break;
                case 6:
                    SetupMoves(isadoe);
                    break;
                case 7:
                    SetupMoves(william);
                    break;
                case 8:
                    SetupMoves(hestia);
                    break;
                case 9:
                    SetupMoves(icer);
                    break;
                case 10:
                    SetupMoves(enya);
                    break;
            }

        }
        catch
        {

        }

    }

    private void ShowMove(Moves a)
    {
        movePrefab.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
        movePrefab.GetComponent<TooltipButton>().text = a.GetTooltipText(true);

        movePrefab.name = a.name;

        Text name = movePrefab.transform.Find("Name").gameObject.GetComponent<Text>();
        name.text = thisLangManager.languageManager.GetText(new ArgumentsFetch(thisLangManager.language, "moves", a.name, ""));

        Text mana = movePrefab.transform.Find("Mana").gameObject.GetComponent<Text>();
        mana.text = a.manaCost.ToString();

        Text stamina = movePrefab.transform.Find("Stamina").gameObject.GetComponent<Text>();
        stamina.text = a.staminaCost.ToString();

        Text cd = movePrefab.transform.Find("Cooldown").gameObject.GetComponent<Text>();
        cd.text = a.cooldown.ToString();

        Image icon = movePrefab.transform.Find("Icon").gameObject.GetComponent<Image>();
        switch (a.type)
        {
            case Moves.MoveType.PHYSICAL:
                icon.sprite = phyAtk;
                break;
            case Moves.MoveType.MAGICAL:
                icon.sprite = magiAtk;
                break;
            case Moves.MoveType.RANGED:
                icon.sprite = rangeAtk;
                break;
            case Moves.MoveType.DEFFENCIVE:
                icon.sprite = defAtk;
                break;
            case Moves.MoveType.SUPPORT:
                icon.sprite = suppAtk;
                break;
            case Moves.MoveType.ENCHANT:
                icon.sprite = statAtk;
                break;
            case Moves.MoveType.SUMMON:
                icon.sprite = summonAtk;
                break;
        }

        Instantiate(movePrefab, moveList.transform);
    }

    private void SetupText(GameObject charc)
    {
        nameDisplay.text = thisLangManager.languageManager.GetText(new ArgumentsFetch(thisLangManager.language, "charc", "name", charc.GetComponent<CharacterInfo>().character.name));
        titleDisplay.text = thisLangManager.languageManager.GetText(new ArgumentsFetch(thisLangManager.language, "charc", "title", charc.GetComponent<CharacterInfo>().character.name));
        charcIcon.sprite = charc.GetComponent<CharacterInfo>().character.charcIcon;
        if (PlayerPrefs.GetInt("isEndless") != 0)
            SetStats(charc.GetComponent<CharacterInfo>().character.GetStatLevel(charcLevel));
        else
            SetStats(charc.GetComponent<CharacterInfo>().character.GetStatLevel(charcLevel));

        SetClassIcon(charc.GetComponent<CharacterInfo>().character.classe);
    }

    public void SetText(int n)
    {
        switch (n)
        {
            case 1:
                SetupText(alex);
                break;
            case 2:
                SetupText(leowind);
                break;
            case 3:
                SetupText(bonsour);
                break;
            case 4:
                SetupText(shine);
                break;
            case 5:
                SetupText(sandew);
                break;
            case 6:
                SetupText(isadoe);
                break;
            case 7:
                SetupText(william);
                break;
            case 8:
                SetupText(hestia);
                break;
            case 9:
                SetupText(icer);
                break;
            case 10:
                SetupText(enya);
                break;
        }
    }

    public void SetClassIcon(Character.Class classe)
    {
        switch (classe)
        {
            case Character.Class.Assassin:
                classIcon.sprite = assassinIcon;
                break;
            case Character.Class.Sourcerer:
                classIcon.sprite = sourcererIcon;
                break;
            case Character.Class.Marksman:
                classIcon.sprite = marksmanIcon;
                break;
            case Character.Class.Support:
                classIcon.sprite = supportIcon;
                break;
            case Character.Class.Duelist:
                classIcon.sprite = duelistIcon;
                break;
            case Character.Class.Tank:
                classIcon.sprite = tankIcon;
                break;
        }

        classIcon.transform.GetComponent<TooltipButton>().text = thisLangManager.languageManager.GetText(new ArgumentsFetch(thisLangManager.language, "class", classe.ToString().ToLower(), ""));
    }

    public void SetStats(Stats statsTemp)
    {
        Text hp = statsDisplay.transform.Find("hp").GetChild(0).GetComponent<Text>();
        Text hpRegen = statsDisplay.transform.Find("hpregen").GetChild(0).GetComponent<Text>();
        Text mana = statsDisplay.transform.Find("mana").GetChild(0).GetComponent<Text>();
        Text manaRegen = statsDisplay.transform.Find("manaregen").GetChild(0).GetComponent<Text>();
        Text stamina = statsDisplay.transform.Find("stamina").GetChild(0).GetComponent<Text>();
        Text staminaRegen = statsDisplay.transform.Find("staminaregen").GetChild(0).GetComponent<Text>();
        Text sanity = statsDisplay.transform.Find("sanity").GetChild(0).GetComponent<Text>();
        Text accuracy = statsDisplay.transform.Find("accuracy").GetChild(0).GetComponent<Text>();
        Text dmgResis = statsDisplay.transform.Find("dmgresis").GetChild(0).GetComponent<Text>();
        Text magicResis = statsDisplay.transform.Find("magicresis").GetChild(0).GetComponent<Text>();
        Text atkDmg = statsDisplay.transform.Find("atkdmg").GetChild(0).GetComponent<Text>();
        Text mp = statsDisplay.transform.Find("mp").GetChild(0).GetComponent<Text>();
        Text critChance = statsDisplay.transform.Find("critchance").GetChild(0).GetComponent<Text>();
        Text critDmg = statsDisplay.transform.Find("critdmg").GetChild(0).GetComponent<Text>();
        Text timing = statsDisplay.transform.Find("timing").GetChild(0).GetComponent<Text>();
        Text movSpeed = statsDisplay.transform.Find("movspeed").GetChild(0).GetComponent<Text>();
        Text evasion = statsDisplay.transform.Find("evasion").GetChild(0).GetComponent<Text>();
        Text lifesteal = statsDisplay.transform.Find("lifesteal").GetChild(0).GetComponent<Text>();

        Stats stats = statsTemp.ReturnStats();

        hp.text = stats.hp.ToString();
        hpRegen.text = stats.hpRegen.ToString();
        mana.text = stats.mana.ToString();
        manaRegen.text = stats.manaRegen.ToString();
        stamina.text = stats.stamina.ToString();
        staminaRegen.text = stats.staminaRegen.ToString();
        sanity.text = stats.sanity.ToString();
        dmgResis.text = stats.dmgResis.ToString();
        magicResis.text = stats.magicResis.ToString();
        atkDmg.text = stats.atkDmg.ToString();
        mp.text = stats.magicPower.ToString();
        critChance.text = (stats.critChance * 100).ToString() + "%";
        critDmg.text = (stats.critDmg * 100).ToString() + "%";
        timing.text = stats.timing.ToString();
        movSpeed.text = stats.movSpeed.ToString();

        lifesteal.text = (stats.lifesteal * 100).ToString() + "%";
        accuracy.text = (stats.accuracy * 100).ToString() + "%";

        if ((stats.movSpeed * 0.02) + (stats.timing * 0.4) + (stats.sanity * 0.09) + stats.evasion > 0)
            evasion.text = ((stats.movSpeed * 0.02) + (stats.timing * 0.4) + (stats.sanity * 0.09) + stats.evasion).ToString("0.00") + "%";
        else
            evasion.text = 0 + "%";
    }

    public void NextCharacter()
    {
        switch (i)
        {
            case 1:
                alexRender.enabled = false;
                leowindRender.enabled = true;
                i++;
                break;
            case 2:
                leowindRender.enabled = false;
                bonsourRender.enabled = true;
                i++;
                break;
            case 3:
                bonsourRender.enabled = false;
                shineRender.enabled = true;
                i++;
                break;
            case 4:
                shineRender.enabled = false;
                sandewRender.enabled = true;
                i++;
                break;
            case 5:
                sandewRender.enabled = false;
                isadoeRender.enabled = true;
                i++;
                break;
            case 6:
                isadoeRender.enabled = false;
                williamRender.enabled = true;
                i++;
                break;
            case 7:
                williamRender.enabled = false;
                hestiaRender.enabled = true;
                i++;
                break;
            case 8:
                hestiaRender.enabled = false;
                icerRender.enabled = true;
                i++;
                break;
            case 9:
                icerRender.enabled = false;
                enyaRender.enabled = true;
                i++;
                break;
            case 10:
                enyaRender.enabled = false;
                alexRender.enabled = true;
                Reset();
                break;
            default:
                Reset();
                break;
        }

        SetText(i);
        SetPassives(i);
        SetMoves(i);
    }

    public void PreviousCharacter()
    {
        switch (i)
        {
            case 1:
                alexRender.enabled = false;
                enyaRender.enabled = true;
                i--;
                Reset();
                break;
            case 2:
                leowindRender.enabled = false;
                alexRender.enabled = true;
                i--;
                break;
            case 3:
                bonsourRender.enabled = false;
                leowindRender.enabled = true;
                i--;
                break;
            case 4:
                shineRender.enabled = false;
                bonsourRender.enabled = true;
                i--;
                break;
            case 5:
                sandewRender.enabled = false;
                shineRender.enabled = true;
                i--;
                break;
            case 6:
                isadoeRender.enabled = false;
                sandewRender.enabled = true;
                i--;
                break;
            case 7:
                williamRender.enabled = false;
                isadoeRender.enabled = true;
                i--;
                break;
            case 8:
                hestiaRender.enabled = false;
                williamRender.enabled = true;
                i--;
                break;
            case 9:
                icerRender.enabled = false;
                hestiaRender.enabled = true;
                i--;
                break;
            case 10:
                enyaRender.enabled = false;
                icerRender.enabled = true;
                i--;
                break;
            default:
                Reset();
                break;
        }

        SetText(i);
        SetPassives(i);
        SetMoves(i);
    }

    public void Return()
    {
        DisablePortraits();
        PlayerPrefs.DeleteKey(selectedCharacter + "1");
        PlayerPrefs.DeleteKey(selectedCharacter + "2");
        PlayerPrefs.DeleteKey(selectedCharacter + "3");
        PlayerPrefs.DeleteKey(selectedEnemy + "1");
        PlayerPrefs.DeleteKey(selectedEnemy + "2");
        PlayerPrefs.DeleteKey(selectedEnemy + "3");
        loader.LoadScene(0, slider, loadPanel);
    }

    private void Reset()
    {
        if (i >= max)
        {
            i = 1;
        }
        else
        {
            i = max;
        }
    }

    IEnumerator WaitWhile()
    {
        yield return null;
    }

    public void DisablePortraits()
    {
        if (arts.activeInHierarchy)
        {
            arts.SetActive(false);
        }
        else
        {
            arts.SetActive(true);
        }

    }

    private void SetCharacterNumber()
    {
        if (PlayerPrefs.HasKey(selectedCharacter + "1"))
            characterId = 2;

        if (PlayerPrefs.HasKey(selectedCharacter + "2"))
            characterId = 3;

        Debug.Log(characterId);
        curCharacterTxt.text = characterId.ToString();
    }

    public void ChangeScene()
    {
        DisablePortraits();
        if (PlayerPrefs.GetInt("isEndless") == 0)
        {
            List<int> list = new List<int>();

            if (PlayerPrefs.HasKey(selectedEnemy + (characterId - 1)))
                list.Add(PlayerPrefs.GetInt(selectedEnemy + (characterId - 1)));

            if (PlayerPrefs.HasKey(selectedEnemy + (characterId - 2)))
                list.Add(PlayerPrefs.GetInt(selectedEnemy + (characterId - 2)));

            if (PlayerPrefs.HasKey(selectedCharacter + (characterId - 1)))
                list.Add(PlayerPrefs.GetInt(selectedCharacter + (characterId - 1)));

            if (PlayerPrefs.HasKey(selectedCharacter + (characterId - 2)))
                list.Add(PlayerPrefs.GetInt(selectedCharacter + (characterId - 2)));

            int random;
            do
            {
                random = Random.Range(1, max + 1);
                StartCoroutine(WaitWhile());
            } while (random == i || list.Contains(random));

            PlayerPrefs.SetInt(selectedLevel, charcLevel);
            PlayerPrefs.SetInt(selectedCharacter + characterId, i);
            PlayerPrefs.SetInt(selectedEnemy+characterId, random);
            PlayerPrefs.SetInt(isPlayerChamp, 1);
            PlayerPrefs.SetInt(isEnemyChamp, 1);
            if (item1.name != "item")
                PlayerPrefs.SetString(selectedItem1 + "_" + characterId, item1.name);

            if (item2.name != "item")
                PlayerPrefs.SetString(selectedItem2 + "_" + characterId, item2.name);

            if (characterId < 3)
                loader.LoadScene(1, slider, loadPanel);
            else
                loader.LoadScene(2, slider, loadPanel);
        }
        else
        {
            PlayerPrefs.SetInt(selectedCharacter, i);
            PlayerPrefs.SetInt(isPlayerChamp, 1);

            loader.LoadScene(3, slider, loadPanel);
        }

    }

    public void ManageMoveInfoHud()
    {
        if (moveHud.activeInHierarchy)
            moveHud.SetActive(false);
        else
            moveHud.SetActive(true);
    }
}
