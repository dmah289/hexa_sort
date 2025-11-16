using System;
using UnityEngine;

namespace LevelEditor.LevelData
{
    public enum eLevelGoalType
    {
        Piece,
        Wood
    }
    
    [Serializable]
    public struct LevelGoal
    {
        public eLevelGoalType type;
        public int targetAmount;
    }
    
    [CreateAssetMenu(fileName = "level_data_", menuName = "Level Editor/Level Data", order = 3)]
    public class LevelDataSO : ScriptableObject
    {
        public int GridLayoutID;
        public LevelGoal[] Goal;
    }
}