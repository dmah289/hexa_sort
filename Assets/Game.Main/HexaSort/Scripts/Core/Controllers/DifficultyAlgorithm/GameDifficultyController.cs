using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.LevelEditor.Scripts;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Controllers.DifficultyAlgorithm
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
        [SerializeField] private int maxColorPerStack;
        
        [Header("----- Self References -----")]
        [SerializeField] private ColorSpawner colorSpawner;

        public int ContinuousRescueSpawnCounter
        {
            get => spawnCycleCounter;
            set => spawnCycleCounter = value % spawnCycle;
        }
        public int MaxColorPerStack => maxColorPerStack;

        // If it's first time in cycle to spawn random
        public bool IsRandomSpawnTurn => spawnCycleCounter % spawnCycle == 0;

        #region Extracted Properties
        public int TotalWeight => colorSpawner.TotalWeight;
        public int[] CumulativeWeights => colorSpawner.CumulativeWeights;
        #endregion

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            
            colorSpawner = GetComponent<ColorSpawner>();
            
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
            
            UpdateProgressBasedLevelConfigs(0);
            spawnCycleCounter = 0;
        }
        
        private void OnTotalPieceCollected(TotalGoalGainedDTO data)
        {
            if(data.goalType == eLevelGoalType.Piece)
                UpdateProgressBasedLevelConfigs(data.levelProgress);
        }

        private void UpdateProgressBasedLevelConfigs(float levelProgress)
        {
            UpdateMaxContinuousRescueSpawnTimes(levelProgress * 100);
            UpdateMaxColorPerStack(levelProgress * 100);
        }

        private void UpdateMaxColorPerStack(float levelProgress)
        {
            for (int i = currLevelDifficultyConfig.levelDifficultyThresholds.Length - 1; i >= 0; i--)
            {
                if (levelProgress >= currLevelDifficultyConfig.levelDifficultyThresholds[i].progressThreshold)
                {
                    maxColorPerStack = currLevelDifficultyConfig.levelDifficultyThresholds[i]
                        .maxColorPerStack;
                        
                    Debug.Log($"maxColorPerStack: {maxColorPerStack} at {levelProgress}");
                    
                    break;
                }
            }
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