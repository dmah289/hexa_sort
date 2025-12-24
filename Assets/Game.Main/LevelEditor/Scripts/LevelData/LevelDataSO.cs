using System;
using HexaSort.Core.Entities.Grid.Piece;
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
        
        public bool IsValid() => UnlockValue > 0;
    }
    
    [Serializable]
    public enum eMechanicsType : byte
    {
        None,
        Wood,
        Packed
    }

    [Serializable]
    public struct CellData
    {
        public eMechanicsType MechanicsType;
        public bool IsActive;
        public bool HasWood;
        public PackedStackData packedStack;
    }
    
    [CreateAssetMenu(fileName = "lv_", menuName = "Level Editor/Level Data", order = 3)]
    public class LevelDataSO : ScriptableObject
    {
        public int Width, Height;
        public CellData[] cells;
        public LevelGoalData[] Goal;
        // Always have at least 3 colors
        public ColorType[] AvailableColors;

        public CellData GetCellData(int i, int j)
            => cells[i * Width + j];
    }
}