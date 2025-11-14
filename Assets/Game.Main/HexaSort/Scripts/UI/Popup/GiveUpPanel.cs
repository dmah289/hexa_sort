using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public enum eGiveUpButton : byte
    {
        Replay,
        BackHome
    }
    
    public class GiveUpPanel : BackgroundClickHandler
    {
        [SerializeField] private eGiveUpButton firedButton;
        public eGiveUpButton FiredButton => firedButton;

        public void ShowBackgroundByButton(eGiveUpButton btnType)
        {
            base.ShowBackground();
            
            firedButton = btnType;
        }
    }
}