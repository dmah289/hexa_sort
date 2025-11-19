using DG.Tweening;
using LevelEditor.LevelData;
using UnityEngine;

namespace HexaSort.UI.Gameplay.Goals
{
    public class OtherTrackerPanel : GoalTrackerPanel
    {
        [Header("Child Components")]
        [SerializeField] private RectTransform doneIconRt;
        
        public override int Counter
        {
            get => counter;
            set
            {
                counter = value;
                
                if (counter <= 0)
                {
                    counter = 0;
                    counterTxt.gameObject.SetActive(false);
                    doneIconRt.gameObject.SetActive(true);
                    
                    doneIconRt.DOScale(1.1f, 0.3f)
                        .SetEase(Ease.OutSine)
                        .SetLoops(1, LoopType.Yoyo);
                }
                else counterTxt.text = $"{counter}";
            }
        }

        public override void SetUp(LevelGoalData goalData)
        {
            base.SetUp(goalData);
            
            doneIconRt.gameObject.SetActive(false);
            Counter = goalData.targetAmount;
        }

        public override void OnGoalCollected(int collectedAmount)
        {
            base.OnGoalCollected(collectedAmount);
            
            Counter -= collectedAmount;
            
            
        }
    }
}