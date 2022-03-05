using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLanguage : MonoBehaviour
{
    public static int GetSteamLanguageID()
    {
        if (SteamManager.Initialized)
        {
            PlayerPrefs.SetInt("FirstLoad", 1);
            PlayerPrefs.Save();
            string steamLanguageApi = SteamUtils.GetSteamUILanguage();

            switch (steamLanguageApi)
            {
                case "english": default: return 0;
                case "polish": return 1;
                case "german": return 2;
                case "spanish": return 3;
                case "french": return 4;
            }
        }
        else return 0;
    }
}
