using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, CHANGETURN, ALLYKILLED, ENEMYKILLED, WIN, LOSE, TIE }

public class BattleSystem : MonoBehaviour
{
    public Player player;
    public Player enemy;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;

    [SerializeField] private BattleState state;
    [SerializeField] private float aiManaRecover = 0.12f;
    [SerializeField] private float aiGaranteedManaRecover = 0.08f;
    [SerializeField] private int manaRecoverCdReducWeak = 3;
    [SerializeField] private float tiredStart = 0.05f;
    [SerializeField] private float tiredGrowth = 0.015f;
    [SerializeField] private int tiredStacks = 0;
    [SerializeField] public int maxShield = 2000;
    [SerializeField] private int levelToConsiderWeak = 15;
    [SerializeField] private float dmgResisPer = 0.18f;
    [SerializeField] private float magicResisPer = 0.12f;
    [SerializeField] private float dotReduc = 0.3f;
    [SerializeField] private float ultComp = 0.25f;
    [SerializeField] private int ultCompDuration = 6;
    [SerializeField] private int summonHpLostBase = 30;
    [SerializeField] private float summonHpLostPer = 0.2f;
    [SerializeField] private int bloodLossStacks = 10;
    //Change blood special tooltip if changed ^
    [SerializeField] private Text dialogText;

    [SerializeField] private GameObject playerHudList;
    [SerializeField] private GameObject enemyHudList;
    [SerializeField] private GameObject battleHudP;
    [SerializeField] private GameObject battleHudE;

    [SerializeField] private SummaryHud sumPlayerHud;
    [SerializeField] private SummaryHud sumEnemyHud;
    [SerializeField] private GameObject sumHud;
    [SerializeField] private GameObject sumHudHideBtn;
    [SerializeField] private GameObject leaveBtn;
    [SerializeField] private Button overviewBtn;

    [SerializeField] private Text turnsText;
    [SerializeField] private Text turnsTextOverview;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private int combatCount = 0;
    [SerializeField] private float ultEnergyDealt;
    [SerializeField] private float ultEnergyTaken;

    [SerializeField] private EffectsMove tiredw;
    [SerializeField] private EffectsMove tiredm;
    [SerializeField] private EffectsMove fear;
    [SerializeField] private Effects scorch;

    [SerializeField] private ActionBox actionBox1p;
    [SerializeField] private ActionBox actionBox2p;
    [SerializeField] private ActionBox actionBox3p;
    [SerializeField] private ActionBox actionBox1e;
    [SerializeField] private ActionBox actionBox2e;
    [SerializeField] private ActionBox actionBox3e;

    [SerializeField] private GameObject moveButton;

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
        PlayerPrefs.DeleteKey("SelectedCharacter1");
        PlayerPrefs.DeleteKey("SelectedCharacter2");
        PlayerPrefs.DeleteKey("SelectedCharacter3");
        PlayerPrefs.DeleteKey("SelectedEnemy1");
        PlayerPrefs.DeleteKey("SelectedEnemy2");
        PlayerPrefs.DeleteKey("SelectedEnemy3");

