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
        [SerializeField] protected int totalCollectedAmount;
        
        [Header("Self Components")]
        [SerializeField] protected TextMeshProUGUI counter;
        [SerializeField] protected RectTransform icon;

        public abstract void SetUp(LevelGoalData goalData);

        public virtual void OnGoalCollected(int collectedAmount)
        {
            icon.DOScale(TargetScale, ScaleDuration).SetEase(Ease.Linear);
        }
    }
}