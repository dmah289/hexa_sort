using System;
using HexaSort.Core.Entities.Grid.Piece;
using Sirenix.OdinInspector;
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
        
        [Button]
        public void ActivateAllCells()
        {
            int total = Math.Max(0, Width * Height);

            if (cells == null)
            {
                cells = new CellData[total];
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    cell.IsActive = true;
                    cells[i] = cell;
                }
                return;
            }

            // If current array already has enough or more elements, do nothing.
            if (cells.Length >= total)
            {
                var old1 = cells;
                var newCells1 = new CellData[total];
                Array.Copy(old1, newCells1, total);
                cells = newCells1;
                for (int i = 0; i < cells.Length; i++)
                {
                    var cell = cells[i];
                    cell.IsActive = true;
                    cells[i] = cell;
                }
                return;
            }

            // Expand array, preserve existing elements, initialize only the new ones.
            var old = cells;
            int oldLen = old.Length;
            var newCells = new CellData[total];
            Array.Copy(old, newCells, oldLen);

            for (int i = oldLen; i < newCells.Length; i++)
            {
                var cell = newCells[i];
                cell.IsActive = true;
                newCells[i] = cell;
            }

            cells = newCells;
        }

        [Button]
        public void DeactivateCells(string indices)
        {
            var idx = Array.ConvertAll(indices.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            
            // Deactivate from the end to avoid messing up indices
            for (int i = 0; i < idx.Length; i++)
            {
                var cell = cells[idx[i]];
                cell.IsActive = false;
                cells[idx[i]] = cell;
            }
        }
        
        [Button]
        public void DisableAllObstacles()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                var cell = cells[i];
                cell.MechanicsType = eMechanicsType.None;
                cell.HasWood = false;
                cell.packedStack = new PackedStackData();
                cells[i] = cell;
            }
        }
    }
}