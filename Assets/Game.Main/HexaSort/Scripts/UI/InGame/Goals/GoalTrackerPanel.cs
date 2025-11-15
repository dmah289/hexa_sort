using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Gameplay.Goals
{
    public abstract class GoalTrackerPanel : MonoBehaviour
    {
        [SerializeField] private Text targetTxt;
        
        [SerializeField] private Text progressTxt;

        [SerializeField] private RectTransform icon;

        [SerializeField] private Image progressBar;
        
        [SerializeField] private RectTransform background;

        public static float ScaleDuration = 0.05f;

        public static Vector2 TargetScale = new(1.08f, 1.08f);
        
        [SerializeField] private Ease scaleEase;

        public static float stdWidth = 300f;

        public static float onBannerWidth = 220f;

        public void SetUp(int target)
        {
            targetTxt.text = $"/{target}";
            progressTxt.text = "0";
            progressBar.fillAmount = 0f;
        }

        public async Task OnScoreGoalUpdate(int target, float percentage)
        {
            if (target == 0)
            {
                progressTxt.text = "0";
                progressBar.fillAmount = 0f;
                return;
            }
            
            Sequence sequence = DOTween.Sequence();

            int count = target - int.Parse(progressTxt.text);
            
            for (int i = 0; i < count; i++)
            {
                sequence.Append(icon.DOScale(TargetScale, ScaleDuration).SetEase(scaleEase).OnComplete(() =>
                {
                    int num = int.Parse(progressTxt.text);
                    num++;
                    progressTxt.text = num.ToString();
                    icon.localScale = Vector3.one;
                }));
            }

            progressBar.DOFillAmount(percentage, ScaleDuration * count)
                .SetEase(scaleEase);

            await Task.Delay((int)(ScaleDuration * count * 1000));
        }
    }
}