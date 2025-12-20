using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.HexaSort.Scripts.UI.Popup;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.UITools.Toast;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class RespawnBoosterButton : ABoosterButton
    {
        [Header("----- Gameplay Components -----")]
        [SerializeField] private TrayController tray;

        public override int Amount => LocalDataManager.BoosterRespawnAmount;

        protected override void OnBoosterUpdated(BoosterUpdatedDTO data)
        {
            if(data.BoosterType == eBoosterType.Respawn)
                SetAmountText(data.NewAmount);
        }

        public override void OnBoosterButtonClicked()
        {
            if (Amount <= 0)
            {
                ToastManager.Instance.Show("Not enough Respawn booster!");
                return;
            }
            
            LocalDataManager.BoosterRespawnAmount--;
            tray.RespawnCurrentStacks().Forget();
        }

        public override void OnAddBoosterButtonClicked()
        {
            PopupManager.Instance.ShowBuyBoosterPopup(eBoosterType.Respawn);
        }

        public override void CheckUnlockBoosterButton()
        {
            if (LocalDataManager.LevelIndex >= ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn])
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
                unlockLevelTxt.text = $"Lv.{ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn] + 1}";
            }
        }

        public override void OnLockBoosterButtonClicked()
        {
            ToastManager.Instance.Show($"Unlock at level {ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn]+1}");
        }
    }
}