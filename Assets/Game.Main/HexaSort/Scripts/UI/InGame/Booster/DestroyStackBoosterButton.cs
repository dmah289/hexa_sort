using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;

namespace HexaSort.UI.Loading.InGame
{
    public class DestroyStackBoosterButton : ABoosterButton
    {
        public override int Amount
        {
            get => LocalDataManager.BoosterDestroyStackAmount;
            set
            {
                LocalDataManager.BoosterDestroyStackAmount = value;
                SetAmountText(value);
            }
        }
        
        protected override void Init()
        {
            if (LocalDataManager.BoosterDestroyStackAmount > 0)
            {
                plusBtn.gameObject.SetActive(false);
                amountTxt.gameObject.SetActive(true);
                
                amountTxt.text = LocalDataManager.BoosterDestroyStackAmount.ToString();
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
            
            
        }

        public override void OnAddBoosterButtonClicked()
        {
            
        }

        
    }
}