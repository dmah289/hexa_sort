using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Controllers;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class TrayController : MonoSingleton<TrayController>, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Self references")]
        [SerializeField] private Transform[] hexStackHolders;
        [SerializeField] private HexStackController[] hexStacks = new HexStackController[3];
        
        [Header("Configurations")]
        [SerializeField] private Vector2 spawnMidStackPos;
        [SerializeField] private int remainStackAmount;

        #region Unity Callbacks
        
        protected override void Awake()
        {
            base.Awake();
            selfTransform = transform;
        }

        private void OnEnable()
        {
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            DeregisterCallbacks();
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
        
        public void CleanUp()
        {
            for (int i = 0; i < hexStackHolders.Length; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexStack, hexStacks[i], destroyCancellationToken);
            }
            
            gameObject.SetActive(false);
        }

        #endregion

        #region Class Methods

        private async UniTaskVoid SpawnHexStacks()
        {
            for (int i = 0; i < 3; i++)
            {
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);
                
                hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos).Forget();
            }

            remainStackAmount = 3;
        }

        #endregion

        #region Stack Laid Down Listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithoutArgs: OnStackLaidDown);
        }

        private void OnStackLaidDown()
        {
            remainStackAmount--;
            
            if (remainStackAmount == 0)
                SpawnHexStacks().Forget();
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithoutArgs: OnStackLaidDown);
        }

        #endregion

        
    }
}