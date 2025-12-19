using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.BaseSystem;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.Cheater
{
    public class CheatLevel : MonoBehaviour
    {
        public void OnCheatLevel(string value)
        {
            LocalDataManager.LevelIndex = int.Parse(value);
            LevelManager.Instance.CleanUpLevel();
            CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
        }
    }
}