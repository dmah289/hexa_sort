using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
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
            GiveUpPanel giveUpPanel = popupPanel as GiveUpPanel;
            if (giveUpPanel.FiredButton == eGiveUpButton.Replay)
            {
                if (LocalDataManager.CanPlay)
                {
                    LocalDataManager.CurrentLife--;
                    LevelManager.Instance.CleanUpLevel();
                    CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
                }
                else
                {
                    ToastManager.Instance.Show(ConstantKey.Toast_InsufficentLives);
                    return;
                }
            }
            else if (giveUpPanel.FiredButton == eGiveUpButton.BackHome)
            {
                LocalDataManager.CurrentLife--;
                LevelManager.Instance.CleanUpLevel();
                CanvasManager.Instance.ShowLoadingScreen(eScreenType.MainMenu);
            }
            
            popupPanel.OnBackgroundHiden?.Invoke();
        }
    }
}