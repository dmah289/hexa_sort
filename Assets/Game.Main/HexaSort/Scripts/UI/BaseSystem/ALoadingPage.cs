using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coffee_Rush.UI.BaseSystem
{
    public abstract class ALoadingPage : MonoBehaviour
    {
        [Header("Shared Fields")]
        [SerializeField] private RectTransform fillRectTransform;
        [SerializeField] private float fillDuration = 3.5f;
        

        protected abstract UniTaskVoid OnFullFillAmount();

        protected async UniTaskVoid StartLoading()
        {
            float timer = 0f;
            float curWidth = 0;
            fillRectTransform.sizeDelta = new Vector2(curWidth, 55);

            while (timer < fillDuration)
            {
                timer += Time.deltaTime;

                curWidth = (timer / fillDuration) * 825;
                fillRectTransform.sizeDelta = new Vector2(curWidth, 55);
                
                await UniTask.Yield();
            }
            
            fillRectTransform.sizeDelta = new Vector2(825, 55);
            
            OnFullFillAmount();
        }
    }
}