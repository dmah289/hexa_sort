using DG.Tweening;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace HexaSort.UI.Gameplay.Goals
{
    public class PieceTrackerPanel : GoalTrackerPanel
    {
        private const float FillHeight = 110f;
        private const float BgHeight = 100;
        private const float MaxFillWidth = 270f;
        private const float MaxBgWidth = 300f;
        
        
        [Header("Self References")]
        [SerializeField] private RectTransform fillRt;
        [SerializeField] private RectTransform progressBg;
        
        public override void SetUp(LevelGoalData goalData)
        {
            this.goalData = goalData;
            
            counter.text = $"0/{goalData.targetAmount}";
            counter.gameObject.SetActive(false);
            
            fillRt.sizeDelta = new Vector2(0, FillHeight);
            progressBg.sizeDelta = new Vector2(0, FillHeight);
        }

        public void AnimateExpansion(float duration)
        {
            progressBg.DOSizeDelta(new Vector2(MaxBgWidth, BgHeight), duration).OnComplete(() =>
            {
                counter.gameObject.SetActive(true);
            });
        }
        
        public override void OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            totalCollectedAmount += collectedAmount;
            counter.text = $"{totalCollectedAmount} / {goalData.targetAmount}";
            
            float newFillWidth = (totalCollectedAmount / (float)goalData.targetAmount) * MaxFillWidth;
            fillRt.sizeDelta = new Vector2(newFillWidth, FillHeight);
            
            EventBus<TotalGoalGainedDTO>.Raise(new TotalGoalGainedDTO(eLevelGoalType.Piece, totalCollectedAmount));
        }
    }
}