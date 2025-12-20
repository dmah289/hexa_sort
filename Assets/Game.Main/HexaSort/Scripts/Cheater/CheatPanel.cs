using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.BaseSystem;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.Cheater
{
    public class CheatPanel : MonoBehaviour
    {
        public void OnCheatLevelEntered(string value)
        {
            LocalDataManager.LevelIndex = int.Parse(value);
            LevelManager.Instance.CleanUpLevel();
            CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
        }
        
        public void OnCheatLifeEntered(string value)
        {
            LocalDataManager.CurrentLife = int.Parse(value);
        }
        
        public void OnCheatCoinEntered(string value)
        {
            LocalDataManager.CoinAmount = int.Parse(value);
        }
        
        public void OnCheatRespawnBoosterEntered(string value)
        {
            LocalDataManager.BoosterRespawnAmount = int.Parse(value);
        }
        
        public void OnCheatDestroyStackBoosterEntered(string value)
        {
            LocalDataManager.BoosterRespawnAmount = int.Parse(value);
        }
        
        public void OnCheatLoseLevelBtnClicked()
        {
            if(LevelManager.Instance.CurrentLevelState == eLevelState.Playing)
                LevelManager.Instance.CurrentLevelState = eLevelState.OutOfSpace;
        }
        
        public void OnCheatWinLevelBtnClicked()
        {
            if(LevelManager.Instance.CurrentLevelState == eLevelState.Playing)
                LevelManager.Instance.CurrentLevelState = eLevelState.Win;
        }
    }
}