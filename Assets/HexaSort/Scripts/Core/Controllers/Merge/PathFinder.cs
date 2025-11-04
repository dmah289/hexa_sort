using System.Collections.Generic;
using HexaSort.Scripts.Core.Entities;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class PathFinder : MonoBehaviour
    {
        public List<HexCell> GetConnectedCells(HexCell cell, GridController grid)
        {
            List<HexCell> cellsToVisit = new();
            List<HexCell> connectedCells = new();

            cellsToVisit.Add(cell);
            connectedCells.Add(cell);
            grid.VisitedCells[cell.GridPos.x, cell.GridPos.y] = true;

            return connectedCells;
        }
    }
}