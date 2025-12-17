using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using TMPro;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public struct BoosterUpdatedDTO : IEventDTO
    {
        public eBoosterType BoosterType;
        public int NewAmount;

        public BoosterUpdatedDTO(eBoosterType boosterType, int newAmount)
        {
            BoosterType = boosterType;
            NewAmount = newAmount;
        }
    }
    
    public enum eBoosterType
    {
        None,
        Respawn,
        DestroyStack
    }
    
    public abstract class ABoosterButton : MButton
    {
        [Header("----- Self References -----")]
        [SerializeField] protected TextMeshProUGUI amountTxt;
        [SerializeField] protected GameObject amountTxtGroup;
        [SerializeField] protected MButton plusBtn;
        [SerializeField] protected RectTransform groupBtnRt;
        
        [Header("----- Lock Booster -----")]
        [SerializeField] protected GameObject lockBoosterGroup;
        [SerializeField] protected TextMeshProUGUI unlockLevelTxt;
        
        [Header("----- Configs -----")]
        [SerializeField] protected float shownPosX;
        [SerializeField] protected float moveDuration = 0.2f;

        protected override void Awake()
        {
            base.Awake();
            
            EventBus<BoosterUpdatedDTO>.Register(onEventWithArgs: OnBoosterUpdated);
        }

        protected abstract void OnBoosterUpdated(BoosterUpdatedDTO data);
        public abstract int Amount { get; }
        public abstract void OnBoosterButtonClicked();
        public abstract void OnAddBoosterButtonClicked();
        public abstract void CheckUnlockBoosterButton();
        public abstract void OnLockBoosterButtonClicked();

        protected void SetAmountText(int amount)
        {
            if (amount > 0)
            {
                plusBtn.gameObject.SetActive(false);
                amountTxtGroup.SetActive(true);
                
                amountTxt.text = amount.ToString();
            }
            else
            {
                plusBtn.gameObject.SetActive(true);
                amountTxtGroup.SetActive(false);
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