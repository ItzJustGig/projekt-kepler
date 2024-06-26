﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using System;
using System.Linq;
using static LanguageManager;
using static Utils;

public class Unit : MonoBehaviour
{
    public BattleHud hud;
    public Transform moveListPanel;
    public Transform actionBoxPanel;
    public Transform effectHud;
    public bool isLeader;
    public int id;
    public Button selectBtn;
    public bool isEnemy;
    public bool isDead;
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

    public Character charc;
    public int level = 0;
    [SerializeField] private GameObject spriteDefault;
    [SerializeField] private GameObject dmgText;
    [SerializeField] private GameObject passiveText;
    private readonly string selectedCharacter = "SelectedCharacter";
    private readonly string selectedEnemy = "SelectedEnemy";
    private readonly string playerchamp = "isPlayerChamp";
    private readonly string enemychamp = "isEnemyChamp";
    private readonly string selectedLevel = "selectedLevel";
    private readonly string selectedLevelEnemy = "selectedLevelEnemy";

    [SerializeField] public CharacterSum summary;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sprite;
    [SerializeField] private Animator particleAnimator;
    public int size = 2;
    float lastSize = 2;
    public bool skipTurn = false;
    public bool hasAttacked = false;

    private int _physicalAnim = Animator.StringToHash("physical");
    private int _magicalAnim = Animator.StringToHash("magical");
    private int _rangedAnim = Animator.StringToHash("ranged");
    private int _suppAnim = Animator.StringToHash("support");
    private int _defAnim = Animator.StringToHash("defencive");
    private int _enchantAnim = Animator.StringToHash("statmod");
    private int _summonAnim = Animator.StringToHash("summon");

    public struct ChosenMove
    {
        public Moves move;
        public Unit target;
    }

    [SerializeField] public ChosenMove chosenMove;

    void Awake()
    {
        if (PlayerPrefs.GetInt("isEndless") == 0)
        {
            level = PlayerPrefs.GetInt(selectedLevel);
        } else
        {
            if (!isEnemy)
                level = PlayerPrefs.GetInt(selectedLevel);
            else
                level = PlayerPrefs.GetInt(selectedLevelEnemy);
        }

        int character = 1;
        int bot = 1;

        switch (this.transform.parent.name)
        {
            case "P1":
                character = PlayerPrefs.GetInt(selectedCharacter + "1");
                bot = PlayerPrefs.GetInt(selectedEnemy + "1");
                break; 
            case "P2":
                character = PlayerPrefs.GetInt(selectedCharacter + "2");
                bot = PlayerPrefs.GetInt(selectedEnemy + "2");
                break;
            case "P3":
                character = PlayerPrefs.GetInt(selectedCharacter + "3");
                bot = PlayerPrefs.GetInt(selectedEnemy + "3");
                break;
        }

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

        if (!charc.isBasicPhysical)
        {
            basicAttack.scale[0].type = DmgType.MAGICAL;
            basicAttack.scale[0].magicPower = basicAttack.scale[0].atkDmg;
            basicAttack.scale[0].atkDmg = 0;

            moves[0].scale[0].type = DmgType.MAGICAL;
            moves[0].scale[0].magicPower = moves[0].scale[0].atkDmg;
            moves[0].scale[0].atkDmg = 0;
        }

        if (charc.growth)
            charc.stats = charc.GetStatLevel(level);
        else
            charc.stats = charc.stats.ReturnStats();

        size = charc.size;

        Debug.Log(basicAttack.scale[0].type);
    }

    void Start()
    {
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
        skipTurn = false;
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
                    if (PlayerPrefs.GetString("selectedItem1_" + id) == a.name || PlayerPrefs.GetString("selectedItem2_" + id) == a.name)
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
        yield return new WaitForSeconds(0.5f);
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

    public bool CountEffectTimer(GameObject panelEffects, int bloodLossStacks, float dotReduc)
    {
        bool isDead = false;
        bool cleanse = false;
        bool skipLastTurn = false;

        if (effects.Any(x=>x.id == "CLS"))
        {
            cleanse = true;
            skipLastTurn = true;
        }

        if (passives.Any(x => x.name == "watcher"))
        {
            skipLastTurn = true;
        }

        foreach (Effects a in effects.ToArray())
        {
            if (cleanse && a.id != "FEA" && a.id != "TRD" && a.id != "GRD" && a.id != "LCK")
            {
                a.duration = 0;
            } else
            {
                if (a.timeReducImmunity)
                    a.timeReducImmunity = false;
                else
                    a.duration--;
            }

            if (a.duration > 0 || !skipLastTurn)
            {
                a.timesInc++;
                bool skipDmg = false;

                if ((a.id == "TAU" || a.id == "GRD") && a.source.isDead)
                    a.duration = 0;

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

                                dmg = this.MitigateDmg(dmg, 0, 0, null, dotReduc);
                                dmg = this.CalcRegens(dmg);

                                isDead = this.TakeDamage(dmg, false, false, this, false);
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
                }
                else
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
            }

            if (a.duration <= 0)
            {
                effects.Remove(a);

                GameObject temp = panelEffects.transform.Find(a.id + "(Clone)").gameObject;

                Destroy(temp.gameObject);
            }
            else
            {
                SetCC();

                panelEffects.transform.Find(a.id + "(Clone)").gameObject.transform.Find("time").gameObject.GetComponent<Text>().text = a.duration.ToString();
            }
        }

        return isDead;
    }

