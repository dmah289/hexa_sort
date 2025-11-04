using System;
using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeController : MonoBehaviour, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private MergeCoordinator mergeCoordinator;
        [SerializeField] private PathFinder pathFinder;
        
        [Header("References")]
        [SerializeField] private GridController grid;
        
        #region Unity APIs

        private void Awake()
        {
            mergeCoordinator = GetComponent<MergeCoordinator>();
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
            HamdleCheckingMerge(dto.cell);
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        #region Class Mehtods

        private void HamdleCheckingMerge(HexCell cell)
        {
            mergeCoordinator.WaitingMergableCells.Add(cell);

            CheckSequentialMerge();
        }

        private void CheckSequentialMerge()
        {
            
        }

        #endregion
    }
}