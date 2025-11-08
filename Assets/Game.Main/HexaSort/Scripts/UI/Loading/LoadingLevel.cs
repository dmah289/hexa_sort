
using Coffee_Rush.UI.BaseSystem;
using Cysharp.Threading.Tasks;

namespace Coffee_Rush.UI
{
    public class LoadingLevel : ALoadingPage, IPage
    {
        private ePageType nextPage;
        public ePageType NextPage
        {
            set => nextPage = value;
        }
        
        protected override async UniTaskVoid OnFullFillAmount()
        {
            CanvasManager.Instance.CurPage = nextPage;
            
            //if(nextPage == ePageType.InGame)
                // LevelManager.Instance.EnterGameplay().Forget();
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                StartLoading().Forget();
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