    public void SetCC()
    {
        foreach (Effects a in effects.ToArray())
        {
            canUsePhysical = canUsePhysical && a.canUsePhysical;
            canUseRanged = canUseRanged && a.canUseRanged;
            canUseMagic = canUseMagic && a.canUseMagic;
            canUseSupp = canUseSupp && a.canUseSupp;
            canUseProtec = canUseProtec && a.canUseProtec;
            canUseEnchant = canUseEnchant && a.canUseEnchant;
            canUseSummon = canUseSummon && a.canUseSummon;
            skipTurn = a.isStun || skipTurn;
        }
    }

    public DMG MitigateDmg(DMG dmg, float armourPen, float magicPen, Unit attacker=null, float dotReduc = 1)
    {
        if (dmg.phyDmg > 0)
        {
            if (attacker != null)
                attacker.summary.phyDmgDealt += dmg.phyDmg;

            float dmgMitigated = dmg.phyDmg * (float)((SetModifiers().dmgResis - (SetModifiers().dmgResis * armourPen)) / ((SetModifiers().dmgResis - (SetModifiers().dmgResis * armourPen)) + 200)) * dotReduc;
            
            if (dmgMitigated < dmg.phyDmg)
            {
                dmg.phyDmg -= dmgMitigated;
                summary.phyDmgMitigated += dmgMitigated;
            }
            else
            {
                dmgMitigated = dmg.phyDmg;
                dmg.phyDmg = 0;
                summary.phyDmgMitigated += dmgMitigated;
            }
        }

        if (dmg.magicDmg > 0)
        {
            if (attacker != null)
                attacker.summary.magicDmgDealt += dmg.magicDmg;

            float dmgMitigated = dmg.magicDmg * (float)((SetModifiers().magicResis - (SetModifiers().magicResis * magicPen)) / ((SetModifiers().magicResis - (SetModifiers().magicResis * magicPen)) + 200)) * dotReduc;

            if (dmgMitigated < dmg.magicDmg)
            {
                dmg.magicDmg -= dmgMitigated;
                summary.magicDmgMitigated += dmgMitigated;
            }
            else
            {
                dmgMitigated = dmg.magicDmg;
                dmg.magicDmg = 0;
                summary.magicDmgMitigated += dmgMitigated;
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
            curMana += dmg.healMana;
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
                        Dotdmg dot = Dotdmg.CreateInstance<Dotdmg>();
                        dot.Setup(dmg.shield, a.num, moveName, Dotdmg.SrcType.MOVE, Dotdmg.DmgType.SHIELD, this, true);
                        dmg.shield = 0;
                        dotDmg.Add(dot);
                    }
                }

            if (curShield > 1500)
                curShield = 1500;
        }

