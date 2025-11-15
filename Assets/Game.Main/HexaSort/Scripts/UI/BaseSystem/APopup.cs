using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Extensions;
using Framework.UI;
using manhnd_sdk.ExtensionMethods;
using manhnd_sdk.Scripts.ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.MainMenu.Home
{
    public class APopup : MonoBehaviour
    {
        public const float MoveSpeed = 2870f;
        
        [Header("UI Elements")]
        [SerializeField] protected BackgroundClickHandler bgClickHandler;
        [SerializeField] protected RectTransform selfRectTransform;
        [SerializeField] protected Image popUpImg;

        [Header("Movement Settings")]
        [SerializeField] protected Vector2 shownPos;
        private Vector2 hidenPos;
        private float animDuration;
        

        private void Awake()
        {
            bgClickHandler = GetComponentInParent<BackgroundClickHandler>();
            selfRectTransform = GetComponent<RectTransform>();
            popUpImg = GetComponent<Image>();
            
            hidenPos = new Vector2(0, -(Screen.height / 2f + selfRectTransform.rect.height / 2f + 50f));
            animDuration = Vector2.Distance(shownPos, hidenPos) / MoveSpeed;
        }

        public virtual void ShowPopup()
        {
            popUpImg.DOKill();
            if (popUpImg == null)
            {
                Debug.Log(gameObject.Path());
            }
            else popUpImg.SetAlpha(1);
            selfRectTransform.localScale = Vector3.one;
            
            selfRectTransform.DOKill();
            selfRectTransform.anchoredPosition = hidenPos;
            selfRectTransform.DOAnchorPos(shownPos, animDuration)
                .SetEase(Ease.OutElastic, 0.01f, 0.5f);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        protected virtual async UniTaskVoid HidePopupAsync()
        {
            popUpImg.DOKill();
            popUpImg.SetAlpha(1);
            popUpImg.DOFade(0.8f, animDuration);
            
            selfRectTransform.DOKill();
            selfRectTransform.DOScale(0f, 0.85f * animDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => bgClickHandler.gameObject.SetActive(false));
        }
    }
}