using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, CHANGETURN, WIN, LOSE }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;

    [SerializeField] private BattleState state;
    [SerializeField] private float aiManaRecover = 0.12f;
    [SerializeField] private float aiGaranteedManaRecover = 0.08f;
    [SerializeField] private float tiredStart = 0.05f;
    [SerializeField] private float tiredGrowth = 0.015f;
    [SerializeField] private int tiredStacks = 0;
    [SerializeField] private float dmgResisPer = 0.18f;
    [SerializeField] private float magicResisPer = 0.12f;
    [SerializeField] private float dotReduc = 0.3f;
    [SerializeField] private float ultComp = 0.25f;
    [SerializeField] private int ultCompDuration = 6;
    [SerializeField] private int bloodLossStacks = 10;
    //Change blood special tooltip if changed ^
    [SerializeField] private Text dialogText;

    Unit playerUnit;
    Unit enemyUnit;

    [SerializeField] private BattleHud playerHUD;
    [SerializeField] private BattleHud enemyHUD;

    [SerializeField] private SummaryHud sumPlayerHud;
    [SerializeField] private SummaryHud sumEnemyHud;
    [SerializeField] private GameObject sumHud;
    [SerializeField] private GameObject sumHudHideBtn;
    [SerializeField] private GameObject leaveBtn;

    [SerializeField] private Text turnsText;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private float ultEnergyDealt;
    [SerializeField] private float ultEnergyTaken;

    [SerializeField] private Button movesBtn;
    [SerializeField] private Button healManaBtn;
    [SerializeField] private Button basicBtn;
    [SerializeField] private Button ultBtn;
    [SerializeField] private Text healBtnText;

    [SerializeField] private EffectsMove tired;
    [SerializeField] private EffectsMove fear;
    [SerializeField] private Effects scorch;

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private GameObject panelMoves;
    [SerializeField] private Transform moveListHud;

    [SerializeField] private GameObject moveButton;

    [SerializeField] private GameObject panelEffectsP;
    [SerializeField] private GameObject panelEffectsE;
    [SerializeField] private GameObject barIconPrefab;

    [SerializeField] private Sprite phyAtk;
    [SerializeField] private Sprite magiAtk;
    [SerializeField] private Sprite rangeAtk;
    [SerializeField] private Sprite suppAtk;
    [SerializeField] private Sprite defAtk;
    [SerializeField] private Sprite statAtk;
    [SerializeField] private Sprite summAtk;

    [SerializeField] private GameObject phyCancel;
    [SerializeField] private GameObject magiCancel;
    [SerializeField] private GameObject rangeCancel;
    [SerializeField] private GameObject suppCancel;
    [SerializeField] private GameObject defCancel;
    [SerializeField] private GameObject statCancel;
    [SerializeField] private GameObject summCancel;

    [SerializeField] private GameObject tooltipMain;
    [SerializeField] private GameObject tooltipSec;

    [SerializeField] private AudioSource cameraAudio;
    [SerializeField] private AudioClip bossfightMusic;
    private FightLang langmanag;

    [SerializeField] private EndlessInfo info;

    [SerializeField] private float perHealthRegenEndless;
    [SerializeField] private float perCostsRegenEndless;
    [SerializeField] private float perSanityRegenEndless;
    [SerializeField] private float perUltReduce;

    [SerializeField] private EndlessFightStuff endlessStuff;
    [SerializeField] private StuffList items;
    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;


    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            info.Load();
        }

        state = BattleState.START;
        langmanag = this.gameObject.GetComponent<FightLang>();
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.GetComponent<SoundSystem>().FFMenu();
        }

        if (Input.GetKeyDown(KeyCode.S) && !(state == BattleState.WIN || state == BattleState.LOSE))
        {
            ShowOverview(false);
        }   
    }

    public void ShowOverview(bool forceShow)
    {
        if (!sumHud.activeInHierarchy || forceShow)
        {
            sumHud.SetActive(true);
            if (forceShow)
                sumHudHideBtn.SetActive(false);
        }
        else
            sumHud.SetActive(false);
    }

    public void Leave()
    {
        if ((state == BattleState.WIN || state == BattleState.LOSE) && PlayerPrefs.GetInt("isEndless") == 1)
            loader.LoadScene(3, slider, loadPanel);
        else
            loader.LoadScene(0, slider, loadPanel);
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogText.text = langmanag.GetInfo("gui", "text", "wantfight", langmanag.GetInfo("charc", "name", enemyUnit.charc.name));

        movesBtn.interactable = false;
        basicBtn.interactable = false;
        healManaBtn.interactable = false;
        ultBtn.interactable = false;
        
        enemyUnit.curHp = enemyUnit.charc.stats.hp;
        enemyUnit.curMana = enemyUnit.charc.stats.mana;
        enemyUnit.curStamina = enemyUnit.charc.stats.stamina;
        enemyUnit.curSanity = enemyUnit.charc.stats.sanity;

        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            playerUnit.curHp = playerUnit.charc.stats.hp * info.playerHp;
            playerUnit.curMana = playerUnit.charc.stats.mana * info.playerMn;
            playerUnit.curStamina = playerUnit.charc.stats.stamina * info.playerSta;
            playerUnit.curSanity = (int)(playerUnit.charc.stats.sanity * info.playerSan);
            playerUnit.ult = info.playerUlt;
        } else
        {
            playerUnit.curHp = playerUnit.charc.stats.hp;
            playerUnit.curMana = playerUnit.charc.stats.mana;
            playerUnit.curStamina = playerUnit.charc.stats.stamina;
            playerUnit.curSanity = playerUnit.charc.stats.sanity;
        }

        playerHUD.SetHud(playerUnit, (int)(playerUnit.curStamina * (tiredStart + (tiredGrowth * tiredStacks))));
        enemyHUD.SetHud(enemyUnit, (int)(enemyUnit.curStamina * (tiredStart + (tiredGrowth * tiredStacks))));

        playerUnit.curShield = 0;
        enemyUnit.curShield = 0;

        playerUnit.statMods.Clear();
        enemyUnit.statMods.Clear();

        playerUnit.effects.Clear();
        enemyUnit.effects.Clear();

        playerUnit.randomItems.Clear();
        enemyUnit.randomItems.Clear();

        //setup preset player items (testing porpuse)
        playerUnit.LoadItems();

        //setup preset enemy items (testing porpuse)
        enemyUnit.LoadItems();

        playerUnit.GetItems(info, items);

        GenItem(playerUnit, enemyUnit);
        enemyUnit.GetItems(info, items);

        SetStatus();

        playerHUD.SetStats(playerUnit.charc.stats, playerUnit.charc.stats, playerUnit.curSanity);
        enemyHUD.SetStats(enemyUnit.charc.stats, enemyUnit.charc.stats, enemyUnit.curSanity);

        int i = 0;
        foreach (Moves move in enemyUnit.moves)
        {
            foreach (EffectsMove a in move.effects)
            {
                a.SetApply(false);
            }

            if (move.name != "recovmana")
                move.inCooldown = 0;
        }

        basicBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        basicBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

        healManaBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        healManaBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

        ultBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        ultBtn.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();

        UpdateTooltips();

        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            /*switch (enemyUnit.charc.strenght)
            {
                case Character.Strenght.BABY:
                    playerUnit.passives.Add(endlessStuff.babyPassive.ReturnPassive());
                    break;
                case Character.Strenght.WEAK:
                    playerUnit.passives.Add(endlessStuff.weakPassive.ReturnPassive());
                    break;
                case Character.Strenght.NORMAL:
                    playerUnit.passives.Add(endlessStuff.normalPassive.ReturnPassive());
                    break;
                case Character.Strenght.STRONG:
                    playerUnit.passives.Add(endlessStuff.strongPassive.ReturnPassive());
                    break;
                case Character.Strenght.SUPERSTRONG:
                    playerUnit.passives.Add(endlessStuff.superStrongPassive.ReturnPassive());
                    break;
                case Character.Strenght.LEGENDARY:
                    playerUnit.passives.Add(endlessStuff.legendaryPassive.ReturnPassive());
                    break;
                case Character.Strenght.CHAMPION:
                    break;
            }*/

            if (PlayerPrefs.GetInt("isEnemyBoss") == 1)
                enemyUnit.passives.Add(endlessStuff.bossPassive.ReturnPassive());
        }

        foreach (Moves move in playerUnit.moves)
        {
            i++;

            foreach (EffectsMove a in move.effects)
            {
                a.SetApply(false);
            }

            if (move.name != "basicattack")
            {
                move.inCooldown = 0;

                moveButton.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
                moveButton.GetComponent<TooltipButton>().tooltipPopupSec = tooltipSec.GetComponent<TooltipPopUp>();
                moveButton.GetComponent<TooltipButton>().text = move.GetTooltipText(false);
                moveButton.GetComponent<TooltipButton>().textSec = move.GetTooltipText(true);

                moveButton.name = move.name;

                Text id = moveButton.transform.Find("Id").gameObject.GetComponent<Text>();
                id.text = i.ToString();

                Text name = moveButton.transform.Find("Name").gameObject.GetComponent<Text>();
                name.text = langmanag.GetInfo("moves", move.name);

                Text mana = moveButton.transform.Find("Mana").gameObject.GetComponent<Text>();
                mana.text = move.manaCost.ToString();

                Text stamina = moveButton.transform.Find("Stamina").gameObject.GetComponent<Text>();
                stamina.text = move.staminaCost.ToString();

                Text cdn = moveButton.transform.Find("Cooldown").gameObject.GetComponent<Text>();
                cdn.text = move.cooldown.ToString();

                Text cd = moveButton.transform.Find("CD").gameObject.GetComponent<Text>();
                cd.text = langmanag.GetInfo("gui", "text", "cd");

                Text sta = moveButton.transform.Find("STA").gameObject.GetComponent<Text>();
                sta.text = langmanag.GetInfo("gui", "text", "sta");

                Text mn = moveButton.transform.Find("MN").gameObject.GetComponent<Text>();
                mn.text = langmanag.GetInfo("gui", "text", "mn");

                Image icon = moveButton.transform.Find("Icon").gameObject.GetComponent<Image>();
                switch (move.type)
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
                        icon.sprite = summAtk;
                        break;
                }

                Instantiate(moveButton, moveListHud);
            }   

        }

        yield return new WaitForSeconds(1.4f);
        if (PlayerPrefs.GetInt("isEndless") == 1 && PlayerPrefs.GetInt("isEnemyBoss") == 1)
            cameraAudio.clip = bossfightMusic;
        else
            cameraAudio.clip = enemyUnit.charc.audio;

        cameraAudio.Play();

        StartCoroutine(NewTurn());
    }

    void GenItem(Unit player, Unit enemy)
    {
        int itemCountP = player.items.Count;
        int itemCountE = enemy.charc.recItems.Count;

        if (PlayerPrefs.GetInt("isEndless") == 0 && itemCountP > 0 && itemCountE > 0)
        {
            List<int> picked = new List<int>();

            for (int i = 0; itemCountP > i; i++)
            {
                bool isPicked = false;
                do
                {
                    isPicked = false;
                    int num = Random.Range(0, itemCountE);
                    if (picked.Count > 0)
                    {
                        foreach (int a in picked)
                        {
                            if (num == a)
                                isPicked = true;
                        }
                    }

                    if (!isPicked)
                        picked.Add(num);

                } while (isPicked);
            }
            if (picked.Count > 0)
                enemy.randomItems = picked;
        }
    }

    IEnumerator Combat(Moves movePlayer)
    {
        panelMoves.SetActive(false);
        tooltipMain.transform.Find("TooltipCanvas").gameObject.SetActive(false);
        movesBtn.interactable = false;
        basicBtn.interactable = false;
        healManaBtn.interactable = false;
        ultBtn.interactable = false;

        Moves moveEnemy = EnemyChooseMove();

        if (movePlayer)
            movePlayer.inCooldown = movePlayer.cooldown;

        if (moveEnemy)
            moveEnemy.inCooldown = moveEnemy.cooldown;

        bool canUseNormal = true;
        float movPlayer = playerUnit.SetModifiers().movSpeed;
        float movEnemy = enemyUnit.SetModifiers().movSpeed;

        if (movePlayer == null || moveEnemy == null)
            canUseNormal = false;

        if (canUseNormal)
        {
            if (movePlayer.priority > moveEnemy.priority)
            {
                state = BattleState.PLAYERTURN;
                yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                playerHUD.SetStatsHud(playerUnit);
                enemyHUD.SetStatsHud(enemyUnit);

                if (state == BattleState.PLAYERTURN)
                {
                    state = BattleState.ENEMYTURN;
                    yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                    playerHUD.SetStatsHud(playerUnit);
                    enemyHUD.SetStatsHud(enemyUnit);
                }
            }
            else if (moveEnemy.priority > movePlayer.priority)
            {
                state = BattleState.ENEMYTURN;
                yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                playerHUD.SetStatsHud(playerUnit);
                enemyHUD.SetStatsHud(enemyUnit);

                if (state == BattleState.ENEMYTURN)
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    playerHUD.SetStatsHud(playerUnit);
                    enemyHUD.SetStatsHud(enemyUnit);
                }
            }
            else if (moveEnemy.priority == movePlayer.priority)
            {
                if (movPlayer > movEnemy)
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    playerHUD.SetStatsHud(playerUnit);
                    enemyHUD.SetStatsHud(enemyUnit);

                    if (state == BattleState.PLAYERTURN)
                    {
                        state = BattleState.ENEMYTURN;
                        yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                        playerHUD.SetStatsHud(playerUnit);
                        enemyHUD.SetStatsHud(enemyUnit);
                    }
                }
                else if (movPlayer < movEnemy)
                {
                    state = BattleState.ENEMYTURN;
                    yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                    playerHUD.SetStatsHud(playerUnit);
                    enemyHUD.SetStatsHud(enemyUnit);

                    if (state == BattleState.ENEMYTURN)
                    {
                        state = BattleState.PLAYERTURN;
                        yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                        playerHUD.SetStatsHud(playerUnit);
                        enemyHUD.SetStatsHud(enemyUnit);
                    }
                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    playerHUD.SetStatsHud(playerUnit);
                    enemyHUD.SetStatsHud(enemyUnit);

                    if (state == BattleState.PLAYERTURN)
                    {
                        state = BattleState.ENEMYTURN;
                        yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                        playerHUD.SetStatsHud(playerUnit);
                        enemyHUD.SetStatsHud(enemyUnit);
                    }
                }
            }
        }
        else
        {
            state = BattleState.PLAYERTURN;
            yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
            playerHUD.SetStatsHud(playerUnit);
            enemyHUD.SetStatsHud(enemyUnit);

            if (state == BattleState.PLAYERTURN)
            {
                state = BattleState.ENEMYTURN;
                yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                playerHUD.SetStatsHud(playerUnit);
                enemyHUD.SetStatsHud(enemyUnit);
            }
        }

        //Debug.Log(state);
        if (state == BattleState.WIN || state == BattleState.LOSE)
            StartCoroutine(EndBattle());
        else
            StartCoroutine(NewTurn());
    }

    IEnumerator Attack(Moves move, Unit user, Unit target)
    {
        user.SetCC();
        bool canMove = true;
        bool canAttack = true;
        switch (move.type)
        {
            case Moves.MoveType.PHYSICAL:
                if (!user.canUsePhysical)
                    canAttack = false;
                break;
            case Moves.MoveType.MAGICAL:
                if (!user.canUseMagic)
                    canAttack = false;
                break;
            case Moves.MoveType.RANGED:
                if (!user.canUseRanged)
                    canAttack = false;
                break;
            case Moves.MoveType.DEFFENCIVE:
                if (!user.canUseProtec)
                    canAttack = false;
                break;
            case Moves.MoveType.SUPPORT:
                if (!user.canUseSupp)
                    canAttack = false;
                break;
            case Moves.MoveType.ENCHANT:
                if (!user.canUseEnchant)
                    canAttack = false;
                break;
            case Moves.MoveType.SUMMON:
                if (!user.canUseSummon)
                    canAttack = false;
                break;
        }

        if (!user.canUseMagic && !user.canUsePhysical && !user.canUseRanged && !user.canUseEnchant && !user.canUseSupp
            && !user.canUseProtec && !user.canUseSummon)
            canMove = false;

        if (move == null || !canAttack)
        {
            if (!canMove)
                dialogText.text = langmanag.GetInfo("gui", "text", "cantmove", langmanag.GetInfo("charc", "name", user.charc.name));
            else
                dialogText.text = langmanag.GetInfo("gui", "text", "cantattack", langmanag.GetInfo("charc", "name", user.charc.name));
            yield return new WaitForSeconds(1.52f);
        }
        else
        {
            int manaCost = (int)(move.manaCost*user.SetModifiers().manaCost);
            int staminaCost = (int)(move.staminaCost*user.SetModifiers().staminaCost);

            user.curMana -= manaCost;

            if (user.curMana < 0)
                user.curMana = 0;

            user.curStamina -= staminaCost;

            if (user.curStamina < 0)
                user.curStamina = 0;

            SetStatus();

            if (move.name == "recovmana")
                move.inCooldown = move.cooldown;

            DMG dmgTarget = default;
            DMG dmgUser = default;

            bool isDead = false;
            bool blockPhysical = false;
            bool blockMagic = false;
            bool blockRanged = false;
            bool isCrit = false;
            bool isMagicCrit = false;

            Stats statsUser = user.SetModifiers();
            Stats statsTarget = target.SetModifiers();

            //set timing to 5 if timing > 5
            if (statsTarget.timing > 5)
                statsTarget.timing = 5;

            if (statsUser.timing > 5)
                statsUser.timing = 5;

            float evasion = 0;
            bool isStoped = false;       

            //calculate evasion
            evasion = (float)((statsTarget.movSpeed * 0.035) + (statsTarget.timing * 0.5) + (target.curSanity * 0.01))/100;

            if ((target.isBlockingPhysical && (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)) || (target.isBlockingMagical && move.type is Moves.MoveType.MAGICAL) || (target.isBlockingRanged && move.type is Moves.MoveType.RANGED))
            {

                dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", langmanag.GetInfo("charc", "name", user.charc.name), langmanag.GetInfo("moves", move.name));

                yield return new WaitForSeconds(1.15f);
                target.DmgNumber("0", Color.white);
                yield return new WaitForSeconds(0.8f);

                foreach (Passives a in target.passives.ToArray())
                {
                    if (a.name == "onewiththeshadows")
                    {
                        if (a.inCd < a.cd)
                        {
                            a.inCd++;
                            ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                        }
                    }
                }
            }
            else
            {
                bool usedUsMod = false;
                bool usedEnMod = false;
                float stopAttackChance = 0;
                string cancelText = "";
                if (move.uses > 0)
                    move.uses--;

                for (int i = 0; i < move.hitTime; i++)
                {
                    user.DoMoveAnim(move.type);

                    isCrit = false;
                    dmgTarget.Reset();
                    dmgUser.Reset();

                    if (Random.Range(0f, 1f) <= (statsUser.critChance + move.critChanceBonus))
                        isCrit = true;

                    foreach (Passives a in target.passives.ToArray())
                    {
                        if (a.name == "galeglide")
                        {
                            if ((move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.RANGED || move.type is Moves.MoveType.BASIC) && a.inCd == 0)
                            {
                                //reset cd
                                a.inCd = a.cd;
                                //show popup
                                target.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                //delete icon
                                DestroyPassiveIcon(a.name, target.isEnemy);
                                //get evasion bonus
                                StatMod mod = a.statMod.ReturnStats();
                                float evasionBonus = a.maxNum * target.SetModifiers().timing;
                                mod.evasion += a.num * evasionBonus;
                                mod.inTime = mod.time;
                                //apply the evasion
                                target.statMods.Add(mod);
                                statsTarget = target.SetModifiers();
                            }
                        }
                    }

                    foreach (Passives a in user.passives.ToArray())
                    {
                        if (a.name == "sharpshooter")
                        {
                            if (move.type is Moves.MoveType.RANGED)
                            {
                                a.num++;
                                bool isReady = false;
                                if (a.num == a.maxNum - 1)
                                    isReady = true;
                                ManagePassiveIcon(a.sprite, a.name, (a.maxNum - a.num).ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                            }

                            if (a.num == a.maxNum)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                a.num = 0;
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                                
                                isCrit = true;
                                DestroyPassiveIcon(a.name, user.isEnemy);
                            }                            
                        }

                        if (a.name == "vendetta")
                        {
                            if (target.SetModifiers().hp >= (user.SetModifiers().hp + (user.SetModifiers().hp * a.num)) && a.stacks != 1)
                            {
                                a.stacks = 1;
                                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                            }  

                            if (a.inCd == 0 && a.stacks == 1 && move.type is Moves.MoveType.PHYSICAL)
                            {
                                a.inCd = a.cd;
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                StatScale scale = a.ifConditionTrueScale();
                                StatScale scale2 = a.ifConditionTrueScale2();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));

                                dmgTarget.AddDmg(scale2.SetScaleDmg(stats, unit));

                                StatMod statMod = a.statMod.ReturnStats();
                                statMod.inTime = statMod.time;
                                user.statMods.Add(statMod);
                                user.usedBonusStuff = false;
                            }
                        }

                        if (a.name == "phantomhand")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                dmgTarget.trueDmg += a.num;
                            }
                        }

                        if (a.name == "manasword")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL && a.stacks == a.maxStacks)
                            {
                                StatScale scale = a.ifConditionTrueScale();
                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }

                            if (move.type is Moves.MoveType.PHYSICAL && a.stacks < a.maxStacks)
                            {
                                a.stacks++;
                            } 
                        }

                        if (a.name == "manascepter")
                        {
                            if ((move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.ENCHANT || move.type is Moves.MoveType.DEFFENCIVE) && a.stacks == a.maxStacks)
                            {
                                StatScale scale = a.ifConditionTrueScale();
                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }

                            if (move.type is Moves.MoveType.MAGICAL && a.stacks < a.maxStacks)
                            {
                                a.stacks++;
                            }
                        }

                        if (a.name == "gravitybelt")
                        {
                            if (a.inCd == 0)
                            {
                                if ((move.type is Moves.MoveType.BASIC || move.type is Moves.MoveType.PHYSICAL) && isCrit)
                                {
                                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                    dmgTarget.trueDmg += a.num;

                                    a.inCd = a.cd;

                                    //apply statmod
                                    StatMod statMod = a.statMod.ReturnStats();
                                    statMod.inTime = statMod.time;
                                    user.statMods.Add(statMod);
                                    user.usedBonusStuff = false;

                                    if (user.isEnemy)
                                        enemyHUD.SetStatsHud(user);
                                    else
                                        playerHUD.SetStatsHud(user);
                                }
                            }
                        }

                        if (a.name == "blazingfists")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "magicremains")
                        {
                            if ((move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.ENCHANT) && a.inCd == 0)
                            {
                                a.inCd = a.cd;
                                ManagePassiveIcon(a.sprite, a.name, (a.maxNum - a.num).ToString(), user.isEnemy, a.GetPassiveInfo());

                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                a.num = 0;
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));

                                DestroyPassiveIcon(a.name, user.isEnemy);
                            }
                        }

                        if (a.name == "plasmablade")
                        {
                            bool isSilence = false;
                            foreach (Effects b in user.effects)
                            {
                                if (b.id == "SLC")
                                    isSilence = true;
                            }

                            int manaPer = (int)((100 * user.curMana) / user.SetModifiers().mana);
                            bool enoughMana = false;

                            if (manaPer > a.maxNum * 100)
                            {
                                enoughMana = true;
                                ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                            }
                            
                            if (!enoughMana || isSilence)
                            {
                                DestroyPassiveIcon(a.name, user.isEnemy);
                            }

                            if ((move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC) && !isSilence && enoughMana)
                            {
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                float magicBonus = 0;
                                magicBonus += scale.SetScaleFlat(stats, unit);

                                int hpPer = (int)(100 * user.curHp / user.SetModifiers().hp);
                                
                                if (hpPer <= a.stacks)
                                {
                                    StatScale scale2 = a.ifConditionTrueScale2();
                                    
                                    magicBonus += magicBonus * a.num;
                                    dmgTarget.trueDmg += scale2.SetScaleFlat(stats, unit);
                                }

                                dmgTarget.magicDmg += magicBonus;
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            }
                        }

                        if (a.name == "bloodbath")
                        {
                            if (a.stacks == 1)
                            {
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }
                                dmgTarget.sanityDmg += (int)scale.SetScaleFlat(stats, unit);
                            }
                        }

                        if (a.name == "successoroffire")
                        {
                            bool isBurn = false;
                            foreach (Effects b in target.effects)
                            {
                                if (b.id == "BRN" || b.id == "SCH")
                                    isBurn = true;
                            }

                            if (isBurn && (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.RANGED))
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "zenmode")
                        {
                            if (a.stacks == 1)
                            {
                                StatScale scale = a.ifConditionTrueScale();
                                StatScale scale2 = a.ifConditionTrueScale2();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));

                                dmgTarget.AddDmg(scale2.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "spectralring")
                        {
                            //hp in %
                            int hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                            if (hpPer <= (a.num * 100) && move.type is Moves.MoveType.MAGICAL)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "shadowdagger")
                        {
                            if (a.inCd == 0 && move.type is Moves.MoveType.PHYSICAL)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                a.inCd = a.cd;

                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));

                                StatMod mod = a.statMod.ReturnStats();
                                mod.inTime = mod.time+1;
                                user.statMods.Add(mod);
                                statsUser = user.SetModifiers();

                                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                            }
                        }

                        if (a.name == "toxicteeth")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "huntersdirk")
                        {
                            if (a.inCd == 0)
                            {
                                float hpPer = (100 * target.curHp) / target.SetModifiers().hp;
                                if ((move.type is Moves.MoveType.BASIC || move.type is Moves.MoveType.PHYSICAL) && hpPer >= a.num)
                                {
                                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                    a.inCd = a.cd;
                                    StatScale scale = a.ifConditionTrueScale();

                                    Unit unit;
                                    Stats stats;
                                    if (scale.playerStat)
                                    {
                                        unit = user;
                                        stats = statsUser;
                                    }
                                    else
                                    {
                                        unit = target;
                                        stats = statsTarget;
                                    }

                                    dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                                }
                            }
                        }

                        if (a.name == "fighterinstinct")
                        {
                            if (a.stacks == 1)
                            {
                                if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)
                                {
                                    StatScale scale = a.ifConditionTrueScale();
                                    Unit unit;
                                    Stats stats;
                                    if (scale.playerStat)
                                    {
                                        unit = user;
                                        stats = statsUser;
                                    }
                                    else
                                    {
                                        unit = target;
                                        stats = statsTarget;
                                    }

                                    dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                                }
                            }
                        }

                        if (a.name == "gravitychange")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "bullsrage")
                        {
                            if (a.stacks >= a.maxStacks && (a.inCd > 0 || a.inCd < 0))
                            {
                                if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)
                                {
                                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                    StatScale scale = a.ifConditionTrueScale();

                                    Unit unit;
                                    Stats stats;
                                    if (scale.playerStat)
                                    {
                                        unit = user;
                                        stats = statsUser;
                                    }
                                    else
                                    {
                                        unit = target;
                                        stats = statsTarget;
                                    }

                                    dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                                }
                            }
                        }

                        if (a.name == "prismaticstaff")
                        {
                            if (move.type is Moves.MoveType.MAGICAL)
                            {
                                a.stacks++;
                                bool isReady = false;
                                if (a.stacks == a.maxStacks - 1)
                                    isReady = true;
                                ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                            }

                            if (a.stacks == a.maxStacks)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                                a.stacks = 0;
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));

                                isMagicCrit = true;
                                DestroyPassiveIcon(a.name, user.isEnemy);
                            }
                        }

                        if (a.name == "magicwand")
                        {
                            if (move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.BASIC)
                            {
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "crossbow")
                        {
                            if (move.type is Moves.MoveType.RANGED || move.type is Moves.MoveType.BASIC)
                            {
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }
                        }

                        if (a.name == "funchase")
                        {
                            if (a.stacks == 1 && (move.type == Moves.MoveType.PHYSICAL || move.type == Moves.MoveType.BASIC))
                            {
                                StatScale scale = a.ifConditionTrueScale();

                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }
                                dmgTarget.phyDmg += scale.SetScaleFlat(stats, unit);
                            }
                        }
                    }

                    dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", langmanag.GetInfo("charc", "name", user.charc.name), langmanag.GetInfo("moves", move.name));

                    dmgTarget.heal += move.heal;
                    dmgTarget.healMana += move.healMana;
                    dmgTarget.healStamina += move.healStamina;
                    dmgTarget.healSanity += move.healSanity;
                    dmgTarget.shield += move.shield;

                    foreach (Effects a in user.effects)
                    {
                        if (a.cancelAtkChance > 0 && !isStoped)
                        {
                            stopAttackChance = a.cancelAtkChance;

                            if (Random.Range(0f, 1f) <= stopAttackChance)
                            {
                                isStoped = true;
                                user.DoAnimParticle(a.specialAnim);
                                cancelText = langmanag.GetInfo("effect", "cancelmsg", a.id.ToLower());
                                float shieldedDmg = 0;
                                float recoil = a.recoil;
                                user.trueDmgTaken += recoil;

                                if (user.curShield > 0)
                                {
                                    float tempDmg = recoil;
                                    float tempShield = user.curShield;
                                    
                                    recoil -= user.curShield;
                                    user.curShield -= tempDmg;

                                    if (user.curShield < 0)
                                        user.curShield = 0;

                                    shieldedDmg = tempShield - user.curShield;
                                }
                                
                                if (recoil > 0 || shieldedDmg > 0)
                                {
                                    DMG recoildmg = default;
                                    recoildmg.Reset();
                                    recoildmg.trueDmg += recoil;
                                    isDead = user.TakeDamage(recoildmg, false, false, user);
                                }

                                sumPlayerHud.UpdateValues(playerUnit, langmanag.GetInfo("charc", "name", playerUnit.charc.name));
                                sumEnemyHud.UpdateValues(enemyUnit, langmanag.GetInfo("charc", "name", enemyUnit.charc.name));

                                if (isDead)
                                    if (user.isEnemy)
                                        state = BattleState.WIN;
                                    else
                                        state = BattleState.LOSE;
                            }
                        }
                    }

                    if (!(Random.Range(0f, 1f) <= statsUser.accuracy) || (state is BattleState.LOSE || state is BattleState.WIN) && !isStoped)
                        isStoped = true;

                    if (!isStoped || 
                        move.type == Moves.MoveType.DEFFENCIVE || move.type == Moves.MoveType.SUPPORT || 
                        move.type == Moves.MoveType.ENCHANT || move.type == Moves.MoveType.SUMMON)
                    {
                        if (Random.Range(0f, 1f) <= (evasion + statsTarget.evasion) && 
                            (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.RANGED))
                        {
                            target.Miss(true);

                            if (move.isUlt)
                                GrantUltCompansation(user);

                            yield return new WaitForSeconds(1.05f);
                            dialogText.text = langmanag.GetInfo("gui", "text", "dodge", langmanag.GetInfo("charc", "name", target.charc.name));
                            yield return new WaitForSeconds(1.05f);

                            foreach (Passives a in target.passives.ToArray())
                            {
                                if (a.name == "onewiththeshadows")
                                {
                                    if (a.inCd < a.cd)
                                    {
                                        a.inCd++;
                                        ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                    }
                                }
                            }
                        }
                        else
                        {
                            dmgTarget.AddBaseDmg(move);

                            blockPhysical = move.blocksPhysical;
                            blockMagic = move.blocksMagical;
                            blockRanged = move.blocksRanged;

                            bool skipEffect = false;

                            if (move.effects.Count > 0)
                            {
                                foreach (EffectsMove a in move.effects)
                                {                                    
                                    Effects effect = a.effect.ReturnEffect();

                                    if (Random.Range(0f, 1f) <= a.chance && !a.WasApplied())
                                    {
                                        foreach (Passives b in target.passives.ToArray())
                                        {
                                            if (b.name == "ecolocation")
                                            {
                                                if (effect.id == "BLI")
                                                {
                                                    skipEffect = true;
                                                    target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                }
                                            }

                                            if (b.name == "strongmind")
                                            {
                                                if (effect.id == "CFS" || effect.id == "SLP" || effect.id == "CHR")
                                                {
                                                    skipEffect = true;
                                                    target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                }
                                            }
                                        }

                                        if (!skipEffect)
                                        {
                                            //Debug.Log("I EFFECT");
                                            effect.duration = Random.Range(a.durationMin, a.durationMax);

                                            if (a.targetPlayer)
                                            {
                                                Effects check = user.CheckIfEffectExists(effect.id);
                                                if (check == null)
                                                {
                                                    user.effects.Add(effect);

                                                    if (!user.isEnemy)
                                                    {
                                                        SetEffectIcon(effect, panelEffectsP);
                                                    }
                                                    else
                                                    {
                                                        SetEffectIcon(effect, panelEffectsE);
                                                    }

                                                    foreach (StatMod b in effect.statMods)
                                                    {
                                                        StatMod statMod = b.ReturnStats();
                                                        statMod.inTime = effect.duration;
                                                        dmgTarget.heal += (int)((user.SetModifiers().hp * statMod.hp) / 2);
                                                        dmgTarget.healMana += (int)((user.SetModifiers().mana * statMod.mana) / 2);
                                                        dmgTarget.healStamina += (int)((user.SetModifiers().stamina * statMod.stamina) / 2);
                                                        dmgTarget.healSanity += (int)((user.SetModifiers().sanity * statMod.sanity) / 2);
                                                        user.statMods.Add(statMod);
                                                        user.usedBonusStuff = false;
                                                    }
                                                } else
                                                {
                                                    if (check.id == "BRN" && effect.id == "BRN")
                                                    {
                                                        Effects newScorch = scorch.ReturnEffect();
                                                        newScorch.duration = check.duration + effect.duration;
                                                        user.effects.Add(newScorch);
                                                        user.effects.Remove(check);

                                                        if (!user.isEnemy)
                                                        {
                                                            GameObject temp = panelEffectsP.transform.Find(check.id + "(Clone)").gameObject;
                                                            Destroy(temp.gameObject);
                                                            SetEffectIcon(newScorch, panelEffectsP);
                                                        }
                                                        else
                                                        {
                                                            GameObject temp = panelEffectsE.transform.Find(check.id + "(Clone)").gameObject;
                                                            Destroy(temp.gameObject);
                                                            SetEffectIcon(newScorch, panelEffectsE);
                                                        }

                                                    } else
                                                        check.duration += effect.duration;
                                                }
                                               
                                            }
                                            else
                                            {
                                                Effects check = target.CheckIfEffectExists(effect.id);
                                                if (check == null)
                                                {
                                                    target.effects.Add(effect);

                                                    if (!target.isEnemy)
                                                    {
                                                        SetEffectIcon(effect, panelEffectsP);
                                                    }
                                                    else
                                                    {
                                                        SetEffectIcon(effect, panelEffectsE);
                                                    }

                                                    foreach (StatMod b in effect.statMods)
                                                    {
                                                        StatMod statMod = b.ReturnStats();
                                                        statMod.inTime = effect.duration;
                                                        dmgTarget.heal += (int)((target.SetModifiers().hp * statMod.hp) / 2);
                                                        dmgTarget.healMana += (int)((target.SetModifiers().mana * statMod.mana) / 2);
                                                        dmgTarget.healStamina += (int)((target.SetModifiers().stamina * statMod.stamina) / 2);
                                                        dmgTarget.healSanity += (int)((target.SetModifiers().sanity * statMod.sanity) / 2);
                                                        target.statMods.Add(statMod);
                                                        target.usedBonusStuff = false;
                                                    }
                                                } else
                                                {
                                                    if (check.id == "BRN" && effect.id == "BRN")
                                                    {
                                                        Effects newScorch = scorch.ReturnEffect();
                                                        newScorch.duration = check.duration + effect.duration;
                                                        target.effects.Add(newScorch);
                                                        target.effects.Remove(check);

                                                        if (!target.isEnemy)
                                                        {
                                                            GameObject temp = panelEffectsP.transform.Find(check.id + "(Clone)").gameObject;
                                                            Destroy(temp.gameObject);
                                                            SetEffectIcon(newScorch, panelEffectsP);
                                                        }
                                                        else
                                                        {
                                                            GameObject temp = panelEffectsE.transform.Find(check.id + "(Clone)").gameObject;
                                                            Destroy(temp.gameObject);
                                                            SetEffectIcon(newScorch, panelEffectsE);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        check.duration += effect.duration;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            skipEffect = false;
                                        }
                                        a.SetApply(true);
                                    }
                                }
                            }

                            if (move.statModUser)
                            {
                                {
                                    if (Random.Range(0f, 1f) < move.statModUser.chance && !usedUsMod)
                                    {
                                        StatMod statMod = move.statModUser.ReturnStats();
                                        statMod.inTime = statMod.time + 1;
                                        dmgTarget.heal += ((user.SetModifiers().hp * statMod.hp) / 2);
                                        dmgTarget.healMana += ((user.SetModifiers().mana * statMod.mana) / 2);
                                        dmgTarget.healStamina += ((user.SetModifiers().stamina * statMod.stamina) / 2);
                                        dmgTarget.healSanity += (int)((user.SetModifiers().sanity * statMod.sanity) / 2);
                                        user.statMods.Add(statMod);
                                        user.usedBonusStuff = false;
                                        usedUsMod = true;
                                    }
                                }
                            }

                            if (move.statModEnemy)
                            {
                                {
                                    if (Random.Range(0f, 1f) < move.statModEnemy.chance && !usedEnMod)
                                    {
                                        StatMod statMod = move.statModEnemy.ReturnStats();
                                        statMod.inTime = statMod.time + 1;
                                        dmgTarget.heal += ((target.SetModifiers().hp * statMod.hp) / 2);
                                        dmgTarget.healMana += ((target.SetModifiers().mana * statMod.mana) / 2);
                                        dmgTarget.healStamina += ((target.SetModifiers().stamina * statMod.stamina) / 2);
                                        dmgTarget.healSanity += (int)((target.SetModifiers().sanity * statMod.sanity) / 2);
                                        target.statMods.Add(statMod);
                                        target.usedBonusStuff = false;
                                        usedEnMod = true;
                                    }
                                }
                            }

                            if (move.summon != null)
                            {
                                user.summons.Add(move.summon.ReturnSummon());
                            }

                            foreach (StatScale scale in move.scale)
                            {
                                Unit unit;
                                Stats stats;
                                if (scale.playerStat)
                                {
                                    unit = user;
                                    stats = statsUser;
                                }
                                else
                                {
                                    unit = target;
                                    stats = statsTarget;
                                }

                                dmgTarget.AddDmg(scale.SetScaleDmg(stats, unit));
                            }

                            if (isMagicCrit && !isCrit)
                                isCrit = true;

                            if (dmgTarget.phyDmg > 0)
                            {
                                if (isCrit == true)
                                    dmgTarget.ApplyCrit(isMagicCrit, statsUser.critDmg + move.critDmgBonus);
                            }
                            else
                            {
                                if (!isMagicCrit)
                                    isCrit = false;
                            }

                            foreach (Passives a in user.passives.ToArray())
                            {
                                if (a.name == "huntingseason")
                                {
                                    dmgTarget.ApplyBonusDmg(a.num, a.num, 0);
                                }

                                if (a.name == "mechashield")
                                {
                                    //convert physical damage into magic damage
                                    float con = dmgTarget.phyDmg * a.num;
                                    dmgTarget.phyDmg -= con;
                                    dmgTarget.magicDmg += con;
                                }
                            }

                            foreach (Dotdmg dot in move.dot)
                            {
                                Dotdmg dotdmg = dot.ReturnDOT();
                                switch (dotdmg.type)
                                {
                                    case Dotdmg.DmgType.PHYSICAL:
                                        dotdmg.Setup(dmgTarget.phyDmg, isCrit, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.phyDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.MAGICAL:
                                        dotdmg.Setup(dmgTarget.magicDmg, isMagicCrit, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.magicDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.TRUE:
                                        dotdmg.Setup(dmgTarget.trueDmg, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.trueDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.SANITY:
                                        dotdmg.Setup(dmgTarget.sanityDmg, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.sanityDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEAL:
                                        dotdmg.Setup(dmgTarget.heal, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.heal = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALMANA:
                                        dotdmg.Setup(dmgTarget.healMana, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.healMana = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALSTAMINA:
                                        dotdmg.Setup(dmgTarget.healStamina, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.healStamina = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALSANITY:
                                        dotdmg.Setup(dmgTarget.healSanity, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.healSanity = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.SHIELD:
                                        dotdmg.Setup(dmgTarget.shield, move.name, Dotdmg.SrcType.MOVE, user);
                                        dmgTarget.shield = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    default:
                                        continue;
                                }
                            }

                            BattleHud userHud;
                            BattleHud enemyHud;

                            if (user.isEnemy)
                            {
                                userHud = enemyHUD;
                                enemyHud = playerHUD;
                            }
                            else
                            {
                                userHud = playerHUD;
                                enemyHud = enemyHUD;
                            }

                            foreach (Passives a in target.passives.ToArray())
                            {
                                if (a.name == "roughskin")
                                {
                                    //if move is physical
                                    if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.RANGED)
                                    {
                                        //show passive popup
                                        target.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                        //get the mitigated dmg
                                        float dmgMitigated = dmgTarget.phyDmg * ((statsTarget.dmgResis * a.num) / 100);
                                        //subtract mitigated dmg from the dmg
                                        if (dmgMitigated < dmgTarget.phyDmg)
                                            dmgTarget.phyDmg -= dmgMitigated;
                                        else
                                        {
                                            dmgMitigated = dmgTarget.phyDmg;
                                            dmgTarget.phyDmg = 0;
                                        }
                                        //add mitigated dmg to the overview
                                        target.phyDmgMitigated += dmgMitigated;
                                    }
                                }

                                if (a.name == "dreadofthesupernatural")
                                {
                                    //if sanityDmg bellow 0
                                    if (dmgTarget.sanityDmg > 0)
                                    {
                                        //show passive popup
                                        target.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                        //get bonus sanityDmg
                                        int bonusSanityDmg = (int)(dmgTarget.sanityDmg * a.num);
                                        //if bonus under a.maxNum, set to it
                                        if (bonusSanityDmg < a.maxNum)
                                            bonusSanityDmg = (int)a.maxNum;
                                        //add bonus damage
                                        dmgTarget.sanityDmg += bonusSanityDmg;
                                    }
                                }

                                if (a.name == "bullsrage")
                                {
                                    if (isCrit)
                                    {
                                        if (a.stacks < a.maxStacks && a.stacks >= 0)
                                        {
                                            a.stacks++;
                                            ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), target.isEnemy, a.GetPassiveInfo());
                                        }
                                    }
                                }
                            }

                            foreach (Passives a in user.passives.ToArray())
                            {
                                if (a.name == "perfectshooter")
                                {
                                    if ((move.type is Moves.MoveType.RANGED || move.type is Moves.MoveType.BASIC) && isCrit == true && a.stacks < a.maxStacks)
                                        a.num++;

                                    bool isReady = false;
                                    if (a.num == a.maxNum - 1)
                                        isReady = true;

                                    if (a.num == a.maxNum && a.stacks < a.maxStacks)
                                    {
                                        a.num = 0;
                                        a.stacks++;
                                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                        StatMod mod = a.ifConditionTrueMod();

                                        StatMod statMod = a.statMod.ReturnStats();
                                        statMod.inTime = statMod.time;
                                        user.statMods.Add(statMod);
                                        user.usedBonusStuff = false;
                                        userHud.SetStatsHud(user);
                                        DestroyPassiveIcon(a.name, user.isEnemy);
                                    }

                                    ManagePassiveIcon(a.sprite, a.name, (a.stacks + "S" + (a.maxNum - a.num) + "T").ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                                }
                            }

                            //magic pen for 0
                            dmgTarget = target.MitigateDmg(dmgTarget, dmgResisPer, magicResisPer, user.SetModifiers().armourPen, 0, user);

                            dmgTarget = user.ApplyHealFrom(dmgTarget, move.healFromDmgType, move.healFromDmg);
                            dmgTarget = user.ApplyLifesteal(dmgTarget);
                            dmgTarget = dmgUser.TransferHeals(dmgTarget);
                            float dmgT = dmgTarget.phyDmg + dmgTarget.magicDmg + dmgTarget.trueDmg;

                            if (dmgT > 0)
                            {
                                if (!(move.isUlt))
                                    SetUltNumber(user, userHud, dmgT, true);

                                if (move.isUlt)
                                    SetUltNumber(target, enemyHud, (dmgT / 2), false);
                                else
                                    SetUltNumber(target, enemyHud, dmgT, false);
                            }

                            isDead = target.TakeDamage(dmgTarget, isCrit, isMagicCrit, user);
                            user.TakeDamage(dmgUser, isCrit, isMagicCrit, user, move.name);

                            target.DoAnimParticle(move.animTarget);
                            user.DoAnimParticle(move.animUser);

                            if (blockPhysical)
                            {
                                user.isBlockingPhysical = blockPhysical;
                            }

                            if (blockMagic)
                            {
                                user.isBlockingMagical = blockMagic;
                            }

                            if (blockRanged)
                            {
                                user.isBlockingRanged = blockRanged;
                            }

                            //grant passive to the user
                            if (move.grantPassive)
                            {
                                user.passives.Add(move.grantPassive.ReturnPassive());
                            }

                            foreach (Passives a in target.passives.ToArray())
                            {
                                if (a.name == "onewiththeshadows")
                                {
                                    if (!(move.type is Moves.MoveType.DEFFENCIVE || move.type is Moves.MoveType.ENCHANT || move.type is Moves.MoveType.SUPPORT)) { 
                                        a.inCd = 0;
                                        ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                    } else
                                    {
                                        if (a.inCd < a.cd)
                                        {
                                            a.inCd++;
                                            ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                        }
                                    }
                                }
                            }

                            SetStatus();

                            yield return new WaitForSeconds(1.75f);

                            if (isDead)
                            {
                                if (target.isEnemy == true)
                                    state = BattleState.WIN;
                                else
                                    state = BattleState.LOSE;
                            }
                        }
                        sumPlayerHud.UpdateValues(playerUnit, langmanag.GetInfo("charc", "name", playerUnit.charc.name));
                        sumEnemyHud.UpdateValues(enemyUnit, langmanag.GetInfo("charc", "name", enemyUnit.charc.name));
                    }
                    else
                    {
                        target.Miss(false);

                        if (move.isUlt)
                        {
                            GrantUltCompansation(user);
                        }

                        yield return new WaitForSeconds(1.15f);

                        if (cancelText == "")
                            cancelText = langmanag.GetInfo("gui", "text", "miss");

                        dialogText.text = cancelText.Replace("%p%", langmanag.GetInfo("charc", "name", user.charc.name));
                        yield return new WaitForSeconds(1.1f);

                        foreach (Passives a in target.passives.ToArray())
                        {
                            if (a.name == "onewiththeshadows")
                            {
                                if (a.inCd < a.cd)
                                {
                                    a.inCd++;
                                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void SetEffectIcon(Effects effect, GameObject pannel)
    {
        barIconPrefab.name = effect.id;

        Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
        icon.sprite = effect.sprite;
        Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
        text.text = effect.duration.ToString();
        TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
        tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
        tooltipButton.text = effect.GetEffectInfo();

        Instantiate(barIconPrefab, pannel.transform);
    }

    void SetUltNumber(Unit user, BattleHud hud, float dmg, bool isDealt)
    {
        Stats temp = user.SetModifiers();

        if (user.ult < 100)
        {
            float value = user.ult;

            if (dmg > 0)
            {
                if (isDealt)
                    value += dmg * ultEnergyDealt;
                else
                    value += dmg * ultEnergyTaken;
            }

            if (value > 0)
                value = value * temp.ultrate;

            if (value > 100)
                value = 100;

            user.ult = value;
        }
    }


    void SetStatus()
    {
        Stats statsP = playerUnit.SetModifiers();
        Stats statsE = enemyUnit.SetModifiers();

        if (turnCount == 1 && state is BattleState.START)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1) {
                playerUnit.curHp = statsP.hp * info.playerHp;
                playerUnit.curMana = statsP.mana * info.playerMn;
                playerUnit.curStamina = statsP.stamina * info.playerSta;

                enemyUnit.curHp = statsE.hp;
                enemyUnit.curMana = statsE.mana;
                enemyUnit.curStamina = statsE.stamina;
            } else
            {
                playerUnit.curHp = statsP.hp;
                playerUnit.curMana = statsP.mana;
                playerUnit.curStamina = statsP.stamina;

                enemyUnit.curHp = statsE.hp;
                enemyUnit.curMana = statsE.mana;
                enemyUnit.curStamina = statsE.stamina;
            }
        }

        StartCoroutine(playerHUD.SetHp(playerUnit.curHp, statsP.hp));
        StartCoroutine(enemyHUD.SetHp(enemyUnit.curHp, statsE.hp));
        StartCoroutine(playerHUD.SetMana(playerUnit.curMana, statsP.mana));
        StartCoroutine(enemyHUD.SetMana(enemyUnit.curMana, statsE.mana));
        StartCoroutine(playerHUD.SetStamina(playerUnit.curStamina, statsP.stamina, (int)(statsP.stamina * (tiredStart + (tiredGrowth * tiredStacks)))));
        StartCoroutine(enemyHUD.SetStamina(enemyUnit.curStamina, statsE.stamina, (int)(statsE.stamina * (tiredStart + (tiredGrowth * tiredStacks)))));
        StartCoroutine(playerHUD.SetShield(playerUnit.curShield));
        StartCoroutine(enemyHUD.SetShield(enemyUnit.curShield));
        playerHUD.SetUlt(playerUnit.ult, playerUnit.SetModifiers().ultrate);
        enemyHUD.SetUlt(enemyUnit.ult, enemyUnit.SetModifiers().ultrate);
        playerHUD.SetBlood(playerUnit.bloodStacks);
        enemyHUD.SetBlood(enemyUnit.bloodStacks);
    }

    Moves EnemyChooseMove()
    {
        Moves move = null;
        bool skip = false;

        if (enemyUnit.curMana < (enemyUnit.SetModifiers().mana * aiManaRecover) && !enemyUnit.moves.Contains(enemyUnit.recoverMana))
            enemyUnit.moves.Add(enemyUnit.recoverMana);
        else if (enemyUnit.curMana > (enemyUnit.SetModifiers().mana * aiManaRecover) && enemyUnit.moves.Contains(enemyUnit.recoverMana))
            enemyUnit.moves.Remove(enemyUnit.recoverMana);

        if (((enemyUnit.ult == 100 && enemyUnit.ultMove.needFullUlt) || 
        (enemyUnit.ult == enemyUnit.ultMove.ultCost && !enemyUnit.ultMove.needFullUlt))
        && !enemyUnit.moves.Contains(enemyUnit.ultMove))
            enemyUnit.moves.Add(enemyUnit.ultMove);
        else if (((enemyUnit.ult < 100 && enemyUnit.ultMove.needFullUlt) ||
        (enemyUnit.ult < enemyUnit.ultMove.ultCost && !enemyUnit.ultMove.needFullUlt))
        && enemyUnit.moves.Contains(enemyUnit.ultMove))
            enemyUnit.moves.Remove(enemyUnit.ultMove);

        int random = 0;
        int i = 0;

        if (!enemyUnit.canUseMagic && !enemyUnit.canUsePhysical && !enemyUnit.canUseRanged && !enemyUnit.canUseEnchant && !enemyUnit.canUseSupp
            && !enemyUnit.canUseProtec && !enemyUnit.canUseSummon)
            skip = true;

        do
        {
            if (enemyUnit.curMana <= (enemyUnit.SetModifiers().mana * aiGaranteedManaRecover))
            {
                if (enemyUnit.canUseSupp && enemyUnit.moves.Contains(enemyUnit.recoverMana) && enemyUnit.recoverMana.inCooldown == 0)
                    random = enemyUnit.moves.Count - 1;
                else
                {
                    Stats statsU = enemyUnit.SetModifiers();
                    Stats statsT = playerUnit.SetModifiers();

                    random = enemyUnit.charc.ai.chooseMove(enemyUnit.moves, enemyUnit, playerUnit, statsU, statsT);
                }
            }
            else
            {
                Stats statsU = enemyUnit.SetModifiers();
                Stats statsT = playerUnit.SetModifiers();

                random = enemyUnit.charc.ai.chooseMove(enemyUnit.moves, enemyUnit, playerUnit, statsU, statsT);
            }

            foreach (Moves a in enemyUnit.moves)
            {
                if (!skip)
                {
                    if (i == random)
                        if (enemyUnit.curMana >= a.manaCost && enemyUnit.curStamina >= a.staminaCost)
                        {
                            int inCd = a.inCooldown;

                            if (inCd <= 0)
                            {
                                bool canUse = true;

                                if (!a.isUlt)
                                {
                                    switch (a.type)
                                    {
                                        case Moves.MoveType.PHYSICAL:
                                            if (!enemyUnit.canUsePhysical)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.MAGICAL:
                                            if (!enemyUnit.canUseMagic)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.RANGED:
                                            if (!enemyUnit.canUseRanged)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.DEFFENCIVE:
                                            if (!enemyUnit.canUseProtec)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.SUPPORT:
                                            if (!enemyUnit.canUseSupp)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.ENCHANT:
                                            if (!enemyUnit.canUseEnchant)
                                                canUse = false;
                                            break;
                                        case Moves.MoveType.SUMMON:
                                            if (!enemyUnit.canUseSummon)
                                                canUse = false;
                                            break;
                                    }
                                } else
                                {
                                    enemyUnit.ult -= enemyUnit.ultMove.ultCost;
                                }

                                if (canUse)
                                    move = a;
                            }
                            else
                                move = null;
                        }
                        else
                        {
                            move = null;
                        }
                }
                else
                    move = a;

                i++;
            }
            i = 0;
            StartCoroutine(WaitWhile());
        } while (move == null);

        if (skip)
            return null;
        else
            return move;
    }

    IEnumerator WaitWhile()
    {
        yield return null;
    }

    IEnumerator EndBattle()
    {
        SetStatus();
        if (state == BattleState.WIN)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1)
            {
                PlayerPrefs.SetInt("wonLastRound", 1);
                info.round++;

                float health = playerUnit.curHp + (playerUnit.charc.stats.hp - playerUnit.curHp) * perHealthRegenEndless;
                float mana = playerUnit.curMana + (playerUnit.charc.stats.mana - playerUnit.curMana) * perCostsRegenEndless;
                float stamina = playerUnit.curStamina + (playerUnit.charc.stats.stamina - playerUnit.curStamina) * perCostsRegenEndless;
                int sanity = (int)(playerUnit.curSanity + (playerUnit.charc.stats.sanity - playerUnit.curSanity) * perSanityRegenEndless);
                float ult = playerUnit.ult - (playerUnit.ult*perUltReduce);

                info.playerHp = ((100 * health) / playerUnit.charc.stats.hp)/100;
                info.playerMn = ((100 * mana) / playerUnit.charc.stats.mana)/100;
                info.playerSta = ((100 * stamina) / playerUnit.charc.stats.stamina)/100;
                info.playerSan = ((float)(100 * sanity) / playerUnit.charc.stats.sanity) / 100;
                info.playerUlt = ult;

                SaveSystem.Save(info);
            }
                
            yield return new WaitForSeconds(1.6f);
            playerUnit.WonLost(true);
            enemyUnit.WonLost(false);
            dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", enemyUnit.charc.name));
            yield return new WaitForSeconds(1.8f);
            dialogText.text = langmanag.GetInfo("gui", "text", "win", langmanag.GetInfo("charc", "name", playerUnit.charc.name));
        } else if (state == BattleState.LOSE)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1)
                PlayerPrefs.SetInt("wonLastRound", 0);

            yield return new WaitForSeconds(1.6f);
            playerUnit.WonLost(false);
            enemyUnit.WonLost(true);
            dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", playerUnit.charc.name));
            yield return new WaitForSeconds(1.8f);
            dialogText.text = langmanag.GetInfo("gui", "text", "win", langmanag.GetInfo("charc", "name", enemyUnit.charc.name));
        }
        yield return new WaitForSeconds(1.5f);

        leaveBtn.SetActive(true);
        sumHudHideBtn.SetActive(false);
        ShowOverview(true);
    }

    void PlayerTurn()
    {
        dialogText.text = langmanag.GetInfo("gui", "text", "choosemove");
        movesBtn.interactable = true;
        basicBtn.interactable = true;

        if ((playerUnit.ult == 100 && playerUnit.ultMove.needFullUlt) ||
        (playerUnit.ult == playerUnit.ultMove.ultCost && !playerUnit.ultMove.needFullUlt))
        {
            bool canUse = true;

            switch (playerUnit.ultMove.type)
            {
                case Moves.MoveType.PHYSICAL:
                    if (!playerUnit.canUsePhysical)
                        canUse = false;
                    break;
                case Moves.MoveType.MAGICAL:
                    if (!playerUnit.canUseMagic)
                        canUse = false;
                    break;
                case Moves.MoveType.RANGED:
                    if (!playerUnit.canUseRanged)
                        canUse = false;
                    break;
                case Moves.MoveType.DEFFENCIVE:
                    if (!playerUnit.canUseProtec)
                        canUse = false;
                    break;
                case Moves.MoveType.SUPPORT:
                    if (!playerUnit.canUseSupp)
                        canUse = false;
                    break;
                case Moves.MoveType.ENCHANT:
                    if (!playerUnit.canUseEnchant)
                        canUse = false;
                    break;
                case Moves.MoveType.SUMMON:
                    if (!playerUnit.canUseSummon)
                        canUse = false;
                    break;
            }

            if (canUse)
                ultBtn.interactable = true;
            else
                ultBtn.interactable = false;
        }

        if (playerUnit.recoverMana.inCooldown <= 0)
        {
            healManaBtn.interactable = true;
            healBtnText.text = langmanag.GetInfo("moves", "recovmana");
            if (!playerUnit.canUseSupp)
                healManaBtn.interactable = false;
        }
        else
        {
            healManaBtn.interactable = false;
            healBtnText.text = langmanag.GetInfo("moves", "recovmana") + " (" + playerUnit.recoverMana.inCooldown + ")";
        }  
    }

    public void OnMoveBtn(int moveId)
    {
        tooltipMain.GetComponent<TooltipPopUp>().HideInfo();
        tooltipMain.GetComponent<TooltipPopUp>().ForceResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().HideInfo();
        tooltipSec.GetComponent<TooltipPopUp>().ForceResetLastBtn();

        Moves move = null;
        int i = 0;

        foreach (Moves a in playerUnit.moves)
        {
            i++;
            if (i == moveId)
                move = a;
        }

        StartCoroutine(Combat(move));
    }

    public void OnAtkBtn()
    {
        int i = 0;
        foreach(Moves move in playerUnit.moves)
        {
            if (move.name != "basicattack")
            {
                string name = move.name + "(Clone)";
                i++;
                GameObject moveBtnGO = moveListHud.Find(name).gameObject;
                Button moveBtn = moveBtnGO.GetComponent<Button>();

                Text cd = moveBtnGO.transform.Find("Cooldown").gameObject.GetComponent<Text>();
                int inCd = move.inCooldown;

                Text sta = moveBtnGO.transform.Find("Stamina").gameObject.GetComponent<Text>();
                Text mn = moveBtnGO.transform.Find("Mana").gameObject.GetComponent<Text>();

                mn.text = ((int)(move.manaCost * playerUnit.SetModifiers().manaCost)).ToString();
                if (playerUnit.curMana < (move.manaCost * playerUnit.SetModifiers().manaCost))
                    mn.color = Color.red;
                else
                    mn.color = Color.black;

                sta.text = ((int)(move.staminaCost * playerUnit.SetModifiers().staminaCost)).ToString();
                if (playerUnit.curStamina < (move.staminaCost * playerUnit.SetModifiers().staminaCost))
                    sta.color = Color.red;
                else
                    sta.color = Color.black;

                if (inCd <= 0)
                    cd.text = move.cooldown.ToString();
                else
                    cd.text = move.cooldown.ToString() + " (" + inCd.ToString() + ")";

                bool canUse = true;

                switch (move.type)
                {
                    case Moves.MoveType.PHYSICAL:
                        if (!playerUnit.canUsePhysical)
                            canUse = false;
                        break;
                    case Moves.MoveType.MAGICAL:
                        if (!playerUnit.canUseMagic)
                            canUse = false;
                        break;
                    case Moves.MoveType.RANGED:
                        if (!playerUnit.canUseRanged)
                            canUse = false;
                        break;
                    case Moves.MoveType.DEFFENCIVE:
                        if (!playerUnit.canUseProtec)
                            canUse = false;
                        break;
                    case Moves.MoveType.SUPPORT:
                        if (!playerUnit.canUseSupp)
                            canUse = false;
                        break;
                    case Moves.MoveType.ENCHANT:
                        if (!playerUnit.canUseEnchant)
                            canUse = false;
                        break;
                    case Moves.MoveType.SUMMON:
                        if (!playerUnit.canUseSummon)
                            canUse = false;
                        break;
                }

                if (playerUnit.curMana < (move.manaCost*playerUnit.SetModifiers().manaCost) 
                    || playerUnit.curStamina < (move.staminaCost * playerUnit.SetModifiers().staminaCost) || inCd > 0)
                    canUse = false;

                if (canUse)
                    moveBtn.interactable = true;
                else
                    moveBtn.interactable = false;
            }
        }

        panelMoves.SetActive(true);
        scrollbar.value = 1;
    }

    public void OnHealBtn()
    {
        playerUnit.recoverMana.inCooldown = playerUnit.recoverMana.cooldown;
        StartCoroutine(Combat(playerUnit.recoverMana.ReturnMove()));
    }

    public void OnUltBtn()
    {
        StartCoroutine(Combat(playerUnit.ultMove));
        playerUnit.ult -= playerUnit.ultMove.ultCost;
    }

    public void OnBasicBtn()
    {
        StartCoroutine(Combat(playerUnit.basicAttack.ReturnMove()));
    }

    public void CancelBtn()
    {
        panelMoves.SetActive(false);
    }

    public void GrantUltCompansation(Unit user)
    {
        StatMod mod = new StatMod();

        mod.time = ultCompDuration;
        mod.inTime = ultCompDuration;
        mod.ultrate = ultComp;

        user.statMods.Add(mod);
    }

    public bool EffectCalcDmg(Effects a, Unit user)
    {
        DMG dmg = default;
        dmg.Reset();
        bool isDead = false;

        dmg.phyDmg = Random.Range(a.phyDmgMin, a.phyDmgMax) + (a.phyDmgInc * a.timesInc);
        dmg.magicDmg = Random.Range(a.magicDmgMin, a.magicDmgMax) + (a.magicDmgInc * a.timesInc);
        dmg.trueDmg = Random.Range(a.trueDmgMin, a.trueDmgMax) + (a.trueDmgInc * a.timesInc);
        dmg.sanityDmg = Random.Range(a.sanityDmgMin, a.sanityDmgMax) + (a.sanityDmgInc * a.timesInc);
        dmg.heal = Random.Range(a.healMin, a.healMax) + (a.healInc * a.timesInc);
        dmg.healMana = Random.Range(a.healManaMin, a.healManaMax) + (a.healManaInc * a.timesInc);
        dmg.healStamina = Random.Range(a.healStaminaMin, a.healStaminaMax) + (a.healStaminaInc * a.timesInc);
        dmg.healSanity = Random.Range(a.sanityHealMin, a.sanityHealMax) + (a.sanityHealInc * a.timesInc);
        dmg.shield = Random.Range(a.shieldMin, a.shieldMax) + (a.shieldInc * a.timesInc);

        if (!a.isScaleSpecial)
            foreach (StatScale scale in a.scale.ToArray())
            {
                Unit unit;
                Stats stats;

                unit = user;
                stats = user.SetModifiers().ReturnStats();

                dmg.AddDmg(scale.SetScaleDmg(stats, unit));
            }

        dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0, null, dotReduc);
        dmg = user.CalcRegens(dmg);

        isDead = user.TakeDamage(dmg, false, false, user);

        /*user.sanityDmgTaken += dmg.sanityDmg;

        user.healDone += dmg.heal;
        user.manaHealDone += dmg.healMana;
        user.staminaHealDone += dmg.healStamina;
        user.sanityHealDone += dmg.healSanity;*/

        return isDead;
    }

    public bool DotCalc(Dotdmg dot, Unit user)
    {
        Dotdmg a = dot.ReturnDOT();
        DMG dmg = default;
        dmg.Reset();

        switch (a.type)
        {
            case Dotdmg.DmgType.PHYSICAL:
                if (a.dmg > 0)
                {
                    dmg.phyDmg += a.dmg;
                    dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0, a.source, dotReduc);
                    user.ApplyLifesteal(dmg);
                }
                break;
            case Dotdmg.DmgType.MAGICAL:
                if (a.dmg > 0)
                {
                    dmg.magicDmg += a.dmg;
                    dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0, a.source, dotReduc);
                }
                break;
            case Dotdmg.DmgType.TRUE:
                    dmg.trueDmg += a.dmg;
                break;
            case Dotdmg.DmgType.SANITY:
                    dmg.sanityDmg += (int)a.dmg;
                break;
            case Dotdmg.DmgType.HEAL:
                    dmg.heal += a.dmg;
                break;
            case Dotdmg.DmgType.HEALMANA:
                    dmg.healMana += a.dmg;
                break;
            case Dotdmg.DmgType.HEALSTAMINA:
                    dmg.healStamina += a.dmg;
                break;
            case Dotdmg.DmgType.HEALSANITY:
                    dmg.healSanity += (int)a.dmg;
                break;
            case Dotdmg.DmgType.SHIELD:
                dmg.shield += a.dmg;
                break;
        }

        foreach (Passives p in user.passives.ToArray())
        {
            if (p.name == "dreadofthesupernatural")
            {
                //if sanityDmg bellow 0
                if (a.dmg > 0)
                {
                    //show passive popup
                    user.PassivePopup(langmanag.GetInfo("passive", "name", p.name));
                    //get bonus sanityDmg
                    int bonusSanityDmg = (int)(a.dmg * p.num);
                    //if bonus under a.maxNum, set to it
                    if (bonusSanityDmg < p.maxNum)
                        bonusSanityDmg = (int)p.maxNum;
                    //add bonus damage
                    a.dmg += bonusSanityDmg;
                }
            }
        }
        
        //isDead
        return user.TakeDamage(dmg, false, false, user);
    }

    void CreatePassiveIcon(Sprite pIcon, string name, string num, bool isEnemy, string passiveDesc)
    {
        barIconPrefab.name = name;

        Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
        icon.sprite = pIcon;
        Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
        text.text = num.ToString();

        TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
        tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
        tooltipButton.text = passiveDesc;

        if (!isEnemy)
            Instantiate(barIconPrefab, panelEffectsP.transform);
        else
            Instantiate(barIconPrefab, panelEffectsE.transform);
    }

    void ManagePassiveIcon(Sprite pIcon, string name, string num, bool isEnemy, string passiveDesc, bool isReady = false)
    {
        if (num == "0")
            num = "";

        if (!isEnemy)
            if (panelEffectsP.transform.Find(name + "(Clone)"))
            {
                panelEffectsP.transform.Find(name + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = num;
                panelEffectsP.transform.Find(name + "(Clone)").gameObject.GetComponent<Animator>().SetBool("ready", isReady);
            }
            else
                CreatePassiveIcon(pIcon, name, num, isEnemy, passiveDesc);
        else
            if (panelEffectsE.transform.Find(name + "(Clone)"))
            {
                panelEffectsE.transform.Find(name + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = num;
                panelEffectsE.transform.Find(name + "(Clone)").gameObject.GetComponent<Animator>().SetBool("ready", isReady);
            }
            else
                CreatePassiveIcon(pIcon, name, num, isEnemy, passiveDesc);
    }

    void DestroyPassiveIcon(string name, bool isEnemy)
    {
        if (!isEnemy)
        {
            if (panelEffectsP.transform.Find(name + "(Clone)"))
            {
                GameObject temp = panelEffectsP.transform.Find(name + "(Clone)").gameObject;
                Destroy(temp.gameObject);
            }
        }
        else
        {
            if (panelEffectsE.transform.Find(name + "(Clone)"))
            {
                GameObject temp = panelEffectsE.transform.Find(name + "(Clone)").gameObject;
                Destroy(temp.gameObject);
            }
        } 
    }

    void CheckPassiveTurn(Unit user, BattleHud userHud, Unit target)
    {
        foreach (Passives a in user.passives.ToArray())
        {
            if (a.name == "baby" || a.name == "weak" || a.name == "normal" || a.name == "strong" || a.name == "superstrong" || a.name == "legendary" || a.name == "mythic" || a.name == "boss")
            {
                if (a.stacks == 0)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    a.stacks++;
                }
            }

            if (a.name == "musicup")
            {
                bool isReady = false;

                if (a.inCd == 1 && a.stacks < a.maxStacks)
                {
                    a.inCd = a.cd;
                    a.stacks++;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }
                else if (a.inCd > 0)
                    a.inCd--;

                if (a.stacks == a.maxStacks)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }

                if (a.inCd == 1)
                    isReady = true;

                if (a.stacks < a.maxStacks)
                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "manathirst")
            {
                if (a.inCd == 0 && (user.curMana <= (user.SetModifiers().mana * a.num)))
                {
                    a.inCd = a.cd;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                    float healMana = a.statScale.SetScale(user.SetModifiers(), user);
                    user.curMana += healMana;
                    user.manaHealDone += healMana;

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }
                else if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "wildinstinct")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                if (hpPer < (a.num * 100))
                {
                    a.stacks = (int)((a.num * 100) - hpPer);

                    StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
                else if (hpPer > (a.num * 100))
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }

            if (a.name == "vendetta")
            {
                if (a.stacks == 1)
                {
                    if (a.inCd > 0)
                        a.inCd--;

                    bool isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                }
            }

            if (a.name == "lastbreath")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                if (hpPer < (a.num * 100))
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
                else if (hpPer > (a.num * 100))
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }

            if (a.name == "bullsrage")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                if (hpPer <= (a.num * 100))
                {
                    if (a.stacks < a.maxStacks + 1)
                    {
                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                        a.stacks = a.maxStacks + 1;
                    }
                } else if (a.stacks > a.maxStacks && hpPer > (a.num * 100))
                {
                    a.stacks = 0;
                    a.inCd = -1;
                }

                if ((a.stacks == a.maxStacks && a.inCd == 0) || a.stacks > a.maxStacks)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    if (a.stacks == a.maxStacks)
                    {
                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                        statMod.inTime = a.cd;
                        a.inCd = a.cd+1;
                    }
                    else
                    {
                        statMod.inTime = 1;
                    }

                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }

                bool isReady = false;
                if (a.inCd > 0 || a.inCd == -1)
                    isReady = true;

                if (a.inCd > 0 || a.stacks > a.maxStacks)
                {
                    if (a.stacks > a.maxStacks)
                        ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    else
                    {
                        a.inCd--;
                        ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    }
                } else
                {
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                }

                if (a.inCd == 0 && a.stacks >= a.maxStacks)
                {
                    a.stacks = 0;
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                }
            }

            if (a.name == "magicremains")
            {
                if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "galeglide")
            {
                if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "fearsmell")
            {
                //enemy sanity in %
                int sanityPer = (int)((100 * target.curSanity) / target.SetModifiers().sanity);

                if (sanityPer < (a.num * 100))
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = 1;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name,"", user.isEnemy, a.GetPassiveInfo());
                }

                bool foundEffect = false;

                foreach (Effects b in target.effects)
                {
                    if (b.id == "FEA")
                    {
                        foundEffect = true;
                        if (a.stacks < 1)
                        {
                            a.stacks++;
                            StatMod statMod2 = a.statMod2.ReturnStats();
                            statMod2.inTime = a.cd;
                            user.statMods.Add(statMod2);
                            user.usedBonusStuff = false;
                            userHud.SetStatsHud(user);
                            ManagePassiveIcon(a.sprite, a.name, "!", user.isEnemy, a.GetPassiveInfo());
                        }
                    }
                }

                if (!foundEffect)
                    a.stacks = 0;

                if (sanityPer > (a.num * 100) && !foundEffect)
                    DestroyPassiveIcon(a.name, user.isEnemy);

            }

            if (a.name == "phantomhand")
            {
                if (a.inCd > 0)
                    a.inCd--;

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "bloodbath")
            {
                bool foundEffect = false;

                foreach (Effects b in target.effects)
                {
                    if (b.id == "BLD")
                    {
                        foundEffect = true;

                        if (a.stacks == 0)
                        {
                            user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            a.stacks = 1;
                        }
                        else if (a.stacks == 1)
                        {
                            a.stacks = 2;
                        }

                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    }
                }

                if (!foundEffect && a.stacks > 0)
                {
                    a.stacks = 0;
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }

            if (a.name == "plasmablade")
            {
                bool isSilence = false;
                foreach (Effects b in user.effects)
                {
                    if (b.id == "SLC")
                        isSilence = true;
                }

                int manaPer = (int)((100 * user.curMana) / user.SetModifiers().mana);
                bool enoughMana = false;

                if (manaPer > a.maxNum * 100)
                {
                    enoughMana = true;
                    ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                }

                if (!enoughMana || isSilence)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }

                int healthPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                if (healthPer <= a.stacks && enoughMana && !isSilence)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    //isReady
                    ManagePassiveIcon(a.sprite, a.name, "!", user.isEnemy, a.GetPassiveInfo(), true);
                }
            }

            if (a.name == "weakbody")
            {
                bool foundEffect = false;

                //check effects
                foreach (Effects b in user.effects)
                {
                    //check if have blood, allergy, tired or poison
                    if (b.id == "BLD" || b.id == "ALR" || b.id == "TRD" || b.id == "PSN" && !foundEffect)
                    {
                        foundEffect = true;

                        //if no stacks
                        if (a.stacks == 0)
                        {
                            //show popup
                            user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            a.stacks = 1;
                        }

                        //apply statmod
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);

                        //display icon
                        ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }
                }

                //if dont find, delete icon
                if (!foundEffect && a.stacks > 0)
                {
                    a.stacks = 0;
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }

            if (a.name == "onewiththeshadows")
            {
                //if the user was not hit for cd turns
                if (a.inCd == a.cd)
                {
                    //if no stacks
                    if (a.stacks == 0)
                    {
                        //show popup
                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                        a.stacks = 1;
                    }

                    //apply statmod
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);

                    //display icon
                    bool isReady = false;
                    if (a.stacks == 1)
                        isReady = true;
                    ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                } else
                {
                    a.stacks = 0;
                }
            }

            if (a.name == "courage")
            {
                //if cooldown higher than 0
                if (a.inCd > 0)
                //reduce cooldown by 1
                    a.inCd--;

                //if cooldown is 0
                if (a.inCd <= 0 && user.curSanity < user.SetModifiers().sanity)
                {
                    //get sanity heal from the passive's scale
                    float healSanity = a.statScale.SetScaleFlat(user.SetModifiers(), user);
                    //heal sanity
                    user.curSanity += (int)healSanity;
                    //add the heal to the overview
                    user.sanityHealDone += (int)healSanity;
                    //reset cooldown
                    a.inCd = a.cd;
                    //show popup
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                }

                bool isFear = false;
                foreach (Effects b in user.effects)
                {
                    //if fear
                    if (b.id == "FEA" && !isFear)
                    {
                        if (a.stacks < 1)
                        {
                            //get sanity heal from the passive's scale
                            float healSanity = a.statScale2.SetScale(user.SetModifiers(), user);
                            //heal sanity
                            user.curSanity += (int)healSanity;
                            //add the heal to the overview
                            user.sanityHealDone += (int)healSanity;
                            //mark as bonus given
                            a.stacks++;
                        }
                        isFear = true;
                    } 
                }
                
                if (!isFear)
                    a.stacks = 0;

                bool isReady = false;
                if (a.inCd == 1)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "bloodpumping")
            {
                if (a.inCd > 0 && user.curHp < user.SetModifiers().hp)
                {
                    bool foundEffect = false;

                    foreach (Effects b in user.effects)
                    {
                        //check if have blood, allergy, tired or poison
                        if (b.id == "BLD" && !foundEffect)
                        {
                            foundEffect = true;
                        }
                    }
                    
                    if (foundEffect)
                    {
                        DMG dmg = default;
                        dmg.Reset();
                        dmg.trueDmg += a.statScale.SetScale(user.SetModifiers(), user);

                        //user.curHp -= trueDmg;
                        //user.trueDmgTaken += trueDmg;
                        user.TakeDamage(dmg, false, false, user);
                    } else
                    {
                        float heal = a.statScale.SetScale(user.SetModifiers(), user);

                        if ((user.curHp + heal) < user.SetModifiers().hp)
                        {
                            heal += heal * user.SetModifiers().healBonus;
                            user.curHp += heal;
                            user.healDone += heal;
                        } else
                        {
                            heal = user.curHp - user.SetModifiers().hp;
                            user.curHp = user.SetModifiers().hp;

                            if (heal < 0)
                                heal = 0;

                            user.healDone += heal;
                        }
                        user.Heal(heal);
                    }

                    a.inCd--;
                }
                    

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "huntingseason")
            {
                if (a.inCd > 0)
                    a.inCd--;

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "blazingfists")
            {
                if (a.inCd > 0)
                    a.inCd--;

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "fighterinstinct")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                if (hpPer <= (a.num*100) && a.stacks != 1)
                {
                    a.stacks = 1;
                }
                else if (hpPer > (a.num * 100))
                {
                    a.stacks = 0;
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }

                if (a.stacks == 1)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
            }

            if (a.name == "successoroffire")
            {
                bool foundEffect = false;

                foreach (Effects b in target.effects)
                {
                    if (b.id == "BRN" || b.id == "SCH")
                    {
                        foundEffect = true;
                    }
                }

                if (foundEffect)
                {
                    ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());

                    Stats statsUser = user.SetModifiers();

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    statMod.atkDmg = statsUser.magicPower * a.num;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }
                else
                    DestroyPassiveIcon(a.name, user.isEnemy);
            }
            
            if (a.name == "zenmode")
            {
                //stamina in %
                int staPer = (int)((100 * user.curStamina) / user.SetModifiers().stamina);

                if (staPer <= a.num*100)
                {
                    if (a.stacks != 1)
                    {
                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                        a.stacks++;
                    }    
                }
                else if (staPer >= a.maxNum*100 && a.stacks == 1)
                {
                    a.stacks = 0;
                }

                if (a.stacks == 1)
                {
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                } else
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }

            if (a.name == "manasword")
            {
                if (a.stacks > 0)
                {
                    StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;

                    if (a.stacks == a.maxStacks)
                    {
                        StatMod statMod2 = a.statMod2.ReturnStats();
                        statMod2.inTime = statMod2.time;
                        user.statMods.Add(statMod2);
                        user.usedBonusStuff = false;
                    }

                    userHud.SetStatsHud(user);
                }

                bool isReady = false;
                if (a.stacks == a.maxStacks)
                    isReady = true;
                ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "manascepter")
            {
                if (a.stacks > 0)
                {
                    StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;

                    if (a.stacks == a.maxStacks)
                    {
                        StatMod statMod2 = a.statMod2.ReturnStats();
                        statMod2.inTime = statMod2.time;
                        user.statMods.Add(statMod2);
                        user.usedBonusStuff = false;
                    }

                    userHud.SetStatsHud(user);
                }

                bool isReady = false;
                if (a.stacks == a.maxStacks)
                    isReady = true;
                ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "spectralring")
            {
                if (a.inCd == 1 && a.stacks < a.maxStacks)
                {
                    a.inCd = a.cd;
                    a.stacks++;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }
                else if (a.inCd > 0)
                    a.inCd--;

                if (a.stacks == a.maxStacks)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }

                if (a.stacks < a.maxStacks)
                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "shadowdagger")
            {
                if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "toxicteeth")
            {
                if (a.inCd > 0)
                    a.inCd--;

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "gravitybelt")
            {
                if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
            }

            if (a.name == "huntersdirk")
            {
                if (a.inCd > 0)
                    a.inCd--;

                bool isReady = false;
                if (a.inCd == 0)
                    isReady = true;

                float hpPer = (100 * target.curHp) / target.SetModifiers().hp;

                if (hpPer < a.num)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                } else
                {
                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                }
            }

            if (a.name == "gravitychange")
            {
                if (a.inCd > 0)
                    a.inCd--;

                if (a.inCd <= 0)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "roughskin")
            {
                ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "combatrepair")
            {
                if (a.stacks > 0)
                {
                    StatMod mod = a.ifConditionTrueMod();

                    StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    userHud.SetStatsHud(user);
                }

                if (a.stacks <= 0)
                    DestroyPassiveIcon(a.name, user.isEnemy);
                else 
                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "mechashield")
            {
                if (user.curShield > 0)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;

                    if (user.ult >= a.stacks)
                    {
                        StatMod statMod2 = a.statMod2.ReturnStats();
                        statMod2.inTime = statMod2.time;
                        user.statMods.Add(statMod2);
                        user.usedBonusStuff = false;

                        float shield = a.statScale.SetScale(user.SetModifiers(), user);
                        shield += shield * user.SetModifiers().shieldBonus;
                        user.curShield += shield;
                        user.shieldDone += shield;

                        user.ult -= a.stacks;
                    }

                    userHud.SetStatsHud(user);
                    ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                } else
                {
                    DMG dmg = default;
                    dmg.Reset();

                    dmg.magicDmg = a.statScale2.SetScaleFlat(user.SetModifiers(), user);
                    dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0);
                    user.TakeDamage(dmg, false, false, user);

                    userHud.SetStatsHud(user);
                    DestroyPassiveIcon(a.name, user.isEnemy);
                    user.passives.Remove(a);
                }
            }

            if (a.name == "magicwand")
            {
                ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "crossbow")
            {
                ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "funchase")
            {
                bool foundEffect = false;

                foreach (Effects b in target.effects)
                {
                    if (b.id == "BLD")
                    {
                        foundEffect = true;

                        if (a.stacks == 0)
                        {
                            user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            a.stacks = 1;
                        }
                        else if (a.stacks == 1)
                        {
                            a.stacks = 2;
                        }

                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    }
                }

                if (!foundEffect && a.stacks > 0)
                {
                    a.stacks = 0;
                    DestroyPassiveIcon(a.name, user.isEnemy);
                }
            }
        }
    }

    void ApplyTired(Unit user, GameObject pannel)
    {
        bool isTired = false;

        foreach (Effects a in user.effects)
        {
            if (a.id == "TRD")
                isTired = true;
        }

        if (!isTired)
        {
            //get effect
            Effects effect = tired.effect.ReturnEffect();
            effect.duration = Random.Range(tired.durationMin, tired.durationMax);

            //add effect to the player
            user.effects.Add(effect);

            //apply stat mod
            foreach (StatMod b in effect.statMods)
            {
                //get statmod
                StatMod statMod = b.ReturnStats();
                statMod.inTime = effect.duration;

                //add stat mod to player
                user.statMods.Add(statMod);
                user.usedBonusStuff = false;
            }

            barIconPrefab.name = effect.id;

            //display effect icon
            Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
            icon.sprite = effect.sprite;
            Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
            text.text = effect.duration.ToString();
            //display popup info on icon
            TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
            tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
            tooltipButton.text = effect.GetEffectInfo();

            Instantiate(barIconPrefab, pannel.transform);
        }
    }

    void ApplyFear(Unit user, GameObject pannel)
    {
        bool isFeared = false;

        foreach (Effects a in user.effects)
        {
            if (a.id == "FEA")
                isFeared = true;
        }

        if (!isFeared)
        {
            Effects effect = fear.effect.ReturnEffect();
            int duration = Random.Range(fear.durationMin, fear.durationMax);
            effect.duration = duration;

            foreach (Passives a in user.passives.ToArray())
            {
                if (a.name == "courage")
                {
                    effect.duration -= (int)a.num;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                }
            }

            user.effects.Add(effect);
            foreach (StatMod b in effect.statMods)
            {
                StatMod statMod = b.ReturnStats();
                statMod.inTime = effect.duration+1;

                user.statMods.Add(statMod);
                user.usedBonusStuff = false;
            }

            barIconPrefab.name = effect.id;

            Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
            icon.sprite = effect.sprite;
            Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
            text.text = effect.duration.ToString();
            TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
            tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
            tooltipButton.text = effect.GetEffectInfo();

            Instantiate(barIconPrefab, pannel.transform);
        }
    }

    public bool SummonDmg(SumMove move, StatsSummon statsSum, Unit target, Unit summoner, BattleHud targetHud)
    {
        Stats statsT = target.SetModifiers();

        Stats statsS = summoner.SetModifiers();

        bool isDead = false;
        bool isCrit = false;
        
        DMG dmgT = default;
        dmgT.Reset();
        DMG dmgS = default;
        dmgT.Reset();

        dmgT.sanityDmg += move.sanityDmg;

        if (Random.Range(0f, 1f) <= statsS.critChance)
            isCrit = true;

        switch (move.dmgType)
        {
            case DmgType.PHYSICAL:
                dmgT.phyDmg += move.getDmg(statsSum);
                //target.phyDmgTaken += dmg;
                break;
            case DmgType.MAGICAL:
                dmgT.magicDmg += move.getDmg(statsSum);
                //target.magicDmgTaken += dmg;
                break;
            case DmgType.TRUE:
                dmgT.trueDmg += move.getDmg(statsSum);
                //target.trueDmgTaken += dmg;
                break;
            case DmgType.HEAL:
                dmgS.heal += move.getDmg(statsSum);
                break;
            case DmgType.SHIELD:
                dmgS.shield += move.getDmg(statsSum);
                //summoner.shieldDone += dmg;
                break;
        }

        if (dmgT.magicDmg > 0 || dmgT.phyDmg > 0)
        {
            //summoner.magicDmgDealt += dmg.magicDmg;
            //summoner.phyDmgDealt += dmg.phyDmg;

            //magic pen = 0
            if (isCrit)
                dmgT.ApplyCrit(false, statsS.critDmg);
            dmgT = target.MitigateDmg(dmgT, dmgResisPer, magicResisPer, statsS.armourPen, 0, summoner);
        }

        isDead = target.TakeDamage(dmgT, isCrit, false, summoner);
        SetUltNumber(target, targetHud, dmgT.phyDmg + dmgT.magicDmg + dmgT.trueDmg, false);
        summoner.TakeDamage(dmgS, isCrit, false, summoner);

        return isDead;
    }

    bool SpawnSummon(Summon sum, Unit summoner, Unit target, GameObject pannel)
    {
        if (sum.summonTurn == 0)
        {
            Stats statsSum = summoner.SetModifiers();

            sum.SetupStats(statsSum, summoner);
            sum.summonTurn = turnCount;
            string name = sum.name + sum.summonTurn;

            if (!pannel.transform.Find(name + "(Clone)"))
            {
                barIconPrefab.name = name;
                Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                icon.sprite = sum.icon;
                Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
                text.text = sum.stats.hp.ToString("0");
                TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
                tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();

                sum.SetIconCombat(Instantiate(barIconPrefab, pannel.transform));
                UpdateSummonTooltip(summoner);
            }

            foreach (Passives a in summoner.passives.ToArray())
            {
                if (a.name == "combatrepair")
                {
                    if (a.stacks < a.maxStacks)
                    a.stacks++;
                    summoner.PassivePopup(langmanag.GetInfo("passive", "name", a.name, a.stacks));
                }
            }
        }
        else
        {
            string name = langmanag.GetInfo("summon", "name", sum.name);
            string debugname = sum.name + sum.summonTurn;
            if (sum.stats.hp > 0)
            {
                pannel.transform.Find(debugname + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = sum.stats.hp.ToString();
                if (sum.move.inCd <= 0)
                {
                    dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", name, langmanag.GetInfo("summon", sum.GetMoveTypeLangId()));
                    sum.move.inCd = sum.move.cd;
                    if (target.isEnemy)
                        return SummonDmg(sum.move, sum.stats, target, summoner, enemyHUD);
                    else
                        return SummonDmg(sum.move, sum.stats, target, summoner, playerHUD);
                } else
                {
                    sum.move.inCd--;
                }
            }
            else
            {
                foreach (Passives a in summoner.passives.ToArray())
                {
                    if (a.name == "combatrepair")
                    {
                        a.stacks--;
                    }
                }
                dialogText.text = langmanag.GetInfo("gui", "text", "defeat", name);
                Destroy(pannel.transform.Find(debugname + "(Clone)").gameObject);
                summoner.summons.Remove(sum);
            }
        }

        return false;
    }

    IEnumerator NewTurn()
    {
        yield return new WaitForSeconds(0.6f);
        bool userCanTired = true;
        bool enemyCanTired = true;
        
        foreach (Passives a in playerUnit.passives.ToArray())
        {
            if (a.name == "zenmode")
                userCanTired = false;
        }
        
        foreach (Passives a in enemyUnit.passives.ToArray())
        {
            if (a.name == "zenmode")
                enemyCanTired = false;
        }

        Stats statsP = playerUnit.SetModifiers();
        Stats statsE = enemyUnit.SetModifiers();

        foreach (Summon a in playerUnit.summons.ToArray())
        {
            bool isDead = SpawnSummon(a, playerUnit, enemyUnit, panelEffectsP);
            yield return new WaitForSeconds(0.5f);

            if (isDead)
                state = BattleState.WIN;
        }

        foreach (Summon a in enemyUnit.summons.ToArray())
        {
            bool isDead = SpawnSummon(a, enemyUnit, playerUnit, panelEffectsE);
            yield return new WaitForSeconds(0.5f);

            if (isDead)
                state = BattleState.LOSE;
        }

        //apply tired
        if (playerUnit.curStamina <= (int)(statsP.stamina * (tiredStart + (tiredGrowth * tiredStacks))) && userCanTired)
        {
            ApplyTired(playerUnit, panelEffectsP);
        }

        if (enemyUnit.curStamina <= (int)(statsE.stamina * (tiredStart + (tiredGrowth * tiredStacks))) && enemyCanTired)
        {
            ApplyTired(enemyUnit, panelEffectsE);
        }

        playerUnit.ResetCanUse();
        enemyUnit.ResetCanUse();

        if (playerUnit.CountEffectTimer(panelEffectsP, bloodLossStacks, dmgResisPer, magicResisPer, dotReduc))
            state = BattleState.LOSE;

        if (state == BattleState.WIN || state == BattleState.LOSE)
        {
            StartCoroutine(EndBattle());
            yield break;
        }

        if (enemyUnit.CountEffectTimer(panelEffectsE, bloodLossStacks, dmgResisPer, magicResisPer, dotReduc))
            state = BattleState.WIN;

        if (state == BattleState.WIN || state == BattleState.LOSE)
        {
            StartCoroutine(EndBattle());
            yield break;
        }

        foreach (Dotdmg a in playerUnit.dotDmg.ToArray())
        {
            a.inTime--;

            bool isDead = DotCalc(a, playerUnit);
            yield return new WaitForSeconds(0.6f);

            if (isDead)
                state = BattleState.LOSE;

            if (a.inTime <= 0)
            {
                playerUnit.dotDmg.Remove(a);

                /*GameObject temp = panelEffectsP.transform.Find(a.id + "(Clone)").gameObject;

                Destroy(temp.gameObject);*/
            }

            if (state == BattleState.WIN || state == BattleState.LOSE)
            {
                StartCoroutine(EndBattle());
                yield break;
            }
        }
        
        foreach (Dotdmg a in enemyUnit.dotDmg.ToArray())
        {
            a.inTime--;

            bool isDead = DotCalc(a, enemyUnit);
            yield return new WaitForSeconds(0.6f);

            if (isDead)
                state = BattleState.WIN;

            if (a.inTime <= 0)
            {
                enemyUnit.dotDmg.Remove(a);

                /*GameObject temp = panelEffectsP.transform.Find(a.id + "(Clone)").gameObject;

                Destroy(temp.gameObject);*/
            }

            if (state == BattleState.WIN || state == BattleState.LOSE)
            {
                StartCoroutine(EndBattle());
                yield break;
            }
        }

        //apply fear

        if (playerUnit.curSanity <= 0)
        {
            ApplyFear(playerUnit, panelEffectsP);
        }

        if (enemyUnit.curSanity <= 0)
        {
            ApplyFear(enemyUnit, panelEffectsE);
        }

        yield return new WaitForSeconds(0.45f);

        //set turn number
        turnCount++;
        turnsText.text = langmanag.GetInfo("gui", "text", "turn", turnCount);

        //change needed stamina to be tired (increases with the number of turns)
        if (turnCount > 25 && turnCount%10 == 0 && tiredStacks < 10)
        {
            tiredStacks++;
        }

        if (playerUnit.moves.Count > 0)
        {
            int i = 0;
            foreach (Moves move in playerUnit.moves.ToArray())
            {
                i++;
                if (move.uses >= 0)
                {
                    foreach (Transform movebtn in moveListHud.transform)
                    {
                        Text id = movebtn.Find("Id").gameObject.GetComponent<Text>();
                        if (id.text == i.ToString())
                        {
                            if (move.uses == 0)
                            {
                                Destroy(movebtn.gameObject);
                                playerUnit.moves.Remove(move);
                            }
                        }
                    }
                }

                if (move.inCooldown > 0 && move.name != "recovmana")
                    move.inCooldown--;

                foreach (EffectsMove a in move.effects)
                {
                    a.SetApply(false);
                }
            }
        }   

        if (enemyUnit.recoverMana.inCooldown > 0)
            enemyUnit.recoverMana.inCooldown--;

        if (enemyUnit.moves.Count > 0)
            foreach (Moves move in enemyUnit.moves.ToArray())
            {
                if (move.uses == 0)
                    enemyUnit.moves.Remove(move);

                if (move.inCooldown > 0)
                    move.inCooldown--;

                foreach (EffectsMove a in move.effects)
                {
                    a.SetApply(false);
                }
            }

        if (playerUnit.recoverMana.inCooldown > 0)
            playerUnit.recoverMana.inCooldown--;

        if (playerUnit.statMods.Count > 0)
            foreach (StatMod statMod in playerUnit.statMods.ToArray())
            {
                if (statMod.inTime > 0)
                    statMod.inTime--;

                if (statMod.inTime == 0)
                    playerUnit.statMods.Remove(statMod);
            }

        if (enemyUnit.statMods.Count > 0)
            foreach (StatMod statMod in enemyUnit.statMods.ToArray())
            {
                if (statMod.inTime > 0)
                    statMod.inTime--;

                if (statMod.inTime == 0)
                    enemyUnit.statMods.Remove(statMod);
            }

        playerHUD.SetStatsHud(playerUnit);
        enemyHUD.SetStatsHud(enemyUnit);

        CheckPassiveTurn(playerUnit, playerHUD, enemyUnit);
        CheckPassiveTurn(enemyUnit, enemyHUD, playerUnit);

        statsP = playerUnit.SetModifiers();
        statsE = enemyUnit.SetModifiers();

        playerUnit.DoAnimParticle("heal");
        playerUnit.Heal(statsP.hpRegen * (1+statsP.healBonus));
        if (turnCount > 1)
            playerUnit.healDone += statsP.hpRegen * (1+statsP.healBonus);

        enemyUnit.DoAnimParticle("heal");
        enemyUnit.Heal(statsE.hpRegen * (1+statsE.healBonus));
        if (turnCount > 1)
            enemyUnit.healDone += statsE.hpRegen * (1 + statsE.healBonus);

        if (playerUnit.curMana < statsP.mana)
            if ((playerUnit.curMana + statsP.manaRegen) > statsP.mana)
            {
                playerUnit.curMana = statsP.mana;
                if (turnCount > 1)
                    playerUnit.manaHealDone += statsP.mana - (playerUnit.curMana + statsP.manaRegen);
            }  
            else
            {
                playerUnit.curMana += statsP.manaRegen;
                if (turnCount > 1)
                    playerUnit.manaHealDone += statsP.manaRegen;
            }  

        if (enemyUnit.curMana < statsE.mana)
            if ((enemyUnit.curMana + statsE.manaRegen) > statsE.mana)
            {
                enemyUnit.curMana = statsE.mana;
                if (turnCount > 1)
                    enemyUnit.manaHealDone += statsE.mana - (enemyUnit.curMana + statsE.manaRegen);
            }
            else
            {
                enemyUnit.curMana += statsE.manaRegen;
                if (turnCount > 1)
                    enemyUnit.manaHealDone += statsE.manaRegen;
            }   

        if (playerUnit.curStamina < statsP.stamina)
            if ((playerUnit.curStamina + statsP.staminaRegen) > statsP.stamina)
            {
                playerUnit.curStamina = statsP.stamina - (playerUnit.curStamina + statsP.staminaRegen);
                if (turnCount > 1)
                    playerUnit.staminaHealDone += statsP.stamina - (playerUnit.curStamina + statsP.staminaRegen);
            }  
            else
            {
                playerUnit.curStamina += statsP.staminaRegen;
                if (turnCount > 1)
                    playerUnit.staminaHealDone += statsP.staminaRegen;
            } 

        if (enemyUnit.curStamina < statsE.stamina)
            if ((enemyUnit.curStamina + statsE.staminaRegen) > statsE.stamina)
            {
                enemyUnit.curStamina = statsE.stamina;
                if (turnCount > 1)
                    enemyUnit.staminaHealDone += statsE.stamina - (enemyUnit.curStamina + statsE.staminaRegen);
            } 
            else
            {
                enemyUnit.curStamina += statsE.staminaRegen;
                if (turnCount > 1)
                    enemyUnit.staminaHealDone += statsE.staminaRegen;
            }  

        playerUnit.isBlockingPhysical = false;
        playerUnit.isBlockingMagical = false;
        playerUnit.isBlockingRanged = false;

        enemyUnit.isBlockingPhysical = false;
        enemyUnit.isBlockingMagical = false;
        enemyUnit.isBlockingRanged = false;

        yield return new WaitForSeconds(0.4f);

        bool skip = false;

        foreach (Effects a in playerUnit.effects)
        {
            if (!a.canUseMagic && !a.canUsePhysical && !a.canUseRanged && !a.canUseEnchant && !a.canUseSupp && !a.canUseProtec && !a.canUseSummon)
                skip = true;
        }

        //apply item buffs at the start
        if (turnCount == 1)
        {
            foreach (Items item in playerUnit.items)
            {
                foreach (StatMod mod in item.statmod)
                {
                    playerUnit.statMods.Add(mod.ReturnStats());
                }
            }

            foreach (Items item in enemyUnit.items)
            {
                foreach (StatMod mod in item.statmod)
                {
                    enemyUnit.statMods.Add(mod.ReturnStats());
                }
            }
        }

        SetStatus();
        playerHUD.SetStatsHud(playerUnit);
        enemyHUD.SetStatsHud(enemyUnit);

        sumPlayerHud.UpdateValues(playerUnit, langmanag.GetInfo("charc", "name", playerUnit.charc.name));
        sumEnemyHud.UpdateValues(enemyUnit, langmanag.GetInfo("charc", "name", enemyUnit.charc.name));
        UpdateTooltips();
        if (skip)
            StartCoroutine(Combat(null));
        else
            PlayerTurn();
    } 

    void UpdateTooltips()
    {
        tooltipMain.GetComponent<TooltipPopUp>().ResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().ResetLastBtn();

        foreach (Transform child in moveListHud.transform)
        {
            int id = child.GetComponent<BtnMoveSetup>().GetId();
            int i = 0;
            foreach (Moves a in playerUnit.moves)
            {
                i++;
                if (i == id)
                {
                    child.GetComponent<BtnMoveSetup>().UpdateToolTip(a.GetTooltipText(false), a.GetTooltipText(true));
                }
            }
        }

        healManaBtn.GetComponent<TooltipButton>().text = playerUnit.recoverMana.GetTooltipText(false);
        healManaBtn.GetComponent<TooltipButton>().textSec = playerUnit.recoverMana.GetTooltipText(true);

        basicBtn.GetComponent<TooltipButton>().text = playerUnit.moves[0].GetTooltipText(false);
        basicBtn.GetComponent<TooltipButton>().textSec = playerUnit.moves[0].GetTooltipText(true);

        ultBtn.GetComponent<TooltipButton>().text = playerUnit.ultMove.GetTooltipText(false);
        ultBtn.GetComponent<TooltipButton>().textSec = playerUnit.ultMove.GetTooltipText(true);

        if (!playerUnit.canUsePhysical)
            phyCancel.SetActive(true);
        else
            phyCancel.SetActive(false);

        if (!playerUnit.canUseMagic)
            magiCancel.SetActive(true);
        else
            magiCancel.SetActive(false);

        if (!playerUnit.canUseRanged)
            rangeCancel.SetActive(true);
        else
            rangeCancel.SetActive(false);

        if (!playerUnit.canUseEnchant)
            statCancel.SetActive(true);
        else
            statCancel.SetActive(false);

        if (!playerUnit.canUseSupp)
            suppCancel.SetActive(true);
        else
            suppCancel.SetActive(false);

        if (!playerUnit.canUseProtec)
            defCancel.SetActive(true);
        else
            defCancel.SetActive(false);

        if (!playerUnit.canUseSummon)
            summCancel.SetActive(true);
        else
            summCancel.SetActive(false);

        UpdateSummonTooltip(playerUnit);
        UpdateSummonTooltip(enemyUnit);
    }

    void UpdateSummonTooltip(Unit unit)
    {
        foreach(Summon sum in unit.summons)
        {
            sum.UpdateInfoCombat();
        }
    }
}