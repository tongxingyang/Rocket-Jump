using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{

    static int _jumpAmount;
    static int _deathAmount;
    static int _fallAmount;

    static int _jumpAmountCurrentGame;
    static int _deathAmountCurrentGame;
    static int _fallAmountCurrentGame;

    public static void LoadStats()
    {
        /*JumpAmount = int.Parse(FileManager.LoadData("stats", "jumpAmount"));
        DeathAmount = int.Parse(FileManager.LoadData("stats", "deathAmount"));
        FallAmount = int.Parse(FileManager.LoadData("stats", "fallAmount"));

        JumpAmountCurrentGame = int.Parse(FileManager.LoadData("currentGame", "jumpAmount"));
        DeathAmountCurrentGame = int.Parse(FileManager.LoadData("currentGame", "deathAmount"));
        FallAmountCurrentGame = int.Parse(FileManager.LoadData("currentGame", "fallAmount"));*/
    }

    public static void SaveStats()
    {
        FileManager.SaveData("stats/jumpAmount", JumpAmount);
        FileManager.SaveData("stats/deathAmount", DeathAmount);
        FileManager.SaveData("stats/fallAmount", FallAmount);

        FileManager.SaveData("currentGame/jumpAmount", JumpAmountCurrentGame);
        FileManager.SaveData("currentGame/deathAmount", DeathAmountCurrentGame);
        FileManager.SaveData("currentGame/fallAmount", FallAmountCurrentGame);
    }

    public static void UnlockAchievement(string apiName)
    {
        if (!SteamManager.Initialized) { return; }

        SteamUserStats.SetAchievement(apiName);

        SteamUserStats.StoreStats();
    }

    public static void CheckWiseMovement()
    {
        for (int i = 0; i < 3; i++)
        {
            if (int.Parse(FileManager.LoadData("stats/wiseMovement" + i)) == 0)
            {
                return;
            }
        }
        UnlockAchievement("NEW_ACHIEVEMENT_1_11");
    }

    public static void CheckSecretRoom()
    {
        for(int i = 0; i < 4; i++)
        {
            if (int.Parse(FileManager.LoadData("stats/secretRoom" + i)) == 0)
            {
                return;
            }
        }
        UnlockAchievement("NEW_ACHIEVEMENT_1_9");
    }

    public static void CheckPlayerSkin()
    {
        for (int i = 0; i < 12; i++)
        {
            if (int.Parse(FileManager.LoadData("stats/skinObj" + i)) == 0)
            {
                return;
            }
        }
        UnlockAchievement("NEW_ACHIEVEMENT_1_10");
    }

    public static int JumpAmount
    {
        set
        {
            _jumpAmount = value;

            if (_jumpAmount >= 250)
            {
                UnlockAchievement("NEW_ACHIEVEMENT_1_1");
            }
            if (_jumpAmount >= 1000)
            {
                UnlockAchievement("NEW_ACHIEVEMENT_1_2");
            }
        }
        get
        {
            return _jumpAmount;
        }
    }
    public static int DeathAmount
    {
        set
        {
            _deathAmount = value;
            if (_deathAmount >= 100)
            {
                UnlockAchievement("NEW_ACHIEVEMENT_1_4");
            }
            if (_deathAmount >= 250)
            {
                UnlockAchievement("NEW_ACHIEVEMENT_1_5");
            }
        }
        get
        {
            return _deathAmount;
        }
    }
    public static int FallAmount
    {
        set
        {
            _fallAmount = value;

            if (_fallAmount >= 250)
            {
                UnlockAchievement("NEW_ACHIEVEMENT_1_3");
            }
        }
        get
        {
            return _fallAmount;
        }
    }
    public static int JumpAmountCurrentGame
    {
        set
        {
            _jumpAmountCurrentGame = value;
        }
        get
        {
            return _jumpAmountCurrentGame;
        }
    }
    public static int DeathAmountCurrentGame
    {
        set
        {
            _deathAmountCurrentGame = value;
        }
        get
        {
            return _deathAmountCurrentGame;
        }
    }
    public static int FallAmountCurrentGame
    {
        set
        {
            _fallAmountCurrentGame = value;
        }
        get
        {
            return _fallAmountCurrentGame;
        }
    }
}