        if ((state == BattleState.WIN || state == BattleState.LOSE) && PlayerPrefs.GetInt("isEndless") == 1)
            loader.LoadScene(3, slider, loadPanel);
        else
            loader.LoadScene(0, slider, loadPanel);
    }

    IEnumerator SetupBattle()
    {
        player.unit1 = Instantiate(playerPrefab, playerBattleStation.Find("P1")).GetComponent<Unit>();
        player.unit2 = Instantiate(playerPrefab, playerBattleStation.Find("P2")).GetComponent<Unit>();
        player.unit3 = Instantiate(playerPrefab, playerBattleStation.Find("P3")).GetComponent<Unit>();

        enemy.unit1 = Instantiate(enemyPrefab, enemyBattleStation.Find("P1")).GetComponent<Unit>();
        enemy.unit2 = Instantiate(enemyPrefab, enemyBattleStation.Find("P2")).GetComponent<Unit>();
        enemy.unit3 = Instantiate(enemyPrefab, enemyBattleStation.Find("P3")).GetComponent<Unit>();

        BattleHud battleHud1 = Instantiate(battleHudP, playerHudList.transform).GetComponent<BattleHud>();
        BattleHud battleHud2 = Instantiate(battleHudP, playerHudList.transform).GetComponent<BattleHud>();
        BattleHud battleHud3 = Instantiate(battleHudP, playerHudList.transform).GetComponent<BattleHud>();

        actionBox1p.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, player.unit1);
        actionBox2p.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, player.unit2);
        actionBox3p.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, player.unit3);
        player.SetStart(aiManaRecover, aiGaranteedManaRecover, battleHud1, battleHud2, battleHud3);

        battleHud1 = Instantiate(battleHudE, enemyHudList.transform).GetComponent<BattleHud>();
        battleHud2 = Instantiate(battleHudE, enemyHudList.transform).GetComponent<BattleHud>();
        battleHud3 = Instantiate(battleHudE, enemyHudList.transform).GetComponent<BattleHud>();

        actionBox1e.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, enemy.unit1);
        actionBox2e.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, enemy.unit2);
        actionBox3e.Setup(levelToConsiderWeak, manaRecoverCdReducWeak, enemy.unit3);
        enemy.SetStart(aiManaRecover, aiGaranteedManaRecover, battleHud1, battleHud2, battleHud3);

        dialogText.text = langmanag.GetInfo("gui", "text", "wantfight", langmanag.GetInfo("charc", "name", enemy.unit1.charc.name));

        //movesBtn.interactable = false;
        //basicBtn.interactable = false;
        //healManaBtn.interactable = false;
        //ultBtn.interactable = false;

        player.SetUpStats(player.unit1, info, tiredStart + (tiredGrowth * tiredStacks));
        player.SetUpStats(player.unit2, info, tiredStart + (tiredGrowth * tiredStacks));
        player.SetUpStats(player.unit3, info, tiredStart + (tiredGrowth * tiredStacks));

        enemy.SetUpStats(enemy.unit1, info, tiredStart + (tiredGrowth * tiredStacks));
        enemy.SetUpStats(enemy.unit2, info, tiredStart + (tiredGrowth * tiredStacks));
        enemy.SetUpStats(enemy.unit3, info, tiredStart + (tiredGrowth * tiredStacks));

        player.unit1.GetItems(info, items);
        player.unit2.GetItems(info, items);
        player.unit3.GetItems(info, items);

        enemy.PickItemAi(enemy.unit1, player.unit1.items.Count());
        enemy.PickItemAi(enemy.unit2, player.unit2.items.Count());
        enemy.PickItemAi(enemy.unit3, player.unit3.items.Count());

        enemy.unit1.GetItems(info, items);
        enemy.unit2.GetItems(info, items);
        enemy.unit3.GetItems(info, items);

        SetStatus();

        player.UpdateStats();
        enemy.UpdateStats();
        
        foreach (Moves move in enemy.unit1.moves)
        {
            foreach (EffectsMove a in move.effects)
            {
                a.SetApply(false);
            }

            if (move.name != "recovmana")
                move.inCooldown = 0;
        }

        foreach (Moves move in enemy.unit2.moves)
        {
            foreach (EffectsMove a in move.effects)
            {
                a.SetApply(false);
            }

            if (move.name != "recovmana")
                move.inCooldown = 0;
        }

        foreach (Moves move in enemy.unit3.moves)
        {
            foreach (EffectsMove a in move.effects)
            {
                a.SetApply(false);
            }

            if (move.name != "recovmana")
                move.inCooldown = 0;
        }

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
                enemy.GetLeader().passives.Add(endlessStuff.bossPassive.ReturnPassive());
        }
        int i = 0;
        foreach (Moves move in player.unit1.moves)
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
                move.id = i;

                Text name = moveButton.transform.Find("Name").gameObject.GetComponent<Text>();
                name.text = langmanag.GetInfo("moves", move.name);

                /*Text mana = moveButton.transform.Find("Mana").gameObject.GetComponent<Text>();
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
                mn.text = langmanag.GetInfo("gui", "text", "mn");*/

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

                Instantiate(moveButton, player.unit1.moveListPanel.GetChild(0));
            }   

        }
        i = 0;
        foreach (Moves move in player.unit2.moves)
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
                move.id = i;

                Text name = moveButton.transform.Find("Name").gameObject.GetComponent<Text>();
                name.text = langmanag.GetInfo("moves", move.name);

                //Text mana = moveButton.transform.Find("Mana").gameObject.GetComponent<Text>();
                //mana.text = move.manaCost.ToString();

                //Text stamina = moveButton.transform.Find("Stamina").gameObject.GetComponent<Text>();
                //stamina.text = move.staminaCost.ToString();

                //Text cdn = moveButton.transform.Find("Cooldown").gameObject.GetComponent<Text>();
                //cdn.text = move.cooldown.ToString();

                //Text cd = moveButton.transform.Find("CD").gameObject.GetComponent<Text>();
                //cd.text = langmanag.GetInfo("gui", "text", "cd");

                //Text sta = moveButton.transform.Find("STA").gameObject.GetComponent<Text>();
                //sta.text = langmanag.GetInfo("gui", "text", "sta");

                //Text mn = moveButton.transform.Find("MN").gameObject.GetComponent<Text>();
                //mn.text = langmanag.GetInfo("gui", "text", "mn");

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

                Instantiate(moveButton, player.unit2.moveListPanel.GetChild(0));
            }

        }
        i = 0;
        foreach (Moves move in player.unit3.moves)
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
                move.id = i;

                Text name = moveButton.transform.Find("Name").gameObject.GetComponent<Text>();
                name.text = langmanag.GetInfo("moves", move.name);

                //Text mana = moveButton.transform.Find("Mana").gameObject.GetComponent<Text>();
                //mana.text = move.manaCost.ToString();

                //Text stamina = moveButton.transform.Find("Stamina").gameObject.GetComponent<Text>();
                //stamina.text = move.staminaCost.ToString();

                //Text cdn = moveButton.transform.Find("Cooldown").gameObject.GetComponent<Text>();
                //cdn.text = move.cooldown.ToString();

                //Text cd = moveButton.transform.Find("CD").gameObject.GetComponent<Text>();
                //cd.text = langmanag.GetInfo("gui", "text", "cd");

                //Text sta = moveButton.transform.Find("STA").gameObject.GetComponent<Text>();
                //sta.text = langmanag.GetInfo("gui", "text", "sta");

                //Text mn = moveButton.transform.Find("MN").gameObject.GetComponent<Text>();
                //mn.text = langmanag.GetInfo("gui", "text", "mn");

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

                Instantiate(moveButton, player.unit3.moveListPanel.GetChild(0));
            }

        }

        UpdateTooltips(player.unit1);
        UpdateTooltips(player.unit2);
        UpdateTooltips(player.unit3);
        yield return new WaitForSeconds(1.2f);
        if (PlayerPrefs.GetInt("isEndless") == 1 && PlayerPrefs.GetInt("isEnemyBoss") == 1)
            cameraAudio.clip = bossfightMusic;
        else
            cameraAudio.clip = enemy.GetLeader().charc.audio;

        cameraAudio.Play();

        StartCoroutine(NewTurn());
    }

    void SelectMovingCharacter()
    {
        if (combatCount < player.GetAliveCharacters() - player.GetIncapacitatedCharacters())
        {
            if (!player.unit1.hasAttacked && !player.unit1.CheckSkipTurn())
                player.EnableBtn(player.unit1);

            if (!player.unit2.hasAttacked && !player.unit2.CheckSkipTurn())
                player.EnableBtn(player.unit2);

            if (!player.unit3.hasAttacked && !player.unit3.CheckSkipTurn())
                player.EnableBtn(player.unit3);
        } else
        {
            if (!player.unit1.hasAttacked && !player.unit1.isDead && !player.unit1.CheckSkipTurn())
                player.EnableBtn(player.unit1);
            else if (!player.unit2.hasAttacked && !player.unit2.isDead && !player.unit2.CheckSkipTurn())
                player.EnableBtn(player.unit2);
            else if (!player.unit3.hasAttacked && !player.unit3.isDead && !player.unit3.CheckSkipTurn())
                player.EnableBtn(player.unit3);
            else
                StartCoroutine(Combat(null, enemy.AIGetAttacker(combatCount)));
        }
    }

    public void DoneSelecting()
    {
        player.DisableAllBtn();
        player.GetAttacker().actionBoxPanel.gameObject.SetActive(true);
        player.GetAttacker().actionBoxPanel.parent.parent.GetComponent<ActionBox>().SetupSpecialMoveBtn(1);
        player.GetAttacker().actionBoxPanel.parent.parent.GetComponent<ActionBox>().SetupSpecialMoveBtn(2);
    }

    public void HideMoveHud()
    {
        player.GetAttacker().actionBoxPanel.gameObject.SetActive(false);
        player.GetAttacker().moveListPanel.parent.parent.gameObject.SetActive(false);
    }

    public void DoneChoosingMove()
    {
        player.DisableAllBtn();
        enemy.DisableAllBtn();
        StartCoroutine(Combat(player.GetAttacker(), enemy.AIGetAttacker(combatCount)));
    }

    IEnumerator Combat(Unit player, Unit enemy)
    {
        tooltipMain.transform.Find("TooltipCanvas").gameObject.SetActive(false);

        List<Unit> characters = new List<Unit>();

        if (player != null)
        {
            characters.Add(player);
            player.hasAttacked = true;
        }

        if (enemy != null)
        {
            int temp = 0;
            while (true)
            {
                temp++;
                enemy.chosenMove.target = this.player.GetRandom();
                if (!enemy.chosenMove.target.isDead)
                    break;

                if (temp == 1000)
                    break;
            }
            Moves moveEnemy = this.enemy.AIChooseMove(enemy, enemy.chosenMove.target);
            enemy.SetAnimHud("isSelected", true);
            enemy.chosenMove.move = moveEnemy;
            characters.Add(enemy);
            enemy.hasAttacked = true;
        }

        characters = characters.OrderByDescending(c => c.chosenMove.move.priority).ThenByDescending(c => c.SetModifiers().movSpeed).ToList();

        foreach (Unit charc in characters)
        {
            charc.chosenMove.move.inCooldown = charc.chosenMove.move.cooldown;

            bool canUseNormal = true;

            if (charc.chosenMove.move == null)
                canUseNormal = false;

            if (charc.isEnemy)
                state = BattleState.ENEMYTURN;
            else
                state = BattleState.PLAYERTURN;

            if (canUseNormal)
            {
                yield return StartCoroutine(Attack(charc.chosenMove.move, charc, charc.chosenMove.target));
                charc.hud.SetStatsHud(charc);
                charc.chosenMove.target.hud.SetStatsHud(charc.chosenMove.target);
            }
            else
            {
                yield return StartCoroutine(Attack(charc.chosenMove.move, charc, charc.chosenMove.target));
                charc.hud.SetStatsHud(charc);
                charc.chosenMove.target.hud.SetStatsHud(charc.chosenMove.target);
            }
        }

        if (player)
            player.SetAnimHud("isSelected", false);

        if (enemy)
            enemy.SetAnimHud("isSelected", false);

        combatCount++;
        this.player.ResetAttacker();
        this.enemy.ResetAttacker();

        CheckVictory();

        if (state == BattleState.WIN || state == BattleState.LOSE)
            StartCoroutine(EndBattle());
        else
        {
            if (combatCount == GetCombatCountMax())
                StartCoroutine(NewTurn());
            else
                SelectMovingCharacter();
        }
            
    }

    int GetCombatCountMax()
    {
        if (player.GetAliveCharacters() > enemy.GetAliveCharacters())
            return player.GetAliveCharacters();
        else if (player.GetAliveCharacters() < enemy.GetAliveCharacters())
            return enemy.GetAliveCharacters();
        else
            return player.GetAliveCharacters();
    }

    IEnumerator Attack(Moves move, Unit user, Unit target)
    {
        user.SetCC();
        bool canMove = true;
        bool canAttack = true;
        if (move)
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

            if (move != null && move.isUlt)
            {
                GrantUltCompansation(user);
            }

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
            {
                move.inCooldown = move.cooldown;
                if (user.level <= levelToConsiderWeak)
                    move.inCooldown -= manaRecoverCdReducWeak;
            }

            if (user.isEnemy)
                sumEnemyHud.AddMoveLog(user, move);
            else
                sumPlayerHud.AddMoveLog(user, move);

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
                            ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
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

                    isCrit = Random.Range(0f, 1f) <= (statsUser.critChance + move.critChanceBonus);
                    dmgTarget.Reset();
                    dmgUser.Reset();

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
                                DestroyPassiveIcon(user.effectHud, a.name, target.isEnemy);
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
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, (a.maxNum - a.num).ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
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
                                DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                            }                            
                        }

                        if (a.name == "vendetta")
                        {
                            if (target.SetModifiers().hp >= (user.SetModifiers().hp + (user.SetModifiers().hp * a.num)) && a.stacks != 1)
                            {
                                a.stacks = 1;
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
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
                                StatScale scale = a.ifConditionTrueScale();
                                dmgTarget.AddDmg(scale.SetScaleDmg(statsUser, user));
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
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
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
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
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
                                    StatScale scale = a.ifConditionTrueScale();
                                    dmgTarget.AddDmg(scale.SetScaleDmg(statsUser, user));

                                    a.inCd = a.cd;

                                    //apply statmod
                                    StatMod statMod = a.statMod.ReturnStats();
                                    statMod.inTime = statMod.time;
                                    user.statMods.Add(statMod);
                                    user.usedBonusStuff = false;

                                    if (user.isEnemy)
                                        target.hud.SetStatsHud(user);
                                    else
                                        user.hud.SetStatsHud(user);
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
                            if ((move.type is Moves.MoveType.MAGICAL || move.type is Moves.MoveType.ENCHANT || move.type is Moves.MoveType.BASIC) && a.inCd == 0)
                            {
                                a.inCd = a.cd;
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, (a.maxNum - a.num).ToString(), user.isEnemy, a.GetPassiveInfo());

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

                                DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
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
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                            }
                            
                            if (!enoughMana || isSilence)
                            {
                                DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
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

                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
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
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
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
                                DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
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

                        if (a.name == "bandofendurance")
                        {
                            if (move.type is Moves.MoveType.BASIC || move.type is Moves.MoveType.PHYSICAL)
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

                                DMG temp = scale.SetScaleDmg(stats, unit);

                                if (isCrit)
                                    temp.Multiply(a.num/100);

                                dmgTarget.AddDmg(temp);
                            }
                        }

                        if (a.name == "mythicearrings")
                        {
                            if (move.type is Moves.MoveType.BASIC || move.type is Moves.MoveType.MAGICAL)
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

                                DMG temp = scale.SetScaleDmg(stats, unit);

                                dmgTarget.AddDmg(temp);
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

                        if (a.name == "combatrythm")
                        {
                            if (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.MAGICAL)
                            {
                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                StatScale scale = a.ifConditionTrueScale();
                                dmgTarget.AddDmg(scale.SetScaleDmg(statsUser, user));
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

                                sumPlayerHud.UpdateValues(user, langmanag.GetInfo("charc", "name", user.charc.name));
                                sumEnemyHud.UpdateValues(target, langmanag.GetInfo("charc", "name", target.charc.name));

                                if (isDead)
                                {
                                    if (user.isEnemy)
                                    {
                                        player.SetAsDead(user);
                                        state = BattleState.ALLYKILLED;
                                    }
                                    else
                                    {
                                        enemy.SetAsDead(user);
                                        state = BattleState.ENEMYKILLED;
                                    }
                                    
                                    dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", user.charc.name));
                                }
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
                                        ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
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
                            int bonusDuration = 0;

                            if (move.effects.Count > 0)
                            {
                                foreach (EffectsMove a in move.effects)
                                {                                    
                                    Effects effect = a.effect.ReturnEffect();

                                    if (Random.Range(0f, 1f) <= a.chance && !a.WasApplied())
                                    {
                                        foreach (Passives b in target.passives.ToArray())
                                        {
                                            switch (b.name)
                                            {
                                                case "ecolocation":
                                                    if (effect.id == "BLI")
                                                    {
                                                        skipEffect = true;
                                                        target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                    }
                                                break;
                                                case "strongmind":
                                                    if (effect.id == "CFS" || effect.id == "SLP" || effect.id == "CHR")
                                                    {
                                                        skipEffect = true;
                                                        target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                    }
                                                break;
                                                case "sackofbones":
                                                    if (effect.id == "BLD")
                                                    {
                                                        skipEffect = true;
                                                        target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                    } else if (effect.id == "CRP")
                                                    {
                                                        dmgTarget.trueDmg = b.statScale.SetScaleFlat(user.SetModifiers(), user);
                                                        target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                        bonusDuration += (int)b.num;
                                                    }
                                                break;
                                                case "firescales":
                                                    if (effect.id == "BRN" || effect.id == "SCH")
                                                    {
                                                        skipEffect = true;
                                                        target.PassivePopup(langmanag.GetInfo("passive", "name", b.name));
                                                        StatMod mod = b.statMod.ReturnStats();
                                                        mod.inTime = mod.time + 1;
                                                        target.statMods.Add(mod);
                                                        statsTarget = target.SetModifiers();
                                                    }
                                                    break;
                                            }
                                        }

                                        if (!skipEffect)
                                        {
                                            //Debug.Log("I EFFECT");
                                            effect.duration = Random.Range(a.durationMin, a.durationMax);
                                            effect.duration += bonusDuration;

                                            if (a.targetPlayer)
                                            {
                                                Effects check = user.CheckIfEffectExists(effect.id);
                                                if (check == null)
                                                {
                                                    effect.source.Add(user);
                                                    user.effects.Add(effect);
                                                    SetEffectIcon(effect, user.effectHud.gameObject);

                                                    foreach (StatMod b in effect.statMods)
                                                    {
                                                        StatMod statMod = b.ReturnStats();
                                                        statMod.inTime = effect.duration+1;
                                                        dmgTarget.heal += (int)((user.SetModifiers().hp * statMod.hp) / 2);
                                                        dmgTarget.healMana += (int)((user.SetModifiers().mana * statMod.mana) / 2);
                                                        dmgTarget.healStamina += (int)((user.SetModifiers().stamina * statMod.stamina) / 2);
                                                        dmgTarget.healSanity += (int)((user.SetModifiers().sanity * statMod.sanity) / 2);
                                                        user.statMods.Add(statMod);
                                                        user.usedBonusStuff = false;
                                                    }
                                                } else
                                                {
                                                    effect.source.Add(user);
                                                    if (check.id == "BRN" && effect.id == "BRN")
                                                    {
                                                        Effects newScorch = scorch.ReturnEffect();
                                                        newScorch.duration = check.duration + effect.duration;
                                                        newScorch.source = check.source;
                                                        user.effects.Add(newScorch);
                                                        user.effects.Remove(check);

                                                        GameObject temp = user.effectHud.transform.Find(check.id + "(Clone)").gameObject;
                                                        Destroy(temp.gameObject);
                                                        SetEffectIcon(newScorch, user.effectHud.gameObject);

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
                                                    SetEffectIcon(effect, target.effectHud.gameObject);

                                                    foreach (StatMod b in effect.statMods)
                                                    {
                                                        StatMod statMod = b.ReturnStats();
                                                        statMod.inTime = effect.duration + 1;
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

                                                        GameObject temp = target.effectHud.transform.Find(check.id + "(Clone)").gameObject;
                                                        Destroy(temp.gameObject);
                                                        SetEffectIcon(newScorch, target.effectHud.gameObject);
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
                                switch (a.name)
                                {
                                    case "huntingseason":
                                        dmgTarget.ApplyBonusDmg(a.num, a.num, 0);
                                        break;
                                    case "mechashield":
                                        //convert physical damage into magic damage
                                        float con = dmgTarget.phyDmg * a.num;
                                        dmgTarget.phyDmg -= con;
                                        dmgTarget.magicDmg += con;
                                        break;
                                    case "spectralpike":
                                        if (a.inCd == 0 && (move.type is Moves.MoveType.PHYSICAL || move.type is Moves.MoveType.RANGED))
                                        {
                                            float chance = a.num;
                                            int hpPercentage = (int)(100 * user.curHp / user.SetModifiers().hp);

                                            if (hpPercentage <= a.maxNum * 100)
                                                chance *= 2;

                                            if (Random.Range(0f, 1f) <= chance)
                                            {
                                                user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                                dmgTarget.ApplyBonusPhyDmg((float)a.maxStacks / 100);
                                                a.inCd = a.cd;
                                            }
                                        }
                                        break;
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


                            foreach (Passives a in target.passives.ToArray())
                            {
                                switch (a.name)
                                {
                                    case "roughskin":
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
                                        break;
                                    case "dreadofthesupernatural":
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
                                        break;
                                    case "bullsrage":
                                        if (isCrit)
                                        {
                                            if (a.stacks < a.maxStacks && a.stacks >= 0)
                                            {
                                                a.stacks++;
                                                ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.stacks.ToString(), target.isEnemy, a.GetPassiveInfo());
                                            }
                                        }
                                        break;
                                    case "thickarmour":
                                        if (isCrit)
                                        {
                                            dmgTarget.ApplyBonusPhyDmg(a.num);
                                            target.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                                        }
                                        break;
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
                                        user.hud.SetStatsHud(user);
                                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                                    }

                                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, (a.stacks + "S" + (a.maxNum - a.num) + "T").ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                                }
                            }

                            dmgTarget = target.MitigateDmg(dmgTarget, dmgResisPer, magicResisPer, user.SetModifiers().armourPen, user.SetModifiers().magicPen, user);

                            dmgTarget = user.ApplyHealFrom(dmgTarget, move.healFromDmgType, move.healFromDmg);
                            dmgTarget = user.ApplyLifesteal(dmgTarget);
                            dmgTarget = dmgUser.TransferHeals(dmgTarget);
                            float dmgT = dmgTarget.phyDmg + dmgTarget.magicDmg + dmgTarget.trueDmg;

                            if (dmgT > 0)
                            {
                                if (!(move.isUlt))
                                    SetUltNumber(user, user.hud, dmgT, true);

                                if (move.isUlt)
                                    SetUltNumber(target, target.hud, (dmgT / 2), false);
                                else
                                    SetUltNumber(target, target.hud, dmgT, false);
                            }

                            isDead = target.TakeDamage(dmgTarget, isCrit, isMagicCrit, user);

                            if (move.target == Moves.Target.ALLY)
                                target.TakeDamage(dmgUser, isCrit, isMagicCrit, user, move.name);
                            else
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
                                        ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                    } else
                                    {
                                        if (a.inCd < a.cd)
                                        {
                                            a.inCd++;
                                            ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
                                        }
                                    }
                                }
                            }

                            SetStatus();

                            yield return new WaitForSeconds(1.75f);

                            if (isDead && !target.isDead)
                            {
                                if (!target.isEnemy)
                                {
                                    player.SetAsDead(target);
                                    state = BattleState.ALLYKILLED;
                                }
                                else
                                {
                                    enemy.SetAsDead(target);
                                    state = BattleState.ENEMYKILLED;
                                }
                                dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", target.charc.name));
                            }
                        }
                        sumPlayerHud.UpdateValues(user, langmanag.GetInfo("charc", "name", user.charc.name));
                        sumEnemyHud.UpdateValues(target, langmanag.GetInfo("charc", "name", target.charc.name));
                    }
                    else
                    {
                        target.Miss(false);
                        isStoped = false;

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
                                    ManagePassiveIcon(target.effectHud, a.sprite, a.name, a.inCd.ToString(), target.isEnemy, a.GetPassiveInfo());
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
            float value = 0;

            if (dmg > 0)
            {
                if (isDealt)
                    value += dmg * ultEnergyDealt;
                else
                    value += dmg * ultEnergyTaken;
            }

            if (value > 0)
                value = value * temp.ultrate;

            user.ult += value;

            if (user.ult > 100)
                user.ult = 100;
        }
    }

    void SetStatus()
    {
        player.SetStats(player.unit1, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);
        player.SetStats(player.unit2, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);
        player.SetStats(player.unit3, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);

        enemy.SetStats(enemy.unit1, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);
        enemy.SetStats(enemy.unit2, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);
        enemy.SetStats(enemy.unit3, turnCount, tiredStart, tiredGrowth, tiredStacks, state, info);
    }

    IEnumerator EndBattle()
    {
        SetStatus();
        if (state == BattleState.WIN)
        {
            //TO DO
            /*if (PlayerPrefs.GetInt("isEndless") == 1)
            {
                PlayerPrefs.SetInt("wonLastRound", 1);
                info.round++;
                
                float health = playerUnit.curHp + (playerUnit.SetModifiers().hp - playerUnit.curHp) * perHealthRegenEndless;
                float mana = playerUnit.curMana + (playerUnit.SetModifiers().mana - playerUnit.curMana) * perCostsRegenEndless;
                float stamina = playerUnit.curStamina + (playerUnit.SetModifiers().stamina - playerUnit.curStamina) * perCostsRegenEndless;
                int sanity = (int)(playerUnit.curSanity + (playerUnit.SetModifiers().sanity - playerUnit.curSanity) * perSanityRegenEndless);
                float ult = playerUnit.ult - (playerUnit.ult*perUltReduce);

                info.playerHp = ((100 * health) / playerUnit.SetModifiers().hp)/100;
                info.playerMn = ((100 * mana) / playerUnit.SetModifiers().mana)/100;
                info.playerSta = ((100 * stamina) / playerUnit.SetModifiers().stamina)/100;
                info.playerSan = ((float)(100 * sanity) / playerUnit.SetModifiers().sanity) / 100;
                info.playerUlt = ult;

                SaveSystem.Save(info);
            }*/
                
            yield return new WaitForSeconds(1.6f);
            player.Victory();
            dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", enemy.GetLeader().charc.name));
            yield return new WaitForSeconds(1.8f);
            dialogText.text = langmanag.GetInfo("gui", "text", "win", langmanag.GetInfo("charc", "name", player.GetLeader().charc.name));
        } else if (state == BattleState.LOSE)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1)
                PlayerPrefs.SetInt("wonLastRound", 0);

            yield return new WaitForSeconds(1.6f);
            enemy.Victory();
            dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", player.GetLeader().charc.name));
            yield return new WaitForSeconds(1.8f);
            dialogText.text = langmanag.GetInfo("gui", "text", "win", langmanag.GetInfo("charc", "name", enemy.GetLeader().charc.name));
        } else if (state == BattleState.TIE)
        {
            if (PlayerPrefs.GetInt("isEndless") == 1)
                PlayerPrefs.SetInt("wonLastRound", 0);

            yield return new WaitForSeconds(1.6f);
            dialogText.text = langmanag.GetInfo("gui", "text", "defeat", langmanag.GetInfo("charc", "name", player.GetLeader().charc.name));
            yield return new WaitForSeconds(1.8f);
            dialogText.text = langmanag.GetInfo("gui", "text", "win", langmanag.GetInfo("charc", "name", enemy.GetLeader().charc.name));
        }

        yield return new WaitForSeconds(1.5f);

        leaveBtn.SetActive(true);
        sumHudHideBtn.SetActive(false);
        ShowOverview(true);
    }

    void PlayerTurn()
    {
        player.ResetAttacker();
        enemy.ResetAttacker();
        dialogText.text = langmanag.GetInfo("gui", "text", "choosemove");
        SelectMovingCharacter();
        //movesBtn.interactable = true;
        //basicBtn.interactable = true;

        //if ((playerUnit.ult == 100 && playerUnit.ultMove.needFullUlt) ||
        //(playerUnit.ult == playerUnit.ultMove.ultCost && !playerUnit.ultMove.needFullUlt))
        //{
        //    bool canUse = true;

        //    switch (playerUnit.ultMove.type)
        //    {
        //        case Moves.MoveType.PHYSICAL:
        //            if (!playerUnit.canUsePhysical)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.MAGICAL:
        //            if (!playerUnit.canUseMagic)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.RANGED:
        //            if (!playerUnit.canUseRanged)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.DEFFENCIVE:
        //            if (!playerUnit.canUseProtec)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.SUPPORT:
        //            if (!playerUnit.canUseSupp)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.ENCHANT:
        //            if (!playerUnit.canUseEnchant)
        //                canUse = false;
        //            break;
        //        case Moves.MoveType.SUMMON:
        //            if (!playerUnit.canUseSummon)
        //                canUse = false;
        //            break;
        //    }

        //    if (canUse)
        //        ultBtn.interactable = true;
        //    else
        //        ultBtn.interactable = false;
        //}

        //if (playerUnit.recoverMana.inCooldown <= 0)
        //{
        //    healManaBtn.interactable = true;
        //    healBtnText.text = langmanag.GetInfo("moves", "recovmana");
        //    if (!playerUnit.canUseSupp)
        //        healManaBtn.interactable = false;
        //}
        //else
        //{
        //    healManaBtn.interactable = false;
        //    healBtnText.text = langmanag.GetInfo("moves", "recovmana") + " (" + playerUnit.recoverMana.inCooldown + ")";
        //}  
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
        float timesInc = a.timesInc;
        bool isDead = false;

        dmg.phyDmg = Random.Range(a.phyDmgMin, a.phyDmgMax) + (a.phyDmgInc * timesInc);
        dmg.magicDmg = Random.Range(a.magicDmgMin, a.magicDmgMax) + (a.magicDmgInc * timesInc);
        dmg.trueDmg = Random.Range(a.trueDmgMin, a.trueDmgMax) + (a.trueDmgInc * timesInc);
        dmg.sanityDmg = Random.Range(a.sanityDmgMin, a.sanityDmgMax) + (int)(a.sanityDmgInc * timesInc);
        dmg.heal = Random.Range(a.healMin, a.healMax) + (a.healInc * timesInc);
        dmg.healMana = Random.Range(a.healManaMin, a.healManaMax) + (a.healManaInc * timesInc);
        dmg.healStamina = Random.Range(a.healStaminaMin, a.healStaminaMax) + (a.healStaminaInc * timesInc);
        dmg.healSanity = Random.Range(a.sanityHealMin, a.sanityHealMax) + (int)(a.sanityHealInc * timesInc);
        dmg.shield = Random.Range(a.shieldMin, a.shieldMax) + (a.shieldInc * timesInc);

        if (!a.isScaleSpecial)
            foreach (StatScale scale in a.scale.ToArray())
            {
                dmg.AddDmg(scale.SetScaleDmg(user.SetModifiers().ReturnStats(), user));
            }

        foreach (Passives b in user.passives)
        {
            switch (b.name)
            {
                case "leafbeing":
                    if (a.name == "BRN" || a.name == "SCH" || a.name == "PSN")
                    {
                        dmg.ApplyBonusDmg(b.num, b.num, b.num);
                        user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                    }
                    break;
            }
        }
        

        dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0, null, dotReduc);
        dmg = user.CalcRegens(dmg);

        isDead = user.TakeDamage(dmg, false, false, user);

        /*user.sanityDmgTaken += dmg.sanityDmg;

        user.healDone += dmg.heal;
        user.manaHealDone += dmg.healMana;
        user.staminaHealDone += dmg.healStamina;
        user.sanityHealDone += dmg.healSanity;*/

        SetStatus();
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
                if (a.dmg > 0 && a.type is Dotdmg.DmgType.SANITY)
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

    void CreatePassiveIcon(Transform panel, Sprite pIcon, string name, string num, bool isEnemy, string passiveDesc)
    {
        barIconPrefab.name = name;

        Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
        icon.sprite = pIcon;
        Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
        text.text = num.ToString();

        TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
        tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();
        tooltipButton.text = passiveDesc;

        Instantiate(barIconPrefab, panel);
    }

    void ManagePassiveIcon(Transform panel, Sprite pIcon, string name, string num, bool isEnemy, string passiveDesc, bool isReady = false)
    {
        if (num == "0")
            num = "";

            if (panel.Find(name + "(Clone)"))
            {
                panel.Find(name + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = num;
                panel.Find(name + "(Clone)").gameObject.GetComponent<Animator>().SetBool("ready", isReady);
            }
            else
                CreatePassiveIcon(panel, pIcon, name, num, isEnemy, passiveDesc);
    }

    void DestroyPassiveIcon(Transform panel, string name, bool isEnemy)
    {
        if (panel.Find(name + "(Clone)"))
        {
            GameObject temp = panel.Find(name + "(Clone)").gameObject;
            Destroy(temp.gameObject);
        }
    }

    void CheckPassiveTurn(Unit user, BattleHud userHud, Player target)
    {
        foreach (Passives a in user.passives.ToArray())
        {
            switch (a.name)
            {
                case "boss":
                    if (a.stacks == 0)
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                        a.stacks++;
                    }
                    break;

                case "musicup":
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
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }

                    if (a.inCd == 1)
                        isReady = true;

                    if (a.stacks < a.maxStacks)
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "manathirst":
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

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "wildinstinct":
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
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }
                    else if (hpPer > (a.num * 100))
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    break;

                case "vendetta":
                    if (a.stacks == 1)
                    {
                        if (a.inCd > 0)
                            a.inCd--;

                        isReady = false;
                        if (a.inCd == 0)
                            isReady = true;

                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    }
                    break;

                case "lastbreath":
                    //hp in %
                    hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                    if (hpPer < (a.num * 100))
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }
                    else if (hpPer > (a.num * 100))
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    break;

                case "bullsrage":
                    //hp in %
                    hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                    if (hpPer <= (a.num * 100))
                    {
                        if (a.stacks < a.maxStacks + 1)
                        {
                            user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            a.stacks = a.maxStacks + 1;
                        }
                    }
                    else if (a.stacks > a.maxStacks && hpPer > (a.num * 100))
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
                            a.inCd = a.cd + 1;
                        }
                        else
                        {
                            statMod.inTime = 1;
                        }

                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }

                    isReady = false;
                    if (a.inCd > 0 || a.inCd == -1)
                        isReady = true;

                    if (isReady || a.stacks > a.maxStacks)
                    {
                        if (a.stacks > a.maxStacks)
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                        else
                        {
                            a.inCd--;
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                        }
                    }
                    else
                    {
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }

                    if (a.inCd == 0 && a.stacks == a.maxStacks)
                    {
                        a.stacks = 0;
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    } else if (a.stacks == a.maxStacks + 1)
                    {
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo(), true);
                    }
                    break;

                case "magicremains":
                    if (a.inCd > 0)
                        a.inCd--;

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "galeglide":
                    if (a.inCd > 0)
                        a.inCd--;

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "fearsmell":
                    //enemy sanity in %
                    int sanityPerU1 = (int)((100 * target.unit1.curSanity) / target.unit1.SetModifiers().sanity);
                    int sanityPerU2 = (int)((100 * target.unit2.curSanity) / target.unit2.SetModifiers().sanity);
                    int sanityPerU3 = (int)((100 * target.unit3.curSanity) / target.unit3.SetModifiers().sanity);

                    if ((sanityPerU1 < (a.num * 100)) || (sanityPerU2 < (a.num * 100)) || (sanityPerU3 < (a.num * 100)))
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = 1;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    }

                    bool foundEffect = false;

                    List<Effects> effects = new List<Effects>();
                    effects.AddRange(target.unit1.effects);
                    effects.AddRange(target.unit2.effects);
                    effects.AddRange(target.unit3.effects);

                    foreach (Effects b in effects)
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
                                ManagePassiveIcon(user.effectHud, a.sprite, a.name, "!", user.isEnemy, a.GetPassiveInfo());
                            }
                        }
                    }

                    if (!foundEffect)
                        a.stacks = 0;

                    if (((sanityPerU1 < (a.num * 100)) || (sanityPerU2 < (a.num * 100)) || (sanityPerU3 < (a.num * 100))) && !foundEffect)
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);

                    break;

                case "phantomhand":
                    if (a.inCd > 0)
                        a.inCd--;

                    if (a.inCd <= 0)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "bloodpath":
                    foundEffect = false;
                    effects = new List<Effects>();
                    effects.AddRange(target.unit1.effects);
                    effects.AddRange(target.unit2.effects);
                    effects.AddRange(target.unit3.effects);

                    foreach (Effects b in effects)
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
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                        }
                    }

                    if (!foundEffect && a.stacks > 0)
                    {
                        a.stacks = 0;
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    break;

                case "plasmablade":
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
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }

                    if (!enoughMana || isSilence)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
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
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "!", user.isEnemy, a.GetPassiveInfo(), true);
                    }
                    break;

                case "weakbody":
                    foundEffect = false;

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
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());
                        }
                    }

                    //if dont find, delete icon
                    if (!foundEffect && a.stacks > 0)
                    {
                        a.stacks = 0;
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    break;

                case "onewiththeshadows":
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
                        isReady = false;
                        if (a.stacks == 1)
                            isReady = true;
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    }
                    else
                    {
                        a.stacks = 0;
                    }
                    break;

                case "courage":
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

                    isReady = false;
                    if (a.inCd == 1)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "bloodpumping":
                    if (a.inCd > 0 && user.curHp < user.SetModifiers().hp)
                    {
                        foundEffect = false;

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
                        }
                        else
                        {
                            float heal = a.statScale.SetScale(user.SetModifiers(), user);

                            if ((user.curHp + heal) < user.SetModifiers().hp)
                            {
                                heal += heal * user.SetModifiers().healBonus;
                                user.curHp += heal;
                                user.healDone += heal;
                            }
                            else
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
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "huntingseason":
                    if (a.inCd > 0)
                        a.inCd--;

                    if (a.inCd <= 0)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "blazingfists":
                    if (a.inCd > 0)
                        a.inCd--;

                    if (a.inCd <= 0)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "fighterinstinct":
                    //hp in %
                    hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                    if (hpPer <= (a.num * 100) && a.stacks != 1)
                    {
                        a.stacks = 1;
                    }
                    else if (hpPer > (a.num * 100))
                    {
                        a.stacks = 0;
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }

                    if (a.stacks == 1)
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                    }
                    break;

                case "successoroffire":
                    foundEffect = false;
                    effects = new List<Effects>();
                    effects.AddRange(target.unit1.effects);
                    effects.AddRange(target.unit2.effects);
                    effects.AddRange(target.unit3.effects);
                    foreach (Effects b in effects)
                    {
                        if (b.id == "BRN" || b.id == "SCH")
                        {
                            foundEffect = true;
                        }
                    }

                    if (foundEffect)
                    {
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, 0.ToString(), user.isEnemy, a.GetPassiveInfo());

                        Stats statsUser = user.SetModifiers();

                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        statMod.atkDmg = statsUser.magicPower * a.num;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }
                    else
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    break;

                case "zenmode":
                    //stamina in %
                    int staPer = (int)((100 * user.curStamina) / user.SetModifiers().stamina);

                    if (staPer <= a.num * 100)
                    {
                        if (a.stacks != 1)
                        {
                            user.PassivePopup(langmanag.GetInfo("passive", "name", a.name));
                            a.stacks++;
                        }
                    }
                    else if (staPer >= a.maxNum * 100 && a.stacks == 1)
                    {
                        a.stacks = 0;
                    }

                    if (a.stacks == 1)
                    {
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo());
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }
                    else
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    break;

                case "manasword":
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

                    isReady = false;
                    if (a.stacks == a.maxStacks)
                        isReady = true;
                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "manascepter":
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

                    isReady = false;
                    if (a.stacks == a.maxStacks)
                        isReady = true;
                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.stacks.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "spectralring":
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
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }

                    if (a.stacks < a.maxStacks)
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "shadowdagger":
                    if (a.inCd > 0)
                        a.inCd--;

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "toxicteeth":
                    if (a.inCd > 0)
                        a.inCd--;

                    if (a.inCd <= 0)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "gravitybelt":
                    if (a.inCd > 0)
                        a.inCd--;

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    break;

                case "huntersdirk":
                    if (a.inCd > 0)
                        a.inCd--;

                    isReady = false;
                    if (a.inCd == 0)
                        isReady = true;

                    float hpPerF1 = ((100 * target.unit1.curHp) / target.unit1.SetModifiers().hp)*100;
                    float hpPerF2 = ((100 * target.unit2.curHp) / target.unit2.SetModifiers().hp)*100;
                    float hpPerF3 = ((100 * target.unit3.curHp) / target.unit3.SetModifiers().hp)*100;

                    if ((hpPerF1 < a.num) && (hpPerF2 < a.num) && (hpPerF3 < a.num))
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                    else
                    {
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), isReady);
                    }
                    break;

                case "gravitychange":
                    if (a.inCd > 0)
                        a.inCd--;

                    if (a.inCd <= 0)
                    {
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }

                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "combatrepair":
                    if (a.stacks > 0)
                    {
                        StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }

                    if (a.stacks <= 0)
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    else
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;

                case "mechashield":
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
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    }
                    else
                    {
                        DMG dmg = default;
                        dmg.Reset();

                        dmg.magicDmg = a.statScale2.SetScaleFlat(user.SetModifiers(), user);
                        dmg = user.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0);
                        user.TakeDamage(dmg, false, false, user);

                        userHud.SetStatsHud(user);
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                        user.passives.Remove(a);
                    }
                    break;

                case "funchase":
                    foundEffect = false;
                    effects = new List<Effects>();
                    effects.AddRange(target.unit1.effects);
                    effects.AddRange(target.unit2.effects);
                    effects.AddRange(target.unit3.effects);

                    foreach (Effects b in effects)
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
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                        }
                        break;
                    }

                    if (!foundEffect && a.stacks > 0)
                    {
                        a.stacks = 0;
                        DestroyPassiveIcon(user.effectHud, a.name, user.isEnemy);
                    }
                break;
                case "spectralpike":
                    int hpPercentage = (int)(100 * user.curHp / user.SetModifiers().hp);

                    if (hpPercentage <= a.maxNum * 100)
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }

                    if (a.inCd > 0)
                        a.inCd--;
                    else if (a.inCd == 0)
                        a.inCd = a.cd;

                    if (a.inCd == 0)
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo(), true);
                    else
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, a.inCd.ToString(), user.isEnemy, a.GetPassiveInfo());
                    break;
                case "spectralcloak":
                    //hp in %
                    hpPer = (int)((100 * user.curHp) / user.SetModifiers().hp);

                    isReady = false;

                    if (hpPer <= (a.num * 100))
                    {
                        StatMod statMod = a.statMod.ReturnStats();
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                        isReady = true;
                    }

                    float temp = a.statScale.SetScaleFlat(user.SetModifiers(), user) * a.stacks;
                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, temp.ToString("0"), user.isEnemy, a.GetPassiveInfo(), isReady);
                    
                break;
                case "magicbody":
                    if (true)
                    {
                        Stats statsUser = user.SetModifiers();

                        StatMod statMod = a.statMod.ReturnStatsTimes(a.stacks);
                        statMod.inTime = statMod.time;
                        user.statMods.Add(statMod);

                        statsUser = user.SetModifiers();
                        statMod = a.statMod2.ReturnStats();
                        statMod.inTime = statMod.time;
                        temp = statsUser.mana * a.num;
                        statMod.hp = temp;

                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, temp.ToString("0"), user.isEnemy, a.GetPassiveInfo());

                        user.statMods.Add(statMod);
                        user.usedBonusStuff = false;
                        userHud.SetStatsHud(user);
                    }
                    break;
                case "leafbeing":
                    foundEffect = false;

                    foreach (Effects b in target.unit1.effects)
                    {
                        if (b.id == "ALR")
                        {
                            foundEffect = true;
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo(), true);
                            DMG dmg = default;
                            dmg.Reset();

                            dmg.magicDmg = a.statScale.SetScaleFlat(user.SetModifiers(), user);
                            dmg = target.unit1.MitigateDmg(dmg, dmgResisPer, magicResisPer, user.SetModifiers().armourPen, user.SetModifiers().magicPen);
                            target.unit1.TakeDamage(dmg, false, false, user);

                            userHud.SetStatsHud(user);
                        }
                    }

                    foreach (Effects b in target.unit2.effects)
                    {
                        if (b.id == "ALR")
                        {
                            foundEffect = true;
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo(), true);
                            DMG dmg = default;
                            dmg.Reset();

                            dmg.magicDmg = a.statScale.SetScaleFlat(user.SetModifiers(), user);
                            dmg = target.unit2.MitigateDmg(dmg, dmgResisPer, magicResisPer, user.SetModifiers().armourPen, user.SetModifiers().magicPen);
                            target.unit2.TakeDamage(dmg, false, false, user);

                            userHud.SetStatsHud(user);
                        }
                    }

                    foreach (Effects b in target.unit3.effects)
                    {
                        if (b.id == "ALR")
                        {
                            foundEffect = true;
                            ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo(), true);
                            DMG dmg = default;
                            dmg.Reset();

                            dmg.magicDmg = a.statScale.SetScaleFlat(user.SetModifiers(), user);
                            dmg = target.unit3.MitigateDmg(dmg, dmgResisPer, magicResisPer, user.SetModifiers().armourPen, user.SetModifiers().magicPen);
                            target.unit3.TakeDamage(dmg, false, false, user);

                            userHud.SetStatsHud(user);
                        }
                    }

                    if (!foundEffect)
                        ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    
                    break;

                case "roughskin":
                case "magicwand":
                case "crossbow":
                case "bandofendurance":
                case "mythicearrings":
                case "thickarmour":
                case "combatrythm":
                case "sackofbones":
                case "firescales":
                case "ecolocation":
                case "strongmind":
                case "dreadofthesupernatural":
                    ManagePassiveIcon(user.effectHud, a.sprite, a.name, "", user.isEnemy, a.GetPassiveInfo());
                    break;
            }
        }
    }

    void ApplyTired(Unit user, GameObject pannel)
    {
        if (user.effects.Any(a => a.id == "TRD")) 
            return;

        //get effect
        Effects effect;
        if (user.level >= levelToConsiderWeak)
        {
            effect = tiredm.effect.ReturnEffect();
            effect.duration = Random.Range(tiredm.durationMin, tiredm.durationMax);
        }
        else
        {
            effect = tiredw.effect.ReturnEffect();
            effect.duration = Random.Range(tiredw.durationMin, tiredw.durationMax);
        }

        //add effect to the player
        user.effects.Add(effect);

        //apply stat mod
        foreach (StatMod b in effect.statMods)
        {
            //get statmod
            StatMod statMod = b.ReturnStats();
            statMod.inTime = effect.duration+1;

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

    void ApplyFear(Unit user, GameObject pannel)
    {
        if (user.effects.Any(a => a.id == "FEA"))
            return;
       
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

    public bool SummonDmg(SumMove move, StatsSummon statsSum, Unit.SummonTarget target, Unit summoner)
    {
        Stats statsT;
        Stats statsS = summoner.SetModifiers();

        bool targetEnemy = true;
        bool isDead = false;
        bool isCrit = Random.Range(0f, 1f) <= statsS.critChance;
        
        DMG dmgT = default;
        dmgT.Reset();
        DMG dmgS = default;
        dmgT.Reset();

        dmgT.sanityDmg += move.sanityDmg;

        switch (move.dmgType)
        {
            case DmgType.PHYSICAL:
                statsT = target.target.SetModifiers();
                dmgT.phyDmg += move.getDmg(statsSum);
                //target.phyDmgTaken += dmg;
                break;
            case DmgType.MAGICAL:
                statsT = target.target.SetModifiers();
                dmgT.magicDmg += move.getDmg(statsSum);
                //target.magicDmgTaken += dmg;
                break;
            case DmgType.TRUE:
                statsT = target.target.SetModifiers();
                dmgT.trueDmg += move.getDmg(statsSum);
                //target.trueDmgTaken += dmg;
                break;
            case DmgType.HEAL:
                targetEnemy = false;
                statsT = target.ally.SetModifiers();
                dmgS.heal += move.getDmg(statsSum);
                break;
            case DmgType.SHIELD:
                targetEnemy = false;
                statsT = target.ally.SetModifiers();
                dmgS.shield += move.getDmg(statsSum);
                //summoner.shieldDone += dmg;
                break;
        }

        if (dmgT.magicDmg > 0 || dmgT.phyDmg > 0)
        {
            //summoner.magicDmgDealt += dmg.magicDmg;
            //summoner.phyDmgDealt += dmg.phyDmg;

            if (isCrit)
                dmgT.ApplyCrit(false, statsS.critDmg);

            if (targetEnemy)
                dmgT = target.target.MitigateDmg(dmgT, dmgResisPer, magicResisPer, statsS.armourPen, statsS.magicPen, summoner);
            else
                dmgT = target.ally.MitigateDmg(dmgT, dmgResisPer, magicResisPer, statsS.armourPen, statsS.magicPen, summoner);
        }

        if (targetEnemy)
        {
            isDead = target.target.TakeDamage(dmgT, isCrit, false, summoner);
            SetUltNumber(target.target, target.target.hud, dmgT.phyDmg + dmgT.magicDmg + dmgT.trueDmg, false);
        }
        else
        {
            isDead = target.ally.TakeDamage(dmgT, isCrit, false, summoner);
            SetUltNumber(target.ally, target.ally.hud, dmgT.phyDmg + dmgT.magicDmg + dmgT.trueDmg, false);
        }
        
        summoner.TakeDamage(dmgS, isCrit, false, summoner);

        return isDead;
    }

    bool SpawnSummon(Summon sum, Unit summoner, Unit.SummonTarget target)
    {
        if (summoner.isDead)
        {
            sum.stats.hp -= summonHpLostBase + (sum.stats.hp * summonHpLostPer);
        }

        if (sum.summonTurn == 0)
        {
            Stats statsSum = summoner.SetModifiers();

            sum.SetupStats(statsSum, summoner);
            sum.summonTurn = turnCount;
            string name = sum.name + sum.summonTurn;

            if (!summoner.effectHud.transform.Find(name + "(Clone)"))
            {
                barIconPrefab.name = name;
                Image icon = barIconPrefab.transform.Find("icon").gameObject.GetComponent<Image>();
                icon.sprite = sum.icon;
                Text text = barIconPrefab.transform.Find("time").gameObject.GetComponent<Text>();
                text.text = sum.stats.hp.ToString("0");
                TooltipButton tooltipButton = barIconPrefab.transform.GetComponent<TooltipButton>();
                tooltipButton.tooltipPopup = tooltipMain.transform.GetComponent<TooltipPopUp>();

                sum.SetIconCombat(Instantiate(barIconPrefab, summoner.effectHud.transform));
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
                summoner.effectHud.transform.Find(debugname + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = sum.stats.hp.ToString();
                Debug.Log("CD: " + sum.move.inCd);
                if (sum.move.inCd <= 0)
                {
                    dialogText.text = langmanag.GetInfo("gui", "text", "usedmove", name, langmanag.GetInfo("summon", sum.GetMoveTypeLangId()));
                    sum.move.inCd = sum.move.cd;
                    return SummonDmg(sum.move, sum.stats, target, summoner);
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
                Destroy(summoner.effectHud.transform.Find(debugname + "(Clone)").gameObject);
                summoner.summons.Remove(sum);
            }
        }

        return false;
    }

    IEnumerator NewTurn()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ManageEndTurn(player.unit1, enemy));
        StartCoroutine(ManageEndTurn(player.unit2, enemy));
        StartCoroutine(ManageEndTurn(player.unit3, enemy));
        StartCoroutine(ManageEndTurn(enemy.unit1, player));
        StartCoroutine(ManageEndTurn(enemy.unit2, player));
        StartCoroutine(ManageEndTurn(enemy.unit3, player));
        yield return new WaitForSeconds(0.6f);

        //set turn number
        combatCount = 0;
        turnCount++;
        turnsText.text = langmanag.GetInfo("gui", "text", "turn", turnCount);
        turnsTextOverview.text = langmanag.GetInfo("gui", "text", "turn", turnCount);

        //change needed stamina to be tired (increases with the number of turns)
        if (turnCount > 25 && turnCount%10 == 0 && tiredStacks < 10)
        {
            tiredStacks++;
        }
        
        if (!overviewBtn.interactable)
            overviewBtn.interactable = true;

        player.ResetHasAttacked();
        enemy.ResetHasAttacked();

        //Some optimization needed
        UpdateTooltips(enemy.unit1);
        UpdateTooltips(enemy.unit2);
        UpdateTooltips(enemy.unit3);
        UpdateTooltips(player.unit1);
        UpdateTooltips(player.unit2);
        UpdateTooltips(player.unit3);
        // ^

        UpdateSummonTooltip(enemy.unit1);
        UpdateSummonTooltip(enemy.unit2);
        UpdateSummonTooltip(enemy.unit3);
        UpdateSummonTooltip(player.unit1);
        UpdateSummonTooltip(player.unit2);
        UpdateSummonTooltip(player.unit3);

        PlayerTurn();
    } 

    IEnumerator ManageEndTurn(Unit unit, Player enemy)
    {
        unit.chosenMove.move = null;
        unit.chosenMove.target = null;

        bool CanTired = true;

        foreach (Passives a in unit.passives.ToArray())
        {
            if (a.name == "zenmode")
                CanTired = false;
        }

        Stats stats = unit.SetModifiers();

        foreach (Summon a in unit.summons.ToArray())
        {
            bool isDead = SpawnSummon(a, unit, unit.summonTarget);
            yield return new WaitForSeconds(0.5f);

            if (isDead)
            {
                if (unit.isEnemy)
                {
                    player.SetAsDead(unit);
                    state = BattleState.ALLYKILLED;
                }
                else
                {
                    this.enemy.SetAsDead(unit);
                    state = BattleState.ENEMYKILLED;
                }
            }
        }

        if (!unit.isDead)
        {
            //apply tired
            if (unit.curStamina <= (int)(stats.stamina * (tiredStart + (tiredGrowth * tiredStacks))) && CanTired)
            {
                ApplyTired(unit, unit.effectHud.gameObject);
            }

            unit.ResetCanUse();
            if (unit.CountEffectTimer(unit.effectHud.gameObject, bloodLossStacks, dmgResisPer, magicResisPer, dotReduc))
            {
                if (unit.isEnemy)
                {
                    player.SetAsDead(unit);
                    state = BattleState.ALLYKILLED;
                }
                else
                {
                    this.enemy.SetAsDead(unit);
                    state = BattleState.ENEMYKILLED;
                }
            }

            foreach (Dotdmg a in unit.dotDmg.ToArray())
            {
                a.inTime--;

                bool isDead = DotCalc(a, unit);
                yield return new WaitForSeconds(0.6f);

                if (isDead)
                {
                    if (unit.isEnemy)
                    {
                        player.SetAsDead(unit);
                        state = BattleState.ALLYKILLED;
                    }
                    else
                    {
                        this.enemy.SetAsDead(unit);
                        state = BattleState.ENEMYKILLED;
                    }
                }

                if (a.inTime <= 0)
                {
                    unit.dotDmg.Remove(a);

                    /*GameObject temp = panelEffectsP.transform.Find(a.id + "(Clone)").gameObject;

                    Destroy(temp.gameObject);*/
                }
            }
            SetStatus();

        }
        CheckVictory();

        if (!unit.isDead)
        {
            //apply fear
            if (unit.curSanity <= 0)
            {
                ApplyFear(unit, unit.effectHud.gameObject);
            }

            if (unit.recoverMana.inCooldown > 0)
                unit.recoverMana.inCooldown--;

            if (unit.moves.Count > 0)
            {
                int i = 0;
                foreach (Moves move in unit.moves.ToArray())
                {
                    i++;
                    if (move.uses >= 0 && !unit.isEnemy)
                    {
                        foreach (Transform movebtn in unit.moveListPanel.transform)
                        {
                            Text id = movebtn.Find("Id").gameObject.GetComponent<Text>();
                            if (id.text == i.ToString())
                            {
                                if (move.uses == 0)
                                {
                                    Destroy(movebtn.gameObject);
                                    unit.moves.Remove(move);
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

            if (unit.statMods.Count > 0)
                foreach (StatMod statMod in unit.statMods.ToArray())
                {
                    if (statMod.inTime > 0)
                        statMod.inTime--;

                    if (statMod.inTime == 0)
                        unit.statMods.Remove(statMod);
                }

            unit.hud.SetStatsHud(unit);

            CheckPassiveTurn(unit, unit.hud, enemy);

            stats = unit.SetModifiers();

            if (turnCount > 1)
            {
                unit.DoAnimParticle("heal");
                unit.Heal(stats.hpRegen * (1 + stats.healBonus));
                if (!(unit.curHp + (stats.hpRegen * (1 + stats.healBonus)) >= unit.SetModifiers().hp))
                    unit.healDone += stats.hpRegen * (1 + stats.healBonus);
                else
                    unit.healDone += unit.SetModifiers().hp - unit.curHp;

                if (unit.curMana < stats.mana)
                    if ((unit.curMana + stats.manaRegen) > stats.mana)
                    {
                        unit.curMana = stats.mana;
                        unit.manaHealDone += stats.mana - (unit.curMana + stats.manaRegen);
                    }
                    else
                    {
                        unit.curMana += stats.manaRegen;
                        unit.manaHealDone += stats.manaRegen;
                    }

                if (unit.curStamina < stats.stamina)
                    if ((unit.curStamina + stats.staminaRegen) > stats.stamina)
                    {
                        unit.curStamina = stats.stamina - (unit.curStamina + stats.staminaRegen);
                        unit.staminaHealDone += stats.stamina - (unit.curStamina + stats.staminaRegen);
                    }
                    else
                    {
                        unit.curStamina += stats.staminaRegen;
                        unit.staminaHealDone += stats.staminaRegen;
                    }
            }
        }
        unit.isBlockingPhysical = false;
        unit.isBlockingMagical = false;
        unit.isBlockingRanged = false;

        yield return new WaitForSeconds(0.9f);

        foreach (Effects a in unit.effects)
        {
            if (!a.canUseMagic && !a.canUsePhysical && !a.canUseRanged && !a.canUseEnchant && !a.canUseSupp && !a.canUseProtec && !a.canUseSummon)
                unit.SetSkipTurn(true);
        }

        //apply item buffs at the start
        if (turnCount == 1)
        {
            foreach (Items item in unit.items)
            {
                foreach (StatMod mod in item.statmod)
                {
                    unit.statMods.Add(mod.ReturnStats());
                }
            }
        }

        SetStatus();
        unit.hud.SetStatsHud(unit);

        sumPlayerHud.UpdateValues(unit, langmanag.GetInfo("charc", "name", unit.charc.name));
    }

    void UpdateTooltips(Unit unit)
    {
        tooltipMain.GetComponent<TooltipPopUp>().ResetLastBtn();
        tooltipSec.GetComponent<TooltipPopUp>().ResetLastBtn();

        if (!unit.isEnemy)
        {
            foreach (Transform child in unit.moveListPanel.transform.GetChild(0).transform)
            {
                int id = child.GetComponent<BtnMoveSetup>().GetId();
                if (id >= 1 && id <= unit.moves.Count)
                {
                    Moves a = unit.moves.Find(x => x.id == id);
                    child.GetComponent<BtnMoveSetup>().UpdateToolTip(a.GetTooltipText(false), a.GetTooltipText(true));
                }
            }

            actionBox1p.UpdateTooltips();
            actionBox2p.UpdateTooltips();
            actionBox3p.UpdateTooltips();
        }

        //if (!unit.canUsePhysical)
        //    phyCancel.SetActive(true);
        //else
        //    phyCancel.SetActive(false);

        //if (!unit.canUseMagic)
        //    magiCancel.SetActive(true);
        //else
        //    magiCancel.SetActive(false);

        //if (!unit.canUseRanged)
        //    rangeCancel.SetActive(true);
        //else
        //    rangeCancel.SetActive(false);

        //if (!unit.canUseEnchant)
        //    statCancel.SetActive(true);
        //else
        //    statCancel.SetActive(false);

        //if (!unit.canUseSupp)
        //    suppCancel.SetActive(true);
        //else
        //    suppCancel.SetActive(false);

        //if (!unit.canUseProtec)
        //    defCancel.SetActive(true);
        //else
        //    defCancel.SetActive(false);

        //if (!unit.canUseSummon)
        //    summCancel.SetActive(true);
        //else
        //    summCancel.SetActive(false);

        UpdateSummonTooltip(unit);
    }

    void UpdateSummonTooltip(Unit unit)
    {
        foreach(Summon sum in unit.summons)
        {
            sum.UpdateInfoCombat();
        }
    }

    void CheckVictory()
    {
        bool enemyS = enemy.HaveLost();
        bool playerS = player.HaveLost();

        if (enemyS && playerS)
            state = BattleState.TIE;
        else if (playerS)
            state = BattleState.LOSE;
        else if (enemyS)
            state = BattleState.WIN;

        if (state == BattleState.TIE || state == BattleState.LOSE || state == BattleState.WIN) 
            StartCoroutine(EndBattle());
    }
}