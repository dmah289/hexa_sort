using Cysharp.Threading.Tasks;
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
        private const float ScaleDuration = 0.07f;
        
        [Header("----- Config -----")]
        [SerializeField] protected LevelGoalData goalData;
        [SerializeField] protected int counter;
        
        [Header("----- Self Components -----")]
        [SerializeField] protected TextMeshProUGUI counterTxt;
        [SerializeField] protected RectTransform iconRt;
        
        public abstract bool IsCompleted { get; }
        

        public virtual void SetUp(LevelGoalData goalData)
        {
            this.goalData = goalData;
        }

        public virtual async UniTask OnGoalCollected(int collectedAmount)
        {
            iconRt.DOScale(TargetScale, ScaleDuration)
                .SetEase(Ease.OutSine)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}