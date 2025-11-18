using System;
using HexaSort.Scripts.Core.Entities.Piece;
using UnityEngine;

namespace LevelEditor.LevelData
{
    public enum eLevelGoalType : byte
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
    public struct ColorLayerData
    {
        public ColorType colorType;
        public int amount;
    }
    
    [Serializable]
    public struct PackedStackData
    {
        public int UnlockValue;
        public ColorLayerData[] ColorLayers;
        
        public bool IsValid() => UnlockValue > 0 && ColorLayers != null && ColorLayers.Length > 0;
    }

    [Serializable]
    public struct CellData
    {
        public bool IsActive;
        public int UnlockCellValue;
        public bool HasWood;
        public PackedStackData packedStack;
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