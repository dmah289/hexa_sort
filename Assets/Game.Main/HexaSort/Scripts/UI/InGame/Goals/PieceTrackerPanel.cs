using Cysharp.Threading.Tasks;
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

        public override bool IsCompleted 
            => counter >= goalData.targetAmount;

        private async UniTask SetCounter(int value)
        {
            value = Mathf.Clamp(value, 0, goalData.targetAmount);
            counter = value;
            counterTxt.text = $"{counter}/{goalData.targetAmount}";
            float newFillWidth = counter / (float)goalData.targetAmount * MaxFillWidth;
            float duration = Mathf.Abs(fillRt.sizeDelta.x - newFillWidth) / 100f;
            Vector2 targetSize = new Vector2(newFillWidth, FillHeight);
            await fillRt.DOSizeDelta(targetSize, duration).SetEase(Ease.OutSine);
        }

        public override void SetUp(LevelGoalData goalData)
        {
            base.SetUp(goalData);
            
            fillRt.sizeDelta = new Vector2(0, FillHeight);
            progressBg.sizeDelta = new Vector2(0, BgHeight);
            
            SetCounter(0).Forget();
            counterTxt.gameObject.SetActive(false);
        }

        public void AnimateExpansion(float duration)
        {
            progressBg.DOSizeDelta(new Vector2(MaxBgWidth, BgHeight), duration)
                .OnComplete(() => counterTxt.gameObject.SetActive(true));
        }
        
        public override async UniTask OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            EventBus<TotalGoalGainedDTO>.Raise(
                new TotalGoalGainedDTO(eLevelGoalType.Piece,
                    counter + collectedAmount));
            await SetCounter(counter + collectedAmount);
        }
    }
}