﻿using System.Collections;
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
    //public bool canUseDeploy = true;

    public List<StatMod> statMods = new List<StatMod>();
    public List<Effects> effects = new List<Effects>();
    public List<Dotdmg> dotDmg = new List<Dotdmg>();

    public List<Moves> moves = new List<Moves>();
    public List<Passives> passives = new List<Passives>();
    public List<Items> items = new List<Items>();

    [SerializeField] private List<Character> characters = new List<Character>();
    [SerializeField] private List<Character> monsters = new List<Character>();
    
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

    void Awake()
    {
        int character = PlayerPrefs.GetInt(selectedCharacter);
        int bot = PlayerPrefs.GetInt(selectedEnemy);

        int isPlayerChamp = PlayerPrefs.GetInt(playerchamp);
        int isEnemyChamp = PlayerPrefs.GetInt(enemychamp);

        if (!isEnemy)
        {
            if (isPlayerChamp == 1)
                charc = characters[character-1].GetCharcInfo();
            else
                charc = monsters[character-1].GetCharcInfo();
        }
        else
        {
            if (isEnemyChamp == 1)
                charc = characters[bot-1].GetCharcInfo();
            else
                charc = monsters[bot-1].GetCharcInfo();
        }

        foreach (Moves move in charc.moves.ToArray())
        {
            moves.Add(move.ReturnMove());
        }
    }

    void Start()
    {
        //int character = PlayerPrefs.GetInt(selectedCharacter);
        //int bot = PlayerPrefs.GetInt(selectedEnemy);
        

        if (charc.sprite)
        {
            GameObject temp = Instantiate(charc.sprite, this.transform) as GameObject;
            Vector3 tempCord = new Vector3(0, 0.4f, 0);
            temp.transform.position += tempCord;
            temp.transform.localScale = new Vector3(0.2433822f, 0.2433822f, 0);

            //if (bot == character && isEnemy)
            //{
            //    temp.GetComponent<SpriteRenderer>().color = Color.grey;
            //}

            animator = temp.gameObject.GetComponent<Animator>();
        }
        else
        {
            GameObject temp = Instantiate(spriteDefault, this.transform) as GameObject;
            Vector3 tempCord = new Vector3(0, -0.6f, 0);
            temp.transform.position += tempCord;
            if (isEnemy)
                temp.transform.localScale += new Vector3(-2, 0, 0);

            animator = temp.gameObject.GetComponent<Animator>();
        }

        foreach (Passives a in charc.passives.ToArray())
        {
            passives.Add(a.ReturnPassive());
        }
    }

    public bool TakeDamage (float dmgTaken, float shieldDmg, bool isCrit)
    {
        if (dmgTaken > 0 && shieldDmg > 0)
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

        if (curHp <= 0)
            return true;
        else
            return false;
    }

    public void Heal (float heal)
    {
        Stats statsP = charc.stats.ReturnStats();

        if (statMods.Count > 0)
            foreach (StatMod statMod in statMods.ToArray())
            {
                statsP = SetModifiers(statMod.ReturnStats(), statsP.ReturnStats(), this.GetComponent<Unit>());
            }

        if (!(curHp + heal >= statsP.hp))
            curHp += heal;
        else
            heal = statsP.hp - curHp;

        GameObject dmg = Instantiate(dmgText, transform.position, Quaternion.identity) as GameObject;
        dmg.transform.GetChild(0).GetComponent<TextMesh>().color = Color.green;
        dmg.transform.GetChild(0).GetComponent<TextMesh>().text = heal.ToString("0");
    }

    Stats SetModifiers(StatMod scale, Stats user, Unit original)
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

        }
        else
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
