using UnityEngine;

namespace LevelEditor.LevelData
{
    public class BoardLayoutSO : ScriptableObject
    {
        public Vector2 Size;
        public bool[] activeCells;
    }
}