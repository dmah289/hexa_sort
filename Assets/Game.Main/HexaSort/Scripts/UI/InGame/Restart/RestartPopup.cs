using Cysharp.Threading.Tasks;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.SystemDesign.EventBus;

namespace HexaSort.UI.Loading.InGame
{
    public class RestartPopup : APopup
    {
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
            // LevelManager.Instance.StopGameplay();
            
            // RestartPanel restartPanel = bgClickHandler as RestartPanel;
            // if(restartPanel.firedBtnType == eRestartButton.Replay && LifeSystem.Instance.CanPlay)
            //     // LevelManager.Instance.ReplayLevelAsync().Forget();
            // else if (restartPanel.firedBtnType == eRestartButton.BackHome || !LifeSystem.Instance.CanPlay)
            //     LevelManager.Instance.FailLevel().Forget();
        }
    }
}