using DG.Tweening;
using LevelEditor.LevelData;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Gameplay.Goals
{
    public abstract class GoalTrackerPanel : MonoBehaviour
    {
        private const float TargetScale = 1.1f;
        private const float ScaleDuration = 0.05f;
        
        [Header("Config")]
        [SerializeField] protected LevelGoal goal;
        [SerializeField] protected int currCollectedAmount;
        
        [Header("Self Components")]
        [SerializeField] protected Text counter;
        [SerializeField] protected RectTransform icon;

        public abstract void SetUp(LevelGoal goal);

        public virtual void OnGoalCollected(int collectedAmount)
        {
            icon.DOScale(TargetScale, ScaleDuration).SetEase(Ease.Linear);
        }
    }
}