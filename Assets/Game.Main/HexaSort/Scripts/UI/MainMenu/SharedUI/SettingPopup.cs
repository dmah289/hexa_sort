
using Coffee_Rush.UI.BaseSystem;
using Coffee_Rush.UI.MainMenu.Home;
using Cysharp.Threading.Tasks;

namespace Coffee_Rush.UI.MainMenu.SharedUI
{
    public class SettingPopup : APopup
    {
        public override void ShowPopup()
        {
            if(CanvasManager.Instance.CurPage == ePageType.InGame)
                //LevelManager.Instance.StopGameplay();
            
            base.ShowPopup();
        }

        protected override async UniTaskVoid HidePopupAsync()
        {
            if(CanvasManager.Instance.CurPage == ePageType.InGame)
                // LevelManager.Instance.ResumeGameplay();
            
            base.HidePopupAsync().Forget();
        }
        
        
    }
}