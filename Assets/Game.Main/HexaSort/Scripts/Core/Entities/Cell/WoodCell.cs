using System;
using HexaSort.UI.BaseSystem;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.Core.Entities.Grid
{
    public class WoodCell : MonoBehaviour, IEventBusListener
    {
        [Header("Components")]
        [SerializeField] private int counter;
        [SerializeField] private TextMeshPro counterTxt;
        
        [Header("References")]
        [SerializeField] private HexCell parentCell;

        public int Counter
        {
            get => counter;
            set
            {
                counter = value;
                if(counter <= 0)
                {
                    gameObject.SetActive(false);
                    counter = 0;
                    parentCell.Selectable = true;
                    VFXManager.Instance.PlayVFXToPieceGoalPanel(parentCell,
                        eLevelGoalType.Wood,
                        1,
                        destroyCancellationToken);
                }
                else counterTxt.text = counter.ToString();
            }
        }

        private void OnEnable()
        {
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            DeregisterCallbacks();
        }

        public void Setup()
        {
            gameObject.SetActive(true);
            parentCell.Selectable = false;
            Counter = 3;
        }

        public void RegisterCallbacks()
        {
            EventBus<GoalCollectedDTO>.Register(onEventWithArgs: OnGoalCollected);
        }
        
        private void OnGoalCollected(GoalCollectedDTO data)
        {
            if (data.GoalType == eLevelGoalType.Piece && parentCell.IsNeighborOf(data.SourceCellGridPos))
            {
                Counter--;
            }
        }

        public void DeregisterCallbacks()
        {
            EventBus<GoalCollectedDTO>.Deregister(onEventWithArgs: OnGoalCollected);
        }
    }
}