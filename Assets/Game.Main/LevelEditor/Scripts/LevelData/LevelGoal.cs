using System;

namespace Game.Main.LevelEditor.Scripts.LevelData
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
}