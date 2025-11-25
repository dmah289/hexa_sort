using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HexaSort.Managers.Level;
using HexaSort.Core.Entities.Grid;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

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
                .Where(c => c != null) 
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
    }
}