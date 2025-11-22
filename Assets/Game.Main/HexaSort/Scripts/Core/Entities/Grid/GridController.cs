using System.Linq;
using Cysharp.Threading.Tasks;
using HexaSort.Managers.Level;
using HexaSort.Core.Entities.Grid;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
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

        public void FinDestroyableStacks(LoosePanel loosePanel)
        {
            var top3HighestCells = GridCells
                .Cast<HexCell>()
                .Where(cell => cell.IsOccupied)
                .OrderByDescending(cell => cell.PiecesCount)
                .Take(3)
                .ToList();

            for (int i = 0; i < top3HighestCells.Count; i++)
            {
                top3HighestCells[i].ShowSightingTarget(loosePanel.GetComponent<RectTransform>(), mainCam)
                    .Forget();
            }
        }

        public void CleanUp()
        {
            gridSpawner.CleanUp();
        }
    }
}