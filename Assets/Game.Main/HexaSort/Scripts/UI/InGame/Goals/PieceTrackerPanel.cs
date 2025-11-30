using Cysharp.Threading.Tasks;
using DG.Tweening;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI targetAmountTxt;

        public override bool IsCompleted 
            => counter >= m_GoalData.targetAmount;

        private async UniTask SetCounter(int value)
        {
            counter = value;
            counterTxt.text = $"{counter}/{m_GoalData.targetAmount}";
            float newFillWidth = counter / (float)m_GoalData.targetAmount * MaxFillWidth;
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
            
            targetAmountTxt.gameObject.SetActive(true);
            targetAmountTxt.text = $"{goalData.targetAmount}";
        }

        public void AnimateExpansion(float duration)
        {
            targetAmountTxt.gameObject.SetActive(false);
            progressBg.DOSizeDelta(new Vector2(MaxBgWidth, BgHeight), duration)
                .OnComplete(() => counterTxt.gameObject.SetActive(true));
        }
        
        public override async UniTask OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);

            int totalGained = Mathf.Clamp(counter + collectedAmount, 0, m_GoalData.targetAmount);
            EventBus<TotalGoalGainedDTO>.Raise(
                new TotalGoalGainedDTO(eLevelGoalType.Piece, totalGained));
            await SetCounter(totalGained);
        }
    }
}