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

        public override int Amount => LocalDataManager.BoosterRespawnAmount;

        protected override void Awake()
        {
            base.Awake();
            
            SetAmountText(Amount);
        }

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
    }
}