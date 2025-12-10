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

        public override void OnBoosterButtonClicked()
        {
            if (LevelManager.Instance.CurrentLevelState == eLevelState.IsUsingRespawnBooster)
                return;
            
            //TODO : Show tut
            
            LevelManager.Instance.CurrentLevelState = eLevelState.IsUsingRespawnBooster;
            tray.RespawnCurrentStacks();
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }
    }
}