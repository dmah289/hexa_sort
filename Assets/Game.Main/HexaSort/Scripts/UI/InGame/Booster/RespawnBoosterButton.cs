using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class RespawnBoosterButton : ABoosterButton
    {
        [Header("----- Gameplay Components -----")]
        [SerializeField] private TrayController tray;
        
        public override int Amount
        {
            get => LocalDataManager.BoosterRespawnAmount;
            set
            {
                LocalDataManager.BoosterRespawnAmount = value;
                SetAmountText(value);
            }
        }
        
        protected override void Init()
        {
            if (LocalDataManager.BoosterRespawnAmount > 0)
            {
                plusBtn.gameObject.SetActive(false);
                amountTxt.gameObject.SetActive(true);
                
                amountTxt.text = LocalDataManager.BoosterRespawnAmount.ToString();
            }
            else
            {
                plusBtn.gameObject.SetActive(true);
                amountTxt.gameObject.SetActive(false);
            }
        }

        public override void OnBoosterButtonClicked()
        {
            LevelManager.Instance.CurrentLevelState = eLevelState.IsUsingBooster;
            
            tray.RespawnCurrentStacks();
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }
    }
}