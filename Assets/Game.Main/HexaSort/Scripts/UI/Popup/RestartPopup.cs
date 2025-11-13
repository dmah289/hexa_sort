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
    public class RestartPopup : APopup
    {
        [Header("References")]
        [SerializeField] private LoadingLevel loadingLevel;
        
        // public override void ShowPopup()
        // {
        //     //LevelManager.Instance.StopGameplay();
        //     base.ShowPopup();
        // }

        // protected override async UniTaskVoid HidePopupAsync()
        // {
        //     // LevelManager.Instance.ResumeGameplay();
        //     base.HidePopupAsync().Forget();
        // }

        public void OnGiveUpClicked()
        {
            bgClickHandler.OnBackgroundHiden?.Invoke();
            
            EventBus<LifeChangedEventDTO>.Raise(new  LifeChangedEventDTO(-1));
            
            RestartPanel restartPanel = bgClickHandler as RestartPanel;
            if (restartPanel.firedBtnType == eRestartButton.Replay)
            {
                if (LocalDataManager.IsEnoughLives)
                    LevelManager.Instance.ReplayLevelAsync().Forget();
                else
                    ToastManager.Instance.Show(ConstantKey.InsufficentLives);
            }
            else if (restartPanel.firedBtnType == eRestartButton.BackHome)
            {
                loadingLevel.NextScreen = eScreenType.MainMenu;
                CanvasManager.Instance.CurScreen = eScreenType.LoadingLevel;
            }
                
        }
    }
}