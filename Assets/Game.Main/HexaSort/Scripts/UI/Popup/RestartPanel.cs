using Framework.UI;
using UnityEngine;
using UnityEngine.Events;

namespace HexaSort.UI.Loading.InGame
{
    public enum eRestartButton
    {
        Replay,
        BackHome
    }
    
    public class RestartPanel : BackgroundClickHandler
    {
        public eRestartButton firedBtnType;

        public void ShowBackgroundByButton(eRestartButton btnType)
        {
            base.ShowBackground();
            
            firedBtnType = btnType;
        }
    }
}