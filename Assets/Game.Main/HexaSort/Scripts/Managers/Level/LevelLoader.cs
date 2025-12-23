using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities.Grid;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Managers.Level
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GoalTrackerManager goalTrackerManager;
        
        [Header("Level Data")]
        [SerializeField] private LevelOrderVersion currLevelOrderVersion;
        
        public async UniTask<int> GetLevelGoalCount()
        {
            LevelDataSO currLevelData = currLevelOrderVersion.levelDatas[LocalDataManager.LevelIndex];
            return currLevelData.Goal.Length;
        }
        
        public async UniTask<LevelDataSO> GetCurrLevelData()
        {
            if (currLevelOrderVersion == null)
            {
                currLevelOrderVersion = await Addressables.LoadAssetAsync<LevelOrderVersion>(ConstantKey.LevelCurveVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            return currLevelOrderVersion.levelDatas[LocalDataManager.LevelIndex];
        }
        
        public async UniTask SetupLevel(GridController grid)
        {
            LevelDataSO currLevelData = await GetCurrLevelData();
            grid.SetupLevel(currLevelData);
            goalTrackerManager.PlayStartLevelAnim(currLevelData.Goal).Forget();
        }

        public void CleanUpLevel(GridController grid, TrayController tray)
        {
            grid.CleanUp();
            tray.CleanUpTray();
        }
    }
}