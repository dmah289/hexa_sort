using System;
using DG.Tweening;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class CellLock : MonoBehaviour, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private TextMeshPro unlockValueText;
        [SerializeField] private Transform lockIcon;
        
        [Header("References")]
        [SerializeField] private HexCell parentCell;
        
        [Header("Config")]
        [SerializeField] private int unlockValue;

        public void Setup(int unlockValue)
        {
            // TODO : Reset states
            gameObject.SetActive(true);
            
            this.unlockValue = unlockValue;
            parentCell.Selectable = false;
            transform.localPosition = ConstantKey.STACK_LOCAL_POS_ON_CELL;
            unlockValueText.text = unlockValue.ToString();
        }

        #region Unity APIs

        private void OnEnable()
        {
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            DeregisterCallbacks();
        }

        #endregion

        #region Event Bus Callbacks

        public void RegisterCallbacks()
        {
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnTotalPieceCollected);
        }

        private void OnTotalPieceCollected(TotalGoalGainedDTO data)
        {
            if(data.goalType == eLevelGoalType.Piece && data.totalCollectedAmount >= unlockValue)
            {
                lockIcon.DOShakeRotation(0.5f, 5f, 20)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() => gameObject.SetActive(false));
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<TotalGoalGainedDTO>.Deregister(onEventWithArgs: OnTotalPieceCollected);
        }

        #endregion

        
    }
}