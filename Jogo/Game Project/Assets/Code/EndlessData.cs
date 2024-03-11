using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndlessData
{
    public EndlessCharacterInfo enemyPrev;
    public bool isEnemyBossPrev;


    public EndlessCharacterInfo enemyNext;
    public bool isEnemyBossNext;

    public EndlessCharacterInfo player;

    public int gold;
    public int round;
    public int shoppass;
    public int shopcoupon;
    public int shoprerolls;

    public int wonLastRound;
    public bool generateShop;
    public bool isShopOpen;
    public bool hasRested;
    public bool wasPassUsed;
    public string[] items;
    public string[] itemShop;

    public EndlessData (EndlessInfo data)
    {
        this.enemyNext = data.enemyNext;
        this.isEnemyBossNext = data.isEnemyBossNext;
        this.enemyPrev = data.enemyPrev;
        this.isEnemyBossPrev = data.isEnemyBossPrev;
        this.player = data.player;
        this.gold = data.gold;
        this.round = data.round;
        this.shoprerolls = data.shoprerolls;
        this.shoppass = data.shoppass;
        this.shopcoupon = data.shopcoupon;
        this.wonLastRound = data.wonLastRound;
        this.generateShop = data.generateShop;
        this.wasPassUsed = data.wasPassUsed;
        this.hasRested = data.hasRested;
        this.isShopOpen = data.isShopOpen;

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
        this.enemyPrev = new EndlessCharacterInfo();
        this.isEnemyBossPrev = false;
        this.enemyNext = new EndlessCharacterInfo();
        this.isEnemyBossNext = false;
        this.player = new EndlessCharacterInfo();
        this.gold = 0;
        this.round = -1;
        this.shoppass = 0;
        this.shopcoupon = 0;
        this.shoprerolls = 2;
        this.round = -1;
        this.wonLastRound = 1;
        this.generateShop = true;
        this.isShopOpen = false;
        this.wasPassUsed = false;
        this.hasRested = false;
    }
}
