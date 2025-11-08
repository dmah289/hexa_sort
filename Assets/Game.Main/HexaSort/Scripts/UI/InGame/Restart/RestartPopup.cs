
using Coffee_Rush.UI.MainMenu.Home;
using Cysharp.Threading.Tasks;

namespace Coffee_Rush.UI.InGame
{
    public class RestartPopup : APopup
    {
        public override void ShowPopup()
        {
            //LevelManager.Instance.StopGameplay();
            base.ShowPopup();
        }

        protected override async UniTaskVoid HidePopupAsync()
        {
            // LevelManager.Instance.ResumeGameplay();
            base.HidePopupAsync().Forget();
        }

        public void OnGiveUpClicked()
        {
            bgClickHandler.OnBackgroundHiden?.Invoke();
            
            LifeSystem.Instance.DecreaseOnLifeLost();
            // LevelManager.Instance.StopGameplay();
            
            RestartPanel restartPanel = bgClickHandler as RestartPanel;
            // if(restartPanel.firedBtnType == eRestartButton.Replay && LifeSystem.Instance.CanPlay)
            //     // LevelManager.Instance.ReplayLevelAsync().Forget();
            // else if (restartPanel.firedBtnType == eRestartButton.BackHome || !LifeSystem.Instance.CanPlay)
            //     LevelManager.Instance.FailLevel().Forget();
        }
    }
}