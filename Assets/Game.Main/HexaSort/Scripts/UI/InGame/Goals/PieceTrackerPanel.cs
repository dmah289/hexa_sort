using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using Unity.VisualScripting;
using UnityEngine;

namespace HexaSort.UI.Gameplay.Goals
{
    public class PieceTrackerPanel : GoalTrackerPanel
    {
        private const float FillHeight = 110f;
        private const float MaxFillWidth = 270f;
        
        [Header("Self References")]
        [SerializeField] private RectTransform fillRT;
        
        public override void SetUp(LevelGoalData goalData)
        {
            this.goalData = goalData;
            counter.text = $"0/{goalData.targetAmount}";
            fillRT.sizeDelta = new Vector2(0, FillHeight);
        }
        
        public override void OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            totalCollectedAmount += collectedAmount;
            counter.text = $"{totalCollectedAmount} / {goalData.targetAmount}";
            
            float fillWidth = (totalCollectedAmount / (float)goalData.targetAmount) * MaxFillWidth;
            fillRT.sizeDelta = new Vector2(fillWidth, FillHeight);
            
            EventBus<TotalGoalCollectedDTO>.Raise(new TotalGoalCollectedDTO(eLevelGoalType.Piece, totalCollectedAmount));
        }
    }
}