using UnityEngine;

namespace Game.Main.LevelEditor.Scripts
{
    public enum eLevelDifficultyType
    {
        Easy,
        Medium,
        MediumPlus,
        Hard
    }
    
    [CreateAssetMenu(fileName = "level_curve_v", menuName = "Level Editor/Level Curve Version", order = 5)]
    public class LevelCurveVersion : ScriptableObject
    {
        public eLevelDifficultyType[] levelsDifficulty;
    }
}