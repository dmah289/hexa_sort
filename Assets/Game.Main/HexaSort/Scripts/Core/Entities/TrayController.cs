using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.Managers.Level;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexaSort.Core.Entities.Grid
{
    public class TrayController : MonoSingleton<TrayController>, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Self references")]
        [SerializeField] private Transform[] hexStackHolders;
        [SerializeField] private HexStackController[] hexStacks;
        
        [Header("Configurations")]
        [SerializeField] private Vector2 spawnMidStackPos;
        [SerializeField] private int remainStackAmount;

        [Header("----- Spawn Configurations -----")]
        [SerializeField] private List<ColorType> rescuedColorsThisTurn;
        [SerializeField] private bool minAmountColorPicked;

        #region Unity Callbacks
        
        protected override void Awake()
        {
            base.Awake();
            selfTransform = transform;
            
            // Initialize hexStacks array with size 3
            if (hexStacks == null || hexStacks.Length != 3)
            {
                hexStacks = new HexStackController[3];
            }
            
            RegisterCallbacks();
        }

        #endregion

        #region API

        public void SetupTray(float centerPosX, float minCellY, float maxCellX, int height)
        {
            gameObject.SetActive(true);
            
            // Ensure hexStacks array is properly initialized
            if (hexStacks == null || hexStacks.Length != 3)
            {
                hexStacks = new HexStackController[3];
            }
            
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
        
        public void CleanUpTray()
        {
            CleanUpCurrentStacks();
            gameObject.SetActive(false);
        }
        
        public void CleanUpCurrentStacks()
        {
            for (int i = 0; i < hexStacks.Length; i++)
            {
                if (hexStacks[i])
                {
                    ObjectPooler.ReturnToPool(PoolingType.HexStack, hexStacks[i], destroyCancellationToken);
                    hexStacks[i] = null;
                }
            }
        }

        #endregion

        #region Class Methods

        private async UniTask SpawnHexStacks(bool allowWaitingSliding = false)
        {
            GameDifficultyController.Instance.ContinuousRescueSpawnCounter++;
            
            if(GameDifficultyController.Instance.IsRandomSpawnTurn)
                await SpawnStacksRandomly(allowWaitingSliding);
            else
                await SpawnStacksToRescue(allowWaitingSliding);

            await UniTask.DelayFrame(5);

            remainStackAmount = 3;
        }

        private async UniTask SpawnStacksRandomly(bool allowWaitingSliding)
        {
            List<ColorType> spawnableColors = LevelManager.Instance.SpawnableColors;
            
            for (int i = 0; i < 3; i++)
            {
                int colorAmount = Random.Range(1, GameDifficultyController.Instance.MaxColorPerStack+1);
                List<ColorType> chosenColors = spawnableColors.GetRandomElements(colorAmount);
                
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);

                if (allowWaitingSliding)
                {
                    await hexStacks[i].OnSpawningOnTray(i, 
                        spawnMidStackPos, chosenColors, true);
                }
                else
                {
                    hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors).Forget();
                }
            }
        }
        
        private async UniTask SpawnStacksToRescue(bool allowWaitingSliding)
        {
            rescuedColorsThisTurn = GameDifficultyController.Instance.GetRescuedColor();
            ColorType minAmountColor = rescuedColorsThisTurn[^1];
            minAmountColorPicked = false;
            
            for (int i = 0; i < 3; i++)
            {
                int colorAmount = Random.Range(1, GameDifficultyController.Instance.MaxColorPerStack+1);
                List<ColorType> chosenColors = rescuedColorsThisTurn.GetRandomElementsPreserveOrder(colorAmount);

                if (chosenColors.Contains(minAmountColor))
                {
                    Debug.Log("Picked min amount color");
                    minAmountColorPicked = true;
                }

                if (i == 2 && !minAmountColorPicked)
                {
                    Debug.Log("Force add min amount color");
                    chosenColors.RemoveFirst();
                    chosenColors.Add(minAmountColor);
                }
                
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);
        
                if (allowWaitingSliding)
                {
                    await hexStacks[i].OnSpawningOnTray(i, 
                        spawnMidStackPos, chosenColors, true);
                }
                else 
                {
                    hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors).Forget();
                }
            }
        }

        #endregion

        #region Stack Laid Down Listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithArgs: OnStackLaidDown);
        }

        private void OnStackLaidDown(LaidDownStackDTO data)
        {
            hexStacks[data.cell.CurrentStack.IdxOnTray] = null;
            data.cell.CurrentStack.IdxOnTray = -1;
            
            remainStackAmount--;
            
            if (remainStackAmount == 0)
                SpawnHexStacks().Forget();
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        public async UniTask RespawnCurrentStacks()
        {
            CleanUpCurrentStacks();
            await SpawnHexStacks();
        }
    }
}