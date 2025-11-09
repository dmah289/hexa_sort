using HexaSort.Scripts.Core.Controllers;
using HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.Helpers;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class GridController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private GridSpawner gridSpawner;
        
        [Header("References")]
        [SerializeField] private TrayController trayController;

        public HexCell[,] GridCells => gridSpawner.gridCells;
        
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
                        if (!GridCells[i, j].IsOccupied)
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

        public void SetupLevel()
        {
            gridSpawner.SetupBoardLayout(trayController).Forget();
        }
        
        #endregion
    }
}