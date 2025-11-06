using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Core.Entities;
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

        private async UniTask HandleCheckingMerge(HexCell cell)
        {
            mergeSequenceExecutor.WaitingMergableCells.Add(cell);

            await CheckSequentialMerge();
        }

        private async UniTask CheckSequentialMerge()
        {
            while (mergeSequenceExecutor.WaitingMergableCells.Count > 0)
            {
                HexCell cell = mergeSequenceExecutor.WaitingMergableCells.RemoveFirst();
                if (cell.IsOccupied)
                {
                    await DoMerge(cell);
                    await UniTask.Yield();
                }
            }
        }

        private async UniTask DoMerge(HexCell cell)
        {
            bool hasMerge = pathFinder.GetConnectedCells(cell, grid);
            if (hasMerge)
            {
                 await mergeSequenceExecutor.ExecuteMergeSequence(pathFinder.ConnectedCells, pathFinder.Parents);
            }
        }

        #endregion
    }
}