        return dmg;
    }

    public bool TakeDamage (DMG dmg, bool isCrit, bool magicCrit, Unit attacker, bool fromAlly, string movename = "")
    {
        foreach (Passives a in passives.ToArray())
        {
            switch (a.name)
            {
                case "watcher":
                    dmg.ResetHeals();
                    int temp = dmg.GetOffenciveNumbers();
                    if (temp > 0)
                    {
                        dmg.ResetOffencive();
                        dmg.trueDmg += a.statScale.flatValue * temp;
                    }
                    break;
            }
        }

        if (dmg.phyDmg > 0)
        {
            summary.phyDmgTaken += dmg.phyDmg;
        }

        if (dmg.magicDmg > 0)
        {
            summary.magicDmgTaken += dmg.magicDmg;
        }

        if (dmg.trueDmg > 0)
        {
            attacker.summary.trueDmgDealt += dmg.trueDmg;
            summary.trueDmgTaken += dmg.trueDmg;
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
            attacker.summary.sanityDmgDealt += dmg.sanityDmg;
            summary.sanityDmgTaken += dmg.sanityDmg;
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
            else
                dmgTaken -= shieldedDmg;
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

        foreach (Passives a in passives)
        {
            switch (a.name)
            {
                case "spectralcloak":
                    if (dmg.magicDmg > 0)
                        dmg.shield += a.statScale.SetScaleFlat(SetModifiers(), this) * a.stacks;
                break;
                case "magicbody":
                    if (dmg.magicDmg > 0)
                        a.stacks++;
                break;
            }
        }

        if (dmg.heal > 0)
        {
            if (fromAlly == false)
                summary.healDone += dmg.heal;
            else
                attacker.summary.healDone += dmg.heal;

            Heal(dmg.heal);
        }

        if (dmg.healMana > 0)
        {
            if (fromAlly == false)
                summary.manaHealDone += dmg.healMana;
            else
                attacker.summary.manaHealDone += dmg.healMana;
        }

        if (dmg.healStamina > 0)
        {
            if (fromAlly == false)
                summary.staminaHealDone += dmg.healStamina;
            else
                attacker.summary.staminaHealDone += dmg.healStamina;
        }

        if (dmg.healSanity > 0)
        {
            if (fromAlly == false)
                summary.sanityHealDone += dmg.healSanity;
            else
                attacker.summary.sanityHealDone += dmg.healSanity;
        }

        if (dmg.shield > 0)
        {
            if (fromAlly == false)
                summary.shieldDone += dmg.shield;
            else
                attacker.summary.shieldDone += dmg.shield;

            curShield += dmg.shield;
        }

        if (dmg.ultenergy > 0)
        {
            ult += dmg.ultenergy;
        }

        if (curHp <= 0)
            return true;
        else
            return false;
    }

    public void DmgNumber(string msg, Color color, int size = 2)
    {
        Vector3 pos = this.transform.position;
        pos += new Vector3(UnityEngine.Random.Range(-0.15f, 0.15f), UnityEngine.Random.Range(-0.3f, 0.5f));

        GameObject go = Instantiate(dmgText, pos, Quaternion.identity);
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
        {
            heal = SetModifiers().hp - curHp;
            curHp = SetModifiers().hp;
        }

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
            temp.magicPen += mod.magicPen;
            temp.ultrate += mod.ultrate;
            temp.manaCost += mod.manaCost;
            temp.staminaCost += mod.staminaCost;
            temp.healBonus += mod.healBonus;
            temp.shieldBonus += mod.shieldBonus;
            temp.sizeMod += mod.size;
        }

        foreach (Passives a in passives)
        {
            switch (a.name)
            {
                case "spectralcloak":
                    a.stacks = (int)(temp.magicResis / a.maxNum);
                break;
            }
        }

        return temp;
    }

    public void PassivePopup(string name)
    {
        Vector3 pos;
        float hight = UnityEngine.Random.Range(1.75f, 2.75f);
        pos = new Vector3(transform.position.x, transform.position.y + hight);

        GameObject passive = Instantiate(passiveText, pos, Quaternion.identity) as GameObject;
        passive.transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = name;
    }

    public void Miss(bool isDodge)
    {
        FightLang langmanag = GameObject.Find("GameManager").GetComponent<FightLang>();
        string msg;

        if (isDodge)
            msg = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "dodgepop"));
        else
            msg = langmanag.GetInfo(new ArgumentsFetch("gui", "text", "misspop"));

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
                animator.SetTrigger(_physicalAnim);
                break;
            case Moves.MoveType.PHYSICAL:
                animator.SetTrigger(_physicalAnim);
                break;
            case Moves.MoveType.MAGICAL:
                animator.SetTrigger(_magicalAnim);
                break;
            case Moves.MoveType.RANGED:
                animator.SetTrigger(_rangedAnim);
                break;
            case Moves.MoveType.SUPPORT:
                animator.SetTrigger(_suppAnim);
                break;
            case Moves.MoveType.DEFFENCIVE:
                animator.SetTrigger(_defAnim);
                break;
            case Moves.MoveType.ENCHANT:
                animator.SetTrigger(_enchantAnim);
                break;
            case Moves.MoveType.SUMMON:
                animator.SetTrigger(_summonAnim);
                break;
        }
    }

    public void SetAnimHud(string what, bool active)
    {
        if (!string.IsNullOrEmpty(what))
        {
            hud.gameObject.GetComponent<Animator>().SetBool(what, active);
        }
    }

    public void DoAnim(string what)
    {
        if (!string.IsNullOrEmpty(what))
            animator.SetTrigger(what);
    }

    public void DoAnimParticle(string what)
    {
        if (!string.IsNullOrEmpty(what))
            particleAnimator.SetTrigger(what);
    }

    public void SetSkipTurn(bool val)
    {
        skipTurn = val;
    }

    public bool CheckSkipTurn()
    {
        return skipTurn;
    }
}
