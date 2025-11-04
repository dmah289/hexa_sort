using manhnd_sdk.Scripts.Helpers;
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

        public HexCell GetCellAtPosition(int row, int col) => gridSpawner.gridCells[row, col];

        private void Awake()
        {
            gridSpawner = GetComponent<GridSpawner>();
            
            SetupLevel();
        }

        public void SetupLevel()
        {
            gridSpawner.SetupBoardLayout(trayController).Forget();
        }
    }
}