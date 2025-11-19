using DG.Tweening;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
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

        public override int Counter
        {
            get => counter;
            set
            {
                counter = value;
                counterTxt.text = $"{counter}/{goalData.targetAmount}";
                float newFillWidth = Counter / (float)goalData.targetAmount * MaxFillWidth;
                float duration = Mathf.Abs(fillRt.sizeDelta.x - newFillWidth) / 100f;
                Vector2 targetSize = new Vector2(newFillWidth, FillHeight);
                fillRt.DOSizeDelta(targetSize, duration).SetEase(Ease.OutSine);
            }
        }

        public override void SetUp(LevelGoalData goalData)
        {
            base.SetUp(goalData);
            
            fillRt.sizeDelta = new Vector2(0, FillHeight);
            progressBg.sizeDelta = new Vector2(0, BgHeight);
            
            Counter = 0;
            counterTxt.gameObject.SetActive(false);
        }

        public void AnimateExpansion(float duration)
        {
            progressBg.DOSizeDelta(new Vector2(MaxBgWidth, BgHeight), duration).OnComplete(() =>
            {
                counterTxt.gameObject.SetActive(true);
            });
        }
        
        public override void OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            Counter += collectedAmount;
            EventBus<TotalGoalGainedDTO>.Raise(new TotalGoalGainedDTO(eLevelGoalType.Piece, Counter));
        }
    }
}