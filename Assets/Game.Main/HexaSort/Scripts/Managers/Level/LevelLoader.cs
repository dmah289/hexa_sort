using System;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.LevelEditor.Scripts;
using HexaSort.Core.Entities.Grid;
using HexaSort.Scripts.Core.Controllers;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Managers.Level
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GoalTrackerManager goalTrackerManager;
        
        [Header("----- Level Data -----")]
        [SerializeField] private LevelOrderVersion currLevelOrderVersion;
        [SerializeField] private LevelDataSO currLevelData;
        [SerializeField] private int maxColorIdx;
        [SerializeField] private int currColorIdx;
        [SerializeField] private float rangeUnitToNextColor;

        private void Awake()
        {
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnTotalPieceCollected);
        }

        private void OnTotalPieceCollected(TotalGoalGainedDTO data)
        {
            if (data.goalType == eLevelGoalType.Piece)
            {
                // start from index 2
                currColorIdx = 2 + (int)(data.levelProgress / rangeUnitToNextColor);
                // Debug.Log($"currColorIdx: {currColorIdx} at {data.levelProgress}");
            }
        }


        public async UniTask<LevelDataSO> GetCurrLevelData()
        {
            if (currLevelOrderVersion == null)
            {
                currLevelOrderVersion = await Addressables.LoadAssetAsync<LevelOrderVersion>(ConstantKey.LevelOrderVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            return currLevelOrderVersion.levelDatas[LocalDataManager.LevelIndex];
        }
        
        public async UniTask SetupLevel(GridController grid)
        {
            await GameDifficultyController.Instance.SetupLevelDifficulty();
            
            currLevelData = await GetCurrLevelData();
            maxColorIdx = currLevelData.AvailableColors.Length-1;
            // always start with 3 color
            currColorIdx = 2;
            // calculate range to next color
            rangeUnitToNextColor = 1.0f / (maxColorIdx - currColorIdx + 1);
            
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