using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.UI
{
    public class MButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("Self Components")]
        [SerializeField] protected RectTransform selfRT;
        
        [Header("Scale Animation Settings")]
        [SerializeField] protected float targetScale = 1.1f;
        [SerializeField] protected float animDuration = 0.1f;
        [SerializeField] protected float clickIntervalThreshold = 0.5f;
        [SerializeField] protected float actionDelay;
        protected float lastClickTime;
        
        [SerializeField] protected UnityEvent OnScaleAnimDone;

        
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
        

        protected virtual void Awake()
        {
            selfRT = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            lastClickTime = -clickIntervalThreshold;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClicked();
        }

        protected virtual void OnButtonClicked()
        {
            selfRT.DOKill();
            selfRT.DOScale(targetScale, animDuration)
                .OnStart(() => selfRT.localScale = Vector3.one)
                .OnComplete(() =>
                {
                    selfRT.localScale = Vector3.one;
                    if(CanClick)
                        DOVirtual.DelayedCall(actionDelay, () => OnScaleAnimDone?.Invoke());
                });
        }
    }
}