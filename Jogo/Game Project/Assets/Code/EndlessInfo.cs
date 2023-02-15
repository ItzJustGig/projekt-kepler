using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndlessInfo : MonoBehaviour
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

    public int wonLastRound;
    public bool generateShop;
    public bool isShopOpen;
    public bool wasPassUsed;
    public bool hasRested;

    public int gold;
    public int level;
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

        this.isEnemyChampPrev = data.isEnemyChampPrev;
        this.enemyIdPrev = data.enemyIdPrev;
        this.enemyLevelPrev = data.enemyLevelPrev;
        this.isEnemyBossPrev = data.isEnemyBossPrev;
        this.isEnemyChampNext = data.isEnemyChampNext;
        this.enemyIdNext = data.enemyIdNext;
        this.enemyLevelNext = data.enemyLevelNext;
        this.isEnemyBossNext = data.isEnemyBossNext;
        this.isPlayerChamp = data.isPlayerChamp;
        this.playerId = data.playerId;
        this.playerHp = data.playerHp;
        this.playerMn = data.playerMn;
        this.playerSta = data.playerSta;
        this.playerSan = data.playerSan;
        this.playerUlt = data.playerUlt;
        this.gold = data.gold;
        this.level = data.level;
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

        this.isEnemyChampPrev = false;
        this.enemyIdPrev = -1;
        this.enemyLevelPrev = 0;
        this.isEnemyBossPrev = false;
        this.isEnemyChampNext = false;
        this.enemyIdNext = -1;
        this.enemyLevelNext = 0;
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
        this.level = 0;
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
        this.isEnemyChampPrev = false;
        this.enemyIdPrev = -1;
        this.enemyLevelPrev = 0;
        this.isEnemyBossPrev = false;
        this.isEnemyChampNext = false;
        this.enemyIdNext = -1;
        this.enemyLevelNext = 0;
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
        this.level = 0;
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
