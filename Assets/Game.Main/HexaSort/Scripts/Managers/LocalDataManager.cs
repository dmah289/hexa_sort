using HexaSort.UI.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.Managers
{
    public static class LocalDataManager
    {
        public static float CoinAmount
        {
            get => PlayerPrefs.GetFloat(ConstantKey.CoinCountKey, 0);
            set => PlayerPrefs.SetFloat(ConstantKey.CoinCountKey, value);
        }
        
        public static int LevelIndex
        {
            get => PlayerPrefs.GetInt(ConstantKey.LevelIndexKey, 0);
            set => PlayerPrefs.SetInt(ConstantKey.LevelIndexKey, value);
        }
        
        public static int CurrentLife
        {
            get => PlayerPrefs.GetInt(ConstantKey.CurLifeKey, LifeSystem.MAX_LIVES);
            set => PlayerPrefs.SetInt(ConstantKey.CurLifeKey, value);
        }
        
        public static string LastLifeSaveTime
        {
            get => PlayerPrefs.GetString(ConstantKey.LastSaveTimeKey);
            set => PlayerPrefs.SetString(ConstantKey.LastSaveTimeKey, value);
        }
        
        public static float LastCountdownRemaining
        {
            get => PlayerPrefs.GetFloat(ConstantKey.LastCountdownRemainingKey, 0);
            set => PlayerPrefs.SetFloat(ConstantKey.LastCountdownRemainingKey, value);
        }
    }
}