using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class GridSpawner : MonoBehaviour
    {
        public HexCell[,] gridCells;
        
        public async UniTaskVoid SetupBoardLayout(TrayController tray)
        {
            int size = 5;
            gridCells = new HexCell[size, size];
            
            int activeTile = 0;
            float minCellY = int.MaxValue;
            float maxCellX = int.MinValue;
            Vector2 centerPos = Vector2.zero;
            
            // Root of the grid is in the bottom left corner
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    HexCell cell = await ObjectPooler.GetFromPool<HexCell>(
                        PoolingType.HexCell, destroyCancellationToken, transform);
                    cell.GridPos = (i,j);
                    gridCells[i, j] = cell;

                    Vector2 pos = new Vector2(1.5f * j * ConstantKey.BOARD_CELL_R, 2 * (i+1) * ConstantKey.BOARD_CELL_r);

                    if ((j & 1) == 1) pos = pos.Add(y: ConstantKey.BOARD_CELL_r);

                    centerPos += pos;
                    minCellY = Mathf.Min(minCellY, pos.y);
                    maxCellX = Mathf.Max(maxCellX, pos.x);

                    pos += ConstantKey.CELL_SPACING;
                    cell.transform.localPosition = pos;

                    activeTile++;
                }
            }
            centerPos /= activeTile;
        
            tray.SetupTray(centerPos.x, minCellY, maxCellX, size);
            LevelManager.Instance.ZoomInCamera(size, centerPos);
        }
        
        public void CleanUp()
        {
            if (gridCells == null) return;
        
            int rows = gridCells.GetLength(0);
            int cols = gridCells.GetLength(1);
        
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (gridCells[i, j] != null)
                    {
                        ObjectPooler.ReturnToPool(PoolingType.HexCell, gridCells[i, j], destroyCancellationToken);
                        gridCells[i, j] = null;
                    }
                }
            }
        
            gridCells = null;
        }
    }
    
    
}