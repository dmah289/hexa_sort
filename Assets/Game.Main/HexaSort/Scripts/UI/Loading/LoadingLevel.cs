using Cysharp.Threading.Tasks;
using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading.BaseSystem;

namespace HexaSort.UI.Loading
{
    public class LoadingLevel : ALoadingScreen, IPage
    {
        private ePageType nextPage;
        public ePageType NextPage
        {
            set => nextPage = value;
        }
        
        protected override async UniTask OnFullFillAmount()
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