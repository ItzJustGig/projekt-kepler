using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

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
    public bool canUseEnchant = true;
    public bool canUseSummon = true;

    public List<StatMod> statMods = new List<StatMod>();
    public List<Effects> effects = new List<Effects>();
    public int bloodStacks = 0;
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
    [SerializeField] private Animator particleAnimator;
    public int size = 2;
    float lastSize = 2;

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
            Vector3 tempCord = new Vector3(0, 0.4f, 0);
            sprite.transform.position += tempCord;
            animator = sprite.gameObject.GetComponent<Animator>();
        }
        else
        {
            sprite = Instantiate(spriteDefault, this.transform) as GameObject;
            Vector3 tempCord = new Vector3(0, -0.6f, 0);
            sprite.transform.position += tempCord;
            if (isEnemy)
                sprite.transform.localScale += new Vector3(-2, 0, 0);

            animator = sprite.gameObject.GetComponent<Animator>();
        }


        LoadSize(size);

        foreach (Passives a in charc.passives.ToArray())
        {
            passives.Add(a.ReturnPassive());
        }
    }

    public void LoadSize(float size)
    {
        float sizeVal = (float)(0.18 + (0.02 * size));
        sprite.transform.localScale = new Vector2(sizeVal, sizeVal);

        if (lastSize > size)
        {
            float hight = (float)(-0.09 * (lastSize - size));
            sprite.transform.position += new Vector3(0, hight);
        } else if (lastSize < size)
        {
            float hight = (float)(0.09 * (size - lastSize));
            sprite.transform.position += new Vector3(0, hight);
        }
        lastSize = size;
    }

    public void ResetCanUse()
    {
        canUsePhysical = true;
        canUseRanged = true;
        canUseMagic = true;
        canUseSupp = true;
        canUseProtec = true;
        canUseEnchant = true;
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
            if ((id == "BRN" && a.id == "SCH") || a.id == id)
                return a;
        }
        return null;
    }

    public bool CountEffectTimer(GameObject panelEffects, int bloodLossStacks, float dmgResisPer, float magicResisPer, float dotReduc)
    {
        bool isDead = false;

        foreach (Effects a in effects.ToArray())
        {
            a.duration--;
            a.timesInc++;
            bool skipDmg = false;

            if (!a.grantsOnRunOut)
            {
                if (a.id == "BLD")
                {
                    bloodStacks++;
                    if (bloodStacks == bloodLossStacks)
                    {
                        foreach (StatScale b in a.scale)
                        {
                            DMG dmg = default;
                            foreach (StatScale scale in a.scale.ToArray())
                            {
                                Stats stats = this.SetModifiers().ReturnStats();

                                dmg.AddDmg(scale.SetScaleDmg(stats, this));
                            }

                            dmg = this.MitigateDmg(dmg, dmgResisPer, magicResisPer, 0, 0, null, dotReduc);
                            dmg = this.CalcRegens(dmg);

                            isDead = this.TakeDamage(dmg, false, false, this);
                            DoAnimParticle(a.specialAnim);
                        }
                        bloodStacks = 0;
                        skipDmg = true;
                    }
                }

                if (!skipDmg)
                {
                    DoAnimParticle(a.hitAnim);
                    isDead = GameObject.Find("GameManager").GetComponent<BattleSystem>().EffectCalcDmg(a, this);
                }
            } else
            {
                if (a.duration <= 0)
                {
                    DoAnimParticle(a.hitAnim);
                    isDead = GameObject.Find("GameManager").GetComponent<BattleSystem>().EffectCalcDmg(a, this);
                }
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
                canUseEnchant = canUseEnchant && a.canUseEnchant;
                canUseSummon = canUseSummon && a.canUseSummon;

                panelEffects.transform.Find(a.id + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = a.duration.ToString();
            }
        }

        return isDead;
    }

    public DMG MitigateDmg(DMG dmg, float dmgResisPer, float magicResisPer, float armourPen, float magicPen, Unit attacker=null, float dotReduc = 1)
    {
        if (dmg.phyDmg > 0)
        {
            if (attacker != null)
                attacker.phyDmgDealt += dmg.phyDmg;

            float dmgMitigated = (float)(((SetModifiers().dmgResis - (SetModifiers().dmgResis * armourPen)) * dmgResisPer)*dotReduc);
            if (dmgMitigated < dmg.phyDmg)
            {
                dmg.phyDmg -= dmgMitigated;
                phyDmgMitigated += dmgMitigated;
            }
            else
            {
                dmgMitigated = dmg.phyDmg;
                dmg.phyDmg = 0;
                phyDmgMitigated += dmgMitigated;
            }
        }

        if (dmg.magicDmg > 0)
        {
            if (attacker != null)
                attacker.magicDmgDealt += dmg.magicDmg;

            float dmgMitigated = (float)(((SetModifiers().magicResis - (SetModifiers().magicResis * magicPen)) * magicResisPer)*dotReduc);
            if (dmgMitigated < dmg.magicDmg)
            {
                dmg.magicDmg -= dmgMitigated;
                magicDmgMitigated += dmgMitigated;
            }
            else
            {
                dmgMitigated = dmg.magicDmg;
                dmg.magicDmg = 0;
                magicDmgMitigated += dmgMitigated;
            }
        }

        return dmg;
    }

    public DMG ApplyHealFrom(DMG dmg, Moves.HealFromDmg healFrom, float healFromPer)
    {
        if (healFrom != Moves.HealFromDmg.NONE)
        {
            if (healFrom is Moves.HealFromDmg.PHYSICAL)
                dmg.heal += dmg.phyDmg * healFromPer;
            else if (healFrom is Moves.HealFromDmg.MAGICAL)
                dmg.heal += dmg.magicDmg * healFromPer;
            else if (healFrom is Moves.HealFromDmg.TRUE)
                dmg.heal += dmg.trueDmg * healFromPer;
            else if (healFrom is Moves.HealFromDmg.PHYSICAL_MAGICAL)
                dmg.heal += (dmg.phyDmg + dmg.magicDmg) * healFromPer;
            else if (healFrom is Moves.HealFromDmg.PHYSICAL_TRUE)
                dmg.heal += (dmg.phyDmg + dmg.trueDmg) * healFromPer;
            else if (healFrom is Moves.HealFromDmg.MAGICAL_TRUE)
                dmg.heal += (dmg.magicDmg + dmg.trueDmg) * healFromPer;
            else if (healFrom is Moves.HealFromDmg.ALL)
                dmg.heal += (dmg.phyDmg + dmg.magicDmg + dmg.trueDmg) * healFromPer;
        }

        return dmg;
    }

    public DMG ApplyLifesteal(DMG dmg)
    {
        if (SetModifiers().lifesteal > 0)
            dmg.ApplyLifesteal(false, SetModifiers().lifesteal);

        return dmg;
    }

    public DMG CalcRegens(DMG dmg, string moveName="")
    {
        if (dmg.heal > 0)
        {
            dmg.heal += dmg.heal * SetModifiers().healBonus;
        }

        if (dmg.healMana > 0)
        {
            if (curMana > SetModifiers().mana)
            {
                curMana = SetModifiers().mana;
                dmg.healMana = curMana - SetModifiers().mana;
            }
        }

        if (dmg.healStamina > 0)
        {
            curStamina += dmg.healStamina;
            if (curStamina > SetModifiers().stamina)
            {
                curStamina = SetModifiers().stamina;
                dmg.healStamina = curStamina - SetModifiers().stamina;
            }
        }

        if (dmg.healSanity > 0)
        {
            curSanity += dmg.healSanity;
            if (curSanity > SetModifiers().sanity)
            {
                curSanity = SetModifiers().sanity;
                dmg.healSanity = curSanity - SetModifiers().sanity;
            }
        }

        if (dmg.shield > 0)
        {
            dmg.shield += dmg.shield * SetModifiers().shieldBonus;
            if (moveName != "")
                foreach (Passives a in passives.ToArray())
                {
                    if (a.name == "combatrepair")
                    {
                        Dotdmg dot = new Dotdmg();
                        dot.Setup(dmg.shield, a.num, moveName, Dotdmg.SrcType.MOVE, Dotdmg.DmgType.SHIELD, this);
                        dmg.shield = 0;
                        dotDmg.Add(dot);
                    }
                }

            if (curShield > 1000)
                curShield = 1000;
        }

        return dmg;
    }

    public bool TakeDamage (DMG dmg, bool isCrit, bool magicCrit, Unit attacker, string movename = "")
    {
        if (dmg.phyDmg > 0)
        {
            phyDmgTaken += dmg.phyDmg;
        }

        if (dmg.magicDmg > 0)
        {
            magicDmgTaken += dmg.magicDmg;
        }

        if (dmg.trueDmg > 0)
        {
            attacker.trueDmgDealt += dmg.trueDmg;
            trueDmgTaken += dmg.trueDmg;
        }

        if (dmg.sanityDmg > 0)
        {
            if ((curSanity - dmg.sanityDmg) >= 0)
            {
                curSanity -= dmg.sanityDmg;
            }
            else
            {
                dmg.sanityDmg = curSanity;
                curSanity = 0;
            }
            attacker.sanityDmgDealt += dmg.sanityDmg;
            sanityDmgTaken += dmg.sanityDmg;
        }

        dmg = CalcRegens(dmg, movename);

        if (curHp > SetModifiers().hp)
            curHp = SetModifiers().hp;

        float dmgTaken = dmg.phyDmg + dmg.magicDmg + dmg.trueDmg;

        float shieldedDmg = 0;

        if (curShield > 0)
        {
            float tempDmg = dmg.phyDmg + dmg.magicDmg + dmg.trueDmg;
            float tempShield = curShield;

            curShield -= tempDmg;

            if (curShield < 0)
                curShield = 0;

            shieldedDmg = tempShield - curShield;
        }

        if (dmgTaken > 0 || shieldedDmg > 0)
            DoAnim("takedmg");

        if (shieldedDmg > 0)
        {
            DmgNumber("-" + shieldedDmg.ToString("0"), Color.white);

            if (dmgTaken <= 0)
                dmgTaken = 0;
        }

        Color tempColor;
        string tempText;

        if (isCrit)
        {
            tempColor = Color.red;
            tempText = dmg.phyDmg.ToString("0") + "!";
        }
        else
        {
            tempColor = new Color (1f, .4431f, 0f);
            tempText = dmg.phyDmg.ToString("0");
        }
            
        if (dmg.phyDmg > 0)
            DmgNumber(tempText, tempColor);

        if (magicCrit)
        {
            tempColor = new Color(.77f, .22f, .71f);
            tempText = dmg.magicDmg.ToString("0")+"!";
        }
        else
        {
            tempColor = new Color(.4941f, .5764f, 1f);
            tempText = dmg.magicDmg.ToString("0");
        }

        if (dmg.magicDmg > 0)
            DmgNumber(tempText, tempColor);

        if (dmg.trueDmg > 0)
            DmgNumber(dmg.trueDmg.ToString("0"), new Color(0.8113208f, 0.8113208f, 0.8113208f));

        if (dmgTaken > 0)
            curHp -= dmgTaken;

        foreach (Summon sum in summons)
        {
            if (sum.summonTurn > 0)
                sum.stats.hp -= dmgTaken;
        }

        if (dmg.heal > 0)
        {
            healDone += dmg.heal;
            Heal(dmg.heal);
        }

        if (dmg.healMana > 0)
        {
            manaHealDone += dmg.healMana;
            curMana += dmg.healMana;
        }

        if (dmg.healStamina > 0)
        {
            staminaHealDone += dmg.healStamina;
            curStamina += dmg.healStamina;
        }

        if (dmg.healSanity > 0)
        {
            sanityHealDone += dmg.healSanity;
            curSanity += dmg.healSanity;
        }

        if (dmg.shield > 0)
        {
            shieldDone += dmg.shield;
            curShield += dmg.shield;
        }

        if (curHp <= 0)
            return true;
        else
            return false;
    }

    public void DmgNumber(string msg, Color color, int size = 2)
    {
        Vector3 pos;

        if (isEnemy)
            pos = new Vector3(Random.Range(4, 6), Random.Range(-1, 0.5f));
        else
            pos = new Vector3(Random.Range(-4, -6), Random.Range(-1, 0.5f));

        GameObject go = Instantiate(dmgText, pos, Quaternion.identity) as GameObject;
        go.transform.GetChild(0).GetComponent<TextMesh>().text = msg;
        go.transform.GetChild(0).GetComponent<TextMesh>().color = color;
        /*string sizeText = "normal";
        switch (size)
        {
            case 1:
                sizeText = "small";
                break;
            case 3:
                sizeText = "big";
                break;
        }
        go.transform.GetComponent<DmgText>().Pop(sizeText);*/
    }

    public void Heal (float heal)
    {
        if (!(curHp + heal >= SetModifiers().hp))
            curHp += heal;
        else
            heal = SetModifiers().hp - curHp;

        DmgNumber(heal.ToString("0"), Color.green);
    }

    public Stats SetModifiers()
    {
        Stats temp = charc.stats.ReturnStats();
        foreach (StatMod mod in statMods.ToArray())
        {
            if (!mod.flat)
            {
                temp.hp += (int)(temp.hp * mod.hp);
                temp.mana += (int)(temp.mana * mod.mana);
                temp.stamina += (int)(temp.stamina * mod.stamina);
                temp.sanity += (int)(temp.sanity * mod.sanity);

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
            temp.manaCost += mod.manaCost;
            temp.staminaCost += mod.staminaCost;
            temp.healBonus += mod.healBonus;
            temp.shieldBonus += mod.shieldBonus;
            temp.sizeMod += mod.size;
        }

        return temp;
    }

    public void PassivePopup(string name)
    {
        Vector3 pos;
        float hight = Random.Range(0.5f, 1.5f);

        if (isEnemy)
            pos = new Vector3(6, hight);
        else
            pos = new Vector3(-6, hight);

        GameObject passive = Instantiate(passiveText, pos, Quaternion.identity) as GameObject;
        passive.transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = name;
    }

    public void Miss(bool isDodge)
    {
        FightLang langmanag = GameObject.Find("GameManager").GetComponent<FightLang>();
        string msg;
        if (isDodge)
            msg = langmanag.GetInfo("gui", "text", "dodgepop");
        else
            msg = langmanag.GetInfo("gui", "text", "misspop");

        DmgNumber(msg, Color.white);
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
            case Moves.MoveType.ENCHANT:
                animator.SetTrigger("statmod");
                break;
            case Moves.MoveType.SUMMON:
                animator.SetTrigger("summon");
                break;
        }
    }

    public void DoAnim(string what)
    {
        if (what != "")
            animator.SetTrigger(what);
    }

    public void DoAnimParticle(string what)
    {
        if (what != "" || what != null)
            particleAnimator.SetTrigger(what);
    }
}
