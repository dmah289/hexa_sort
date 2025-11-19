using System;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class WoodCell : MonoBehaviour
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
                }
                else counterTxt.text = counter.ToString();
            }
        }
        
        public void Setup()
        {
            gameObject.SetActive(true);
            parentCell.Selectable = false;
            Counter = 3;
        }

        private void OnGoalCollected(GoalCollectedDTO data)
        {
            if (data.GoalType == eLevelGoalType.Piece && parentCell.IsNeighborOf(data.SourceCellGridPos))
            {
                Counter--;
            }
        }
    }
}