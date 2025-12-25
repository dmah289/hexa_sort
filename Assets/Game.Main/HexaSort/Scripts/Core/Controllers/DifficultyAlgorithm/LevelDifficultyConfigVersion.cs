using System;
using Game.Main.LevelEditor.Scripts;
using UnityEngine;

namespace HexaSort.Controllers.DifficultyAlgorithm
{
    [Serializable]
    public class LevelDifficultyThreshold
    {
        public int progressThreshold;
        public int continuousRescueSpawnTimes;
        public int maxColorPerStack;
    }

    [Serializable]
    public class LevelDifficultyConfig
    {
        public eLevelDifficultyType levelDifficultyType;
        public LevelDifficultyThreshold[] levelDifficultyThresholds;
    }
    
    [CreateAssetMenu(fileName = "level_difficulty_config_version", menuName = "Level Editor/ Level Difficulty Config Version", order = 0)]
    public class LevelDifficultyConfigVersion : ScriptableObject
    {
        public LevelDifficultyConfig[] levelDifficultyConfigs;
    }
}