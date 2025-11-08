using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI
{
    public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public static float FadeDuration = 0.2f;
        
        [SerializeField] private Image selfImg;
        [SerializeField] private float targetAlpha = 200/255f;
        
        [SerializeField] protected float clickIntervalThreshold;
        [SerializeField] protected float lastClickTime;
        
        public UnityEvent OnBackgroundHiden;
        public UnityEvent OnBackgroundShown;
        
        protected bool CanClick
        {
            get
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                if (timeSinceLastClick >= clickIntervalThreshold)
                {
                    lastClickTime = Time.time;
                    return true;
                }
                return false;
            }
        }

        private void OnEnable()
        {
            lastClickTime = Time.time;
        }


        private void Awake()
        {
            selfImg = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gameObject && CanClick)
                HideBackground();
        }

        public void HideBackground()
        {
            // selfImg.FadeAlphaToTarget(FadeDuration).Forget();
            OnBackgroundHiden?.Invoke();
        }
        
        public virtual void ShowBackground()
        {
            gameObject.SetActive(true);
            // selfImg.SetAlpha(targetAlpha);
            OnBackgroundShown?.Invoke();
        }
    }
}