using Cysharp.Threading.Tasks;
using DG.Tweening;
using LevelEditor.LevelData;
using UnityEngine;

namespace HexaSort.UI.Gameplay.Goals
{
    public class OtherTrackerPanel : GoalTrackerPanel
    {
        [Header("Child Components")]
        [SerializeField] private RectTransform doneIconRt;

        public override bool IsCompleted => counter <= 0;

        public override void SetUp(LevelGoalData goalData)
        {
            base.SetUp(goalData);
            
            doneIconRt.gameObject.SetActive(false);
            SetCounter(goalData.targetAmount);
        }

        public void SetCounter(int value)
        {
            value = Mathf.Clamp(value, 0, goalData.targetAmount);
            counter = value;
                
            if (counter <= 0)
            {
                counter = 0;
                counterTxt.gameObject.SetActive(false);
                doneIconRt.gameObject.SetActive(true);
            }
            else counterTxt.text = $"{counter}";
        }

        public override async UniTask OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount).Forget();
            
            SetCounter(counter - collectedAmount);
        }
    }
}