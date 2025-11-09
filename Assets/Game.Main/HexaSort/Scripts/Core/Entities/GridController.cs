using System;
using System.Linq;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Core.Entities
{
    public class GridController : MonoBehaviour, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private GridSpawner gridSpawner;
        
        [Header("References")]
        [SerializeField] private TrayController trayController;
        
        [Header("UI References")]
        [SerializeField] private WinPanel winPanel;
        
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
            
            RegisterCallbacks();
        }

        #endregion

        #region Class Mehtods

        public void SetupLevel()
        {
            gridSpawner.SetupBoardLayout(trayController).Forget();
        }
        
        #endregion

        public void RegisterCallbacks()
        {
            EventBus<OutOfSpaceEventDTO>.Register(onEventWithoutArgs: OnGridOutOfSpace);
        }

        private void OnGridOutOfSpace()
        {
            var top3HighestCells = GridCells
                .Cast<HexCell>()
                .Where(cell => cell.IsOccupied)
                .OrderByDescending(cell => cell.PiecesCount)
                .Take(3)
                .ToList();

            for (int i = 0; i < top3HighestCells.Count; i++)
            {
                top3HighestCells[i].ShowSightingTarget();
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<OutOfSpaceEventDTO>.Register(onEventWithoutArgs: OnGridOutOfSpace);
        }
    }
}