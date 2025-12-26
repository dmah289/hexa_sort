using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.LevelEditor.Scripts;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.Core.Entities;
using HexaSort.Core.Entities.Grid;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using HexaSort.Managers.Level;

namespace HexaSort.Controllers.DifficultyAlgorithm
{
    public class GameDifficultyController : MonoSingleton<GameDifficultyController>
    {
        [Header("----- References -----")]
        [SerializeField] private GridController gridController;
        
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
        [SerializeField] private ColorDifficultySpawner colorDifficultySpawner;

        public int ContinuousRescueSpawnCounter
        {
            get => spawnCycleCounter;
            set => spawnCycleCounter = value % spawnCycle;
        }
        public int MaxColorPerStack => maxColorPerStack;

        // If it's first time in cycle to spawn random
        public bool IsRandomSpawnTurn => spawnCycleCounter % spawnCycle == 0;

        #region Extracted Properties
        public int TotalWeight => colorDifficultySpawner.TotalWeight;
        public int[] CumulativeWeights => colorDifficultySpawner.CumulativeWeights;
        #endregion

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            
            colorDifficultySpawner = GetComponent<ColorDifficultySpawner>();
            
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
                        
                    // Debug.Log($"maxColorPerStack: {maxColorPerStack} at {levelProgress}");
                    
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


        public List<ColorType> GetRescuedColor()
        {
            List<HexStackController> stacks = gridController.StacksOnGrid;
            
            // calculate total pieces count per color across entire grid
            var colorsAmountStat = new Dictionary<ColorType, int>();
            for (int i = 0; i < stacks.Count; i++)
            {
                if (colorsAmountStat.ContainsKey(stacks[i].ColorOnTop))
                    colorsAmountStat[stacks[i].ColorOnTop] += stacks[i].TopColorAmount;
                else
                    colorsAmountStat[stacks[i].ColorOnTop] = stacks[i].TopColorAmount;
            }
            
            // collect unique top colors from all stacks
            var topColorSet = new HashSet<ColorType>(colorsAmountStat.Keys);
            // sort top colors by total count descending (most pieces first)
            var sortedTopColors = new List<ColorType>(topColorSet);
            sortedTopColors.Sort((a, b) 
                => colorsAmountStat[b].CompareTo(colorsAmountStat[a]));
            
            // fill remaining slots with random colors from SpawnableColors if needed
            if (sortedTopColors.Count < maxColorPerStack)
            {
                var spawnableColors = LevelManager.Instance.SpawnableColors;
                var rand = new System.Random();
                int maxIterations = 10; // safety to prevent infinite loop
                
                for(int i1 = 0; i1 < maxIterations; i1++)
                {
                    if (sortedTopColors.Count >= maxColorPerStack)
                        break;
                    
                    // try to get colors not yet in result
                    var candidates = new List<ColorType>();
                    for (int i = 0; i < spawnableColors.Count; i++)
                    {
                        if (!sortedTopColors.Contains(spawnableColors[i]))
                            candidates.Add(spawnableColors[i]);
                    }
                    
                    // pick from candidates (not yet in result)
                    if (candidates.Count > 0)
                    {
                        ColorType pickedColor = candidates[rand.Next(candidates.Count)];
                        sortedTopColors.Add(pickedColor);
                    }
                    else break;
                }
            }
            
            return sortedTopColors;
        }
    }
}