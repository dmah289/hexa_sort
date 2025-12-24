using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.LevelEditor.Scripts;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Scripts.Core.Controllers
{
    public class GameDifficultyController : MonoSingleton<GameDifficultyController>
    {
        [Header("----- Level Curve -----")]
        [SerializeField] private LevelCurveVersion currLevelCurveVersion;
        [SerializeField] private eLevelDifficultyType currLevelDifficultyType;
        
        [Header("----- Level Difficulty Manager -----")]
        [SerializeField] private LevelDifficultyConfigVersion currLevelDifficultyConfigVersion;
        [SerializeField] private LevelDifficultyConfig currLevelDifficultyConfig;
        [Tooltip("First spawn is random, n-1 last rescue spawns")]
        [SerializeField] private int spawnCycle;
        [SerializeField] private int spawnCycleCounter;

        public int ContinuousRescueSpawnCounter
        {
            get => spawnCycleCounter;
            set => spawnCycleCounter = value % spawnCycle;
        }

        // If it's first time in cycle to spawn random
        public bool IsRandomSpawnTurn => spawnCycleCounter % spawnCycle == 0;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnTotalPieceCollected);
        }

        #endregion

        #region Setup Config
        
        private async UniTask<eLevelDifficultyType> GetCurrLevelDifficultyType()
        {
            if (currLevelCurveVersion == null)
            {
                currLevelCurveVersion = await Addressables
                    .LoadAssetAsync<LevelCurveVersion>(ConstantKey.LevelCurveVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            return currLevelCurveVersion.levelsDifficulty[LocalDataManager.LevelIndex];
        }
        
        private async UniTask<LevelDifficultyConfig> GetCurrLevelDifficultyConfig()
        {
            if (currLevelDifficultyConfigVersion == null)
            {
                currLevelDifficultyConfigVersion = await Addressables
                    .LoadAssetAsync<LevelDifficultyConfigVersion>(ConstantKey.LevelDifficultyConfigVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            for(int i = 0; i < currLevelDifficultyConfigVersion.levelDifficultyConfigs.Length; i++)
            {
                if (currLevelDifficultyConfigVersion.levelDifficultyConfigs[i].levelDifficultyType 
                    == currLevelDifficultyType)
                {
                    return currLevelDifficultyConfigVersion.levelDifficultyConfigs[i];
                }
            }

            return null;
        }
        
        public async UniTask SetupLevelDifficulty()
        {
            currLevelDifficultyType = await GetCurrLevelDifficultyType();
            currLevelDifficultyConfig = await GetCurrLevelDifficultyConfig();
            
            UpdateMaxContinuousRescueSpawnTimes(0);
            spawnCycleCounter = 0;
        }
        
        private void OnTotalPieceCollected(TotalGoalGainedDTO data)
        {
            if(data.goalType == eLevelGoalType.Piece)
                UpdateMaxContinuousRescueSpawnTimes(data.levelProgress * 100);
        }

        // Update the max continuous rescue spawn times based on level progress when pieces are collected
        public void UpdateMaxContinuousRescueSpawnTimes(float levelProgress)
        {
            for (int i = currLevelDifficultyConfig.levelDifficultyThresholds.Length - 1; i >= 0; i--)
            {
                if (levelProgress >= currLevelDifficultyConfig.levelDifficultyThresholds[i].progressThreshold)
                {
                    if (spawnCycle != currLevelDifficultyConfig.levelDifficultyThresholds[i]
                            .continuousRescueSpawnTimes+1)
                    {
                        // 1 more times for random
                        spawnCycle = currLevelDifficultyConfig.levelDifficultyThresholds[i]
                            .continuousRescueSpawnTimes+1;
                        
                        // Reset counter to last rescue spawn to start with random spawn in next spawn turn
                        spawnCycleCounter = spawnCycle - 1;
                        
                        // Debug.Log($"spawnCycle: {spawnCycle} - spawnCycleCounter: {spawnCycleCounter} at {levelProgress}");
                        break;
                    }
                }
            }
        }
        
        #endregion
        
        
    }
}