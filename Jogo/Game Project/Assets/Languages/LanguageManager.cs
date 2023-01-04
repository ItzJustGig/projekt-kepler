using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private List<TextAsset> languages;

    [Serializable]
    public class Json
    {
        public Language language;
        public Gui gui;
        public Stats stats;
        public Class classe;
        public Charc charc;
        public ShowDetail showdetail;
        public Effect effect;
        public Moves moves;
        public Passive passive;
        public Items items;
        public Summon summon;

        public string GetStuff(string main, string sec, string tri)
        {
            string returns;
            switch (main)
            {
                case "language":
                    returns = language.Get_Language(sec);
                    break;
                case "gui":
                    returns = gui.Get_Gui(sec, tri);
                    break;
                case "stats":
                    returns = stats.Get_Stats(sec, tri);
                    break;
                case "class":
                    returns = classe.Get_Class(sec);
                    break;
                case "charc":
                    returns = charc.Get_Charc(sec, tri);
                    break;
                case "showdetail":
                    returns = showdetail.Get_ShowDetail(sec);
                    break;
                case "effect":
                    returns = effect.Get_Effect(sec, tri);
                    break;
                case "moves":
                    returns = moves.Get_Moves(sec, tri);
                    break;
                case "passive":
                    returns = passive.Get_Passive(sec, tri);
                    break;
                case "items":
                    returns = items.Get_Item(sec, tri);
                    break;
                case "summon":
                    returns = summon.Get_Summon(sec, tri);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Language
    {
        public string code;
        public string name;

        public string Get_Language(string a)
        {
            string returns;
            switch (a)
            {
                case "code":
                    returns = code;
                    break;
                case "name":
                    returns = name;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Gui
    {
        public Gui_Button button;
        public Gui_Text text;

        public string Get_Gui(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "button":
                    returns = button.Get_ButtonCont(b);
                    break;
                case "text":
                    returns = text.Get_TextCont(b);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Stats
    {
        public Stats_Info name;
        public Stats_Info desc;

        public string Get_Stats(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "name":
                    returns = name.Get_StatCont(b);
                    break;
                case "desc":
                    returns = desc.Get_StatCont(b);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Class
    {
        public string none;
        public string tank;
        public string assassin;
        public string sourcerer;
        public string marksman;
        public string support;
        public string duelist;
        public string brawler;
        public string vanguard;
        public string summoner;
        public string enchanter;

        public string Get_Class(string a)
        {
            string returns;
            switch (a)
            {
                case "none":
                    returns = none;
                    break;
                case "tank":
                    returns = tank;
                    break;
                case "assassin":
                    returns = assassin;
                    break;
                case "sourcerer":
                    returns = sourcerer;
                    break;
                case "marksman":
                    returns = marksman;
                    break;
                case "support":
                    returns = support;
                    break;
                case "duelist":
                    returns = duelist;
                    break;
                case "brawler":
                    returns = brawler;
                    break;
                case "vanguard":
                    returns = vanguard;
                    break;
                case "summoner":
                    returns = summoner;
                    break;
                case "enchanter":
                    returns = enchanter;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Charc
    {
        public Charc_Info name;
        public Charc_Info title;

        public string Get_Charc(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "name":
                    returns = name.Get_CharcCont(b);
                    break;
                case "title":
                    returns = title.Get_CharcCont(b);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class ShowDetail
    {
        public string user;
        public string enemy;
        public string statscale;
        public string healfromdmg;
        public string dealphysicdmg;
        public string dealmagicdmg;
        public string dealtruedmg;
        public string dealsanitydmg;
        public string takephysicdmg;
        public string takemagicdmg;
        public string taketruedmg;
        public string takesanitydmg;
        public string heal;
        public string healmana;
        public string healstamina;
        public string healsanity;
        public string shield;
        public string bonuscritchance;
        public string bonuscritdmg;
        public string blockphysic;
        public string blockmagic;
        public string blockranged;
        public string hitimes;
        public string priority;
        public string cantuse;
        public string cantphysical;
        public string cantmagic;
        public string cantranged;
        public string cantsupport;
        public string cantenchant;
        public string cantdefence;
        public string cantsummon;
        public string eachturn;
        public string onend;
        public string increasedmg;
        public string chancetostop;
        public string chancetostopoptional;
        public string chancetostatmod;
        public string statmodtime;
        public string statmodstat;
        public string statmod;
        public string statmodwho;
        public string physicdmg;
        public string magicdmg;
        public string truedmg;
        public string alldmg;
        public string sanitydmg;
        public string healdmg;
        public string healmanadmg;
        public string healstaminadmg;
        public string healsanitydmg;
        public string shielddmg;
        public string inflicteffect;
        public string overtime;
        public string as_;
        public string of;
        public string and;
        public string uses;

        public string Get_ShowDetail(string a)
        {
            string returns;
            switch (a)
            {
                case "user":
                    returns = user;
                    break;
                case "enemy":
                    returns = enemy;
                    break;
                case "statscale":
                    returns = statscale;
                    break;
                case "healfromdmg":
                    returns = healfromdmg;
                    break;
                case "dealphysicdmg":
                    returns = dealphysicdmg;
                    break;
                case "dealmagicdmg":
                    returns = dealmagicdmg;
                    break;
                case "dealtruedmg":
                    returns = dealtruedmg;
                    break;
                case "dealsanitydmg":
                    returns = dealsanitydmg;
                    break;
                case "takephysicdmg":
                    returns = takephysicdmg;
                    break;
                case "takemagicdmg":
                    returns = takemagicdmg;
                    break;
                case "taketruedmg":
                    returns = taketruedmg;
                    break;
                case "takesanitydmg":
                    returns = takesanitydmg;
                    break;
                case "heal":
                    returns = heal;
                    break;
                case "healmana":
                    returns = healmana;
                    break;
                case "healstamina":
                    returns = healstamina;
                    break;
                case "healsanity":
                    returns = healsanity;
                    break;
                case "shield":
                    returns = shield;
                    break;
                case "bonuscritchance":
                    returns = bonuscritchance;
                    break;
                case "bonuscritdmg":
                    returns = bonuscritdmg;
                    break;
                case "blockphysic":
                    returns = blockphysic;
                    break;
                case "blockmagic":
                    returns = blockmagic;
                    break;
                case "blockranged":
                    returns = blockranged;
                    break;
                case "hitimes":
                    returns = hitimes;
                    break;
                case "priority":
                    returns = priority;
                    break;
                case "cantuse":
                    returns = cantuse;
                    break;
                case "cantphysical":
                    returns = cantphysical;
                    break;
                case "cantmagic":
                    returns = cantmagic;
                    break;
                case "cantsupport":
                    returns = cantsupport;
                    break;
                case "cantranged":
                    returns = cantranged;
                    break;
                case "cantdefence":
                    returns = cantdefence;
                    break;
                case "cantsummon":
                    returns = cantsummon;
                    break;
                case "cantenchant":
                    returns = cantenchant;
                    break;
                case "eachturn":
                    returns = eachturn;
                    break;
                case "onend":
                    returns = onend;
                    break;
                case "increasedmg":
                    returns = increasedmg;
                    break;
                case "chancetostop":
                    returns = chancetostop;
                    break;
                case "chancetostopoptional":
                    returns = chancetostopoptional;
                    break;
                case "chancetostatmod":
                    returns = chancetostatmod;
                    break;
                case "statmodstat":
                    returns = statmodstat;
                    break;
                case "statmodtime":
                    returns = statmodtime;
                    break;
                case "statmod":
                    returns = statmod;
                    break;
                case "statmodwho":
                    returns = statmodwho;
                    break;
                case "physicdmg":
                    returns = physicdmg;
                    break;
                case "magicdmg":
                    returns = magicdmg;
                    break;
                case "truedmg":
                    returns = truedmg;
                    break;
                case "alldmg":
                    returns = alldmg;
                    break;
                case "sanitydmg":
                    returns = sanitydmg;
                    break;
                case "healdmg":
                    returns = healdmg;
                    break;
                case "healmanadmg":
                    returns = healmanadmg;
                    break;
                case "healstaminadmg":
                    returns = healstaminadmg;
                    break;
                case "healsanitydmg":
                    returns = healsanitydmg;
                    break;
                case "shielddmg":
                    returns = shielddmg;
                    break;
                case "inflicteffect":
                    returns = inflicteffect;
                    break;
                case "overtime":
                    returns = overtime;
                    break;
                case "as_":
                    returns = as_;
                    break;
                case "of":
                    returns = of;
                    break;
                case "and":
                    returns = and;
                    break;
                case "uses":
                    returns = uses;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Gui_Button
    {
        public string fight;
        public string info;
        public string quit;
        public string back;
        public string select;
        public string moves;
        public string hide;
        public string ultimate;
        public string overview;
        public string stats;
        public string cancel;
        public string endless;
        public string shop;
        public string start;
        public string leave;
        public string delsave;
        public string inventory;
        public string items;
        public string forfeit;
        public string cancelforfeit;

        public string Get_ButtonCont(string a)
        {
            string returns;
            switch (a)
            {
                case "fight":
                    returns = fight;
                    break;
                case "info":
                    returns = info;
                    break;
                case "quit":
                    returns = quit;
                    break;
                case "back":
                    returns = back;
                    break;
                case "select":
                    returns = select;
                    break;
                case "moves":
                    returns = moves;
                    break;
                case "hide":
                    returns = hide;
                    break;
                case "ultimate":
                    returns = ultimate;
                    break;
                case "overview":
                    returns = overview;
                    break;
                case "stats":
                    returns = stats;
                    break;
                case "cancel":
                    returns = cancel;
                    break;
                case "endless":
                    returns = endless;
                    break;
                case "shop":
                    returns = shop;
                    break;
                case "start":
                    returns = start;
                    break;
                case "leave":
                    returns = leave;
                    break;
                case "delsave":
                    returns = delsave;
                    break;
                case "inventory":
                    returns = inventory;
                    break;
                case "items":
                    returns = items;
                    break;
                case "forfeit":
                    returns = forfeit;
                    break;
                case "cancelforfeit":
                    returns = cancelforfeit;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Gui_Text
    {
        public string game;
        public string passives;
        public string moves;
        public string overview;
        public string wantfight;
        public string cantmove;
        public string usedmove;
        public string dodge;
        public string miss;
        public string win;
        public string defeat;
        public string choosemove;
        public string turn;
        public string staminatired;
        public string shield;
        public string ultimate;
        public string misspop;
        public string dodgepop;
        public string physicdmgdealt;
        public string magicdmgdealt;
        public string truedmgdealt;
        public string sanitydmgdealt;
        public string totaldmgdealt;
        public string physicdmgtaken;
        public string magicdmgtaken;
        public string truedmgtaken;
        public string sanitydmgtaken;
        public string totaldmgtaken;
        public string physicdmgmiti;
        public string magicdmgmiti;
        public string totaldmgmiti;
        public string healdone;
        public string manarecov;
        public string staminarecov;
        public string sanityrecov;
        public string shielddone;
        public string cd;
        public string sta;
        public string mn;
        public string gold;
        public string round;
        public string shop;
        public string goldinicial;
        public string leavetitle;
        public string leavetext;

        public string Get_TextCont(string a)
        {
            string returns;
            switch (a)
            {
                case "game":
                    returns = game;
                    break;
                case "moves":
                    returns = moves;
                    break;
                case "passives":
                    returns = passives;
                    break;
                case "overview":
                    returns = overview;
                    break;
                case "wantfight":
                    returns = wantfight;
                    break;
                case "cantmove":
                    returns = cantmove;
                    break;
                case "usedmove":
                    returns = usedmove;
                    break;
                case "dodge":
                    returns = dodge;
                    break;
                case "miss":
                    returns = miss;
                    break;
                case "win":
                    returns = win;
                    break;
                case "defeat":
                    returns = defeat;
                    break;
                case "choosemove":
                    returns = choosemove;
                    break;
                case "turn":
                    returns = turn;
                    break;
                case "staminatired":
                    returns = staminatired;
                    break;
                case "shield":
                    returns = shield;
                    break;
                case "ultimate":
                    returns = ultimate;
                    break;
                case "misspop":
                    returns = misspop;
                    break;
                case "dodgepop":
                    returns = dodgepop;
                    break;
                case "physicdmgdealt":
                    returns = physicdmgdealt;
                    break;
                case "magicdmgdealt":
                    returns = magicdmgdealt;
                    break;
                case "truedmgdealt":
                    returns = truedmgdealt;
                    break;
                case "sanitydmgdealt":
                    returns = sanitydmgdealt;
                    break;
                case "totaldmgdealt":
                    returns = totaldmgdealt;
                    break;
                case "physicdmgtaken":
                    returns = physicdmgtaken;
                    break;
                case "magicdmgtaken":
                    returns = magicdmgtaken;
                    break;
                case "truedmgtaken":
                    returns = truedmgtaken;
                    break;
                case "sanitydmgtaken":
                    returns = sanitydmgtaken;
                    break;
                case "totaldmgtaken":
                    returns = totaldmgtaken;
                    break;
                case "physicdmgmiti":
                    returns = physicdmgmiti;
                    break;
                case "magicdmgmiti":
                    returns = magicdmgmiti;
                    break;
                case "totaldmgmiti":
                    returns = totaldmgmiti;
                    break;
                case "healdone":
                    returns = healdone;
                    break;
                case "manarecov":
                    returns = manarecov;
                    break;
                case "staminarecov":
                    returns = staminarecov;
                    break;
                case "sanityrecov":
                    returns = sanityrecov;
                    break;
                case "shielddone":
                    returns = shielddone;
                    break;
                case "cd":
                    returns = cd;
                    break;
                case "sta":
                    returns = sta;
                    break;
                case "mn":
                    returns = mn;
                    break;
                case "gold":
                    returns = gold;
                    break;
                case "round":
                    returns = round;
                    break;
                case "shop":
                    returns = shop;
                    break;
                case "goldinicial":
                    returns = goldinicial;
                    break;
                case "leavetitle":
                    returns = leavetitle;
                    break;
                case "leavetext":
                    returns = leavetext;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Passive
    {
        public string title;
        public string titlemove;
        public Passive_Info name;
        public Passive_Info desc;

        public string Get_Passive(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "title":
                    returns = title;
                    break;
                case "titlemove":
                    returns = titlemove;
                    break;
                case "name":
                    returns = name.Get_Cont(b);
                    break;
                case "desc":
                    returns = desc.Get_Cont(b);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Passive_Info
    {
        public string bloodbath;
        public string bullsrage;
        public string courage;
        public string dreadofthesupernatural;
        public string ecolocation;
        public string fearsmell;
        public string galeglide;
        public string lastbreath;
        public string magicremains;
        public string manathirst;
        public string musicup;
        public string onewiththeshadows;
        public string perfectshooter;
        public string plasmablade;
        public string roughskin;
        public string sharpshooter;
        public string strongmind;
        public string vendetta;
        public string weakbody;
        public string wildinstinct;
        public string phantomhand;
        public string bloodpumping;
        public string huntingseason;
        public string blazingfists;
        public string fighterinstinct;
        public string successoroffire;
        public string zenmode;
        public string baby;
        public string weak;
        public string normal;
        public string strong;
        public string superstrong;
        public string legendary;
        public string boss;
        public string toxicteeth;
        public string gravitybelt;
        public string manasword;
        public string manascepter;
        public string shadowdagger;
        public string spectralring;
        public string huntersdirk;
        public string gravitychange;
        public string prismaticstaff;
        public string mechashield;
        public string combatrepair;
        public string magicwand;
        public string crossbow;

        public string Get_Cont(string a)
        {
            string returns;

            switch (a)
            {
                case "bloodbath":
                    returns = bloodbath;
                    break;
                case "bullsrage":
                    returns = bullsrage;
                    break;
                case "courage":
                    returns = courage;
                    break;
                case "dreadofthesupernatural":
                    returns = dreadofthesupernatural;
                    break;
                case "ecolocation":
                    returns = ecolocation;
                    break;
                case "fearsmell":
                    returns = fearsmell;
                    break;
                case "galeglide":
                    returns = galeglide;
                    break;
                case "lastbreath":
                    returns = lastbreath;
                    break;
                case "magicremains":
                    returns = magicremains;
                    break;
                case "manathirst":
                    returns = manathirst;
                    break;
                case "musicup":
                    returns = musicup;
                    break;
                case "onewiththeshadows":
                    returns = onewiththeshadows;
                    break;
                case "perfectshooter":
                    returns = perfectshooter;
                    break;
                case "plasmablade":
                    returns = plasmablade;
                    break;
                case "roughskin":
                    returns = roughskin;
                    break;
                case "sharpshooter":
                    returns = sharpshooter;
                    break;
                case "strongmind":
                    returns = strongmind;
                    break;
                case "vendetta":
                    returns = vendetta;
                    break;
                case "weakbody":
                    returns = weakbody;
                    break;
                case "wildinstinct":
                    returns = wildinstinct;
                    break;
                case "phantomhand":
                    returns = phantomhand;
                    break;
                case "bloodpumping":
                    returns = bloodpumping;
                    break;
                case "huntingseason":
                    returns = huntingseason;
                    break;
                case "blazingfists":
                    returns = blazingfists;
                    break;
                case "fighterinstinct":
                    returns = fighterinstinct;
                    break;
                case "successoroffire":
                    returns = successoroffire;
                    break;
                case "zenmode":
                    returns = zenmode;
                    break;
                case "baby":
                    returns = baby;
                    break;
                case "weak":
                    returns = weak;
                    break;
                case "normal":
                    returns = normal;
                    break;
                case "strong":
                    returns = strong;
                    break;
                case "superstrong":
                    returns = superstrong;
                    break;
                case "legendary":
                    returns = legendary;
                    break;
                case "boss":
                    returns = boss;
                    break;
                case "toxicteeth":
                    returns = toxicteeth;
                    break;
                case "gravitybelt":
                    returns = gravitybelt;
                    break;
                case "manasword":
                    returns = manasword;
                    break;
                case "manascepter":
                    returns = manascepter;
                    break;
                case "shadowdagger":
                    returns = shadowdagger;
                    break;
                case "spectralring":
                    returns = spectralring;
                    break;
                case "huntersdirk":
                    returns = huntersdirk;
                    break;
                case "gravitychange":
                    returns = gravitychange;
                    break;
                case "prismaticstaff":
                    returns = prismaticstaff;
                    break;
                case "mechashield":
                    returns = mechashield;
                    break;
                case "combatrepair":
                    returns = combatrepair;
                    break;
                case "magicwand":
                    returns = magicwand;
                    break;
                case "crossbow":
                    returns = crossbow;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Stats_Info
    {
        public string hp;
        public string maxhp;
        public string curhp;
        public string misshp;
        public string hpregen;
        public string mana;
        public string maxmana;
        public string missmana;
        public string curmana;
        public string manaregen;
        public string stamina;
        public string maxstamina;
        public string missstamina;
        public string curstamina;
        public string staminaregen;
        public string sanity;
        public string maxsanity;
        public string misssanity;
        public string cursanity;
        public string def;
        public string magicdef;
        public string attack;
        public string magicpower;
        public string critchance;
        public string critdmg;
        public string movspeed;
        public string timing;
        public string lifesteal;
        public string evasion;
        public string accuracy;
        public string armourpen;
        public string ultimate;
        public string ultrate;
        public string attackpower;
        public string manacost;
        public string staminacost;
        public string healbonus;
        public string shieldbonus;
        public string size;

        public string Get_StatCont(string a)
        {
            string returns;
            switch (a)
            {
                case "hp":
                    returns = hp;
                    break;
                case "curhp":
                    returns = curhp;
                    break;
                case "maxhp":
                    returns = maxhp;
                    break;
                case "misshp":
                    returns = misshp;
                    break;
                case "hpregen":
                    returns = hpregen;
                    break;
                case "mana":
                    returns = mana;
                    break;
                case "maxmana":
                    returns = maxmana;
                    break;
                case "missmana":
                    returns = missmana;
                    break;
                case "curmana":
                    returns = curmana;
                    break;
                case "manaregen":
                    returns = manaregen;
                    break;
                case "stamina":
                    returns = stamina;
                    break;
                case "maxstamina":
                    returns = maxstamina;
                    break;
                case "missstamina":
                    returns = missstamina;
                    break;
                case "curstamina":
                    returns = curstamina;
                    break;
                case "staminaregen":
                    returns = staminaregen;
                    break;
                case "sanity":
                    returns = sanity;
                    break;
                case "maxsanity":
                    returns = maxsanity;
                    break;
                case "misssanity":
                    returns = misssanity;
                    break;
                case "cursanity":
                    returns = cursanity;
                    break;
                case "def":
                    returns = def;
                    break;
                case "magicdef":
                    returns = magicdef;
                    break;
                case "attack":
                    returns = attack;
                    break;
                case "magicpower":
                    returns = magicpower;
                    break;
                case "critchance":
                    returns = critchance;
                    break;
                case "critdmg":
                    returns = critdmg;
                    break;
                case "movspeed":
                    returns = movspeed;
                    break;
                case "timing":
                    returns = timing;
                    break;
                case "lifesteal":
                    returns = lifesteal;
                    break;
                case "evasion":
                    returns = evasion;
                    break;
                case "accuracy":
                    returns = accuracy;
                    break;
                case "armourpen":
                    returns = armourpen;
                    break;
                case "ultimate":
                    returns = ultimate;
                    break;
                case "ultrate":
                    returns = ultrate;
                    break;
                case "attackpower":
                    returns = attackpower;
                    break;
                case "manacost":
                    returns = manacost;
                    break;
                case "staminacost":
                    returns = staminacost;
                    break;
                case "healbonus":
                    returns = healbonus;
                    break;
                case "shieldbonus":
                    returns = shieldbonus;
                    break;
                case "size":
                    returns = size;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Charc_Info
    {
        public string alex;
        public string leowind;
        public string bonsour;
        public string isadoe;
        public string shine;
        public string sandew;
        public string william;
        public string hestia;
        public string icer;
        public string babymimi;
        public string seedling;
        public string blob;
        public string worm;
        public string childinfernus;
        public string goblin;
        public string haunteddoll;
        public string mimi;
        public string crobus;
        public string skulltroop;
        public string aventis;
        public string blazamander;
        public string imp;
        public string phantom;
        public string viper;
        public string basilisk;
        public string golem;
        public string predatormantis;

        public string Get_CharcCont(string a)
        {
            string returns;
            switch (a)
            {
                case "alex":
                    returns = alex;
                    break;
                case "leowind":
                    returns = leowind;
                    break;
                case "isadoe":
                    returns = isadoe;
                    break;
                case "bonsour":
                    returns = bonsour;
                    break;
                case "william":
                    returns = william;
                    break;
                case "sandew":
                    returns = sandew;
                    break;
                case "shine":
                    returns = shine;
                    break;
                case "hestia":
                    returns = hestia;
                    break;
                case "icer":
                    returns = icer;
                    break;
                case "babymimi":
                    returns = babymimi;
                    break;
                case "seedling":
                    returns = seedling;
                    break;
                case "blob":
                    returns = blob;
                    break;
                case "worm":
                    returns = worm;
                    break;
                case "childinfernus":
                    returns = childinfernus;
                    break;
                case "goblin":
                    returns = goblin;
                    break;
                case "haunteddoll":
                    returns = haunteddoll;
                    break;
                case "mimi":
                    returns = mimi;
                    break;
                case "crobus":
                    returns = crobus;
                    break;
                case "skulltroop":
                    returns = skulltroop;
                    break;
                case "aventis":
                    returns = aventis;
                    break;
                case "blazamander":
                    returns = blazamander;
                    break;
                case "imp":
                    returns = imp;
                    break;
                case "phantom":
                    returns = phantom;
                    break;
                case "viper":
                    returns = viper;
                    break;
                case "basilisk":
                    returns = basilisk;
                    break;
                case "golem":
                    returns = golem;
                    break;
                case "predatormantis":
                    returns = predatormantis;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Effect
    {
        public string title;
        public Effect_Info name;
        public Effect_Info cancelmsg;
        public Effect_Info desc;

        public string Get_Effect(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "title":
                    returns = title;
                    break;
                case "name":
                    returns = name.Get_EffectInfo(b);
                    break;
                case "cancelmsg":
                    returns = cancelmsg.Get_EffectInfo(b);
                    break;
                case "desc":
                    returns = desc.Get_EffectInfo(b);
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Effect_Info
    {
        public string alr;
        public string bli;
        public string bld;
        public string brn;
        public string cfs;
		public string crp;
        public string fea;
        public string frz;
        public string hau;
        public string lck;
        public string plz;
        public string psn;
        public string rot;
        public string slc;
        public string slp;
        public string stn;
        public string trd;
        public string ulk;
        public string sch;

        public string Get_EffectInfo(string a)
        {
            string returns;
            switch (a)
            {
                case "alr":
                    returns = alr;
                    break;
                case "bli":
                    returns = bli;
                    break;
                case "bld":
                    returns = bld;
                    break;
                case "brn":
                    returns = brn;
                    break;
                case "cfs":
                    returns = cfs;
                    break;
                case "crp":
                    returns = crp;
                    break;
                case "fea":
                    returns = fea;
                    break;
                case "frz":
                    returns = frz;
                    break;
                case "hau":
                    returns = hau;
                    break;
                case "lck":
                    returns = lck;
                    break;
                case "plz":
                    returns = plz;
                    break;
                case "psn":
                    returns = psn;
                    break;
                case "rot":
                    returns = rot;
                    break;
                case "slc":
                    returns = slc;
                    break;
                case "slp":
                    returns = slp;
                    break;
                case "stn":
                    returns = stn;
                    break;
                case "trd":
                    returns = trd;
                    break;
                case "ulk":
                    returns = ulk;
                    break;
                case "sch":
                    returns = sch;
                    break;
                default:
                    returns = "null";
                    break;
            }

            return returns;
        }
    }

    [Serializable]
    public class Moves
    {
        public Move_Type type;
        public string basicattack;
        public string recovmana;
        public string accurateshot;
        public string agility;
        public string assaultshot;
        public string bite;
        public string blasthammer;
        public string blindingcurse;
        public string blooddrink;
        public string bonecrush;
        public string bullscharge;
        public string manabarrier;
        public string cursedrose;
        public string darkaura;
        public string defstand;
        public string dig;
        public string doublepunch;
        public string duskblade;
        public string earthquake;
        public string enchantedfangs;
        public string explosiveneedles;
        public string flash;
        public string focuswind;
        public string gravaxekick;
        public string gravchanger;
        public string healingapple;
        public string ironfangs;
        public string jawsofthebeast;
        public string keeneye;
        public string leechshot;
        public string lightningsprint;
        public string lowstealth;
        public string magicshot;
        public string magicslash;
        public string manablast;
        public string mudtoss;
        public string megahowl;
        public string phantomhand;
        public string plasmacut;
        public string poisonshot;
        public string predatorjaws;
        public string psychic;
        public string quickslash;
        public string refreshbreeze;
        public string sandattack;
        public string shadowimage;
        public string shadowsneak;
        public string shadowwarp;
        public string shockwave;
        public string silentshot;
        public string spindash;
        public string spinkick;
        public string stab;
        public string stomp;
        public string taunt;
        public string telekinesis;
        public string tornado;
        public string warmup;
        public string windblow;
        public string windcut;
        public string closecombat;
        public string firebreath;
        public string calmmind;
        public string blazingfists;
        public string blazinguppercut;
        public string blazingstrike;
        public string flamingaura;
        public string tackle;
        public string poke;
        public string photosynthesis;
        public string toughroots;
        public string headbutt;
        public string faketears;
        public string pound;
        public string spores;
        public string splash;
        public string gunkshot;
        public string hauntingscream;
        public string jumpscare;
        public string shadowclaws;
        public string willowisp;
        public string energyball;
        public string boneclub;
        public string slash;
        public string sandcover;
        public string bonetoss;
        public string harden;
        public string grip;
        public string sing;
        public string shriek;
        public string peck;
        public string preyattack;
        public string spit;
        public string taillash;
        public string fireball;
        public string glare;
        public string toxicteeth;
        public string triggerupdate;
        public string suddenjab;
        public string crosscut;
        public string blizzard;
        public string forcefield;
        public string icepunch;
        public string iceshards;
        public string turret;
        public string frostbarrier;

        public string bloodpressure;
        public string enchdeadlyshadow;
        public string gigaimpact;
        public string headshot;
        public string letthehuntbegin;
        public string lightspeedplasmacut;
        public string windyvalleytornado;
        public string feirydance;
        public string mechasuit;

        public string healthpotion;
        public string manapotion;
        public string ravensfeather;
        public string bigsteak;
        public string zap;
        public string huntersdirk;
        public string jewelofthedruid;
        public string shadowshroom;
        public string prismaticstaff;

        public string Get_Moves(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "type":
                    returns = type.Get_Move_Type(b);
                    break;
                case "basicattack":
                    returns = basicattack;
                    break;
                case "recovmana":
                    returns = recovmana;
                    break;
                case "accurateshot":
                    returns = accurateshot;
                    break;
                case "agility":
                    returns = agility;
                    break;
                case "assaultshot":
                    returns = assaultshot;
                    break;
                case "bite":
                    returns = bite;
                    break;
                case "blasthammer":
                    returns = blasthammer;
                    break;
                case "blindingcurse":
                    returns = blindingcurse;
                    break;
                case "blooddrink":
                    returns = blooddrink;
                    break;
                case "bonecrush":
                    returns = bonecrush;
                    break;
                case "bullscharge":
                    returns = bullscharge;
                    break;
                case "manabarrier":
                    returns = manabarrier;
                    break;
                case "cursedrose":
                    returns = cursedrose;
                    break;
                case "darkaura":
                    returns = darkaura;
                    break;
                case "defstand":
                    returns = defstand;
                    break;
                case "dig":
                    returns = dig;
                    break;
                case "doublepunch":
                    returns = doublepunch;
                    break;
                case "duskblade":
                    returns = duskblade;
                    break;
                case "earthquake":
                    returns = earthquake;
                    break;
                case "enchantedfangs":
                    returns = enchantedfangs;
                    break;
                case "explosiveneedles":
                    returns = explosiveneedles;
                    break;
                case "flash":
                    returns = flash;
                    break;
                case "focuswind":
                    returns = focuswind;
                    break;
                case "gravaxekick":
                    returns = gravaxekick;
                    break;
                case "gravchanger":
                    returns = gravchanger;
                    break;
                case "healingapple":
                    returns = healingapple;
                    break;
                case "ironfangs":
                    returns = ironfangs;
                    break;
                case "jawsofthebeast":
                    returns = jawsofthebeast;
                    break;
                case "keeneye":
                    returns = keeneye;
                    break;
                case "leechshot":
                    returns = leechshot;
                    break;
                case "lightningsprint":
                    returns = lightningsprint;
                    break;
                case "lowstealth":
                    returns = lowstealth;
                    break;
                case "magicshot":
                    returns = magicshot;
                    break;
                case "magicslash":
                    returns = magicslash;
                    break;
                case "manablast":
                    returns = manablast;
                    break;
                case "mudtoss":
                    returns = mudtoss;
                    break;
                case "megahowl":
                    returns = megahowl;
                    break;
                case "phantomhand":
                    returns = phantomhand;
                    break;
                case "plasmacut":
                    returns = plasmacut;
                    break;
                case "poisonshot":
                    returns = poisonshot;
                    break;
                case "predatorjaws":
                    returns = predatorjaws;
                    break;
                case "psychic":
                    returns = psychic;
                    break;
                case "quickslash":
                    returns = quickslash;
                    break;
                case "refreshbreeze":
                    returns = refreshbreeze;
                    break;
                case "sandattack":
                    returns = sandattack;
                    break;
                case "shadowimage":
                    returns = shadowimage;
                    break;
                case "shadowsneak":
                    returns = shadowsneak;
                    break;
                case "shadowwarp":
                    returns = shadowwarp;
                    break;
                case "shockwave":
                    returns = shockwave;
                    break;
                case "silentshot":
                    returns = silentshot;
                    break;
                case "spindash":
                    returns = spindash;
                    break;
                case "spinkick":
                    returns = spinkick;
                    break;
                case "stab":
                    returns = stab;
                    break;
                case "stomp":
                    returns = stomp;
                    break;
                case "taunt":
                    returns = taunt;
                    break;
                case "telekinesis":
                    returns = telekinesis;
                    break;
                case "tornado":
                    returns = tornado;
                    break;
                case "warmup":
                    returns = warmup;
                    break;
                case "windblow":
                    returns = windblow;
                    break;
                case "windcut":
                    returns = windcut;
                    break;
                case "closecombat":
                    returns = closecombat;
                    break;
                case "firebreath":
                    returns = firebreath;
                    break;
                case "calmmind":
                    returns = calmmind;
                    break;
                case "blazingfists":
                    returns = blazingfists;
                    break;
                case "blazinguppercut":
                    returns = blazinguppercut;
                    break;
                case "blazingstrike":
                    returns = blazingstrike;
                    break;
                case "flamingaura":
                    returns = flamingaura;
                    break;
                case "tackle":
                    returns = tackle;
                    break;
                case "poke":
                    returns = poke;
                    break;
                case "photosynthesis":
                    returns = photosynthesis;
                    break;
                case "toughroots":
                    returns = toughroots;
                    break;
                case "headbutt":
                    returns = headbutt;
                    break;
                case "faketears":
                    returns = faketears;
                    break;
                case "pound":
                    returns = pound;
                    break;
                case "spores":
                    returns = spores;
                    break;
                case "splash":
                    returns = splash;
                    break;
                case "gunkshot":
                    returns = gunkshot;
                    break;
                case "hauntingscream":
                    returns = hauntingscream;
                    break;
                case "jumpscare":
                    returns = jumpscare;
                    break;
                case "shadowclaws":
                    returns = shadowclaws;
                    break;
                case "willowisp":
                    returns = willowisp;
                    break;
                case "energyball":
                    returns = energyball;
                    break;
                case "boneclub":
                    returns = boneclub;
                    break;
                case "slash":
                    returns = slash;
                    break;
                case "sandcover":
                    returns = sandcover;
                    break;
                case "bonetoss":
                    returns = bonetoss;
                    break;
                case "harden":
                    returns = harden;
                    break;
                case "grip":
                    returns = grip;
                    break;
                case "sing":
                    returns = sing;
                    break;
                case "shriek":
                    returns = shriek;
                    break;
                case "peck":
                    returns = peck;
                    break;
                case "preyattack":
                    returns = preyattack;
                    break;
                case "spit":
                    returns = spit;
                    break;
                case "taillash":
                    returns = taillash;
                    break;
                case "fireball":
                    returns = fireball;
                    break;
                case "glare":
                    returns = glare;
                    break;
                case "toxicteeth":
                    returns = toxicteeth;
                    break;
                case "triggerupdate":
                    returns = triggerupdate;
                    break;
                case "suddenjab":
                    returns = suddenjab;
                    break;
                case "crosscut":
                    returns = crosscut;
                    break;
                case "blizzard":
                    returns = blizzard;
                    break;
                case "forcefield":
                    returns = forcefield;
                    break;
                case "icepunch":
                    returns = icepunch;
                    break;
                case "iceshards":
                    returns = iceshards;
                    break;
                case "turret":
                    returns = turret;
                    break;
                case "frostbarrier":
                    returns = frostbarrier;
                    break;
                case "bloodpressure":
                    returns = bloodpressure;
                    break;
                case "enchdeadlyshadow":
                    returns = enchdeadlyshadow;
                    break;
                case "gigaimpact":
                    returns = gigaimpact;
                    break;
                case "headshot":
                    returns = headshot;
                    break;
                case "letthehuntbegin":
                    returns = letthehuntbegin;
                    break;
                case "lightspeedplasmacut":
                    returns = lightspeedplasmacut;
                    break;
                case "windyvalleytornado":
                    returns = windyvalleytornado;
                    break;
                case "feirydance":
                    returns = feirydance;
                    break;
                case "mechasuit":
                    returns = mechasuit;
                    break;
                case "healthpotion":
                    returns = healthpotion;
                    break;
                case "manapotion":
                    returns = manapotion;
                    break;
                case "ravensfeather":
                    returns = ravensfeather;
                    break;
                case "bigsteak":
                    returns = bigsteak;
                    break;
                case "zap":
                    returns = zap;
                    break;
                case "huntersdirk":
                    returns = huntersdirk;
                    break;
                case "jewelofthedruid":
                    returns = jewelofthedruid;
                    break;
                case "shadowshroom":
                    returns = shadowshroom;
                    break;
                case "prismaticstaff":
                    returns = prismaticstaff;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Move_Type
    {
        public string physical;
        public string magic;
        public string ranged;
        public string enchant;
        public string support;
        public string defence;
        public string summon;

        public string Get_Move_Type(string a)
        {
            string returns;
            switch (a)
            {
                case "physical":
                    returns = physical;
                    break;
                case "magic":
                    returns = magic;
                    break;
                case "ranged":
                    returns = ranged;
                    break;
                case "enchant":
                    returns = enchant;
                    break;
                case "support":
                    returns = support;
                    break;
                case "defence":
                    returns = defence;
                    break;
                case "summon":
                    returns = summon;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

        [Serializable]
    public class Items
    {
        public string passive;
        public string active;
        public string stat;
        public string cd;
        public string uses;
        public string title;
        public Item_Name name;
        public Item_Move move;

        public string Get_Item(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "title":
                    returns = title;
                    break;
                case "passive":
                    returns = passive;
                    break;
                case "active":
                    returns = active;
                    break;
                case "stat":
                    returns = stat;
                    break;
                case "cd":
                    returns = cd;
                    break;
                case "uses":
                    returns = uses;
                    break;
                case "name":
                    returns = name.Get_ItemName(b);
                    break;
                case "move":
                    returns = move.Get_ItemMove(b);
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Item_Name
    {
        public string bigsteak;
        public string manacrystal;
        public string leathercloth;
        public string manapotion;
        public string healthpotion;
        public string rustyknife;
        public string leechingbranch;
        public string longbow;
        public string enchantedspear;
        public string sharpsword;
        public string magicwand;
        public string ravensfeather;
        public string zap;
        public string spectralring;
        public string gravitybelt;
        public string manasword;
        public string manascepter;
        public string shadowdagger;
        public string bookofdisaster;
        public string bootsoftherunner;
        public string shadowshroom;
        public string huntersdirk;
        public string jewelofthedruid;
        public string prismaticstaff;
        public string crossbow;

        public string Get_ItemName(string a)
        {
            string returns;
            switch (a)
            {
                case "bigsteak":
                    returns = bigsteak;
                    break;
                case "manacrystal":
                    returns = manacrystal;
                    break;
                case "leathercloth":
                    returns = leathercloth;
                    break;
                case "manapotion":
                    returns = manapotion;
                    break;
                case "healthpotion":
                    returns = healthpotion;
                    break;
                case "rustyknife":
                    returns = rustyknife;
                    break;
                case "leechingbranch":
                    returns = leechingbranch;
                    break;
                case "longbow":
                    returns = longbow;
                    break;
                case "enchantedspear":
                    returns = enchantedspear;
                    break;
                case "sharpsword":
                    returns = sharpsword;
                    break;
                case "magicwand":
                    returns = magicwand;
                    break;
                case "ravensfeather":
                    returns = ravensfeather;
                    break;
                case "zap":
                    returns = zap;
                    break;
                case "spectralring":
                    returns = spectralring;
                    break;
                case "gravitybelt":
                    returns = gravitybelt;
                    break;
                case "manasword":
                    returns = manasword;
                    break;
                case "manascepter":
                    returns = manascepter;
                    break;
                case "shadowdagger":
                    returns = shadowdagger;
                    break;
                case "bookofdisaster":
                    returns = bookofdisaster;
                    break;
                case "bootsoftherunner":
                    returns = bootsoftherunner;
                    break;
                case "shadowshroom":
                    returns = shadowshroom;
                    break;
                case "huntersdirk":
                    returns = huntersdirk;
                    break;
                case "jewelofthedruid":
                    returns = jewelofthedruid;
                    break;
                case "prismaticstaff":
                    returns = prismaticstaff;
                    break;
                case "crossbow":
                    returns = crossbow;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Item_Move
    {
        public string zap;
        public string manapotion;
        public string healthpotion;
        public string bigsteak;
        public string ravensfeather;
        public string huntersdirk;
        public string jewelofthedruid;
        public string shadowshroom;
        public string prismaticstaff;

        public string Get_ItemMove(string a)
        {
            string returns;
            switch (a)
            {
                case "zap":
                    returns = zap;
                    break;
                case "manapotion":
                    returns = manapotion;
                    break;
                case "healthpotion":
                    returns = healthpotion;
                    break;
                case "bigsteak":
                    returns = bigsteak;
                    break;
                case "ravensfeather":
                    returns = ravensfeather;
                    break;
                case "huntersdirk":
                    returns = huntersdirk;
                    break;
                case "jewelofthedruid":
                    returns = jewelofthedruid;
                    break;
                case "shadowshroom":
                    returns = shadowshroom;
                    break;
                case "prismaticstaff":
                    returns = prismaticstaff;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Summon
    {
        public string title;
        public string desc;
        public string descsum;
        public string cd;
        public string physic;
        public string magic;
        public string trued;
        public string heal;
        public string shield;
        public Summon_Name name;
        public string Get_Summon(string a, string b)
        {
            string returns;
            switch (a)
            {
                case "title":
                    returns = title;
                    break;
                case "desc":
                    returns = desc;
                    break;
                case "descsum":
                    returns = descsum;
                    break;
                case "cd":
                    returns = cd;
                    break;
                case "physic":
                    returns = physic;
                    break;
                case "magic":
                    returns = magic;
                    break;
                case "trued":
                    returns = trued;
                    break;
                case "heal":
                    returns = heal;
                    break;
                case "shield":
                    returns = shield;
                    break;
                case "name":
                    returns = name.Get_Summon_Name(b);
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    [Serializable]
    public class Summon_Name
    {
        public string fairy;
        public string turret;

        public string Get_Summon_Name(string a)
        {
            string returns;
            switch (a)
            {
                case "fairy":
                    returns = fairy;
                    break;
                case "turret":
                    returns = turret;
                    break;
                default:
                    returns = "null";
                    break;
            }
            return returns;
        }
    }

    public string GetText(string languageId, string main, string sec, string tri)
    {
        foreach (TextAsset a in languages)
        {
            string json = a.ToString();
            Json lang = JsonUtility.FromJson<Json>(json);
            
            if (lang.language.Get_Language("code") == languageId)
            {   
                return lang.GetStuff(main, sec, tri);
            }
        }
        return "null";
    }

    public string GetText(string languageId, string main, string sec)
    {
        foreach (TextAsset a in languages)
        {
            string json = a.ToString();
            Json lang = JsonUtility.FromJson<Json>(json);

            if (lang.language.Get_Language("code") == languageId)
            {
                return lang.GetStuff(main, sec, "");
            }
        }
        return "null";
    }
}