using UnityEngine;

namespace LevelEditor.LevelData
{
    [CreateAssetMenu(fileName = "grid_layout_", menuName = "Level Editor/Grid Layout SO", order = 1)]
    public class GridLayoutSO : ScriptableObject
    {
        public int width, height;
        public int[] inactiveCells;
    }
}