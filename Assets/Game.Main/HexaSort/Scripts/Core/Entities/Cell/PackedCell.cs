using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Controllers;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.Core.Entities.Grid
{
    public class PackedCell : MonoBehaviour, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private TextMeshPro unlockValueText;
        [SerializeField] private Transform lockIcon;
        
        [Header("References")]
        [SerializeField] private HexCell parentCell;
        
        [Header("State Manager")]
        [SerializeField] private int unlockValue;
        
        public int UnlockValue
        {
            get => unlockValue;
            set
            {
                unlockValue = value;
                unlockValueText.text = unlockValue.ToString();
            }
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
        
        #region Class Methods

        public async UniTask Setup(PackedStackData packedStackData)
        {
            gameObject.SetActive(true);
            UnlockValue = packedStackData.UnlockValue;
            parentCell.Selectable = false;

            if (packedStackData.ColorLayers.Length > 0)
            {
                HexStackController hexStack = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, parentCell.selfTransform);
                await hexStack.OnSpawningOnCell(parentCell, packedStackData);
                
                transform.localPosition = transform.localPosition.With(z: -hexStack.Height - 0.1f);
            }
        }

        #endregion

        #region Total Goal Collected Callbacks

        public void RegisterCallbacks()
        {
            Debug.Log("Registered - " + gameObject.Path());
            EventBus<TotalGoalGainedDTO>.Register(onEventWithArgs: OnTotalGoalCollected);
        }
        
        public void OnTotalGoalCollected(TotalGoalGainedDTO data)
        {
            Debug.Log($"Received - {data.totalCollectedAmount}");
            if (data.goalType == eLevelGoalType.Piece && data.totalCollectedAmount >= UnlockValue)
            {
                // gameObject.SetActive(false);
                
                lockIcon.DOShakeRotation(0.5f, 5f, 20)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        if(!parentCell.IsOccupied)
                            parentCell.Selectable = true;
                        else MergeController.Instance.HandleCheckingMerge(parentCell).Forget();
                        
                        gameObject.SetActive(false);
                    });
            }
        }

        public void DeregisterCallbacks()
        {
            Debug.Log("Deregistered - " + gameObject.Path());
            EventBus<TotalGoalGainedDTO>.Deregister(onEventWithArgs: OnTotalGoalCollected);
        }

        #endregion
    }
}