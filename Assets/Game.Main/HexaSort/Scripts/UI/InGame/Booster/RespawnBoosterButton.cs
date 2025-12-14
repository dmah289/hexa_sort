using Cysharp.Threading.Tasks;
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
            if (LocalDataManager.BoosterRespawnAmount <= 0)
                return;
            
            tray.RespawnCurrentStacks().Forget();
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }
    }
}