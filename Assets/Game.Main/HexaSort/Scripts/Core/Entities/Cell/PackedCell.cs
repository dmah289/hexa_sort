using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Core.Entities;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class PackedCell : MonoBehaviour, IEventBusListener
    {
        [Header("References")]
        [SerializeField] private HexCell parentCell;
        [SerializeField] private TextMeshPro unlockValueText;
        [SerializeField] private Transform lockIcon;
        
        [Header("State Manager")]
        [SerializeField] private int unlockValue;
        

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

        #region Goal Collected Callbacks

        public void RegisterCallbacks()
        {
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnGoalCollected);
        }
        
        public void OnGoalCollected(TotalGoalGainedDTO data)
        {
            if (data.goalType == eLevelGoalType.Piece && data.totalCollectedAmount >= unlockValue)
            {
                lockIcon.DOShakeRotation(0.5f, 5f, 20)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() => gameObject.SetActive(false));
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<TotalGoalGainedDTO>.Deregister(onEventWithArgs: OnGoalCollected);
        }

        #endregion

        #region Class Methods

        public async UniTask Setup(PackedStackData packedStackData)
        {
            gameObject.SetActive(true);
            
            HexStackController hexStack = await ObjectPooler.GetFromPool<HexStackController>(
                PoolingType.HexStack, destroyCancellationToken, parentCell.selfTransform);
                
            await hexStack.OnSpawningOnCell(parentCell, packedStackData);
            
            unlockValue = packedStackData.UnlockValue;
            unlockValueText.text = unlockValue.ToString();
            
            unlockValueText.transform.localPosition = new Vector3(0, 0, hexStack.Height + 0.1f);
            lockIcon.localPosition = new Vector3(0, 0, hexStack.Height + 0.1f);
        }

        #endregion
    }
}