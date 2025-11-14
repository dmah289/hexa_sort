using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI
{
    public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private const float FadeDuration = 0.2f;
        private const float TargetAlpha = 200/255f;
        private const float ClickIntervalThreshold = 1.5f;
        
        [SerializeField] private Image selfImg;
        private float lastClickTime;
        
        public UnityEvent OnBackgroundHiden;
        public UnityEvent OnBackgroundShown;

        private bool CanClick
        {
            get
            {
                float timeSinceLastClick = Time.time - lastClickTime;
                if (timeSinceLastClick >= ClickIntervalThreshold)
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
        
        public virtual void ShowBackground()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                selfImg.SetAlpha(TargetAlpha);
                OnBackgroundShown?.Invoke();
            }
        }
        
        public void HideBackground()
        {
            if (gameObject.activeSelf)
            {
                selfImg.DOFade(0, FadeDuration);
                OnBackgroundHiden?.Invoke();
            }
        }
    }
}