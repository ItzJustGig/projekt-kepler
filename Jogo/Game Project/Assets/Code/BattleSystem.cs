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
    [SerializeField] private float tiredStart = 0.05f;
    [SerializeField] private float tiredGrowth = 0.015f;
    [SerializeField] private int tiredStacks = 0;
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

    [SerializeField] private Moves recoverManaMoveP;
    [SerializeField] private Moves recoverManaMoveE;
    [SerializeField] private Moves basicMove;
    [SerializeField] private EffectsMove tired;
    [SerializeField] private EffectsMove fear;

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

    public struct DMG
    {
        public float phyDmg;
        public float magicDmg;
        public float trueDmg;
        public int sanityDmg;
        public float heal;
        public float healMana;
        public float healStamina;
        public int healSanity;
        public int shield;

        public void Reset()
        {
            phyDmg = 0;
            magicDmg = 0;
            trueDmg = 0;
            sanityDmg = 0;
            heal = 0;
            healMana = 0;
            healStamina = 0;
            healSanity = 0;
            shield = 0;
        }

        public void AddDmg(DMG dmg)
        {
            phyDmg += dmg.phyDmg;
            magicDmg += dmg.magicDmg;
            trueDmg += dmg.trueDmg;
            sanityDmg += dmg.sanityDmg;
            heal += dmg.heal;
            healMana += dmg.healMana;
            healStamina += dmg.healStamina;
            healSanity += dmg.healSanity;
            shield += dmg.shield;
        }

        public void AddBaseDmg(Moves move)
        {
            phyDmg += move.phyDmg;
            magicDmg += move.magicDmg;
            trueDmg += move.trueDmg;
            sanityDmg += move.sanityDmg;
        }

        public void AddBaseDmgHeal(Moves move)
        {
            phyDmg += move.phyDmg;
            magicDmg += move.magicDmg;
            trueDmg += move.trueDmg;
            sanityDmg += move.sanityDmg;

            heal += move.heal;
            healMana += move.healMana;
            healStamina += move.healStamina;
            healSanity += move.healSanity;

            shield += move.shield;
        }

        public void Multiply(int num)
        {
            phyDmg *= num;
            magicDmg *= num;
            trueDmg *= num;
            sanityDmg *= num;

            heal *= num;
            healMana *= num;
            healStamina *= num;
            healSanity *= num;

            shield *= num;
        }
    };

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
            Leave();
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
        foreach (Items a in playerUnit.items)
        {
            SetupItems(a, playerUnit);
        }

        //setup preset enemy items (testing porpuse)
        foreach (Items a in enemyUnit.items)
        {
            SetupItems(a, enemyUnit);
        }

        GetItems(playerUnit);

        GenItem(playerUnit, enemyUnit);
        GetItems(enemyUnit);

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

        recoverManaMoveE.inCooldown = 5;
        recoverManaMoveP.inCooldown = 5;

        if (playerUnit.moves.Contains(recoverManaMoveE))
            playerUnit.moves.Remove(recoverManaMoveE);

        healManaBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        healManaBtn.GetComponent<TooltipButton>().text = recoverManaMoveP.GetTooltipText();

        ultBtn.GetComponent<TooltipButton>().tooltipPopup = tooltipMain.GetComponent<TooltipPopUp>();
        ultBtn.GetComponent<TooltipButton>().text = playerUnit.charc.ultimate.GetTooltipText();

        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            switch (enemyUnit.charc.strenght)
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
            }

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
                moveButton.GetComponent<TooltipButton>().text = move.GetTooltipText();

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
                    case Moves.MoveType.STATMOD:
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

                    Debug.Log("PICKED: " + num);
                } while (isPicked);
            }
            Debug.Log("PICKED N: " + picked.Count);
            if (picked.Count > 0)
                enemy.randomItems = picked;
        }
    }

    void SetupItems(Items a, Unit unit)
    {
        foreach (Passives p in a.passives)
        {
            unit.passives.Add(p.ReturnPassive());
        }

        foreach (Moves m in a.moves)
        {
            unit.moves.Add(m.ReturnMove());
        }
    }

    void GetItems(Unit unit)
    {
        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            if (!unit.isEnemy)
            {
                foreach (string b in info.items)
                {
                    foreach (Items a in items.returnStuff())
                    {
                        if (b == a.name)
                        {
                            unit.items.Add(a.returnItem());

                            SetupItems(a, unit);
                        }
                    }
                }
            }
        } else
        {
            if (!unit.isEnemy)
            {
                foreach (Items a in items.returnStuff())
                {
                    if (PlayerPrefs.GetString("selectedItem1") == a.name || PlayerPrefs.GetString("selectedItem2") == a.name)
                    {
                        unit.items.Add(a.returnItem());

                        SetupItems(a, unit);
                    }
                }
            } else
            {
                int i = 0;
                foreach (Items a in unit.charc.recItems)
                {
                    foreach (int b in unit.randomItems)
                    {
                        if (b == i)
                        {
                            unit.items.Add(a.returnItem());

                            SetupItems(a, unit);
                        }
                            
                    }
                    i++;
                }
            }
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
        float movPlayer = playerUnit.charc.stats.movSpeed;
        float movEnemy = enemyUnit.charc.stats.movSpeed;

        foreach (StatMod a in playerUnit.statMods)
        {
            Stats stat = SetModifiers(a.ReturnStats(), playerUnit.charc.stats.ReturnStats(), playerUnit).ReturnStats();
            stat.movSpeed -= playerUnit.charc.stats.movSpeed;
            movPlayer += stat.movSpeed;
        }

        foreach (StatMod a in enemyUnit.statMods)
        {
            Stats stat = SetModifiers(a.ReturnStats(), enemyUnit.charc.stats.ReturnStats(), enemyUnit).ReturnStats();
            stat.movSpeed -= enemyUnit.charc.stats.movSpeed;
            movEnemy += stat.movSpeed;
        }

        if (movePlayer == null || moveEnemy == null)
            canUseNormal = false;

        if (canUseNormal)
        {
            if (movePlayer.priority > moveEnemy.priority)
            {
                state = BattleState.PLAYERTURN;
                yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                SetStatsHud(playerUnit, playerHUD);
                SetStatsHud(enemyUnit, enemyHUD);

                if (state == BattleState.PLAYERTURN)
                {
                    state = BattleState.ENEMYTURN;
                    yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                    SetStatsHud(playerUnit, playerHUD);
                    SetStatsHud(enemyUnit, enemyHUD);
                }
            }
            else if (moveEnemy.priority > movePlayer.priority)
            {
                state = BattleState.ENEMYTURN;
                yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                SetStatsHud(playerUnit, playerHUD);
                SetStatsHud(enemyUnit, enemyHUD);

                if (state == BattleState.ENEMYTURN)
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    SetStatsHud(playerUnit, playerHUD);
                    SetStatsHud(enemyUnit, enemyHUD);
                }
            }
            else if (moveEnemy.priority == movePlayer.priority)
            {
                if (movPlayer > movEnemy)
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    SetStatsHud(playerUnit, playerHUD);
                    SetStatsHud(enemyUnit, enemyHUD);

                    if (state == BattleState.PLAYERTURN)
                    {
                        state = BattleState.ENEMYTURN;
                        yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                        SetStatsHud(playerUnit, playerHUD);
                        SetStatsHud(enemyUnit, enemyHUD);
                    }
                }
                else if (movPlayer < movEnemy)
                {
                    state = BattleState.ENEMYTURN;
                    yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                    SetStatsHud(playerUnit, playerHUD);
                    SetStatsHud(enemyUnit, enemyHUD);

                    if (state == BattleState.ENEMYTURN)
                    {
                        state = BattleState.PLAYERTURN;
                        yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                        SetStatsHud(playerUnit, playerHUD);
                        SetStatsHud(enemyUnit, enemyHUD);
                    }
                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
                    SetStatsHud(playerUnit, playerHUD);
                    SetStatsHud(enemyUnit, enemyHUD);

                    if (state == BattleState.PLAYERTURN)
                    {
                        state = BattleState.ENEMYTURN;
                        yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                        SetStatsHud(playerUnit, playerHUD);
                        SetStatsHud(enemyUnit, enemyHUD);
                    }
                }
            }
        }
        else
        {
            state = BattleState.PLAYERTURN;
            yield return StartCoroutine(Attack(movePlayer, playerUnit, enemyUnit));
            SetStatsHud(playerUnit, playerHUD);
            SetStatsHud(enemyUnit, enemyHUD);

            if (state == BattleState.PLAYERTURN)
            {
                state = BattleState.ENEMYTURN;
                yield return StartCoroutine(Attack(moveEnemy, enemyUnit, playerUnit));
                SetStatsHud(playerUnit, playerHUD);
                SetStatsHud(enemyUnit, enemyHUD);
            }
        }

        Debug.Log(state);
        if (state == BattleState.WIN || state == BattleState.LOSE)
            StartCoroutine(EndBattle());
        else
            StartCoroutine(NewTurn());
    }

    IEnumerator Attack(Moves move, Unit user, Unit target)
    {
        if (move == null)
        {
            dialogText.text = langmanag.GetInfo("gui", "text", "cantmove", langmanag.GetInfo("charc", "name", user.charc.name));
            yield return new WaitForSeconds(1.52f);
        }
        else
        {
            int manaCost = move.manaCost;
            int staminaCost = move.staminaCost;
            foreach (Passives a in user.passives.ToArray())
            {
                if (a.name == "zenmode" && a.stacks == 1)
                {
                    manaCost = manaCost / 2;
                    staminaCost = staminaCost / 2;
                }
            }

            user.curMana -= manaCost;

            if (user.curMana < 0)
                user.curMana = 0;

            user.curStamina -= staminaCost;

            if (user.curStamina < 0)
                user.curStamina = 0;

            if (move.name == "recovmana")
                move.inCooldown = move.cooldown;

            DMG dmg = default;

            bool isDead = false;
            bool blockPhysical = false;
            bool blockMagic = false;
            bool blockRanged = false;
            bool isCrit = false;

            Stats statsUser = user.charc.stats.ReturnStats();

            Stats statsTarget = target.charc.stats.ReturnStats();

            float evasion = 0;
            bool isStoped = false;       

            //apply stat modifiers
            if (user.statMods.Count > 0)
                foreach (StatMod statMod in user.statMods.ToArray())
                {
                    statsUser = SetModifiers(statMod, statsUser, user);
                }

            if (target.statMods.Count > 0)
                foreach (StatMod statMod in target.statMods.ToArray())
                {
                    statsTarget = SetModifiers(statMod, statsTarget, target);
                }

            //set timing to 5 if timing > 5
            if (statsTarget.timing > 5)
                statsTarget.timing = 5;

            if (statsUser.timing > 5)
                statsUser.timing = 5;

            //calculate evasion
            evasion = (float)((statsTarget.movSpeed * 0.035) + (statsTarget.timing * 0.5) + (target.curSanity * 0.01))/100;

            if ((target.isBlockingPhysical && (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.BASIC)) || (target.isBlockingMagical && move.type is Moves.MoveType.MAGICAL) || (target.isBlockingRanged && move.type is Moves.MoveType.RANGED))
            {

                dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", langmanag.GetInfo("charc", "name", user.charc.name), langmanag.GetInfo("moves", move.name));

                yield return new WaitForSeconds(1.15f);
                target.TakeDamage(0, 0, false);
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
                    dmg.Reset();

                    if (Random.Range(0f, 1f) < (statsUser.critChance + move.critChanceBonus))
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
                                float evasionBonus = a.maxNum * target.charc.stats.timing;
                                mod.evasion += a.num * evasionBonus;
                                mod.inTime = mod.time;
                                //apply the evasion
                                target.statMods.Add(mod);
                                statsTarget = SetModifiers(mod, statsTarget, target);
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
                                ManagePassiveIcon(a.sprite, a.name, (a.maxNum - a.num).ToString(), user.isEnemy, a.GetPassiveInfo());
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                                
                                isCrit = true;
                                DestroyPassiveIcon(a.name, user.isEnemy);
                            }                            
                        }

                        if (a.name == "vendetta")
                        {
                            if (target.charc.stats.hp >= (user.charc.stats.hp + (user.charc.stats.hp * a.num)) && a.stacks != 1)
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));

                                dmg.AddDmg(SetScaleDmg(scale2, stats, unit));

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
                                dmg.trueDmg += a.num;
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                            }

                            if (move.type is Moves.MoveType.PHYSICAL && a.stacks < a.maxStacks)
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
                                    dmg.trueDmg += a.num;

                                    a.inCd = a.cd;

                                    //apply statmod
                                    StatMod statMod = a.statMod.ReturnStats();
                                    statMod.inTime = statMod.time;
                                    user.statMods.Add(statMod);
                                    user.usedBonusStuff = false;

                                    if (user.isEnemy)
                                        SetStatsHud(user, enemyHUD);
                                    else
                                        SetStatsHud(user, playerHUD);
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                            }
                        }

                        if (a.name == "magicremains")
                        {
                            if ((move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.STATMOD) && a.inCd == 0)
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));

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

                            int manaPer = (int)((100 * user.curMana) / user.charc.stats.mana);
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
                                magicBonus += scale.flatValue;
                                magicBonus += SetScale(scale, stats, unit);

                                int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

                                if (hpPer <= a.num * 100)
                                {
                                    StatScale scale2 = a.ifConditionTrueScale2();
                                    
                                    magicBonus += magicBonus * a.num;
                                    dmg.trueDmg += scale2.flatValue;
                                    dmg.trueDmg += SetScale(scale2, stats, unit);
                                }

                                dmg.magicDmg += magicBonus;
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

                                dmg.sanityDmg += scale.flatValue;
                                dmg.sanityDmg += (int)SetScale(scale, stats, unit);
                            }
                        }

                        if (a.name == "successoroffire")
                        {
                            bool isBurn = false;
                            foreach (Effects b in target.effects)
                            {
                                if (b.id == "BRN")
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));

                                dmg.AddDmg(SetScaleDmg(scale2, stats, unit));

                                Debug.Log("STAMINA: " + dmg.healStamina + "\nMana:" + dmg.healMana);
                            }
                        }

                        if (a.name == "spectralring")
                        {
                            //hp in %
                            int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));

                                StatMod mod = a.statMod.ReturnStats();
                                mod.inTime = mod.time+1;
                                user.statMods.Add(mod);
                                statsUser = SetModifiers(mod, statsUser, user);

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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                            }
                        }

                        if (a.name == "huntersdirk")
                        {
                            if (a.inCd == 0)
                            {
                                float hpPer = (100 * target.curHp) / target.charc.stats.hp;
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

                                    dmg.AddDmg(SetScaleDmg(scale, stats, unit));
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

                                    dmg.AddDmg(SetScaleDmg(scale, stats, unit));
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
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

                                    dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                                }
                            }
                        }
                    }

                    dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", langmanag.GetInfo("charc", "name", user.charc.name), langmanag.GetInfo("moves", move.name));

                    dmg.heal += move.heal;
                    dmg.healMana += move.healMana;
                    dmg.healStamina += move.healStamina;
                    dmg.healSanity += move.healSanity;
                    dmg.shield += move.shield;

                    foreach (Effects a in user.effects)
                    {
                        if (a.cancelAtkChance > 0 && !isStoped)
                        {
                            stopAttackChance = a.cancelAtkChance;

                            if (Random.Range(0f, 1f) <= stopAttackChance)
                            {
                                isStoped = true;
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
                                    isDead = user.TakeDamage(recoil, shieldedDmg, false);

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

                    if ((Random.Range(0f, 1f) > statsUser.accuracy) || (state is BattleState.LOSE || state is BattleState.WIN) && !isStoped)
                        isStoped = true;

                    if (!isStoped || move.type == Moves.MoveType.DEFFENCIVE || move.type == Moves.MoveType.SUPPORT || move.type == Moves.MoveType.STATMOD || move.type == Moves.MoveType.ULT || move.type == Moves.MoveType.SUMMON)
                    {
                        if (Random.Range(0f, 1f) < (evasion + statsTarget.evasion) && (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.RANGED))
                        {
                            target.Miss(true);
                            yield return new WaitForSeconds(1.15f);
                            dialogText.text = langmanag.GetInfo("gui", "text", "dodge", langmanag.GetInfo("charc", "name", target.charc.name));
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
                        else
                        {
                            dmg.AddBaseDmg(move);

                            blockPhysical = move.blocksPhysical;
                            blockMagic = move.blocksMagical;
                            blockRanged = move.blocksRanged;

                            bool skipEffect = false;

                            if (move.effects.Count > 0)
                            {
                                foreach (EffectsMove a in move.effects)
                                {                                    
                                    Effects effect = a.effect.ReturnEffect();

                                    if (Random.Range(0f, 1f) < a.chance && !a.WasApplied())
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
                                            Debug.Log("I EFFECT");
                                            effect.duration = Random.Range(a.durationMin, a.durationMax) + 1;

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
                                                        dmg.heal += (int)((user.charc.stats.hp * statMod.hp) / 2);
                                                        dmg.healMana += (int)((user.charc.stats.mana * statMod.mana) / 2);
                                                        dmg.healStamina += (int)((user.charc.stats.stamina * statMod.stamina) / 2);
                                                        dmg.healSanity += (int)((user.charc.stats.sanity * statMod.sanity) / 2);
                                                        user.statMods.Add(statMod);
                                                        user.usedBonusStuff = false;
                                                        SetModifiers(statMod, statsUser, user);
                                                    }
                                                } else
                                                {
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
                                                        dmg.heal += (int)((target.charc.stats.hp * statMod.hp) / 2);
                                                        dmg.healMana += (int)((target.charc.stats.mana * statMod.mana) / 2);
                                                        dmg.healStamina += (int)((target.charc.stats.stamina * statMod.stamina) / 2);
                                                        dmg.healSanity += (int)((target.charc.stats.sanity * statMod.sanity) / 2);
                                                        target.statMods.Add(statMod);
                                                        target.usedBonusStuff = false;
                                                        SetModifiers(statMod, statsTarget, target);
                                                    }
                                                } else
                                                {
                                                    check.duration += effect.duration;
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
                                        dmg.heal += ((user.charc.stats.hp * statMod.hp) / 2);
                                        dmg.healMana += ((user.charc.stats.mana * statMod.mana) / 2);
                                        dmg.healStamina += ((user.charc.stats.stamina * statMod.stamina) / 2);
                                        dmg.healSanity += (int)((user.charc.stats.sanity * statMod.sanity) / 2);
                                        user.statMods.Add(statMod);
                                        user.usedBonusStuff = false;
                                        SetModifiers(statMod, statsUser, user);
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
                                        dmg.heal += ((target.charc.stats.hp * statMod.hp) / 2);
                                        dmg.healMana += ((target.charc.stats.mana * statMod.mana) / 2);
                                        dmg.healStamina += ((target.charc.stats.stamina * statMod.stamina) / 2);
                                        dmg.healSanity += (int)((target.charc.stats.sanity * statMod.sanity) / 2);
                                        target.statMods.Add(statMod);
                                        target.usedBonusStuff = false;
                                        SetModifiers(statMod, statsTarget, target);
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

                                dmg.AddDmg(SetScaleDmg(scale, stats, unit));
                            }
    
                            float dmgMitigated = 0;

                            if (dmg.phyDmg > 0)
                            {
                                if (isCrit == true)
                                {
                                    dmg.phyDmg += (dmg.phyDmg * (statsUser.critDmg + move.critDmgBonus));
                                }
                            }
                            else
                                isCrit = false;

                            foreach (Passives a in user.passives.ToArray())
                            {
                                if (a.name == "huntingseason")
                                {
                                    dmg.phyDmg += dmg.phyDmg * a.num;
                                    dmg.magicDmg += dmg.magicDmg * a.num;
                                }
                            }

                            foreach (Dotdmg dot in move.dot)
                            {
                                Dotdmg dotdmg = dot.ReturnDOT();
                                switch (dotdmg.type)
                                {
                                    case Dotdmg.DmgType.PHYSICAL:
                                        dotdmg.Setup(dmg.phyDmg, isCrit, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.phyDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.MAGICAL:
                                        dotdmg.Setup(dmg.magicDmg, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.magicDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.TRUE:
                                        dotdmg.Setup(dmg.trueDmg, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.trueDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.SANITY:
                                        dotdmg.Setup(dmg.sanityDmg, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.sanityDmg = 0;
                                        target.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEAL:
                                        dotdmg.Setup(dmg.heal, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.heal = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALMANA:
                                        dotdmg.Setup(dmg.healMana, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.healMana = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALSTAMINA:
                                        dotdmg.Setup(dmg.healStamina, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.healStamina = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.HEALSANITY:
                                        dotdmg.Setup(dmg.healSanity, move.name, Dotdmg.SrcType.MOVE);
                                        dmg.healSanity = 0;
                                        user.dotDmg.Add(dotdmg);
                                        break;
                                    case Dotdmg.DmgType.SHIELD:
                                        dotdmg.Setup(dmg.shield, move.name, Dotdmg.SrcType.MOVE);
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


                            if (dmg.phyDmg > 0)
                            {
                                user.phyDmgDealt += dmg.phyDmg;
                                dmgMitigated = (float)((statsTarget.dmgResis - (statsTarget.dmgResis * statsUser.armourPen)) * 0.18);
                                dmg.phyDmg -= dmgMitigated;
                                target.phyDmgMitigated += dmgMitigated;
                            }

                            if (statsUser.lifesteal > 0)
                                dmg.heal += dmg.phyDmg * statsUser.lifesteal;

                            if (dmg.magicDmg > 0)
                            {
                                user.magicDmgDealt += dmg.magicDmg;
                                dmgMitigated = (float)(statsTarget.magicResis * 0.12);
                                dmg.magicDmg -= dmgMitigated;
                                target.magicDmgMitigated += dmgMitigated;
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
                                        dmgMitigated = dmg.phyDmg * ((statsTarget.dmgResis * a.num) / 100);
                                        //subtract mitigated dmg from the dmg
                                        dmg.phyDmg -= dmgMitigated;
                                        //add mitigated dmg to the overview
                                        target.phyDmgMitigated += dmgMitigated;
                                    }
                                }

                                if (a.name == "dreadofthesupernatural")
                                {
                                    //if sanityDmg bellow 0
                                    if (dmg.sanityDmg > 0)
                                    {
                                        //show passive popup
                                        target.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                        //get bonus sanityDmg
                                        int bonusSanityDmg = (int)(dmg.sanityDmg * a.num);
                                        //if bonus under a.maxNum, set to it
                                        if (bonusSanityDmg < a.maxNum)
                                            bonusSanityDmg = (int)a.maxNum;
                                        //add bonus damage
                                        dmg.sanityDmg += bonusSanityDmg;
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
                                        SetStatsHud(user, userHud);
                                        DestroyPassiveIcon(a.name, user.isEnemy);
                                    }

                                    ManagePassiveIcon(a.sprite, a.name, (a.stacks + "S" + (a.maxNum - a.num) + "T").ToString(), user.isEnemy, a.GetPassiveInfo());
                                }
                            }

                            target.phyDmgTaken += dmg.phyDmg;
                            target.magicDmgTaken += dmg.magicDmg;
                            target.trueDmgTaken += dmg.trueDmg;

                            user.trueDmgDealt += dmg.trueDmg;

                            float dmgT = dmg.phyDmg + dmg.magicDmg + dmg.trueDmg;
                            float shieldedDmg = 0;

                            if (move.healFromDmgType != Moves.HealFromDmg.NONE)
                            {
                                if (move.healFromDmgType is Moves.HealFromDmg.PHYSICAL)
                                    dmg.heal += dmg.phyDmg * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.MAGICAL)
                                    dmg.heal += dmg.magicDmg * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.TRUE)
                                    dmg.heal += dmg.trueDmg * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.PHYSICAL_MAGICAL)
                                    dmg.heal += (dmg.phyDmg + dmg.magicDmg) * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.PHYSICAL_TRUE)
                                    dmg.heal += (dmg.phyDmg + dmg.trueDmg) * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.MAGICAL_TRUE)
                                    dmg.heal += (dmg.magicDmg + dmg.trueDmg) * move.healFromDmg;
                                else if (move.healFromDmgType is Moves.HealFromDmg.ALL)
                                    dmg.heal += (dmg.phyDmg + dmg.magicDmg + dmg.trueDmg) * move.healFromDmg;
                            }

                            if (target.curShield > 0)
                            {
                                float tempDmg = dmgT;
                                float tempShield = target.curShield;

                                dmgT -= target.curShield;
                                target.curShield -= tempDmg;

                                if (target.curShield < 0)
                                    target.curShield = 0;

                                shieldedDmg = tempShield - target.curShield;
                            }

                            if (dmgT > 0 || shieldedDmg > 0)
                            {
                                isDead = target.TakeDamage(dmgT, shieldedDmg, isCrit);
                                if (!(move.type is Moves.MoveType.ULT))
                                    SetUltNumber(user, userHud, (dmgT + shieldedDmg), true);

                                if (move.type is Moves.MoveType.ULT)
                                    SetUltNumber(target, enemyHud, ((dmgT + shieldedDmg) / 2), false);
                                else
                                    SetUltNumber(target, enemyHud, (dmgT + shieldedDmg), false);
                            }

                            if (dmg.sanityDmg > 0)
                            {
                                if ((target.curSanity - dmg.sanityDmg) >= 0)
                                {
                                    target.curSanity -= dmg.sanityDmg;
                                }
                                else
                                {
                                    dmg.sanityDmg = target.curSanity;
                                    target.curSanity = 0;
                                }

                                user.sanityDmgDealt += dmg.sanityDmg;
                                target.sanityDmgTaken += dmg.sanityDmg;
                            }

                            if (dmg.heal > 0)
                            {
                                user.healDone += dmg.heal;
                                user.Heal(dmg.heal);
                            }

                            Stats statsU = user.charc.stats.ReturnStats();

                            if (user.statMods.Count > 0)
                                foreach (StatMod statMod in user.statMods.ToArray())
                                {
                                    statsU = SetModifiers(statMod.ReturnStats(), statsU.ReturnStats(), user);
                                }

                            if (dmg.healMana > 0)
                            {
                                user.curMana += dmg.healMana;
                                if (user.curMana > statsU.mana)
                                {
                                    user.curMana = statsU.mana;
                                    dmg.healMana = user.curMana - statsU.mana;
                                }
                                user.manaHealDone += dmg.healMana;
                            }

                            if (dmg.healStamina > 0)
                            {
                                user.curStamina += dmg.healStamina;
                                if (user.curStamina > statsU.stamina)
                                {
                                    user.curStamina = statsU.stamina;
                                    dmg.healStamina = user.curStamina - statsU.stamina;
                                }
                                user.staminaHealDone += dmg.healStamina;
                            }

                            if (dmg.healSanity > 0)
                            {
                                user.curSanity += dmg.healSanity;
                                if (user.curSanity > statsU.sanity)
                                {
                                    user.curSanity = statsU.sanity;
                                    dmg.healSanity = user.curSanity - statsU.sanity;
                                }
                                user.sanityHealDone += dmg.healSanity;
                            }

                            if (dmg.shield > 0)
                            {
                                user.shieldDone += dmg.shield;
                                user.curShield += dmg.shield;
                                if (user.curShield > 1000)
                                    user.curShield = 1000;
                            }
                            

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
                                    if (!(move.type is Moves.MoveType.DEFFENCIVE || move.type is Moves.MoveType.STATMOD || move.type is Moves.MoveType.SUPPORT)) { 
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

                            yield return new WaitForSeconds(1.8f);

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

            if (value > 100)
                value = 100;

            user.ult = value;
        }
    }

    DMG SetScaleDmg(StatScale scale, Stats stats, Unit unit)
    {
        DMG dmg;

        dmg.phyDmg = 0;
        dmg.magicDmg = 0;
        dmg.trueDmg = 0;
        dmg.sanityDmg = 0;
        dmg.heal = 0;
        dmg.healMana = 0;
        dmg.healStamina = 0;
        dmg.healSanity = 0;
        dmg.shield = 0;

        switch (scale.type)
        {
            case StatScale.DmgType.PHYSICAL:
                dmg.phyDmg += scale.flatValue;
                dmg.phyDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.MAGICAL:
                dmg.magicDmg += scale.flatValue;
                dmg.magicDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.TRUE:
                dmg.trueDmg += scale.flatValue;
                dmg.trueDmg += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.SANITY:
                dmg.sanityDmg += scale.flatValue;
                dmg.sanityDmg += (int)SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEAL:
                dmg.heal += scale.flatValue;
                dmg.heal += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALMANA:
                dmg.healMana += scale.flatValue;
                dmg.healMana += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALSTAMINA:
                dmg.healStamina += scale.flatValue;
                dmg.healStamina += SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.HEALSANITY:
                dmg.healSanity += scale.flatValue;
                dmg.healSanity += (int)SetScale(scale, stats, unit);
                break;
            case StatScale.DmgType.SHIELD:
                dmg.shield += scale.flatValue;
                dmg.shield += (int)SetScale(scale, stats, unit);
                break;
        }

        return dmg;
    }

    float SetScale(StatScale scale, Stats stats, Unit user)
    {
        float temp = 0;

        temp += (user.curHp * scale.curHp);
        temp += ((stats.hp - user.curHp) * scale.missHp);
        temp += (stats.hp * scale.maxHp);
        temp += (stats.hpRegen * scale.hpRegen);

        temp += (user.curMana * scale.curMana);
        temp += ((stats.mana - user.curMana) * scale.missMana);
        temp += (stats.mana * scale.maxMana);
        temp += (stats.manaRegen * scale.manaRegen);

        temp += (user.curStamina * scale.curStamina);
        temp += ((stats.stamina - user.curStamina) * scale.missStamina);
        temp += (stats.stamina * scale.maxStamina);
        temp += (stats.staminaRegen * scale.staminaRegen);

        temp += (user.curSanity * scale.curSanity);
        temp += (stats.sanity - user.curSanity) * scale.missSanity;
        temp += (stats.sanity * scale.maxSanity);

        temp += (stats.atkDmg * scale.atkDmg);
        temp += (stats.magicPower * scale.magicPower);

        temp += (stats.dmgResis * scale.dmgResis);
        temp += (stats.magicResis * scale.magicResis);

        temp += (stats.timing * scale.timing);
        temp += (stats.movSpeed * scale.movSpeed);

        return temp;
    }

    public Stats SetModifiers(StatMod scale, Stats user, Unit original)
    {
        Stats temp = user.ReturnStats();

        if (!scale.flat)
        {
            temp.hp += (int)((user.hp + temp.hp) * scale.hp);
            temp.mana += (int)((user.mana + temp.mana) * scale.mana);
            temp.stamina += (int)((user.stamina + temp.stamina) * scale.stamina);
            temp.sanity += (int)((user.sanity + temp.sanity) * scale.sanity);

            temp.hpRegen += (user.hpRegen * scale.hpRegen);
            temp.manaRegen += user.manaRegen * scale.manaRegen;
            temp.staminaRegen += user.staminaRegen * scale.staminaRegen;

            temp.atkDmg += (user.atkDmg * scale.atkDmg);
            temp.magicPower += (user.magicPower * scale.magicPower);

            temp.dmgResis += (user.dmgResis * scale.dmgResis);
            temp.magicResis += (user.magicResis * scale.magicResis);

            temp.timing += (user.timing * scale.timing);
            temp.movSpeed += (user.movSpeed * scale.movSpeed);

        } else
        {
            temp.hp += scale.hp;
            temp.mana += scale.mana;
            temp.stamina += scale.stamina;
            temp.sanity += (int)scale.sanity;

            temp.hpRegen += scale.hpRegen;
            temp.manaRegen += scale.manaRegen;
            temp.staminaRegen += scale.staminaRegen;

            temp.atkDmg += scale.atkDmg;
            temp.magicPower += scale.magicPower;

            temp.dmgResis += scale.dmgResis;
            temp.magicResis += scale.magicResis;

            temp.timing += scale.timing;
            temp.movSpeed += scale.movSpeed;
        }

        temp.critChance += scale.critChance;
        temp.critDmg += scale.critDmg;
        temp.lifesteal += scale.lifesteal;
        temp.evasion += scale.evasion;
        temp.accuracy += scale.accuracy;
        temp.armourPen += scale.armourPen;

        return temp;
    }

    void SetStatsHud(Unit user, BattleHud userHud)
    {
        Stats stats = user.charc.stats.ReturnStats();
        foreach (StatMod statMod in user.statMods.ToArray())
        {
            stats = SetModifiers(statMod.ReturnStats(), stats.ReturnStats(), user);
        }

        userHud.SetStats(stats.ReturnStats(), user.charc.stats.ReturnStats(), user.curSanity);
    }

    void SetStatus()
    {
        Stats statsP = playerUnit.charc.stats.ReturnStats();

        foreach (StatMod statMod in playerUnit.statMods.ToArray())
        {
            statsP = SetModifiers(statMod.ReturnStats(), statsP.ReturnStats(), playerUnit);
        }

        Stats statsE = enemyUnit.charc.stats.ReturnStats();

        foreach (StatMod statMod in enemyUnit.statMods.ToArray())
        {
            statsE = SetModifiers(statMod.ReturnStats(), statsE.ReturnStats(), enemyUnit);
        }

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
        playerHUD.SetUlt(playerUnit.ult);
        enemyHUD.SetUlt(enemyUnit.ult);
    }

    Moves EnemyChooseMove()
    {
        Moves move = null;
        bool skip = false;

        if (enemyUnit.curMana < (enemyUnit.charc.stats.mana * 0.12) && !enemyUnit.moves.Contains(recoverManaMoveE))
            enemyUnit.moves.Add(recoverManaMoveE);
        else if (enemyUnit.curMana > (enemyUnit.charc.stats.mana * 0.12) && enemyUnit.moves.Contains(recoverManaMoveE))
            enemyUnit.moves.Remove(recoverManaMoveE);

        if (enemyUnit.ult == 100 && !enemyUnit.moves.Contains(enemyUnit.ultMove))
            enemyUnit.moves.Add(enemyUnit.ultMove);
        else if (enemyUnit.ult < 100 && enemyUnit.moves.Contains(enemyUnit.ultMove))
            enemyUnit.moves.Remove(enemyUnit.ultMove);

        int random = 0;
        int i = 0;

        foreach (Effects a in enemyUnit.effects)
        {
            if (!a.canUseMagic && !a.canUsePhysical && !a.canUseRanged && !a.canUseStatMod && !a.canUseSupp && !a.canUseProtec && !a.canUseSummon)
                skip = true;
        }

        do
        {
            if (enemyUnit.curMana <= (enemyUnit.charc.stats.mana * 0.08))
            {
                if (Random.Range(0f, 1f) <= 0.95)
                {
                    random = enemyUnit.moves.Count - 1;
                }
                else
                    random = Random.Range(0, enemyUnit.moves.Count - 2);
            }
            else
            {
                Stats statsU = enemyUnit.charc.stats.ReturnStats();
                if (enemyUnit.statMods.Count > 0)
                    foreach (StatMod statMod in enemyUnit.statMods.ToArray())
                    {
                        statsU = SetModifiers(statMod.ReturnStats(), statsU.ReturnStats(), enemyUnit);
                    }

                Stats statsT = playerUnit.charc.stats.ReturnStats();
                if (enemyUnit.statMods.Count > 0)
                    foreach (StatMod statMod in playerUnit.statMods.ToArray())
                    {
                        statsT = SetModifiers(statMod.ReturnStats(), statsT.ReturnStats(), playerUnit);
                    }

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
                                    case Moves.MoveType.STATMOD:
                                        if (!enemyUnit.canUseStatMod)
                                            canUse = false;
                                        break;
                                    case Moves.MoveType.SUMMON:
                                        if (!enemyUnit.canUseSummon)
                                            canUse = false;
                                        break;
                                    case Moves.MoveType.ULT:
                                        enemyUnit.ult = 0;
                                        break;
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
        if (playerUnit.ult == 100)
            ultBtn.interactable = true;

        if (recoverManaMoveP.inCooldown <= 0)
        {
            healManaBtn.interactable = true;
            healBtnText.text = langmanag.GetInfo("moves", "recovmana");
            if (!playerUnit.canUseSupp)
                healManaBtn.interactable = false;
        }
        else
        {
            healManaBtn.interactable = false;
            healBtnText.text = langmanag.GetInfo("moves", "recovmana") + " (" + recoverManaMoveP.inCooldown + ")";
        }  
    }

    public void OnMoveBtn(int moveId)
    {
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
                    case Moves.MoveType.STATMOD:
                        if (!playerUnit.canUseStatMod)
                            canUse = false;
                        break;
                    case Moves.MoveType.SUMMON:
                        if (!playerUnit.canUseSummon)
                            canUse = false;
                        break;
                }

                if (playerUnit.curMana < move.manaCost || playerUnit.curStamina < move.staminaCost || inCd > 0)
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
        StartCoroutine(Combat(recoverManaMoveP));
    }

    public void OnUltBtn()
    {
        StartCoroutine(Combat(playerUnit.charc.ultimate));
        playerUnit.ult = 0;
    }

    public void OnBasicBtn()
    {
        StartCoroutine(Combat(basicMove));
    }

    public void CancelBtn()
    {
        panelMoves.SetActive(false);
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

        foreach (StatScale scale in a.scale.ToArray())
        {
            Unit unit;
            Stats stats;

            unit = user;
            stats = user.charc.stats.ReturnStats();

            dmg.AddDmg(SetScaleDmg(scale, stats, unit));
        }

        if (dmg.phyDmg > 0)
        {
            float dmgMitigated = (float)(user.charc.stats.dmgResis * 0.18);
            dmg.phyDmg -= dmgMitigated;
            user.phyDmgMitigated += dmgMitigated;
            user.phyDmgTaken += dmg.phyDmg - dmgMitigated;
        }

        if (dmg.magicDmg > 0)
        {
            float dmgMitigated = (float)(user.charc.stats.magicResis * 0.12);
            dmg.magicDmg -= dmgMitigated;
            user.magicDmgMitigated += dmgMitigated;
            user.magicDmgTaken += dmg.magicDmg - dmgMitigated;
        }

        user.trueDmgTaken += dmg.trueDmg;

        float dmgT = dmg.phyDmg + dmg.magicDmg + dmg.trueDmg;
        float shieldedDmg = 0;

        if (user.curShield > 0)
        {
            float tempDmg = dmgT;
            float tempShield = user.curShield;

            dmgT -= user.curShield;
            user.curShield -= tempDmg;

            if (user.curShield < 0)
                user.curShield = 0;

            shieldedDmg = tempShield - user.curShield;
        }

        if (dmgT > 0 || shieldedDmg > 0)
            isDead = user.TakeDamage(dmgT, shieldedDmg, false);

        if (dmg.sanityDmg > 0)
        {
            if ((user.curSanity - dmg.sanityDmg) >= 0)
                user.curSanity -= dmg.sanityDmg;
            else
            {
                dmg.sanityDmg = user.curSanity;
                user.curSanity = 0;
            }
        } 

        user.sanityDmgTaken += dmg.sanityDmg;

        user.healDone += dmg.heal;
        user.manaHealDone += dmg.healMana;
        user.staminaHealDone += dmg.healStamina;
        user.sanityHealDone += dmg.healSanity;
        user.shieldDone += dmg.shield;

        if (dmg.heal > 0)
            user.Heal(dmg.heal);

        if (dmg.healMana > 0)
        {
            user.curMana += dmg.healMana;
            if (user.curMana > user.charc.stats.mana)
                user.curMana = user.charc.stats.mana;
        }

        if (dmg.healStamina > 0)
        {
            user.curStamina += dmg.healStamina;
            if (user.curStamina > user.charc.stats.stamina)
                user.curStamina = user.charc.stats.stamina;
        }

        if (dmg.healSanity > 0)
        {
            user.curSanity += dmg.healSanity;
            if (user.curSanity > user.charc.stats.sanity)
                user.curSanity = user.charc.stats.sanity;
        }

        if (dmg.shield > 0)
        {
            user.curShield += dmg.shield;
            if (user.curShield > 1000)
                user.curShield = 1000;
        }

        return isDead;
    }

    public bool DotCalc(Dotdmg dot, Unit user)
    {
        Stats stats = user.charc.stats.ReturnStats();
        if (user.statMods.Count > 0)
            foreach (StatMod statMod in user.statMods.ToArray())
            {
                stats = SetModifiers(statMod.ReturnStats(), stats.ReturnStats(), user);
            }

        Dotdmg a = dot.ReturnDOT();

        switch (a.type)
        {
            case Dotdmg.DmgType.PHYSICAL:
                if (a.dmg > 0)
                {
                    float dmgMitigated = (float)(stats.dmgResis * 0.12);
                    a.dmg -= dmgMitigated;
                    user.phyDmgMitigated += dmgMitigated;
                    user.phyDmgTaken += a.dmg - dmgMitigated;

                    Stats statsUser = user.charc.stats.ReturnStats();
                    if (statsUser.lifesteal > 0)
                    {
                        float heal = a.dmg * statsUser.lifesteal;
                        user.Heal(heal);
                        user.healDone += heal;
                    } 
                }
                break;
            case Dotdmg.DmgType.MAGICAL:
                if (a.dmg > 0)
                {
                    float dmgMitigated = (float)(stats.magicResis * 0.06);
                    a.dmg -= dmgMitigated;
                    user.magicDmgMitigated += dmgMitigated;
                    user.magicDmgTaken += a.dmg - dmgMitigated;
                }
                break;
            case Dotdmg.DmgType.TRUE:
                user.trueDmgTaken += a.dmg;
                break;
            case Dotdmg.DmgType.SANITY:
                if (a.dmg > 0)
                {
                    if ((user.curSanity - a.dmg) >= 0)
                        user.curSanity -= (int)a.dmg;
                    else
                    {
                        a.dmg = user.curSanity;
                        user.curSanity = 0;
                    }
                }
                user.sanityDmgTaken += a.dmg;
                break;
            case Dotdmg.DmgType.HEAL:
                if (a.dmg > 0)
                    user.Heal(a.dmg);
                user.healDone += a.dmg;
                break;
            case Dotdmg.DmgType.HEALMANA:
                if (a.dmg > 0)
                {
                    user.curMana += a.dmg;
                    if (user.curMana > stats.mana)
                        user.curMana = stats.mana;
                }
                user.manaHealDone += a.dmg;
                break;
            case Dotdmg.DmgType.HEALSTAMINA:
                if (a.dmg > 0)
                {
                    user.curStamina += a.dmg;
                    if (user.curStamina > stats.stamina)
                        user.curStamina = stats.stamina;
                }
                user.staminaHealDone += a.dmg;
                break;
            case Dotdmg.DmgType.HEALSANITY:
                if (a.dmg > 0)
                {
                    user.curSanity += (int)a.dmg;
                    if (user.curSanity > stats.sanity)
                        user.curSanity = stats.sanity;
                }
                user.sanityHealDone += a.dmg;
                break;
            case Dotdmg.DmgType.SHIELD:
                if (a.dmg > 0)
                {
                    user.curShield += a.dmg;
                    if (user.curShield > 1000)
                        user.curShield = 1000;
                }
                user.shieldDone += a.dmg;
                break;
        }

        foreach (Passives p in user.passives.ToArray())
        {
            if (p.name == "roughskin")
            {
                //if move is physical
                if (dot.type is Dotdmg.DmgType.PHYSICAL)
                {
                    //show passive popup
                    user.PassivePopup(langmanag.GetInfo("passive", "name", p.name));
                    //get the mitigated dmg
                    float dmgMitigated = a.dmg * ((stats.dmgResis * p.num) / 100);
                    //subtract mitigated dmg from the dmg
                    a.dmg -= dmgMitigated;
                    //add mitigated dmg to the overview
                    user.phyDmgMitigated += dmgMitigated;
                }
            }

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

        float shieldedDmg = 0;
        bool isDead = false;

        if (a.type is Dotdmg.DmgType.PHYSICAL|| a.type is Dotdmg.DmgType.MAGICAL || a.type is Dotdmg.DmgType.TRUE)
        {
            if (user.curShield > 0)
            {
                float tempDmg = a.dmg;
                float tempShield = user.curShield;

                a.dmg -= user.curShield;
                user.curShield -= tempDmg;

                if (user.curShield < 0)
                    user.curShield = 0;

                shieldedDmg = tempShield - user.curShield;
            }

            if (a.dmg > 0 || shieldedDmg > 0)
                isDead = user.TakeDamage(a.dmg, shieldedDmg, a.isCrit);
        }

        return isDead;
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

    void ManagePassiveIcon(Sprite pIcon, string name, string num, bool isEnemy, string passiveDesc)
    {
        if (num == "0")
            num = "";

        if (!isEnemy)
            if (panelEffectsP.transform.Find(name + "(Clone)"))
                panelEffectsP.transform.Find(name + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = num;
            else
                CreatePassiveIcon(pIcon, name, num, isEnemy, passiveDesc);
        else
            if (panelEffectsE.transform.Find(name + "(Clone)"))
                panelEffectsE.transform.Find(name + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = num;
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
                    SetStatsHud(user, userHud);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    a.stacks++;
                }
            }

            if (a.name == "musicup")
            {
                if (a.inCd == 1 && a.stacks < a.maxStacks)
                {
                    a.inCd = a.cd;
                    a.stacks++;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                    StatMod mod = a.ifConditionTrueMod();

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
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

            if (a.name == "manathirst")
            {
                if (a.inCd == 0 && (user.curMana <= (user.charc.stats.mana * a.num)))
                {
                    a.inCd = a.cd;
                    user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));

                    float healMana = SetScale(a.statScale, user.charc.stats, user);
                    healMana = a.statScale.flatValue;
                    user.curMana += healMana;
                    user.manaHealDone += healMana;

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
                }
                else if (a.inCd > 0)
                    a.inCd--;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "wildinstinct")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

                if (hpPer < (a.num * 100))
                {
                    a.stacks = (int)((a.num * 100) - hpPer);

                    StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
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

                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
            }

            if (a.name == "lastbreath")
            {
                //hp in %
                int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

                if (hpPer < (a.num * 100))
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
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
                int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

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
                    SetStatsHud(user, userHud);
                }
                
                if (a.inCd > 0 || a.stacks > a.maxStacks)
                {
                    if (a.stacks > a.maxStacks)
                        ManagePassiveIcon(a.sprite, a.name, "!", user.isEnemy, a.GetPassiveInfo());
                    else
                    {
                        a.inCd--;
                        ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
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

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "galeglide")
            {
                if (a.inCd > 0)
                    a.inCd--;

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "fearsmell")
            {
                //enemy sanity in %
                int sanityPer = (int)((100 * target.curSanity) / target.charc.stats.sanity);

                if (sanityPer < (a.num * 100))
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = 1;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
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
                            SetStatsHud(user, userHud);
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
                        SetStatsHud(user, userHud);
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

                int manaPer = (int)((100 * user.curMana) / user.charc.stats.mana);
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

                int healthPer = (int)((100 * user.curHp) / user.charc.stats.hp);

                if (healthPer <= a.stacks)
                {
                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);

                    ManagePassiveIcon(a.sprite, a.name, 1.ToString(), user.isEnemy, a.GetPassiveInfo());
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
                        SetStatsHud(user, userHud);

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
                    SetStatsHud(user, userHud);

                    //display icon
                    ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
            }

            if (a.name == "courage")
            {
                //if cooldown higher than 0
                if (a.inCd > 0)
                //reduce cooldown by 1
                    a.inCd--;

                //if cooldown is 0
                if (a.inCd <= 0 && user.curSanity < user.charc.stats.sanity)
                {
                    //get sanity heal from the passive's scale
                    float healSanity = SetScale(a.statScale, user.charc.stats, user);
                    //get the flat value sanity heal from the passive's scale
                    healSanity += a.statScale.flatValue;
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
                            float healSanity = SetScale(a.statScale2, user.charc.stats, user);
                            //get the flat value sanity heal from the passive's scale
                            healSanity += a.statScale2.flatValue;
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

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "bloodpumping")
            {
                if (a.inCd > 0 && user.curHp < user.charc.stats.hp)
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
                        float trueDmg = SetScale(a.statScale, user.charc.stats, user);
                        trueDmg += a.statScale.flatValue;

                        user.curHp -= trueDmg;
                        user.trueDmgTaken += trueDmg;
                        user.TakeDamage(trueDmg, 0, false);
                    } else
                    {
                        float heal = SetScale(a.statScale, user.charc.stats, user);
                        heal += a.statScale.flatValue;

                        if ((user.curHp + heal) < user.charc.stats.hp)
                        {
                            user.curHp += heal;
                            user.healDone += heal;
                        } else
                        {
                            heal = user.curHp - user.charc.stats.hp;
                            user.curHp = user.charc.stats.hp;

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
                int hpPer = (int)((100 * user.curHp) / user.charc.stats.hp);

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
                    SetStatsHud(user, userHud);
                    ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                }
            }

            if (a.name == "successoroffire")
            {
                bool foundEffect = false;

                foreach (Effects b in target.effects)
                {
                    if (b.id == "BRN")
                    {
                        foundEffect = true;
                    }
                }

                if (foundEffect)
                {
                    ManagePassiveIcon(a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());

                    Stats statsUser = user.charc.stats.ReturnStats();
                    if (user.statMods.Count > 0)
                        foreach (StatMod b in user.statMods.ToArray())
                        {
                            statsUser = SetModifiers(b, statsUser, user);
                        }

                    StatMod statMod = a.statMod.ReturnStats();
                    statMod.inTime = statMod.time;
                    statMod.atkDmg = statsUser.magicPower * a.num;
                    user.statMods.Add(statMod);
                    user.usedBonusStuff = false;
                    SetStatsHud(user, userHud);
                }
                else
                    DestroyPassiveIcon(a.name, user.isEnemy);
            }
            
            if (a.name == "zenmode")
            {
                //stamina in %
                int staPer = (int)((100 * user.curStamina) / user.charc.stats.stamina);

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
                    SetStatsHud(user, userHud);
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

                    SetStatsHud(user, userHud);
                }
                ManagePassiveIcon(a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
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
                    SetStatsHud(user, userHud);
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

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
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

                ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
            }

            if (a.name == "huntersdirk")
            {
                if (a.inCd > 0)
                    a.inCd--;

                float hpPer = (100 * target.curHp) / target.charc.stats.hp;

                if (hpPer < a.num)
                {
                    DestroyPassiveIcon(a.name, user.isEnemy);
                } else
                {
                    ManagePassiveIcon(a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
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
                SetModifiers(statMod, user.charc.stats.ReturnStats(), user);
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
                SetModifiers(statMod, user.charc.stats.ReturnStats(), user);
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
        Stats statsT = target.charc.stats.ReturnStats();
        if (target.statMods.Count > 0)
            foreach (StatMod statMod in target.statMods.ToArray())
            {
                statsT = SetModifiers(statMod.ReturnStats(), statsT.ReturnStats(), target);
            }

        Stats statsS = summoner.charc.stats.ReturnStats();
        if (target.statMods.Count > 0)
            foreach (StatMod statMod in target.statMods.ToArray())
            {
                statsS = SetModifiers(statMod.ReturnStats(), statsS.ReturnStats(), target);
            }

        bool isDead = false;
        bool isCrit = false;
        float dmg = move.getDmg(statsSum);

        float dmgMitigated = 0;

        switch (move.dmgType)
        {
            case SumMove.DmgType.PHYSICAL:

                if (Random.Range(0f, 1f) < statsS.critChance)
                    isCrit = true;

                if (dmg > 0)
                {
                    if (isCrit == true)
                    {
                        dmg += dmg * statsS.critDmg;
                    }
                }
                else
                    isCrit = false;

                if (dmg > 0)
                {
                    summoner.phyDmgDealt += dmg;
                    dmgMitigated = (float)(statsT.dmgResis * 0.18);
                    dmg -= dmgMitigated;
                    target.phyDmgMitigated += dmgMitigated;
                }

                target.phyDmgTaken += dmg;

                break;
            case SumMove.DmgType.MAGICAL:
                if (dmg > 0)
                {
                    summoner.magicDmgDealt += dmg;
                    dmgMitigated = (float)(statsT.magicResis * 0.12);
                    dmg -= dmgMitigated;
                    target.magicDmgMitigated += dmgMitigated;
                }

                target.magicDmgTaken += dmg;
                break;
            case SumMove.DmgType.TRUE:
                target.trueDmgTaken += dmg;
                break;
            case SumMove.DmgType.HEAL:
                if (dmg > 0)
                {
                    summoner.healDone += dmg;
                    summoner.Heal(dmg);
                }
                break;
            case SumMove.DmgType.SHIELD:
                if (dmg > 0)
                {
                    summoner.curShield += dmg;
                    if (summoner.curShield > 1000)
                        summoner.curShield = 1000;
                }
                summoner.shieldDone += dmg;
                break;
        }

        float shieldedDmg = 0;

        if (target.curShield > 0)
        {
            float tempDmg = dmg;
            float tempShield = target.curShield;

            dmg -= target.curShield;
            target.curShield -= tempDmg;

            if (target.curShield < 0)
                target.curShield = 0;

            shieldedDmg = tempShield - target.curShield;
        }

        if ((dmg > 0 && (move.dmgType != SumMove.DmgType.HEAL) && (move.dmgType != SumMove.DmgType.SHIELD)) || shieldedDmg > 0)
        {
            isDead = target.TakeDamage(dmg, shieldedDmg, isCrit);
            SetUltNumber(target, targetHud, (dmg + shieldedDmg), false);
        }

        if (move.sanityDmg > 0)
        {
            if ((target.curSanity - move.sanityDmg) >= 0)
            {
                target.curSanity -= move.sanityDmg;
            }
            else
            {
                move.sanityDmg = target.curSanity;
                target.curSanity = 0;
            }

            summoner.sanityDmgDealt += move.sanityDmg;
            target.sanityDmgTaken += move.sanityDmg;
        }

        return isDead;
    }

    bool SpawnSummon(Summon sum, Unit summoner, Unit target, GameObject pannel)
    {
        if (sum.summonTurn == 0)
        {
            Stats statsSum = summoner.charc.stats.ReturnStats();

            if (summoner.statMods.Count > 0)
                foreach (StatMod statMod in summoner.statMods.ToArray())
                {
                    statsSum = SetModifiers(statMod.ReturnStats(), statsSum.ReturnStats(), summoner);
                }

            sum.SetupStats(statsSum, summoner);
            sum.summonTurn = turnCount;
            string name = sum.name + sum.summonTurn;

            if (!pannel.transform.Find(name + "(Clone)"))
            {
                barIconPrefab.name = name;
                Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                icon.sprite = sum.icon;
                Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
                text.text = sum.stats.hp.ToString();
                TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
                tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
                tooltipButton.text = langmanag.GetInfo("summon", "name", sum.name);
                Instantiate(barIconPrefab, pannel.transform);
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
                dialogText.text = langmanag.GetInfo("gui", "text", "defeat", name);
                Destroy(pannel.transform.Find(debugname + "(Clone)").gameObject);
                summoner.summons.Remove(sum);
            }
        }

        return false;
    }

    IEnumerator NewTurn()
    {
        yield return new WaitForSeconds(0.7f);
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

        Stats statsP = playerUnit.charc.stats.ReturnStats();
        Stats statsE = enemyUnit.charc.stats.ReturnStats();

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

        if (playerUnit.CountEffectTimer(panelEffectsP))
            state = BattleState.LOSE;

        if (state == BattleState.WIN || state == BattleState.LOSE)
        {
            StartCoroutine(EndBattle());
            yield break;
        }

        if (enemyUnit.CountEffectTimer(panelEffectsE))
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
            yield return new WaitForSeconds(0.65f);

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
            yield return new WaitForSeconds(0.65f);

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
                            if (move.uses > 0)
                            {
                                movebtn.GetComponent<TooltipButton>().text = move.GetTooltipText();
                            } else if (move.uses == 0)
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

        if (recoverManaMoveE.inCooldown > 0)
            recoverManaMoveE.inCooldown--;

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

        if (recoverManaMoveP.inCooldown > 0)
            recoverManaMoveP.inCooldown--;

        statsP = playerUnit.charc.stats.ReturnStats();
        statsE = enemyUnit.charc.stats.ReturnStats();

        if (playerUnit.statMods.Count > 0)
            foreach (StatMod statMod in playerUnit.statMods.ToArray())
            {
                if (statMod.inTime > 0 || statMod.inTime == -1)
                    statsP = SetModifiers(statMod.ReturnStats(), statsP.ReturnStats(), playerUnit);

                if (statMod.inTime > 0)
                    statMod.inTime--;

                if (statMod.inTime == 0)
                    playerUnit.statMods.Remove(statMod);
            }

        if (enemyUnit.statMods.Count > 0)
            foreach (StatMod statMod in enemyUnit.statMods.ToArray())
            {
                if (statMod.inTime > 0 || statMod.inTime == -1)
                    statsE = SetModifiers(statMod.ReturnStats(), statsE.ReturnStats(), enemyUnit);

                if (statMod.inTime > 0)
                    statMod.inTime--;

                if (statMod.inTime == 0)
                    enemyUnit.statMods.Remove(statMod);
            }

        SetStatsHud(playerUnit, playerHUD);
        SetStatsHud(enemyUnit, enemyHUD);

        CheckPassiveTurn(playerUnit, playerHUD, enemyUnit);
        CheckPassiveTurn(enemyUnit, enemyHUD, playerUnit);

        playerUnit.Heal(statsP.hpRegen);
        if (turnCount > 1)
            playerUnit.healDone += statsP.hpRegen;

        enemyUnit.Heal(statsE.hpRegen);
        if (turnCount > 1)
            enemyUnit.healDone += statsE.hpRegen;

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
            if (!a.canUseMagic && !a.canUsePhysical && !a.canUseRanged && !a.canUseStatMod && !a.canUseSupp && !a.canUseProtec && !a.canUseSummon)
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
        SetStatsHud(playerUnit, playerHUD);
        SetStatsHud(enemyUnit, enemyHUD);

        sumPlayerHud.UpdateValues(playerUnit, langmanag.GetInfo("charc", "name", playerUnit.charc.name));
        sumEnemyHud.UpdateValues(enemyUnit, langmanag.GetInfo("charc", "name", enemyUnit.charc.name));

        if (skip)
            StartCoroutine(Combat(null));
        else
            PlayerTurn();
    } 
}