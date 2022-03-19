using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

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

    [SerializeField] private Text nameDisplay;
    [SerializeField] private Text titleDisplay;
    [SerializeField] private GameObject passiveList;
    [SerializeField] private GameObject passivePrefab;

    [SerializeField] private GameObject moveHud;
    [SerializeField] private GameObject moveList;
    [SerializeField] private GameObject movePrefab;
    [SerializeField] private GameObject ultInfo;

    [SerializeField] private TooltipPopUp tooltipPopup;

    [SerializeField] private SpriteRenderer alexRender, leowindRender, bonsourRender, shineRender, sandewRender, isadoeRender, williamRender, hestiaRender;
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

    private CharcSelectLang thisLangManager;

    private int i = 1;
    private int max = 0;
    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string isPlayerChamp = "isPlayerChamp";
    private readonly string isEnemyChamp = "isEnemyChamp";

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

        alexRender.enabled = true;
    }

    void Start()
    {
        SetText(i);
        SetPassives(i);
        SetMoves(i);
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
        }
    }

    public void SetMoves(int n)
    {
        foreach (Transform child in moveList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        int i = 1;
        try
        {
            switch (n)
            {
                case 1:
                    foreach (Moves a in alex.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                alex.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = alex.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 2:
                    foreach (Moves a in leowind.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                leowind.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = leowind.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 3:
                    foreach (Moves a in bonsour.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                bonsour.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = bonsour.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 4:
                    foreach (Moves a in shine.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                shine.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = shine.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 5:
                    foreach (Moves a in sandew.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                sandew.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = sandew.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 6:
                    foreach (Moves a in isadoe.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                isadoe.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = isadoe.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 7:
                    foreach (Moves a in william.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                william.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = william.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
                    break;
                case 8:
                    foreach (Moves a in hestia.GetComponent<CharacterInfo>().character.moves)
                    {
                        if (i > 1)
                        {
                            if (a.name != "Recover Mana")
                                ShowMove(a);
                            else
                                hestia.GetComponent<CharacterInfo>().character.moves.Remove(a);
                        }
                        i++;
                    }
                    ultInfo.GetComponent<TooltipButton>().tooltipPopup = tooltipPopup.GetComponent<TooltipPopUp>();
                    ultInfo.GetComponent<TooltipButton>().text = hestia.GetComponent<CharacterInfo>().character.ultimate.GetTooltipText();
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
        movePrefab.GetComponent<TooltipButton>().text = a.GetTooltipText();

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
        }

        Instantiate(movePrefab, moveList.transform);
    }

    public void SetText(int n)
    {
        switch (n)
        {
            case 1:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", alex.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", alex.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = alex.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(alex.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(alex.GetComponent<CharacterInfo>().character.classe);
                break;
            case 2:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", leowind.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", leowind.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = leowind.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(leowind.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(leowind.GetComponent<CharacterInfo>().character.classe);
                break;
            case 3:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", bonsour.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", bonsour.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = bonsour.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(bonsour.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(bonsour.GetComponent<CharacterInfo>().character.classe);
                break;
            case 4:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", shine.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", shine.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = shine.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(shine.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(shine.GetComponent<CharacterInfo>().character.classe);
                break;
            case 5:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", sandew.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", sandew.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = sandew.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(sandew.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(sandew.GetComponent<CharacterInfo>().character.classe);
                break;
            case 6:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", isadoe.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", isadoe.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = isadoe.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(isadoe.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(isadoe.GetComponent<CharacterInfo>().character.classe);
                break;
            case 7:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", william.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", william.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = william.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(william.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(william.GetComponent<CharacterInfo>().character.classe);
                break;
            case 8:
                nameDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "name", hestia.GetComponent<CharacterInfo>().character.name);
                titleDisplay.text = thisLangManager.languageManager.GetText(thisLangManager.language, "charc", "title", hestia.GetComponent<CharacterInfo>().character.name);
                charcIcon.sprite = hestia.GetComponent<CharacterInfo>().character.charcIcon;
                SetStats(hestia.GetComponent<CharacterInfo>().character.stats);
                SetClassIcon(hestia.GetComponent<CharacterInfo>().character.classe);
                break;
        }
    }

    public void SetClassIcon (Character.Class classe)
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
        accuracy.text = (stats.accuracy*100).ToString() + "%";
        dmgResis.text = stats.dmgResis.ToString();
        magicResis.text = stats.magicResis.ToString();
        atkDmg.text = stats.atkDmg.ToString();
        mp.text = stats.magicPower.ToString();
        critChance.text = (stats.critChance * 100).ToString() + "%";
        critDmg.text = (stats.critDmg * 100).ToString() + "%";
        timing.text = stats.timing.ToString();
        movSpeed.text = stats.movSpeed.ToString();
        lifesteal.text = (stats.lifesteal * 100).ToString() + "%";

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
                hestiaRender.enabled = true;
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
        SceneManager.LoadScene(0, LoadSceneMode.Single);
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

    public void ChangeScene()
    {
        if (PlayerPrefs.GetInt("isEndless") == 0)
        {
            int random = 0;

            do
            {
                random = Random.Range(1, max + 1);
                StartCoroutine(WaitWhile());
            } while (random == i);

            PlayerPrefs.SetInt(selectedCharacter, i);
            PlayerPrefs.SetInt(selectedEnemy, random);
            PlayerPrefs.SetInt(isPlayerChamp, 1);
            PlayerPrefs.SetInt(isEnemyChamp, 1);

            SceneManager.LoadScene(2, LoadSceneMode.Single);
        } else
        {
            PlayerPrefs.SetInt(selectedCharacter, i);
            PlayerPrefs.SetInt(isPlayerChamp, 1);

            SceneManager.LoadScene(3, LoadSceneMode.Single);
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
