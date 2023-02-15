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
    [SerializeField] private Button shopBtn;
    [SerializeField] private Button passBtn;
    [SerializeField] private Text roundTxt;
    [SerializeField] private Text goldCurTxt;
    [SerializeField] private Text goldOldTxt;
    [SerializeField] private Text goldBonusTxt;
    [SerializeField] private Animator goldAnim;

    [SerializeField] private Text nameChampTxt;
    [SerializeField] private Image champIcon;

    [SerializeField] private EndlessBattleHud battleHud;

    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string isPlayerChamp = "isPlayerChamp";
    private readonly string isEnemyChamp = "isEnemyChamp";
    private readonly string isEnemyBoss = "isEnemyBoss";
    private readonly string selectedLevel = "selectedLevel";
    private readonly string selectedLevelEnemy = "selectedLevelEnemy";

    private EndlessLanguageManager langmanag;

    [SerializeField] private StuffList champions;
    [SerializeField] private StuffList monsters;
    [SerializeField] private StuffList monsEncounters;

    [SerializeField] private int shopCd;
    [SerializeField] private int baseGold;
    [SerializeField] private float bossDropsChance;
    [SerializeField] private List<BonusGold> bonusGold = new List<BonusGold>();
    [SerializeField] private BonusGold bonusGoldBoss;

    [SerializeField] private TooltipButton hpInfo;
    [SerializeField] private TooltipButton manaInfo;
    [SerializeField] private TooltipButton staminaInfo;
    [SerializeField] private TooltipButton sanityInfo;
    [SerializeField] private TooltipButton ultInfo;

    [SerializeField] GameObject dropItemBox;
    [SerializeField] GameObject deleteSaveBox;
    [SerializeField] GameObject dropItemPanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] Slider slider;
    [SerializeField] AudioSource speaker;
    [SerializeField] AudioClip lvlup;

    private SceneLoader loader;

    int enemyId;
    int enemyLevel;
    int gainedGold;
    int oldGold;
    Character.Strenght strenght;
    bool isBoss;
    [SerializeField] GameObject[] iconsArray;
    [System.Serializable]
    struct BossDrops
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField] List<BossDrops> bossDrops = new List<BossDrops>();

    void Awake()
    {
        langmanag = this.gameObject.GetComponent<EndlessLanguageManager>();

        info.Load();
        EndlessInfo data = new EndlessInfo();

        data.wonLastRound = info.wonLastRound;
        data.round = info.round;
        data.shoprerolls = info.shoprerolls;
        data.shopcoupon = info.shopcoupon;
        data.shoppass = info.shoppass;
        data.wasPassUsed = info.wasPassUsed;
        data.hasRested = info.hasRested;
        int roundTemp = data.round;

        if (data.round > -1)
        {
            if (PlayerPrefs.HasKey("wonLastRound"))
                data.wonLastRound = PlayerPrefs.GetInt("wonLastRound");
            else
                data.wonLastRound = -1;
        } else
        {
            data.round = 1;
        }

        List<Character> champs = new List<Character>();
        foreach (Character t in champions.returnStuff())
        {
            champs.Add(t.GetCharcInfo());
        }

        List<Character> mons = new List<Character>();
        foreach (Character t in monsters.returnStuff())
        {
            mons.Add(t.GetCharcInfo());
        }
        data.level = info.level;
        if (data.wonLastRound == 1)
        {
            if (info.enemyIdNext != -1)
            {
                int gold = baseGold;
                int tempId = info.enemyIdNext;
                bool tempIsBoss = info.isEnemyBossNext;
                Character.Strenght tempStrenght;

                if (info.isEnemyChampNext)
                    tempStrenght = Character.Strenght.CHAMPION;
                else
                    tempStrenght = Character.Strenght.BABY;

                if (!(tempStrenght is Character.Strenght.CHAMPION))
                    tempStrenght = mons[tempId].strenght;

                foreach (BonusGold a in bonusGold)
                {
                    if (a.id == tempStrenght)
                        gold += Random.Range(a.minGold, a.maxGold);
                }

                if (tempIsBoss)
                {
                    gold += Random.Range(bonusGoldBoss.minGold, bonusGoldBoss.maxGold);
                    if (Random.Range(0f, 1f) <= bossDropsChance)
                    {
                        int temp = Random.Range(0, bossDrops.Count);
                        dropItemBox.transform.Find("drop").Find("dropTxt").GetComponent<Text>().text = langmanag.GetInfo("items", "name", bossDrops[temp].name);
                        dropItemBox.transform.Find("drop").Find("dropIcon").GetComponent<Image>().sprite = bossDrops[temp].sprite;
                        dropItemBox.transform.Find("drop").GetComponent<TooltipButton>().text = langmanag.GetInfo("items", "purchase", bossDrops[temp].name);
                        dropItemPanel.SetActive(true);
                        dropItemBox.SetActive(true);
                        switch (bossDrops[temp].name)
                        {
                            case "shoppass":
                                data.shoppass++;
                                break;
                            case "shopcoupon":
                                data.shopcoupon++;
                                break;
                        }
                    }
                }

                oldGold = info.gold;
                info.gold += gold;
                gainedGold = gold;
            }
            GenEnemy(info.round);
            info.itemShop.Clear();
            info.generateShop = true;
            if (info.round != -1 && info.round % 3 == 0)
            {
                data.level = info.level + 1;
                PlayLevelUp();
            }
            data.isShopOpen = false;
            data.wasPassUsed = false;
        }
        else
        {
            enemyId = info.enemyIdNext;
            isBoss = info.isEnemyBossNext;
            enemyLevel = info.enemyLevelNext;

            if (info.isEnemyChampNext)
                strenght = Character.Strenght.CHAMPION;
            else
                strenght = Character.Strenght.BABY;

            data.isShopOpen = info.isShopOpen;
        }

        if (isBoss)
            nextCard.Boss();

        if (strenght is Character.Strenght.CHAMPION)
        {
            nextCard.charc = champs[enemyId];
            data.isEnemyChampNext = true;
        }
        else
        {
            nextCard.charc = mons[enemyId];
            data.isEnemyChampNext = false;
        }
        data.isEnemyBossNext = isBoss;
        data.enemyIdNext = enemyId;
        data.enemyLevelNext = enemyLevel;

        if (info.enemyIdPrev <= -1)
        {
            prevCard.charc = mons[0];
            data.enemyIdPrev = 0;
            data.enemyLevelPrev = 0;
        }
        else
        {
            if (info.isEnemyChampNext)
            {
                prevCard.charc = champs[info.enemyIdNext];
            }
            else
            {
                prevCard.charc = mons[info.enemyIdNext];
            }
            data.isEnemyChampPrev = info.isEnemyChampNext;
            data.enemyIdPrev = info.enemyIdNext;
            data.enemyLevelPrev = info.enemyLevelNext;
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
        data.gold = info.gold;
        data.generateShop = info.generateShop;

        if (roundTemp == -1)
            data.wonLastRound = -1;

        PlayerPrefs.DeleteKey("wonLastRound");

        SaveSystem.Save(data);
        info.Load();
        prevCard.level = info.enemyLevelPrev;
        nextCard.level = info.enemyLevelNext;
        Debug.Log("AAAA " + info.level);
    }

    void Start()
    {
        gameObject.AddComponent<SceneLoader>();
        loader = gameObject.GetComponent<SceneLoader>();

        List<Character> champs = new List<Character>();
        foreach (Character t in champions.returnStuff())
        {
            champs.Add(t.GetCharcInfo());
        }

        List<Character> mons = new List<Character>();
        foreach (Character t in monsters.returnStuff())
        {
            mons.Add(t.GetCharcInfo());
        }

        battleHud.SetHud(info);

        goldCurTxt.text = info.gold.ToString();
        goldOldTxt.text = oldGold.ToString();
        goldBonusTxt.text = "+" + gainedGold.ToString();
        roundTxt.text = info.round.ToString();

        if (info.round % shopCd == 0)
        {
            info.isShopOpen = true;
        } else if (!info.isShopOpen && info.shoppass > 0)
        {
            passBtn.interactable = true;
        }

        if (info.wonLastRound == 1)
            goldAnim.SetTrigger("g");

        nameChampTxt.text = langmanag.GetInfo("charc", "name", champs[info.playerId-1].name);
        champIcon.sprite = champs[info.playerId-1].charcIcon;

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
            info.isShopOpen = false;
            info.DeleteNoSceneChange();
            info.Load();
        } else
        {
            Destroy(prevCard.gameObject);
        }

        SaveSystem.Save(info);

        if (info.isShopOpen)
            shopBtn.interactable = true;
        else
            shopBtn.interactable = false;

        hpInfo.text = langmanag.GetInfo("stats", "name", "hp");
        manaInfo.text = langmanag.GetInfo("stats", "name", "mana");
        staminaInfo.text = langmanag.GetInfo("stats", "name", "stamina");
        sanityInfo.text = langmanag.GetInfo("stats", "name", "sanity");
        ultInfo.text = langmanag.GetInfo("stats", "name", "ultimate");
    }

    void PlayLevelUp()
    {
        if (PlayerPrefs.HasKey("volume") && PlayerPrefs.HasKey("muted"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            bool muted = System.Convert.ToBoolean(PlayerPrefs.GetInt("muted"));

            speaker.volume = volume;
            speaker.mute = muted;

            speaker.clip = lvlup;
            speaker.Play();
        }
    }

    public void OpenShop()
    {
        shopBtn.interactable = true;
        passBtn.interactable = false;
        info.shoppass--;
        info.isShopOpen = true;
        info.wasPassUsed = true;
        info.Save();
    }

    void GenEnemy(int round)
    {
        Character.Strenght selectedStre = Character.Strenght.None;
        int minLvl = 0;
        int maxLvl = 0;
        foreach (Encounters enc in monsEncounters.returnStuff())
        {
            if (round >= enc.startRound && round <= enc.endRound)
            {
                do
                {
                    float rng = Random.Range(0f, 1f);
                    float counter = 0;
                    for (int i = 0; i < enc.strenghts.Count; i++)
                    {
                        counter += enc.strenghts[i].chance;
                        if (rng <= counter)
                        {
                            isBoss = enc.isBoss;
                            selectedStre = enc.strenghts[i].strenght;
                            minLvl = enc.strenghts[i].minLvl;
                            maxLvl = enc.strenghts[i].maxLvl;
                            break;
                        }
                    }

                    if (selectedStre != Character.Strenght.None)
                        break;

                } while (true);
            }
        }
        strenght = selectedStre;

        List<int> num = new List<int>();
        int d = 0;

        if (selectedStre != Character.Strenght.CHAMPION)
        {
            foreach (Character charc in monsters.returnStuff())
            {
                if (charc.strenght == selectedStre)
                {
                    num.Add(d);
                }
                d++;
            }
            
            enemyId = num[Random.Range(0, num.Count)];
        }
        else
        {
            if (info.isEnemyChampNext == true)
            {
                do
                {
                    enemyId = Random.Range(0, champions.returnStuff().Count);
                    StartCoroutine(WaitWhile());
                } while (enemyId == (info.playerId - 1) || enemyId == info.enemyIdPrev);
            } else
            {
                do
                {
                    enemyId = Random.Range(0, champions.returnStuff().Count);
                    StartCoroutine(WaitWhile());
                } while (enemyId == (info.playerId - 1));
            }
            
        }
        enemyLevel = Random.Range(minLvl, maxLvl);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //HideIcons();
            loader.LoadScene(0, slider, loadPanel);
        }
    }

    public int GetLevel()
    {
        return info.level;
    }

    public void BackBtn()
    {
        SaveSystem.Save(info);
        //HideIcons();
        loader.LoadScene(0, slider, loadPanel);
    }

    public void ShopBtn()
    {
        SaveSystem.Save(info);
        //HideIcons();
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
        PlayerPrefs.SetInt(selectedLevel, info.level);
        PlayerPrefs.SetInt(selectedLevelEnemy, info.enemyLevelNext);

        //HideIcons();
        loader.LoadScene(2, slider, loadPanel);
    }

    public void DropBossOkBtn()
    {
        dropItemPanel.SetActive(false);
        dropItemBox.SetActive(false);
    }

    public void DeleteBtn()
    {
        dropItemPanel.SetActive(true);
        deleteSaveBox.SetActive(true);
    }

    public void DeleteCancelBtn()
    {
        dropItemPanel.SetActive(false);
        deleteSaveBox.SetActive(false);
    }

    IEnumerator WaitWhile()
    {
        yield return null;
    }
}
