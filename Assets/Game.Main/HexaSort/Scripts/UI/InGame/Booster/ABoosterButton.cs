using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ExtensionMethods;
using TMPro;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public enum eBoosterType
    {
        Respawn,
        DestroyStack
    }
    
    public abstract class ABoosterButton : MButton
    {
        [Header("----- Self References -----")]
        [SerializeField] protected TextMeshProUGUI amountTxt;
        [SerializeField] protected MButton plusBtn;
        
        
        [SerializeField] protected RectTransform groupBtnRt;
        //TODO : Disable btn, enable after used
        
        [Header("----- Configs -----")]
        [SerializeField] protected float shownPosX;
        [SerializeField] protected float moveDuration = 0.2f;
        
        public abstract int Amount { get; set; }
        public abstract void OnBoosterButtonClicked();
        public abstract void OnAddBoosterButtonClicked();

        protected void SetAmountText(int amount)
        {
            if (amount > 0)
            {
                plusBtn.gameObject.SetActive(false);
                amountTxt.gameObject.SetActive(true);
                
                amountTxt.text = amount.ToString();
            }
            else
            {
                plusBtn.gameObject.SetActive(true);
                amountTxt.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            groupBtnRt.anchoredPosition = groupBtnRt.anchoredPosition.With(x: -shownPosX);
            groupBtnRt.DOAnchorPosX(shownPosX, moveDuration)
                .SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            groupBtnRt.anchoredPosition = groupBtnRt.anchoredPosition.With(x: shownPosX);
            groupBtnRt.DOAnchorPosX(-shownPosX, moveDuration)
                .SetEase(Ease.InBack);
        }
    }
}