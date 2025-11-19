using DG.Tweening;
using LevelEditor.LevelData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Gameplay.Goals
{
    public abstract class GoalTrackerPanel : MonoBehaviour
    {
        private const float TargetScale = 1.1f;
        private const float ScaleDuration = 0.05f;
        
        [Header("Config")]
        [SerializeField] protected LevelGoalData goalData;
        [SerializeField] protected int counter;
        
        [Header("Self Components")]
        [SerializeField] protected TextMeshProUGUI counterTxt;
        [SerializeField] protected RectTransform iconRt;

        public abstract int Counter { get; set; }

        public virtual void SetUp(LevelGoalData goalData)
        {
            this.goalData = goalData;
        }

        public virtual void OnGoalCollected(int collectedAmount)
        {
            iconRt.DOScale(TargetScale, ScaleDuration).SetEase(Ease.Linear);
        }
    }
}