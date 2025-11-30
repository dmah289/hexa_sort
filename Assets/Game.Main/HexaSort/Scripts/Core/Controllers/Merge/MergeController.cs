using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HexaSort.Managers.Level;
using HexaSort.Core.Entities.Grid;
using HexaSort.Core.Entities;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeController : MonoSingleton<MergeController>, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private MergeSequenceExecutor mergeSequenceExecutor;
        [SerializeField] private PathFinder pathFinder;
        
        [Header("References")]
        [SerializeField] private GridController grid;
        
        [Header("State Managers")]
        [SerializeField] private bool isCheckingMergeSequence;
        
        public bool IsCheckingMergeSequence => isCheckingMergeSequence;
        
        #region Unity APIs

        protected override void Awake()
        {
            base.Awake();
            
            mergeSequenceExecutor = GetComponent<MergeSequenceExecutor>();
            pathFinder = GetComponent<PathFinder>();
            
            RegisterCallbacks();
        }

        #endregion
        
        #region Stack laid down listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithArgs: OnStackLaidDown);
        }
        
        private void OnStackLaidDown(LaidDownStackDTO dto)
        {
            if (LevelManager.Instance.CurrentLevelState == eLevelState.Playing)
            {
                HandleCheckingMerge(dto.cell).Forget();
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        #region Class Mehtods

        public async UniTask HandleCheckingMerge(HexCell cell)
        {
            await UniTask.WaitUntil(() => !mergeSequenceExecutor.IsBusy);
            
            mergeSequenceExecutor.WaitingMergableCells.Add(cell);
            
            if(!isCheckingMergeSequence) await CheckMergeSequence();
            else mergeSequenceExecutor.NewStackLaidDown = true;
        }

        private async UniTask CheckMergeSequence()
        {
            isCheckingMergeSequence = true;

            while (mergeSequenceExecutor.WaitingMergableCells.Count > 0)
            {
                HexCell cell = mergeSequenceExecutor.WaitingMergableCells.RemoveFirst();
                if (cell.IsOccupied)
                {
                    await HandleMergeSequence(cell);
                    await UniTask.Yield();
                }
            }

            if (grid.IsOutOfSpace)
                LevelManager.Instance.CurrentLevelState = eLevelState.OutOfSpace;

            isCheckingMergeSequence = false;
        }

        private async UniTask HandleMergeSequence(HexCell cell)
        {
            
            
            pathFinder.GetConnectedCells(cell, grid);
            await mergeSequenceExecutor.ExecuteMergeSequence(pathFinder.ConnectedCells, pathFinder.Parents);
        }

        #endregion
    }
}