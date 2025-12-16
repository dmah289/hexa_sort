using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using manhnd_sdk.UITools.Toast;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class DestroyStackBoosterButton : ABoosterButton
    {
        [Header("----- Gameplay Components -----")]
        [SerializeField] private GridController grid;

        public override int Amount => LocalDataManager.BoosterDestroyStackAmount;

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
            
            Debug.Log("Destroy Stack Booster Used");
            ToastManager.Instance.Show("Choose a stack to destroy");
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }

        
    }
}