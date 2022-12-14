using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Unit : MonoBehaviour
{
    
    public bool isEnemy;
    public float curHp;
    public float curMana;
    public float curStamina;
    public float curShield;
    public int curSanity;
    public float ult;

    public float bonusHp;
    public float bonusMana;
    public float bonusStamina;
    public int bonusSanity;

    public bool usedBonusStuff = true;

    public bool isBlockingPhysical = false;
    public bool isBlockingMagical = false;
    public bool isBlockingRanged = false;

    public bool canUsePhysical = true;
    public bool canUseRanged = true;
    public bool canUseMagic = true;
    public bool canUseSupp = true;
    public bool canUseProtec = true;
    public bool canUseStatMod = true;
    public bool canUseSummon = true;

    public List<StatMod> statMods = new List<StatMod>();
    public List<Effects> effects = new List<Effects>();
    public List<Dotdmg> dotDmg = new List<Dotdmg>();
    public List<Summon> summons = new List<Summon>();

    public List<Moves> moves = new List<Moves>();
    public Moves ultMove;
    public Moves recoverMana;
    public Moves basicAttack;
    public List<Passives> passives = new List<Passives>();
    public List<Items> items = new List<Items>();
    public List<int> randomItems = new List<int>();

    [SerializeField] private StuffList champions;
    [SerializeField] private StuffList monsters;
    [SerializeField] private Stats statGrowth;
    [SerializeField] private Stats statsLevel;

    public Character charc;
    [SerializeField] private GameObject spriteDefault;
    [SerializeField] private GameObject dmgText;
    [SerializeField] private GameObject passiveText;
    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string playerchamp = "isPlayerChamp";
    private readonly string enemychamp = "isEnemyChamp";

    public float phyDmgDealt, magicDmgDealt, trueDmgDealt, sanityDmgDealt;
    public float healDone, shieldDone, manaHealDone, staminaHealDone, sanityHealDone;
    public float phyDmgTaken, magicDmgTaken, trueDmgTaken, sanityDmgTaken;
    public float phyDmgMitigated, magicDmgMitigated;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sprite;
    public int size = 2;

    void Awake()
    {
        int character = PlayerPrefs.GetInt(selectedCharacter);
        int bot = PlayerPrefs.GetInt(selectedEnemy);

        int isPlayerChamp = PlayerPrefs.GetInt(playerchamp);
        int isEnemyChamp = PlayerPrefs.GetInt(enemychamp);

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

        if (!isEnemy)
        {
            if (isPlayerChamp == 1)
                charc = champs[character-1];
            else
                charc = mons[character-1];
        }
        else
        {
            if (isEnemyChamp == 1)
                charc = champs[bot-1];
            else
                charc = mons[bot-1];
        }

        ultMove = charc.ultimate.ReturnMove();
        ultMove.SetOwner(this);
        ultMove.isUlt = true;
        ultMove.manaCost = 0;
        ultMove.staminaCost = 0;

        basicAttack = basicAttack.ReturnMove();
        basicAttack.SetOwner(this);

        recoverMana = recoverMana.ReturnMove();
        recoverMana.inCooldown = 5;
        recoverMana.SetOwner(this);

        foreach (Moves move in charc.moves.ToArray())
        {
            move.SetOwner(this);
            moves.Add(move.ReturnMove());
        }

        if (PlayerPrefs.GetInt("isEndless") != 0)
            charc.stats = charc.stats.ReturnStats();
        else
            charc.stats = charc.stats.ReturnStatsLevel(statsLevel, statGrowth);

        size = charc.size;
    }

    void Start()
    {
        //int character = PlayerPrefs.GetInt(selectedCharacter);
        //int bot = PlayerPrefs.GetInt(selectedEnemy);
       
        if (charc.sprite)
        {
            sprite = Instantiate(charc.sprite, this.transform) as GameObject;
            LoadSize();
            Vector3 tempCord = new Vector3(0, 0.4f, 0);
            sprite.transform.position += tempCord;
            animator = sprite.gameObject.GetComponent<Animator>();
        }
        else
        {
            sprite = Instantiate(spriteDefault, this.transform) as GameObject;
            LoadSize();
            Vector3 tempCord = new Vector3(0, -0.6f, 0);
            sprite.transform.position += tempCord;
            if (isEnemy)
                sprite.transform.localScale += new Vector3(-2, 0, 0);

            animator = sprite.gameObject.GetComponent<Animator>();
        }

        foreach (Passives a in charc.passives.ToArray())
        {
            passives.Add(a.ReturnPassive());
        }
    }

    public void LoadSize()
    {
        float val = (float)(0.18 + (0.02*size));
        sprite.transform.localScale = new Vector3(val, val, 0);
    }

    public void ResetCanUse()
    {
        canUsePhysical = true;
        canUseRanged = true;
        canUseMagic = true;
        canUseSupp = true;
        canUseProtec = true;
        canUseStatMod = true;
        canUseSummon = true;
    }

    public void SetupItem(Items a)
    {
        foreach (Passives p in a.passives)
        {
            passives.Add(p.ReturnPassive());
        }

        foreach (Moves m in a.moves)
        {
            Moves temp = m.ReturnMove();
            temp.SetOwner(this);
            moves.Add(temp);
        }
    }

    public void LoadItems()
    {
        foreach (Items a in items)
        {
            SetupItem(a);
        }
    }

    public void GetItems(EndlessInfo info, StuffList items)
    {
        if (PlayerPrefs.GetInt("isEndless") == 1)
        {
            if (!isEnemy)
            {
                foreach (string b in info.items)
                {
                    foreach (Items a in items.returnStuff())
                    {
                        if (b == a.name)
                        {
                            this.items.Add(a.returnItem());

                            SetupItem(a);
                        }
                    }
                }
            }
        }
        else
        {
            if (!isEnemy)
            {
                foreach (Items a in items.returnStuff())
                {
                    if (PlayerPrefs.GetString("selectedItem1") == a.name || PlayerPrefs.GetString("selectedItem2") == a.name)
                    {
                        this.items.Add(a.returnItem());

                        SetupItem(a);
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (Items a in charc.recItems)
                {
                    foreach (int b in randomItems)
                    {
                        if (b == i)
                        {
                            this.items.Add(a.returnItem());

                            SetupItem(a);
                        }

                    }
                    i++;
                }
            }
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.65f);
    }

    public Effects CheckIfEffectExists(string id)
    {
        foreach (Effects a in effects)
        {
            if (a.id == id)
                return a;
        }
        return null;
    }

    public bool CountEffectTimer(GameObject panelEffects)
    {
        bool isDead = false;


        foreach (Effects a in effects.ToArray())
        {
            a.duration--;
            a.timesInc++;

            if (!a.grantsOnRunOut)
            {
                isDead = GameObject.Find("GameManager").GetComponent<BattleSystem>().EffectCalcDmg(a, this);
            } else
            {
                if (a.duration <= 0)
                    isDead = GameObject.Find("GameManager").GetComponent<BattleSystem>().EffectCalcDmg(a, this);
            }

            if (isDead)
                break;

            Wait();

            if (a.duration <= 0)
            {
                effects.Remove(a);

                GameObject temp = panelEffects.transform.Find(a.id + "(Clone)").gameObject;

                Destroy(temp.gameObject);
            }
            else
            {
                canUsePhysical = canUsePhysical && a.canUsePhysical;
                canUseRanged = canUseRanged && a.canUseRanged;
                canUseMagic = canUseMagic && a.canUseMagic;
                canUseSupp = canUseSupp && a.canUseSupp;
                canUseProtec = canUseProtec && a.canUseProtec;
                canUseStatMod = canUseStatMod && a.canUseStatMod;
                canUseSummon = canUseSummon && a.canUseSummon;

                panelEffects.transform.Find(a.id + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = a.duration.ToString();
            }
        }

        return isDead;
    }

    public bool TakeDamage (float dmgTaken, float shieldDmg, bool isCrit)
    {
        if (dmgTaken > 0 || shieldDmg > 0)
            DoAnim("takedmg");

        Vector3 pos;

        if (shieldDmg > 0)
        {
            if (isEnemy)
                pos = new Vector3(Random.Range(4, 6), Random.Range(-1, 0.5f));
            else
                pos = new Vector3(Random.Range(-4, -6), Random.Range(-1, 0.5f));

            GameObject shielded = Instantiate(dmgText, pos, Quaternion.identity) as GameObject;
            shielded.transform.GetChild(0).GetComponent<TextMesh>().text = "-" + shieldDmg.ToString("0");
            shielded.transform.GetChild(0).GetComponent<TextMesh>().color = Color.white;

            if (dmgTaken <= 0)
                dmgTaken = 0;
        }

        if (isEnemy)
            pos = new Vector3(Random.Range(4, 6), Random.Range(-1, 0.5f));
        else
            pos = new Vector3(Random.Range(-4, -6), Random.Range(-1, 0.5f));

        GameObject dmg = Instantiate(dmgText, pos, Quaternion.identity) as GameObject;
        dmg.transform.GetChild(0).GetComponent<TextMesh>().text = dmgTaken.ToString("0");

        if (isCrit)
        {
            dmg.transform.GetChild(0).GetComponent<TextMesh>().color = Color.red;
        }

        if (dmgTaken <= 0 && shieldDmg <= 0)
        {
            dmg.transform.GetChild(0).GetComponent<TextMesh>().color = Color.white;
        }

        curHp -= dmgTaken;

        foreach (Summon sum in summons)
        {
            if (sum.summonTurn > 0)
                sum.stats.hp -= dmgTaken+shieldDmg;
        }

        if (curHp <= 0)
            return true;
        else
            return false;
    }

    public void Heal (float heal)
    {
        Stats statsP = SetModifiers();

        if (!(curHp + heal >= statsP.hp))
            curHp += heal;
        else
            heal = statsP.hp - curHp;

        GameObject dmg = Instantiate(dmgText, transform.position, Quaternion.identity) as GameObject;
        dmg.transform.GetChild(0).GetComponent<TextMesh>().color = Color.green;
        dmg.transform.GetChild(0).GetComponent<TextMesh>().text = heal.ToString("0");
    }

    public Stats SetModifiers()
    {
        Stats temp = charc.stats.ReturnStats();
        foreach (StatMod mod in statMods.ToArray())
        {
            if (!mod.flat)
            {
                temp.hp += (int)((temp.hp + temp.hp) * mod.hp);
                temp.mana += (int)((temp.mana + temp.mana) * mod.mana);
                temp.stamina += (int)((temp.stamina + temp.stamina) * mod.stamina);
                temp.sanity += (int)((temp.sanity + temp.sanity) * mod.sanity);

                temp.hpRegen += (temp.hpRegen * mod.hpRegen);
                temp.manaRegen += temp.manaRegen * mod.manaRegen;
                temp.staminaRegen += temp.staminaRegen * mod.staminaRegen;

                temp.atkDmg += (temp.atkDmg * mod.atkDmg);
                temp.magicPower += (temp.magicPower * mod.magicPower);

                temp.dmgResis += (temp.dmgResis * mod.dmgResis);
                temp.magicResis += (temp.magicResis * mod.magicResis);

                temp.timing += (temp.timing * mod.timing);
                temp.movSpeed += (temp.movSpeed * mod.movSpeed);

            }
            else
            {
                temp.hp += mod.hp;
                temp.mana += mod.mana;
                temp.stamina += mod.stamina;
                temp.sanity += (int)mod.sanity;

                temp.hpRegen += mod.hpRegen;
                temp.manaRegen += mod.manaRegen;
                temp.staminaRegen += mod.staminaRegen;

                temp.atkDmg += mod.atkDmg;
                temp.magicPower += mod.magicPower;

                temp.dmgResis += mod.dmgResis;
                temp.magicResis += mod.magicResis;

                temp.timing += mod.timing;
                temp.movSpeed += mod.movSpeed;
            }

            temp.critChance += mod.critChance;
            temp.critDmg += mod.critDmg;
            temp.lifesteal += mod.lifesteal;
            temp.evasion += mod.evasion;
            temp.accuracy += mod.accuracy;
            temp.armourPen += mod.armourPen;
            temp.ultrate += mod.ultrate;
        }

        return temp;
    }

    public void PassivePopup(string name)
    {
        Vector3 pos;

        if (isEnemy)
            pos = new Vector3(6, 1);
        else
            pos = new Vector3(-6, 1);

        GameObject passive = Instantiate(passiveText, pos, Quaternion.identity) as GameObject;
        passive.transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = name;
    }

    public void Miss(bool isDodge)
    {
        FightLang langmanag = GameObject.Find("GameManager").GetComponent<FightLang>();

        GameObject miss = Instantiate(dmgText, transform.position, Quaternion.identity) as GameObject;
        miss.transform.GetChild(0).GetComponent<TextMesh>().color = Color.white;

        if (isDodge)
            miss.transform.GetChild(0).GetComponent<TextMesh>().text = langmanag.GetInfo("gui", "text", "dodgepop");
        else
            miss.transform.GetChild(0).GetComponent<TextMesh>().text = langmanag.GetInfo("gui", "text", "misspop");
    }

    public void WonLost(bool win)
    {
        if (animator.runtimeAnimatorController != null)
            if (win)
                animator.SetBool("won", true);
            else
                animator.SetBool("lose", true);
    }

    public void DoMoveAnim(Moves.MoveType moveType)
    {
        switch (moveType)
        {
            case Moves.MoveType.BASIC:
                animator.SetTrigger("physical");
                break;
            case Moves.MoveType.PHYSICAL:
                animator.SetTrigger("physical");
                break;
            case Moves.MoveType.MAGICAL:
                animator.SetTrigger("magical");
                break;
            case Moves.MoveType.RANGED:
                animator.SetTrigger("ranged");
                break;
            case Moves.MoveType.SUPPORT:
                animator.SetTrigger("support");
                break;
            case Moves.MoveType.DEFFENCIVE:
                animator.SetTrigger("defencive");
                break;
            case Moves.MoveType.STATMOD:
                animator.SetTrigger("statmod");
                break;
            case Moves.MoveType.SUMMON:
                animator.SetTrigger("summon");
                break;
        }
    }

    public void DoAnim(string what)
    {
        switch (what)
        {
            case "takedmg":
                animator.SetTrigger("takedmg");
                break;
        }
    }
}
