using UnityEngine;

namespace LevelEditor.LevelData
{
    [CreateAssetMenu(fileName = "level_order_v", menuName = "Level Editor/Level Order Version", order = 4)]
    public class LevelOrderVersion : ScriptableObject
    {
        public LevelDataSO[] levelDatas;
    }
}