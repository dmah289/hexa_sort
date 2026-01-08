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

        protected override void OnBoosterChanged(BoosterChangedDTO data)
        {
            if(data.BoosterType == eBoosterType.DestroyStack)
                SetAmountText(data.NewAmount);
        }

        public override void OnBoosterButtonClicked()
        {
            if (Amount <= 0)
            {
                ToastManager.Instance.Show("Not enough Destroy Stack booster!");
                return;
            }
            
            if (grid.HasNoMergableStacks)
            {
                ToastManager.Instance.Show("No stacks to destroy!");
                return;
            }
            
            LocalDataManager.BoosterDestroyStackAmount--;
            ToastManager.Instance.Show("Choose a stack to destroy");
            LevelManager.Instance.CurrentLevelState = eLevelState.IsUsingDestroyStackBooster;
            InGamePage.Instance.HideBoosterButtons();
            grid.SetStacksOnGridSelectableState(true);
        }

        public override void OnAddBoosterButtonClicked()
        {
            PopupManager.Instance.ShowBuyBoosterPopup(eBoosterType.DestroyStack);
        }

        public override void CheckUnlockBoosterButton()
        {
            if (LocalDataManager.LevelIndex >= ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack])
            {
                unlockBoosterGroup.alpha = 1;
                
                lockBoosterGroup.SetActive(false);
                SetAmountText(Amount);
            }
            else
            {
                unlockBoosterGroup.alpha = 0;
                amountTxtGroup.SetActive(false);
                plusBtn.gameObject.SetActive(false);
                
                lockBoosterGroup.SetActive(true);
                unlockLevelTxt.text = $"Lv.{ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack] + 1}";
            }
        }

        public override void OnLockBoosterButtonClicked()
        {
            ToastManager.Instance.Show($"Unlock at level {ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack]+1}");
        }
    }
}