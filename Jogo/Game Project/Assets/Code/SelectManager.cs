using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private GameObject alex;
    [SerializeField] private GameObject leowind;
    [SerializeField] private GameObject bonsour;
    [SerializeField] private GameObject shine;
    [SerializeField] private GameObject sandew;
    [SerializeField] private GameObject isadoe;
    [SerializeField] private GameObject william;
    [SerializeField] private GameObject hestia;
    [SerializeField] private GameObject icer;
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

    private SpriteRenderer alexRender, leowindRender, bonsourRender, shineRender, sandewRender, isadoeRender, williamRender, hestiaRender, icerRender;
    [SerializeField] private Sprite marksmanIcon, sourcererIcon, vanguardIcon, assassinIcon, tankIcon, brawlerIcon, duelistIcon, supportIcon, enchanterIcon;
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

    [SerializeField] private Text pointsText;
    [SerializeField] private Stats statsGrowth;
    [SerializeField] private Stats statsLevels;
    [SerializeField] private int maxLevel;
    [SerializeField] private int curPoints;
    [SerializeField] private int maxPoints;
    [SerializeField] private GameObject levelBtn;

    [SerializeField] private GameObject statsLevelsBtns;

    private CharcSelectLang thisLangManager;

    private int i = 1;
    private int max = 0;
    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string isPlayerChamp = "isPlayerChamp";
    private readonly string isEnemyChamp = "isEnemyChamp";
    private readonly string selectedItem1 = "selectedItem1";
    private readonly string selectedItem2 = "selectedItem2";

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

        alexRender.enabled = true;
    }

    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();
        SetText(i);
        SetPassives(i);
        SetMoves(i);

        CountPoints();
        pointsText.text = curPoints.ToString() + "/" + maxPoints.ToString();
        LevelSetUp("hp");
        LevelSetUp("hpregen");
        LevelSetUp("mana");
        LevelSetUp("manaregen");
        LevelSetUp("stamina");
        LevelSetUp("staminaregen");
        LevelSetUp("sanity");
        LevelSetUp("atkdmg");
        LevelSetUp("mp");
        LevelSetUp("dmgresis");
        LevelSetUp("magicresis");
        LevelSetUp("movspeed");
        LevelSetUp("timing");
        LevelSetUp("critchance");
        LevelSetUp("critdmg");
        if (curPoints >= maxPoints)
            LockAll();

        if (PlayerPrefs.GetInt("isEndless") != 0)
        {
            itemSelect.SetActive(false);
            levelBtn.SetActive(false);
        }
    }

    void Update()
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
        name.text = thisLangManager.languageManager.GetText(thisLangManager.language, "moves", a.name);

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
            case Moves.MoveType.STATMOD:
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
        nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", charc.GetComponent<CharacterInfo>().character.name);
        titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", charc.GetComponent<CharacterInfo>().character.name);
        charcIcon.sprite = charc.GetComponent<CharacterInfo>().character.charcIcon;
        if (PlayerPrefs.GetInt("isEndless") != 0)
            SetStats(charc.GetComponent<CharacterInfo>().character.stats.ReturnStats());
        else
            SetStats(charc.GetComponent<CharacterInfo>().character.stats.ReturnStatsLevel(statsLevels, statsGrowth));

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
        }
    }

    public void SetClassIcon(Character.Class classe)
    {
        switch (classe)
        {
            case Character.Class.Assassin:
                classIcon.sprite = assassinIcon;
                break;
            case Character.Class.Brawler:
                classIcon.sprite = brawlerIcon;
                break;
            case Character.Class.Vanguard:
                classIcon.sprite = vanguardIcon;
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
            case Character.Class.Enchanter:
                classIcon.sprite = enchanterIcon;
                break;
        }

        classIcon.transform.GetComponent<TooltipButton>().text = thisLangManager.languageManager.GetText(thisLangManager.language, "class", classe.ToString().ToLower());
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

        if ((stats.movSpeed * 0.035) + (stats.timing * 0.5) + (stats.sanity * 0.01) + stats.evasion > 0)
            evasion.text = ((stats.movSpeed * 0.035) + (stats.timing * 0.5) + (stats.sanity * 0.01) + stats.evasion).ToString("0.00") + "%";
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
                alexRender.enabled = true;
                i++;
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
                icerRender.enabled = true;
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

    public void ChangeScene()
    {
        DisablePortraits();
        if (PlayerPrefs.GetInt("isEndless") == 0)
        {
            int random;
            do
            {
                random = Random.Range(1, max + 1);
                StartCoroutine(WaitWhile());
            } while (random == i);

            PlayerPrefs.SetInt(selectedCharacter, i);
            PlayerPrefs.SetInt(selectedEnemy, random);
            PlayerPrefs.SetInt(isPlayerChamp, 1);
            PlayerPrefs.SetInt(isEnemyChamp, 1);
            if (item1.name != "item")
                PlayerPrefs.SetString(selectedItem1, item1.name);

            if (item2.name != "item")
                PlayerPrefs.SetString(selectedItem2, item2.name);

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

    public void ManageLevelHud()
    {
        if (statsLevelsBtns.activeInHierarchy)
        {
            statsLevelsBtns.SetActive(false);
            SetText(i);
        } 
        else
            statsLevelsBtns.SetActive(true);
    }

    private void CountPoints()
    {
        curPoints += (int)statsLevels.hp;
        curPoints += (int)statsLevels.hpRegen;
        curPoints += (int)statsLevels.mana;
        curPoints += (int)statsLevels.manaRegen;
        curPoints += (int)statsLevels.stamina;
        curPoints += (int)statsLevels.staminaRegen;
        curPoints += (int)statsLevels.sanity;
        curPoints += (int)statsLevels.dmgResis;
        curPoints += (int)statsLevels.magicResis;
        curPoints += (int)statsLevels.atkDmg;
        curPoints += (int)statsLevels.magicPower;
        curPoints += (int)statsLevels.movSpeed;
        curPoints += (int)statsLevels.timing;
        curPoints += (int)statsLevels.critChance;
        curPoints += (int)statsLevels.critDmg;
    }

    public void PointsManag(int point)
    {
        int before = curPoints;
        curPoints += point;

        if (curPoints == maxPoints)
        {
            LockAll();
        } else if (before == maxPoints && curPoints < maxPoints)
        {
            LevelSetUp("hp");
            LevelSetUp("hpregen");
            LevelSetUp("mana");
            LevelSetUp("manaregen");
            LevelSetUp("stamina");
            LevelSetUp("staminaregen");
            LevelSetUp("sanity");
            LevelSetUp("atkdmg");
            LevelSetUp("mp");
            LevelSetUp("dmgresis");
            LevelSetUp("magicresis");
            LevelSetUp("movspeed");
            LevelSetUp("timing");
            LevelSetUp("critchance");
            LevelSetUp("critdmg");
        }
    }

    private void LockAll()
    {
        if (statsLevels.hp == 0)
            LockButtons(true, true, "hp");
        else
            LockButtons(true, false, "hp");

        if (statsLevels.hpRegen == 0)
            LockButtons(true, true, "hpregen");
        else
            LockButtons(true, false, "hpregen");

        if (statsLevels.mana == 0)
            LockButtons(true, true, "mana");
        else
            LockButtons(true, false, "mana");

        if (statsLevels.manaRegen == 0)
            LockButtons(true, true, "manaregen");
        else
            LockButtons(true, false, "manaregen");

        if (statsLevels.stamina == 0)
            LockButtons(true, true, "stamina");
        else
            LockButtons(true, false, "stamina");

        if (statsLevels.staminaRegen == 0)
            LockButtons(true, true, "staminaregen");
        else
            LockButtons(true, false, "staminaregen");

        if (statsLevels.sanity == 0)
            LockButtons(true, true, "sanity");
        else
            LockButtons(true, false, "sanity");

        if (statsLevels.atkDmg == 0)
            LockButtons(true, true, "atkdmg");
        else
            LockButtons(true, false, "atkdmg");

        if (statsLevels.magicPower == 0)
            LockButtons(true, true, "mp");
        else
            LockButtons(true, false, "mp");

        if (statsLevels.dmgResis == 0)
            LockButtons(true, true, "dmgresis");
        else
            LockButtons(true, false, "dmgresis");

        if (statsLevels.magicResis == 0)
            LockButtons(true, true, "magicresis");
        else
            LockButtons(true, false, "magicresis");

        if (statsLevels.critChance == 0)
            LockButtons(true, true, "critchance");
        else
            LockButtons(true, false, "critchance");

        if (statsLevels.critDmg == 0)
            LockButtons(true, true, "critdmg");
        else
            LockButtons(true, false, "critdmg");

        if (statsLevels.movSpeed == 0)
            LockButtons(true, true, "movspeed");
        else
            LockButtons(true, false, "movspeed");

        if (statsLevels.timing == 0)
            LockButtons(true, true, "timing");
        else
            LockButtons(true, false, "timing");
    }
    
    private void LockButtons(bool lockPlus, bool lockMinus, string name)
    {
        if (lockPlus)
            statsLevelsBtns.transform.Find(name).transform.Find("+").transform.GetComponent<Button>().interactable = false;
        else
            statsLevelsBtns.transform.Find(name).transform.Find("+").transform.GetComponent<Button>().interactable = true;

        if (lockMinus)
            statsLevelsBtns.transform.Find(name).transform.Find("-").transform.GetComponent<Button>().interactable = false;
        else
            statsLevelsBtns.transform.Find(name).transform.Find("-").transform.GetComponent<Button>().interactable = true;
    }

    public void LevelUp(string name)
    {
        bool ismaxed = false;
        switch (name)
        {
            case "hp":
                statsLevels.hp++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hp.ToString();
                if (statsLevels.hp == maxLevel)
                    ismaxed = true;
                break;
            case "hpregen":
                statsLevels.hpRegen++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hpRegen.ToString();
                if (statsLevels.hpRegen == maxLevel)
                    ismaxed = true;
                break;
            case "mana":
                statsLevels.mana++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.mana.ToString();
                if (statsLevels.mana == maxLevel)
                    ismaxed = true;
                break;
            case "manaregen":
                statsLevels.manaRegen++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.manaRegen.ToString();
                if (statsLevels.manaRegen == maxLevel)
                    ismaxed = true;
                break;
            case "stamina":
                statsLevels.stamina++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.stamina.ToString();
                if (statsLevels.stamina == maxLevel)
                    ismaxed = true;
                break;
            case "staminaregen":
                statsLevels.staminaRegen++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.staminaRegen.ToString();
                if (statsLevels.staminaRegen == maxLevel)
                    ismaxed = true;
                break;
            case "sanity":
                statsLevels.sanity++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.sanity.ToString();
                if (statsLevels.sanity == maxLevel)
                    ismaxed = true;
                break;
            case "atkdmg":
                statsLevels.atkDmg++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.atkDmg.ToString();
                if (statsLevels.atkDmg == maxLevel)
                    ismaxed = true;
                break;
            case "mp":
                statsLevels.magicPower++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicPower.ToString();
                if (statsLevels.magicPower == maxLevel)
                    ismaxed = true;
                break;
            case "dmgresis":
                statsLevels.dmgResis++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.dmgResis.ToString();
                if (statsLevels.dmgResis == maxLevel)
                    ismaxed = true;
                break;
            case "magicresis":
                statsLevels.magicResis++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicResis.ToString();
                if (statsLevels.magicResis == maxLevel)
                    ismaxed = true;
                break;
            case "critchance":
                statsLevels.critChance++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critChance.ToString();
                if (statsLevels.critChance == maxLevel)
                    ismaxed = true;
                break;
            case "critdmg":
                statsLevels.critDmg++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critDmg.ToString();
                if (statsLevels.critDmg == maxLevel)
                    ismaxed = true;
                break;
            case "movspeed":
                statsLevels.movSpeed++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.movSpeed.ToString();
                if (statsLevels.movSpeed == maxLevel)
                    ismaxed = true;
                break;
            case "timing":
                statsLevels.timing++;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.timing.ToString();
                if (statsLevels.timing == maxLevel)
                    ismaxed = true;
                break;
        }

        if (ismaxed)
            LockButtons(true, false, name);
        else
            LockButtons(false, false, name);

        PointsManag(1);
        pointsText.text = curPoints.ToString() + "/" + maxPoints.ToString();
    }

    public void LevelDown(string name)
    {
        bool isreset = false;
        switch (name)
        {
            case "hp":
                statsLevels.hp--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hp.ToString();
                if (statsLevels.hp <= 0)
                    isreset = true;
                break;
            case "hpregen":
                statsLevels.hpRegen--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hpRegen.ToString();
                if (statsLevels.hpRegen <= 0)
                    isreset = true;
                break;
            case "mana":
                statsLevels.mana--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.mana.ToString();
                if (statsLevels.mana <= 0)
                    isreset = true;
                break;
            case "manaregen":
                statsLevels.manaRegen--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.manaRegen.ToString();
                if (statsLevels.manaRegen <= 0)
                    isreset = true;
                break;
            case "stamina":
                statsLevels.stamina--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.stamina.ToString();
                if (statsLevels.stamina <= 0)
                    isreset = true;
                break;
            case "staminaregen":
                statsLevels.staminaRegen--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.staminaRegen.ToString();
                if (statsLevels.staminaRegen <= 0)
                    isreset = true;
                break;
            case "sanity":
                statsLevels.sanity--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.sanity.ToString();
                if (statsLevels.sanity <= 0)
                    isreset = true;
                break;
            case "atkdmg":
                statsLevels.atkDmg--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.atkDmg.ToString();
                if (statsLevels.atkDmg <= 0)
                    isreset = true;
                break;
            case "mp":
                statsLevels.magicPower--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicPower.ToString();
                if (statsLevels.magicPower <= 0)
                    isreset = true;
                break;
            case "dmgresis":
                statsLevels.dmgResis--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.dmgResis.ToString();
                if (statsLevels.dmgResis <= 0)
                    isreset = true;
                break;
            case "magicresis":
                statsLevels.magicResis--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicResis.ToString();
                if (statsLevels.magicResis <= 0)
                    isreset = true;
                break;
            case "critchance":
                statsLevels.critChance--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critChance.ToString();
                if (statsLevels.critChance <= 0)
                    isreset = true;
                break;
            case "critdmg":
                statsLevels.critDmg--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critDmg.ToString();
                if (statsLevels.critDmg <= 0)
                    isreset = true;
                break;
            case "movspeed":
                statsLevels.movSpeed--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.movSpeed.ToString();
                if (statsLevels.movSpeed <= 0)
                    isreset = true;
                break;
            case "timing":
                statsLevels.timing--;
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.timing.ToString();
                if (statsLevels.timing <= 0)
                    isreset = true;
                break;
        }

        if (isreset)
            LockButtons(false, true, name);
        else
            LockButtons(false, false, name);

        PointsManag(-1);
        pointsText.text = curPoints.ToString() + "/" + maxPoints.ToString();
    }

    public void LevelSetUp(string name)
    {
        bool isreset = false;
        bool ismaxed = false;

        switch (name)
        {
            case "hp":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hp.ToString();
                if (statsLevels.hp == 0)
                    isreset = true;
                else if (statsLevels.hp == maxLevel)
                    ismaxed = true;
                break;
            case "hpregen":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.hpRegen.ToString();
                if (statsLevels.hpRegen == 0)
                    isreset = true;
                else if (statsLevels.hpRegen == maxLevel)
                    ismaxed = true;
                break;
            case "mana":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.mana.ToString();
                if (statsLevels.mana == 0)
                    isreset = true;
                else if (statsLevels.mana == maxLevel)
                    ismaxed = true;
                break;
            case "manaregen":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.manaRegen.ToString();
                if (statsLevels.manaRegen == 0)
                    isreset = true;
                else if (statsLevels.manaRegen == maxLevel)
                    ismaxed = true;
                break;
            case "stamina":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.stamina.ToString();
                if (statsLevels.stamina == 0)
                    isreset = true;
                else if (statsLevels.stamina == maxLevel)
                    ismaxed = true;
                break;
            case "staminaregen":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.staminaRegen.ToString();
                if (statsLevels.staminaRegen == 0)
                    isreset = true;
                else if (statsLevels.staminaRegen == maxLevel)
                    ismaxed = true;
                break;
            case "sanity":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.sanity.ToString();
                if (statsLevels.sanity == 0)
                    isreset = true;
                else if (statsLevels.sanity == maxLevel)
                    ismaxed = true;
                break;
            case "atkdmg":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.atkDmg.ToString();
                if (statsLevels.atkDmg == 0)
                    isreset = true;
                else if (statsLevels.atkDmg == maxLevel)
                    ismaxed = true;
                break;
            case "mp":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicPower.ToString();
                if (statsLevels.magicPower == 0)
                    isreset = true;
                else if (statsLevels.magicPower == maxLevel)
                    ismaxed = true;
                break;
            case "dmgresis":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.dmgResis.ToString();
                if (statsLevels.dmgResis == 0)
                    isreset = true;
                else if (statsLevels.dmgResis == maxLevel)
                    ismaxed = true;
                break;
            case "magicresis":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.magicResis.ToString();
                if (statsLevels.magicResis == 0)
                    isreset = true;
                else if (statsLevels.magicResis == maxLevel)
                    ismaxed = true;
                break;
            case "critchance":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critChance.ToString();
                if (statsLevels.critChance == 0)
                    isreset = true;
                else if (statsLevels.critChance == maxLevel)
                    ismaxed = true;
                break;
            case "critdmg":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.critDmg.ToString();
                if (statsLevels.critDmg == 0)
                    isreset = true;
                else if (statsLevels.critDmg == maxLevel)
                    ismaxed = true;
                break;
            case "movspeed":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.movSpeed.ToString();
                if (statsLevels.movSpeed == 0)
                    isreset = true;
                else if (statsLevels.movSpeed == maxLevel)
                    ismaxed = true;
                break;
            case "timing":
                statsLevelsBtns.transform.Find(name).transform.Find("txt").transform.GetComponent<Text>().text = statsLevels.timing.ToString();
                if (statsLevels.timing == 0)
                    isreset = true;
                else if (statsLevels.timing == maxLevel)
                    ismaxed = true;
                break;
        }

        if (isreset)
        {
            LockButtons(false, true, name);
        }
        else if (ismaxed)
        {
            LockButtons(true, false, name);
        } else
        {
            LockButtons(false, false, name);
        }

    }
}
