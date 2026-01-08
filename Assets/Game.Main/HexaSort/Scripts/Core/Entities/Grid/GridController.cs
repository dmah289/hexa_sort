using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using HexaSort.Controllers.DifficultyAlgorithm;

namespace HexaSort.Core.Entities.Grid
{
    public class GridController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private GridSpawner gridSpawner;
        
        [Header("References")]
        [SerializeField] private TrayController trayController;
        [SerializeField] private Camera mainCam;
        
        [Header("UI References")]
        [SerializeField] private WinPanel winPanel;

        public HexCell[,] GridCells
        {
            get => gridSpawner.gridCells;
            set => gridSpawner.gridCells = value;
        }
        
        public List<HexStackController> NonSurroundedStacksOnGrid
        {
            get
            {
                var nonSurroundedStacks = new List<HexStackController>();
                
                for(int i = 0; i < GridSize.height; i++)
                {
                    for(int j = 0; j < GridSize.width; j++)
                    {
                        if (GridCells[i, j] != null && GridCells[i, j].CurrentStack != null && GridCells[i, j].IsMergable)
                        {
                            if (GridCells[i, j].HasAtLeastOneUnoccupiedNeighbor(this))
                            {
                                nonSurroundedStacks.Add(GridCells[i, j].CurrentStack);
                            }
                        }
                    }
                }

                return nonSurroundedStacks;
            }
        }
        
        public List<HexCell> EmptyCells
        {
            get
            {
                List<HexCell> emptyCells = new List<HexCell>();
                
                for(int i = 0; i < GridSize.height; i++)
                {
                    for(int j = 0; j < GridSize.width; j++)
                    {
                        if (GridCells[i, j] != null && !GridCells[i, j].IsOccupied)
                            emptyCells.Add(GridCells[i, j]);
                    }
                }

                return emptyCells;
            }
        }
        
        public List<HexCell> GetNeighbors(HexCell cell)
        {
            List<HexCell> neighbors = new List<HexCell>();
            var gridPos = cell.GridPos;
            int startIdx = (gridPos.col & 1) == 1 ? 0 : 6;

            for (int i = startIdx; i < startIdx + 6; i++)
            {
                int newCol = gridPos.col + PathFinder.colOffsets[i % 6];
                int newRow = gridPos.row + PathFinder.rowOffsets[i];

                if (newCol < 0 || newCol >= GridSize.width || newRow < 0 || newRow >= GridSize.height)
                    continue;

                HexCell neighbor = GridCells[newRow, newCol];
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
        
        public (int width, int height) GridSize
            => (GridCells.GetLength(1), GridCells.GetLength(0));
        
        public bool IsOutOfSpace
        {
            get
            {
                for(int i = 0; i < GridSize.height; i++)
                {
                    for(int j = 0; j < GridSize.width; j++)
                    {
                        if (GridCells[i, j] && !GridCells[i, j].IsOccupied)
                            return false;
                    }
                }

                return true;
            }
        }
        
        public bool IsEmptyGrid
        {
            get
            {
                for(int i = 0; i < GridSize.height; i++)
                {
                    for(int j = 0; j < GridSize.width; j++)
                    {
                        if (GridCells[i, j] && GridCells[i, j].IsOccupied)
                            return false;
                    }
                }

                return true;
            }
        }
        
        public bool HasNoMergableStacks
        {
            get
            {
                for(int i = 0; i < GridSize.height; i++)
                {
                    for(int j = 0; j < GridSize.width; j++)
                    {
                        if (GridCells[i, j] && GridCells[i, j].IsMergable)
                            return false;
                    }
                }

                return true;
            }
        }
        
        #region Unity APIs

        private void Awake()
        {
            gridSpawner = GetComponent<GridSpawner>();
        }

        #endregion

        #region Class Mehtods

        public void SetupLevel(LevelDataSO currLevelData)
        {
            gridSpawner.SetupBoardLayout(trayController, currLevelData).Forget();
        }
        
        #endregion

        public async UniTask FindDestroyableStacks(LoosePanel loosePanel)
        {
            var occupiedCells = new List<HexCell>();
    
            for (int i = 0; i < GridSize.height; i++)
            {
                for (int j = 0; j < GridSize.width; j++)
                {
                    if (GridCells[i, j] != null && GridCells[i, j].CurrentStack != null)
                    {
                        occupiedCells.Add(GridCells[i, j]);
                    }
                }
            }

            var topHighestCells = occupiedCells
                .Where(c => c != null && c.IsMergable) 
                .OrderByDescending(cell => cell.PiecesCount)
                .Take(ConstantKey.MaxDestroyableStackOnLoose)
                .ToList();
            
            for (int i = 0; i < topHighestCells.Count; i++)
            {
                loosePanel.sightingTargets[i].ParentCell = topHighestCells[i]; 
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    loosePanel.GetComponent<RectTransform>(),
                    mainCam.WorldToScreenPoint(topHighestCells[i].CurrentStack.TopPiece.selfTransform.position),
                    null,
                    out Vector2 sightingTargetlocalPos);
                loosePanel.sightingTargets[i].GetComponent<RectTransform>().anchoredPosition = sightingTargetlocalPos;
            }
        }

        public void CleanUp()
        {
            gridSpawner.CleanUp();
        }

        public void SetStacksOnGridSelectableState(bool selectable)
        {
            for(int i = 0; i < GridSize.height; i++)
            {
                for(int j = 0; j < GridSize.width; j++)
                {
                    if (GridCells[i, j] != null && GridCells[i, j].CurrentStack != null)
                    {
                        GridCells[i, j].CurrentStack.Selectable = selectable;
                    }
                }
            }
        }
    }
}