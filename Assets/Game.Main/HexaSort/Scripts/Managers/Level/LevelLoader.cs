using System;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Entities;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Scripts.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GoalTrackerManager goalTrackerManager;
        
        [Header("Level Data")]
        [SerializeField] private LevelCurveVersion currLevelCurveVersion;
        
        public async UniTask<LevelDataSO> GetCurrLevelData()
        {
            if (currLevelCurveVersion == null)
            {
                currLevelCurveVersion = await Addressables.LoadAssetAsync<LevelCurveVersion>(ConstantKey.LevelCurveVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            return currLevelCurveVersion.levelDatas[LocalDataManager.LevelIndex];
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
            tray.CleanUp();
        }
    }
}