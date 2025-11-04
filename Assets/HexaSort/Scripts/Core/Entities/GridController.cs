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
        
        [Header("Path Finder Managers")]
        [SerializeField] private bool[,] visitedCells = new bool[5, 5];
        
        public bool[,] VisitedCells => visitedCells;

        public HexCell[,] GridCells => gridSpawner.gridCells;

        public HexCell GetCellAtPosition(int x, int y) => gridSpawner.gridCells[x, y];

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