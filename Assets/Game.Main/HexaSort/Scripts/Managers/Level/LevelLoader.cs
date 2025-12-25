using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities.Grid;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.Controllers.DifficultyAlgorithm;
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
        [SerializeField] private ColorType[] availableColors;
        [SerializeField] private List<ColorType> spawnableColors;
        [SerializeField] private int maxColorIdx;
        [SerializeField] private int currColorIdx;
        [SerializeField] private float rangeUnitToNextColor;
        
        public List<ColorType> SpawnableColors => spawnableColors;

        private void Awake()
        {
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnTotalPieceCollected);
            spawnableColors = new List<ColorType>();
        }

        private void OnTotalPieceCollected(TotalGoalGainedDTO data)
        {
            if (data.goalType == eLevelGoalType.Piece)
                UpdateSpawnableColorList(data);
        }

        private void UpdateSpawnableColorList(TotalGoalGainedDTO data)
        {
            // start from index 2
            if (currColorIdx != 2 + (int)(data.levelProgress / rangeUnitToNextColor))
            {
                currColorIdx = 2 + (int)(data.levelProgress / rangeUnitToNextColor);
                if(!spawnableColors.Contains(availableColors[currColorIdx]))
                    spawnableColors.Add(availableColors[currColorIdx]);
            }
            Debug.Log($"currColorIdx: {currColorIdx} at {data.levelProgress}");
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
            
            availableColors = currLevelData.AvailableColors;
            maxColorIdx = currLevelData.AvailableColors.Length-1;
            // always start with 3 color
            currColorIdx = 2;
            // calculate range to next color
            rangeUnitToNextColor = 1.0f / (maxColorIdx - currColorIdx + 1);
            for(int i = 0; i < 3; i++)
                spawnableColors.Add(availableColors[i]);
            
            grid.SetupLevel(currLevelData);
            goalTrackerManager.PlayStartLevelAnim(currLevelData.Goal).Forget();
        }

        public void CleanUpLevel(GridController grid, TrayController tray)
        {
            grid.CleanUp();
            tray.CleanUpTray();
            
            currLevelData = null;
            availableColors = null;
            spawnableColors.Clear();
            maxColorIdx = 0;
            currColorIdx = 0;
            rangeUnitToNextColor = 0f;
        }
    }
}