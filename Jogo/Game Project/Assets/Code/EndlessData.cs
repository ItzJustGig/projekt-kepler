using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndlessData
{
    public bool isEnemyChampPrev;
    public int enemyIdPrev;
    public int enemyLevelPrev;
    public bool isEnemyBossPrev;

    public bool isEnemyChampNext;
    public int enemyIdNext;
    public int enemyLevelNext;
    public bool isEnemyBossNext;

    public bool isPlayerChamp;
    public int playerId;

    public float playerHp;
    public float playerMn;
    public float playerSta;
    public float playerSan;
    public float playerUlt;

    public int gold;
    public int level;
    public int round;
    public int shoppass;
    public int shopcoupon;
    public int shoprerolls;

    public int wonLastRound;
    public bool generateShop;
    public string[] items;
    public string[] itemShop;

    public EndlessData (EndlessInfo data)
    {
        this.isEnemyChampNext = data.isEnemyChampNext;
        this.isEnemyBossNext = data.isEnemyBossNext;
        this.enemyIdNext = data.enemyIdNext;
        this.enemyLevelNext = data.enemyLevelNext;
        this.isEnemyChampPrev = data.isEnemyChampPrev;
        this.enemyLevelPrev = data.enemyLevelPrev;
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
        this.level = data.level;
        this.shoprerolls = data.shoprerolls;
        this.shoppass = data.shoppass;
        this.shopcoupon = data.shopcoupon;
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
        this.enemyLevelPrev = 0;
        this.isEnemyBossPrev = false;
        this.isEnemyChampNext = false;
        this.enemyIdNext = -1;
        this.enemyLevelNext = -1;
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
        this.shoppass = 0;
        this.shopcoupon = 0;
        this.shoprerolls = 2;
        this.round = -1;
        this.level = 0;
        this.wonLastRound = 1;
        this.generateShop = true;
    }
}
