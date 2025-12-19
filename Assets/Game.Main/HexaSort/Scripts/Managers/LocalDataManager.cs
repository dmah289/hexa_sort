using HexaSort.Audio;
using HexaSort.UI.Loading.InGame;
using HexaSort.UI.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.Managers
{
    public static class LocalDataManager
    {
        public static float CoinAmount
        {
            // TODO : restore coin count
            //get => PlayerPrefs.GetFloat(ConstantKey.CoinCountKey, 0);
            get => 1000;
            set => PlayerPrefs.SetFloat(ConstantKey.CoinCountKey, value);
        }
        
        public static int LevelIndex
        {
            get => PlayerPrefs.GetInt(ConstantKey.LevelIndexKey, 0) % ConstantKey.MAX_LEVEL;
            set => PlayerPrefs.SetInt(ConstantKey.LevelIndexKey, value % ConstantKey.MAX_LEVEL);
        }
        
        public static int CurrentLife
        {
            // TODO : restore life system
            // get => PlayerPrefs.GetInt(ConstantKey.CurLifeKey, LifeSystem.MAX_LIVES);
            get => 5;
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
        
        public static bool IsEnoughLives => CurrentLife > 0;
        
        public static bool GetSettingState(eSettingType type)
        {
            return PlayerPrefs.GetInt($"Setting_{type}", 1) == 1;
        }

        public static void SetSettingState(eSettingType type, bool isActive)
        {
            PlayerPrefs.SetInt($"Setting_{type}", isActive ? 1 : 0);
        }

        #region Booster

        public static int BoosterRespawnAmount
        {
            get => PlayerPrefs.GetInt(ConstantKey.BoosterRespawnKey, 0);
            set
            {
                PlayerPrefs.SetInt(ConstantKey.BoosterRespawnKey, value);
                EventBus<BoosterUpdatedDTO>.Raise(new BoosterUpdatedDTO(eBoosterType.Respawn, value));
            }
        }
        
        public static int BoosterDestroyStackAmount
        {
            get => PlayerPrefs.GetInt(ConstantKey.BoosterDestroyStackKey, 0);
            set
            {
                PlayerPrefs.SetInt(ConstantKey.BoosterDestroyStackKey, value);
                EventBus<BoosterUpdatedDTO>.Raise(new BoosterUpdatedDTO(eBoosterType.DestroyStack, value));
            }
        }
        
        public static bool HasShownAndClaimedRespawnBoosterTutorial
        {
            get => PlayerPrefs.GetInt(ConstantKey.HasShownAndClaimedRespawnBoosterTutorialKey, 0) == 1;
            set => PlayerPrefs.SetInt(ConstantKey.HasShownAndClaimedRespawnBoosterTutorialKey, value ? 1 : 0);
        }

        public static bool HasShownAndClaimedDestroyBoosterTutorial
        {
            get => PlayerPrefs.GetInt(ConstantKey.HasShownAndClaimedDestroyBoosterTutorialKey, 0) == 1;
            set => PlayerPrefs.SetInt(ConstantKey.HasShownAndClaimedDestroyBoosterTutorialKey, value ? 1 : 0);
        }
        
        public static bool GetHasShownAndClaimedBoosterTutorial(eBoosterType boosterType)
        {
            switch (boosterType)
            {
                case eBoosterType.Respawn:
                    return HasShownAndClaimedRespawnBoosterTutorial;
                case eBoosterType.DestroyStack:
                    return HasShownAndClaimedDestroyBoosterTutorial;
                default:
                    return true;
            }
        }

        #endregion
    }
}