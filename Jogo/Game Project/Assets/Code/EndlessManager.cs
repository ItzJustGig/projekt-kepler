using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndlessManager : MonoBehaviour
{
    [SerializeField] private Card prevCard;
    [SerializeField] private Card nextCard;
    [SerializeField] private EndlessInfo info;
    [SerializeField] private Button startBtn;
    [SerializeField] private Text goldTxt;
    [SerializeField] private Text roundTxt;

    [SerializeField] private Text nameChampTxt;
    [SerializeField] private Text titleChampTxt;
    [SerializeField] private Image champIcon;

    [SerializeField] private EndlessBattleHud battleHud;

    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string isPlayerChamp = "isPlayerChamp";
    private readonly string isEnemyChamp = "isEnemyChamp";
    private readonly string isEnemyBoss = "isEnemyBoss";
    private EndlessLanguageManager langmanag;

    public List<Character> characters = new List<Character>();
    public List<Character> monsters = new List<Character>();

    public List<Encounters> encounters = new List<Encounters>();

    [SerializeField] private List<BonusGold> bonusGold = new List<BonusGold>();
    [SerializeField] private BonusGold bonusGoldBoss;

    [SerializeField] private TooltipButton hpInfo;
    [SerializeField] private TooltipButton manaInfo;
    [SerializeField] private TooltipButton staminaInfo;
    [SerializeField] private TooltipButton sanityInfo;
    [SerializeField] private TooltipButton ultInfo;

    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;

    private SceneLoader loader;

    int enemyId;
    Character.Strenght strenght;
    bool isBoss;
    [SerializeField] GameObject[] iconsArray;

    void Awake()
    {
        langmanag = this.gameObject.GetComponent<EndlessLanguageManager>();

        info.Load();
        EndlessInfo data = new EndlessInfo();

        data.wonLastRound = info.wonLastRound;
        data.round = info.round;
        int roundTemp = data.round;

        if (data.round > -1)
        {
            if (PlayerPrefs.HasKey("wonLastRound"))
                data.wonLastRound = PlayerPrefs.GetInt("wonLastRound");
            else
                data.wonLastRound = -1;
        } else
        {
            data.round++;
        }

        if (data.wonLastRound == 1)
        {
            if (info.enemyIdNext != -1)
            {
                int gold = 5;
                int tempId = info.enemyIdNext;
                bool tempIsBoss = info.isEnemyBossNext;
                Character.Strenght tempStrenght;

                if (info.isEnemyChampNext)
                    tempStrenght = Character.Strenght.CHAMPION;
                else
                    tempStrenght = Character.Strenght.BABY;

                if (!(tempStrenght is Character.Strenght.CHAMPION))
                    tempStrenght = monsters[tempId].strenght;

                foreach (BonusGold a in bonusGold)
                {
                    if (a.id == tempStrenght)
                        gold += Random.Range(a.minGold, a.maxGold);
                }

                if (tempIsBoss)
                    gold += Random.Range(bonusGoldBoss.minGold, bonusGoldBoss.maxGold);

                info.gold += gold;
            }

            GenEnemy(info.round);
            info.itemShop.Clear();
            info.generateShop = true;
        }
        else
        {
            enemyId = info.enemyIdNext;
            isBoss = info.isEnemyBossNext;

            if (info.isEnemyChampNext)
                strenght = Character.Strenght.CHAMPION;
            else
                strenght = Character.Strenght.BABY;
        }

        if (isBoss)
            nextCard.Boss();

        if (strenght is Character.Strenght.CHAMPION)
        {
            nextCard.charc = characters[enemyId];
            data.isEnemyBossNext = isBoss;
            data.isEnemyChampNext = true;
            data.enemyIdNext = enemyId;
        }
        else
        {
            nextCard.charc = monsters[enemyId];
            data.isEnemyBossNext = isBoss;
            data.isEnemyChampNext = false;
            data.enemyIdNext = enemyId;
        }

        if (info.enemyIdPrev <= -1)
        {
            prevCard.charc = monsters[0];
            data.enemyIdPrev = 0;
        }
        else
        {
            if (info.isEnemyChampNext)
            {
                prevCard.charc = characters[info.enemyIdNext];
            }
            else
            {
                prevCard.charc = monsters[info.enemyIdNext];
            }
            data.isEnemyChampPrev = info.isEnemyChampNext;
            data.enemyIdPrev = info.enemyIdNext;
            data.isEnemyBossPrev = info.isEnemyBossNext;
        }

        data.isPlayerChamp = PlayerPrefs.GetInt(isPlayerChamp) != 0;

        if (info.playerId == -1)
            data.playerId = PlayerPrefs.GetInt(selectedCharacter);
        else
            data.playerId = info.playerId;

        data.playerHp = info.playerHp;
        data.playerMn = info.playerMn;
        data.playerSta = info.playerSta;
        data.playerSan = info.playerSan;
        data.playerUlt = info.playerUlt;
        data.gold = info.gold;
        data.generateShop = info.generateShop;

        if (roundTemp == -1)
            data.wonLastRound = -1;

        PlayerPrefs.DeleteKey("wonLastRound");

        SaveSystem.Save(data);
        info.Load();
    }

    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        Character charc;
        if (info.isPlayerChamp)
        {
            charc = characters[info.playerId-1];
        }
        else
        {
            charc = monsters[info.playerId-1];
        }
        battleHud.SetHud(charc, info, 0);

        goldTxt.text = info.gold.ToString();
        roundTxt.text = info.round.ToString();

        nameChampTxt.text = langmanag.GetInfo("charc", "name", characters[info.playerId-1].name);
        champIcon.sprite = characters[info.playerId-1].charcIcon;
        titleChampTxt.text = langmanag.GetInfo("charc", "title", characters[info.playerId - 1].name);

        if (info.wonLastRound == 1)
        {
            prevCard.gameObject.GetComponent<Animator>().SetTrigger("lose");
            info.wonLastRound = -1;
        }
        else if (info.wonLastRound == 0)
        {
            Destroy(nextCard.gameObject);
            prevCard.gameObject.GetComponent<Animator>().SetTrigger("win");
            startBtn.interactable = false;
            info.DeleteNoSceneChange();
            info.Load();
        } else
        {
            Destroy(prevCard.gameObject);
        }

        SaveSystem.Save(info);

        hpInfo.text = langmanag.GetInfo("stats", "name", "hp");
        manaInfo.text = langmanag.GetInfo("stats", "name", "mana");
        staminaInfo.text = langmanag.GetInfo("stats", "name", "stamina");
        sanityInfo.text = langmanag.GetInfo("stats", "name", "sanity");
        ultInfo.text = langmanag.GetInfo("stats", "name", "ultimate");
    }

    void HideIcons()
    {
        for (int i = 0; i < iconsArray.Length; i++)
        {
            iconsArray[i].SetActive(false);
        }
    }

    void GenEnemy(int round)
    {
        Character.Strenght selectedStre = Character.Strenght.BABY;
        
        foreach (Encounters enc in encounters){
            if (round >= enc.startRound && round <= enc.endRound)
            {
                for (int i = 0; i < enc.strenghts.Count; i++)
                {
                    if (Random.Range(0f, 1f) <= enc.strenghts[i].chance)
                    {
                        isBoss = enc.isBoss;
                        selectedStre = enc.strenghts[i].strenght;
                        Debug.Log(selectedStre);
                        break;
                    }
                }
            }
        }
        
        strenght = selectedStre;
        
        List<int> num = new List<int>();
        int d = 0;

        if (selectedStre != Character.Strenght.CHAMPION)
        {
            foreach (Character charc in monsters)
            {
                if (charc.strenght == selectedStre)
                {
                    num.Add(d);
                }
                d++;
            }

            enemyId = num[Random.Range(0, num.Count)];
        } else
        {
            if (info.isEnemyChampNext == true)
            {
                do
                {
                    enemyId = Random.Range(0, characters.Count);
                    StartCoroutine(WaitWhile());
                } while (enemyId == (info.playerId - 1) || enemyId == info.enemyIdPrev);
            } else
            {
                do
                {
                    enemyId = Random.Range(0, characters.Count);
                    StartCoroutine(WaitWhile());
                } while (enemyId == (info.playerId - 1));
            }
            
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideIcons();
            loader.LoadScene(0, slider, loadPanel);
        }
    }

    public void BackBtn()
    {
        SaveSystem.Save(info);
        HideIcons();
        loader.LoadScene(0, slider, loadPanel);
    }

    public void ShopBtn()
    {
        SaveSystem.Save(info);
        HideIcons();
        loader.LoadScene(5, slider, loadPanel);
    }

    public void StartBtn()
    {
        SaveSystem.Save(info);
        PlayerPrefs.SetInt(selectedCharacter, info.playerId);
        PlayerPrefs.SetInt(selectedEnemy, info.enemyIdNext+1);
        PlayerPrefs.SetInt(isPlayerChamp, System.Convert.ToInt32(info.isPlayerChamp));
        PlayerPrefs.SetInt(isEnemyChamp, System.Convert.ToInt32(info.isEnemyChampNext));
        PlayerPrefs.SetInt(isEnemyBoss, System.Convert.ToInt32(isBoss));

        HideIcons();
        loader.LoadScene(2, slider, loadPanel);
    }

    IEnumerator WaitWhile()
    {
        yield return null;
    }
}
