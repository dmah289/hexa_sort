using UnityEngine;

namespace LevelEditor.LevelData
{
    [CreateAssetMenu(fileName = "grid_layout_v", menuName = "Level Editor/Grid Layout Version", order = 2)]
    public class GridLayoutVersion : ScriptableObject
    {
        public GridLayoutSO[] gridLayouts;
    }
}