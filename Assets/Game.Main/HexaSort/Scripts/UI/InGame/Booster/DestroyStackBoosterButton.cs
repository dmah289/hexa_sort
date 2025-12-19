using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.HexaSort.Scripts.UI.Popup;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.UITools.Toast;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class DestroyStackBoosterButton : ABoosterButton
    {
        [Header("----- Gameplay Components -----")]
        [SerializeField] private GridController grid;

        public override int Amount => LocalDataManager.BoosterDestroyStackAmount;

        protected override void OnBoosterUpdated(BoosterUpdatedDTO data)
        {
            SetAmountText(data.NewAmount);
        }

        public override void OnBoosterButtonClicked()
        {
            if (Amount <= 0) return;
            
            Debug.Log("Destroy Stack Booster Used");
            ToastManager.Instance.Show("Choose a stack to destroy");
        }

        public override void OnAddBoosterButtonClicked()
        {
            PopupManager.Instance.ShowBuyBoosterPopup(eBoosterType.DestroyStack);
        }

        public override void CheckUnlockBoosterButton()
        {
            if (LocalDataManager.LevelIndex >= ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack])
            {
                lockBoosterGroup.SetActive(false);
                SetAmountText(Amount);
            }
            else
            {
                amountTxtGroup.SetActive(false);
                plusBtn.gameObject.SetActive(false);
                
                lockBoosterGroup.SetActive(true);
                unlockLevelTxt.text = $"Lv.{ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack]}";
            }
        }

        public override void OnLockBoosterButtonClicked()
        {
            ToastManager.Instance.Show($"Unlock at level {ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack]}");
        }
    }
}