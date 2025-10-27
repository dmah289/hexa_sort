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
        public async UniTaskVoid SetupBoardLayout(TrayController tray)
        {
            int activeTile = 0;
            float minCellY = int.MaxValue;
            float maxCellX = int.MinValue;
            Vector2 centerPos = Vector2.zero;
            int size = 5;
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    HexCellController cell = await ObjectPooler.GetFromPool<HexCellController>(
                        PoolingType.HexCell, destroyCancellationToken, transform);

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
    }
}