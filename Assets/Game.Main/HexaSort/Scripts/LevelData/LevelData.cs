using System.Drawing;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.LevelData
{
    public class LevelData : ScriptableObject
    {
        public Vector2 Size;
        public bool[] activeCells;
    }
}