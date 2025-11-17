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

        public void Setup(int unlockValue, HexCell parentCell)
        {
            // TODO : Reset states
            
            this.unlockValue = unlockValue;
            this.parentCell = parentCell;
            parentCell.Selectable = false;
            transform.localPosition = ConstantKey.STACK_LOCAL_POS_ON_CELL;
            unlockValueText.text = unlockValue.ToString();
        }

        #region Event Bus Callbacks

        public void RegisterCallbacks()
        {
            EventBus<TotalGoalCollectedDTO>.Register(onEventWithArgs: OnTotalPieceCollected);
        }

        private void OnTotalPieceCollected(TotalGoalCollectedDTO data)
        {
            if(data.goalType == eLevelGoalType.Piece && data.totalCollectedAmount >= unlockValue)
            {
                lockIcon.DOShakePosition(0.5f, new Vector2(1f, 1f), 20).OnComplete(() =>
                {
                    parentCell.Selectable = true;
                    ObjectPooler.ReturnToPool(PoolingType.CellLock, this, destroyCancellationToken);
                });
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<TotalGoalCollectedDTO>.Deregister(onEventWithArgs: OnTotalPieceCollected);
        }

        #endregion

        
    }
}