using System;
using System.Collections.Generic;
using DG.Tweening;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Core.Entities.Piece;
using manhnd_sdk.Scripts.ExtensionMethods;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class PathFinder : MonoBehaviour
    {
        private static readonly int[] colOffsets = { -1, 0, 1, 1, 0, -1 };
        // Odd -> even
        private static readonly int[] rowOffsets = { 1, 1, 1, 0, -1, 0, 0, 1, 0, -1, -1, -1 };
        
        [Header("Path Finding Settings")] 
        private bool[,] visitedCells;
        private List<HexCell> connectedCells;
        private List<HexCell> cellsToVisit;
        private HexCell[,] parents;

        private void OnEnable()
        {
            connectedCells = new ();
            cellsToVisit = new ();
        }
        
        private void InitBfsGridState((int width, int height) gridSize)
        {
            if(visitedCells == null) visitedCells = new bool[gridSize.height, gridSize.width];
            Array.Clear(visitedCells, 0, visitedCells.Length);
            
            if(parents == null) parents = new HexCell[gridSize.height, gridSize.width];
            Array.Clear(parents, 0, parents.Length);

            // for (int i = 0; i < 5; i++)
            // {
            //     for (int j = 0; j < 5; j++)
            //     {
            //         Debug.Log(visitedCells[i, j] + " - " + parents[i, j]);
            //     }
            // }
            
            connectedCells.Clear();
            cellsToVisit.Clear();
        }

        public List<HexCell> GetConnectedCells(HexCell cell, GridController grid)
        {
            InitBfsGridState(grid.GridSize);
            
            cellsToVisit.Add(cell);
            connectedCells.Add(cell);
            visitedCells[cell.GridPos.row, cell.GridPos.col] = true;

            while (cellsToVisit.Count > 0)
            {
                HexCell currCell = cellsToVisit.RemoveFirst();

                if (!currCell.IsOccupied) break;
                
                FindNeighbourCells(currCell, grid);
            }

            foreach (HexCell cell2 in connectedCells)
            {
                cell2.selfTransform.position = cell2.selfTransform.position.With(z: -2f);
                DOVirtual.DelayedCall(0.2f,() => cell2.selfTransform.position = cell2.selfTransform.position.With(z: 0f));
            }

            return connectedCells;
        }

        private void FindNeighbourCells(HexCell currCell, GridController grid)
        {
            int startIdx = (currCell.GridPos.col & 1) == 1 ? 0 : 6;

            for (int i = startIdx; i < startIdx + 6; i++)
            {
                int newCol = currCell.GridPos.col + colOffsets[i % 6];
                int newRow = currCell.GridPos.row + rowOffsets[i];
                
                if (newCol < 0 || newCol >= grid.GridSize.width || newRow < 0 || newRow >= grid.GridSize.height)
                    continue;
                
                HexCell neighbour = grid.GridCells[newRow, newCol];
                
                if (neighbour.IsOccupied && neighbour.ColorOnTop == currCell.ColorOnTop &&
                    !visitedCells[newRow, newCol])
                {
                    visitedCells[newRow, newCol] = true;
                    parents[newRow, newCol] = currCell;
                    
                    connectedCells.Add(neighbour);
                    cellsToVisit.Add(neighbour);
                }
            }
        }
    }
}