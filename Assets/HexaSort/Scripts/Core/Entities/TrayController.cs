using Cysharp.Threading.Tasks;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class TrayController : MonoSingleton<TrayController>
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Self references")]
        [SerializeField] private Transform[] hexStackHolders;
        
        [Header("Configurations")]
        [SerializeField] private Vector2 spawnMidStackPos;
        [SerializeField] private int remainStackAmount;

        public int RemainStackAmount
        {
            get => remainStackAmount;
            set
            {
                remainStackAmount = value;
                if (remainStackAmount == 0)
                {
                    SpawnHexStacks().Forget();
                    remainStackAmount = 3;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            selfTransform = transform;
        }

        public void SetupTray(float centerPosX, float minCellY, float maxCellX, int height)
        {
            float trayToGridOffsetY = 0.157f * height + 0.314f;
            
            Vector2 targetPos= new Vector2(
                centerPosX,
                minCellY - ConstantKey.BOARD_CELL_r - trayToGridOffsetY);
            
            spawnMidStackPos = new Vector2(
                maxCellX + ConstantKey.BOARD_CELL_R + ConstantKey.SPAWN_POS_OFFSET_X + ConstantKey.TRAY_WIDTH / 2f,
                targetPos.y);
            
            selfTransform.position = targetPos;
            SpawnHexStacks().Forget();
        }

        public async UniTaskVoid SpawnHexStacks()
        {
            for (int i = 0; i < 3; i++)
            {
                HexStackController hexStack = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);

                hexStack.OnSpawned(i, spawnMidStackPos).Forget();
            }

            remainStackAmount = 3;
        }
    }
}