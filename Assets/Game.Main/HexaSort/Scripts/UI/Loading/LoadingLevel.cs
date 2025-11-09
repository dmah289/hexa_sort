using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Managers;
using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading.BaseSystem;

namespace HexaSort.UI.Loading
{
    public class LoadingLevel : ALoadingScreen, IPage
    {
        private eScreenType nextScreen;
        public eScreenType NextScreen
        {
            set => nextScreen = value;
        }
        
        protected override async UniTask OnFullFillAmount()
        {
            CanvasManager.Instance.CurScreen = nextScreen;
            
            if(CanvasManager.Instance.CurScreen == eScreenType.InGame)
                 LevelManager.Instance.EnterGameplay();
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                DoLoading().Forget();
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}