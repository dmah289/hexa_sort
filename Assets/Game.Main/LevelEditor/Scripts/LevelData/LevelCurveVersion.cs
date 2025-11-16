using UnityEngine;

namespace LevelEditor.LevelData
{
    [CreateAssetMenu(fileName = "level_curve_v", menuName = "Level Editor/Level Curve Version", order = 4)]
    public class LevelCurveVersion : ScriptableObject
    {
        public LevelDataSO[] levelDatas;
    }
}