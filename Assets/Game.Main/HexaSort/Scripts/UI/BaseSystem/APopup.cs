using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Home
{
    public abstract class APopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] protected BackgroundClickHandler bgClickHandler;
        [SerializeField] protected RectTransform selfRectTransform;
        [SerializeField] protected Image popUpImg;


        [Header("Movement Settings")]
        [SerializeField] protected Vector2 hidenPos;
        [SerializeField] protected Vector2 shownPos;
        [SerializeField] protected float animDuration;
        
        [SerializeField] protected Ease easeType;
        [SerializeField] protected float amplitude;
        [SerializeField] protected float period;

        private void Awake()
        {
            bgClickHandler = GetComponentInParent<BackgroundClickHandler>();
            selfRectTransform = GetComponent<RectTransform>();
            popUpImg = GetComponent<Image>();
        }

        public virtual void ShowPopup()
        {
            popUpImg.DOKill();
            //popUpImg.SetAlpha(1);
            
            selfRectTransform.DOKill();
            selfRectTransform.anchoredPosition = hidenPos;
            selfRectTransform.DOAnchorPos(shownPos, animDuration)
                .SetEase(easeType, amplitude, period);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        protected virtual async UniTaskVoid HidePopupAsync()
        {
            popUpImg.DOKill();
            //popUpImg.SetAlpha(1);
            popUpImg.DOFade(0.5f, animDuration);
            
            selfRectTransform.DOKill();
            selfRectTransform.DOAnchorPos(hidenPos, animDuration * 0.5f)
                .SetEase(Ease.Linear);
            
            await UniTask.Delay(TimeSpan.FromSeconds(animDuration));
            
            bgClickHandler.gameObject.SetActive(false);
        }
    }
}