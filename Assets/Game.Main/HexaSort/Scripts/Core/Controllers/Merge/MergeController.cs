using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeController : MonoBehaviour, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private MergeSequenceExecutor mergeSequenceExecutor;
        [SerializeField] private PathFinder pathFinder;
        
        [Header("References")]
        [SerializeField] private GridController grid;
        
        [Header("State Managers")]
        [SerializeField] private bool isCheckingMergeSequence;
        
        #region Unity APIs

        private void Awake()
        {
            mergeSequenceExecutor = GetComponent<MergeSequenceExecutor>();
            pathFinder = GetComponent<PathFinder>();
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
        
        #region Stack laid down listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithArgs: OnStackLaidDown);
        }
        
        private void OnStackLaidDown(LaidDownStackDTO dto)
        {
            HandleCheckingMerge(dto.cell);
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        #region Class Mehtods

        private async UniTaskVoid HandleCheckingMerge(HexCell cell)
        {
            await UniTask.WaitUntil(() => !mergeSequenceExecutor.IsBusy);
            
            mergeSequenceExecutor.WaitingMergableCells.Add(cell);
            
            if(!isCheckingMergeSequence) CheckMergeSequence();
            else mergeSequenceExecutor.NewStackLaidDown = true;
        }

        private async UniTaskVoid CheckMergeSequence()
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