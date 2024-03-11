using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessInfo : MonoBehaviour
{

    public EndlessCharacterInfo enemyPrev;
    public bool isEnemyBossPrev;

    public EndlessCharacterInfo enemyNext;
    public bool isEnemyBossNext;

    public EndlessCharacterInfo player;

    public int wonLastRound;
    public bool generateShop;
    public bool isShopOpen;
    public bool wasPassUsed;
    public bool hasRested;

    public int gold;
    public int round;
    public int shopcoupon;
    public int shoppass;
    public int shoprerolls;

    public List<string> items = new List<string>();
    public List<string> itemShop = new List<string>();

    public void Save()
    {
        SaveSystem.Save(this);
    }

    public void Load()
    {
        EndlessData data = SaveSystem.Load();
        this.enemyPrev = data.enemyPrev;
        this.isEnemyBossPrev = data.isEnemyBossPrev;
        this.enemyNext = data.enemyNext;
        this.isEnemyBossNext = data.isEnemyBossNext;
        this.player = data.player;
        this.gold = data.gold;
        this.round = data.round;
        this.shopcoupon = data.shopcoupon;
        this.shoppass = data.shoppass;
        this.shoprerolls = data.shoprerolls;
        this.wonLastRound = data.wonLastRound;
        this.generateShop = data.generateShop;
        this.isShopOpen = data.isShopOpen;
        this.hasRested = data.hasRested;
        this.wasPassUsed = data.wasPassUsed;

        if (data.items != null)
            foreach(string a in data.items)
            {
                this.items.Add(a);
            }

        if (data.itemShop != null)
            foreach(string a in data.itemShop)
            {
                this.itemShop.Add(a);
            }
    }

    public void Delete()
    {
        EndlessData data = SaveSystem.Load();

        this.enemyPrev = new EndlessCharacterInfo();
        this.isEnemyBossPrev = false;
        this.enemyNext = new EndlessCharacterInfo();
        this.isEnemyBossNext = false;
        this.player = new EndlessCharacterInfo();
        this.gold = 0;
        this.round = -1;
        this.shopcoupon = 0;
        this.shoppass = 0;
        this.shoprerolls = 2;
        this.wonLastRound = 1;
        this.generateShop = true;
        this.isShopOpen = false;
        this.wasPassUsed = false;
        this.hasRested = false;
        this.items.Clear();
        this.itemShop.Clear();

        Save();

        PlayerPrefs.DeleteKey("wonLastRound");
        PlayerPrefs.SetInt("isEndless", 1);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void DeleteNoSceneChange()
    {
        this.isEnemyBossPrev = false;
        this.isEnemyBossNext = false;
        this.gold = 0;
        this.round = -1;
        this.shoprerolls = 2;
        this.wonLastRound = 1;
        this.generateShop = true;
        this.isShopOpen = false;
        this.items.Clear();
        this.itemShop.Clear();

        Save();

        PlayerPrefs.DeleteKey("wonLastRound");
    }
}
