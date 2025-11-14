using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.BaseSystem;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using manhnd_sdk.UITools.Toast;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class GiveUpPopup : APopup
    {
        
        
        public void OnGiveUpClicked()
        {
            GiveUpPanel giveUpPanel = bgClickHandler as GiveUpPanel;
            if (giveUpPanel.FiredButton == eGiveUpButton.Replay)
            {
                if (LocalDataManager.IsEnoughLives)
                {
                    EventBus<LifeChangedEventDTO>.Raise(new LifeChangedEventDTO(-1));
                    CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
                    
                    LevelManager.Instance.ReplayLevelAsync().Forget();
                }
                else
                {
                    ToastManager.Instance.Show(ConstantKey.InsufficentLives);
                    return;
                }
            }
            else if (giveUpPanel.FiredButton == eGiveUpButton.BackHome)
            {
                EventBus<LifeChangedEventDTO>.Raise(new LifeChangedEventDTO(-1));
                CanvasManager.Instance.ShowLoadingScreen(eScreenType.MainMenu);
                
            }
            
            bgClickHandler.OnBackgroundHiden?.Invoke();
        }
    }
}