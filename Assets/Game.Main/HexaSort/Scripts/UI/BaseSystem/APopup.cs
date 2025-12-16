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
        [SerializeField] protected PopupPanel popupPanel;
        [SerializeField] protected RectTransform selfRT;
        [SerializeField] protected Image popupImg;

        [Header("Movement Settings")]
        [SerializeField] protected Vector2 shownPos;
        private Vector2 hidenPos;
        private float animDuration;
        

        protected virtual void Awake()
        {
            popupPanel = GetComponentInParent<PopupPanel>();
            selfRT = GetComponent<RectTransform>();
            popupImg = GetComponent<Image>();
            
            hidenPos = new Vector2(0, -(Screen.height / 2f + selfRT.rect.height / 2f + 50f));
            animDuration = Vector2.Distance(shownPos, hidenPos) / MoveSpeed;
        }

        public virtual void ShowPopup()
        {
            popupImg.DOKill();
            if (popupImg == null)
            {
                Debug.Log(gameObject.Path());
            }
            else popupImg.SetAlpha(1);
            selfRT.localScale = Vector3.one;
            
            selfRT.DOKill();
            selfRT.anchoredPosition = hidenPos;
            selfRT.DOAnchorPos(shownPos, animDuration)
                .SetEase(Ease.OutElastic, 0.01f, 0.5f);
        }

        public void HidePopup() => HidePopupAsync().Forget();
        protected virtual async UniTaskVoid HidePopupAsync()
        {
            popupImg.DOKill();
            popupImg.SetAlpha(1);
            popupImg.DOFade(0.8f, animDuration);
            
            selfRT.DOKill();
            selfRT.DOScale(0f, 0.85f * animDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => popupPanel.gameObject.SetActive(false));
        }
    }
}