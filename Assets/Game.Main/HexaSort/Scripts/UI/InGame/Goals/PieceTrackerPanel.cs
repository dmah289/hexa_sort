using Game.Main.LevelEditor.Scripts.LevelData;
using UnityEngine;

namespace HexaSort.UI.Gameplay.Goals
{
    public class PieceTrackerPanel : GoalTrackerPanel
    {
        private const float FillHeight = 110f;
        private const float MaxFillWidth = 270f;
        
        [Header("Self References")]
        [SerializeField] private RectTransform fillRT;
        
        public override void SetUp(LevelGoal goal)
        {
            this.goal = goal;
            counter.text = $"0/{goal.targetAmount}";
            fillRT.sizeDelta = new Vector2(0, FillHeight);
        }
        
        public override void OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            currCollectedAmount += collectedAmount;
            counter.text = $"{currCollectedAmount} / {goal.targetAmount}";
            
            float fillWidth = (currCollectedAmount / (float)goal.targetAmount) * MaxFillWidth;
            fillRT.sizeDelta = new Vector2(fillWidth, FillHeight);
        }
    }
}