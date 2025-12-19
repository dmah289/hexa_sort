using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Managers.Level;
using HexaSort.Scripts.Core.Controllers;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

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

        #region Unity Callbacks
        
        protected override void Awake()
        {
            base.Awake();
            selfTransform = transform;
            RegisterCallbacks();
        }

        #endregion

        #region API

        public void SetupTray(float centerPosX, float minCellY, float maxCellX, int height)
        {
            gameObject.SetActive(true);
            
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
            for (int i = 0; i < 3; i++)
            {
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);

                if (allowWaitingSliding)
                {
                    await hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, allowWaitingSliding);
                }
                else 
                {
                    hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, allowWaitingSliding).Forget();
                }
            }

            await UniTask.DelayFrame(5);

            remainStackAmount = 3;
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