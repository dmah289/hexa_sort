using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class DestroyStackBoosterButton : ABoosterButton
    {
        [Header("----- Gameplay Components -----")]
        [SerializeField] private GridController grid;
        
        public override int Amount
        {
            get => LocalDataManager.BoosterDestroyStackAmount;
            set
            {
                LocalDataManager.BoosterDestroyStackAmount = value;
                SetAmountText(value);
            }
        }

        public override void OnBoosterButtonClicked()
        {
            if (LevelManager.Instance.CurrentLevelState == eLevelState.IsUsingDestroyStackBooster)
                return;
            
            LevelManager.Instance.CurrentLevelState = eLevelState.IsUsingDestroyStackBooster;
            Debug.Log("Destroy Stack Booster Used");
            // TODO : Enable stack selection + OnSelect stack
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }

        
    }
}