using System;
using HexaSort.Scripts.Core.Entities.Piece;
using UnityEngine;

namespace LevelEditor.LevelData
{
    public enum eLevelGoalType
    {
        Piece,
        Wood
    }
    
    [Serializable]
    public struct LevelGoalData
    {
        public eLevelGoalType type;
        public int targetAmount;
    }

    [Serializable]
    public struct CellData
    {
        public bool IsActive;
        public int UnlockValue;
        public bool HasWood;
        public ColorType HidenColorBelowIce;
    }
    
    [CreateAssetMenu(fileName = "lv_", menuName = "Level Editor/Level Data", order = 3)]
    public class LevelDataSO : ScriptableObject
    {
        public int Width, Height;
        public CellData[] cells;
        public LevelGoalData[] Goal;

        public CellData GetCellData(int i, int j)
            => cells[i * Width + j];
    }
}