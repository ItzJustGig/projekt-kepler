using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndlessData
{
    public bool isEnemyChampPrev;
    public int enemyIdPrev;
    public bool isEnemyBossPrev;

    public bool isEnemyChampNext;
    public int enemyIdNext;
    public bool isEnemyBossNext;

    public bool isPlayerChamp;
    public int playerId;

    public float playerHp;
    public float playerMn;
    public float playerSta;
    public float playerSan;
    public float playerUlt;

    public float gold;
    public int round;

    public int wonLastRound;
    public bool generateShop;
    public string[] items;
    public string[] itemShop;

    public EndlessData (EndlessInfo data)
    {
        this.isEnemyChampNext = data.isEnemyChampNext;
        this.isEnemyBossNext = data.isEnemyBossNext;
        this.enemyIdNext = data.enemyIdNext;
        this.isEnemyChampPrev = data.isEnemyChampPrev;
        this.isEnemyBossPrev = data.isEnemyBossPrev;
        this.enemyIdPrev = data.enemyIdPrev;
        this.isPlayerChamp = data.isPlayerChamp;
        this.playerId = data.playerId;
        this.playerHp = data.playerHp;
        this.playerMn = data.playerMn;
        this.playerSta = data.playerSta;
        this.playerSan = data.playerSan;
        this.playerUlt = data.playerUlt;
        this.gold = data.gold;
        this.round = data.round;
        this.wonLastRound = data.wonLastRound;
        this.generateShop = data.generateShop;

        this.items = new string[data.items.Count];
        int i = 0;
        foreach (string a in data.items)
        {
            this.items[i] = a;
            i++;
        }

        this.itemShop = new string[data.itemShop.Count];
        i = 0;
        foreach (string a in data.itemShop)
        {
            this.itemShop[i] = a;
            i++;
        }
    }

    public EndlessData ()
    {
        this.isEnemyChampPrev = false;
        this.enemyIdPrev = -1;
        this.isEnemyBossPrev = false;
        this.isEnemyChampNext = false;
        this.enemyIdNext = -1;
        this.isEnemyBossNext = false;
        this.isPlayerChamp = false;
        this.playerId = -1;
        this.playerHp = 1;
        this.playerMn = 1;
        this.playerSta = 1;
        this.playerSan = 1;
        this.playerUlt = 0;
        this.gold = 0;
        this.round = -1;
        this.wonLastRound = 1;
        this.generateShop = true;
    }
}
