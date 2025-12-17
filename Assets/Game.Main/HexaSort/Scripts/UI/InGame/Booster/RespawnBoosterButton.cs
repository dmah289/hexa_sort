using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
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
            SetAmountText(data.NewAmount);
        }

        public override void OnBoosterButtonClicked()
        {
            if (Amount <= 0) return;
            
            LocalDataManager.BoosterRespawnAmount--;
            tray.RespawnCurrentStacks().Forget();
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }

        public override void CheckUnlockBoosterButton()
        {
            if (LocalDataManager.LevelIndex >= ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn])
            {
                lockBoosterGroup.SetActive(false);
                SetAmountText(Amount);
            }
            else
            {
                amountTxtGroup.SetActive(false);
                plusBtn.gameObject.SetActive(false);
                
                lockBoosterGroup.SetActive(true);
                unlockLevelTxt.text = $"Lv.{ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn]}";
            }
        }

        public override void OnLockBoosterButtonClicked()
        {
            ToastManager.Instance.Show($"Unlock at level {ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn]}");
        }
    }
}