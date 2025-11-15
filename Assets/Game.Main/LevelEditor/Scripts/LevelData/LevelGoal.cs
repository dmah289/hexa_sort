using manhnd_sdk.Scripts.SystemDesign.EventBus;

namespace Game.Main.LevelEditor.Scripts.LevelData
{
    public enum eLevelGoalType
    {
        Piece,
        Wood
    }
    
    public struct LevelGoal
    {
        public eLevelGoalType type;
        public int targetAmount;
    }
}