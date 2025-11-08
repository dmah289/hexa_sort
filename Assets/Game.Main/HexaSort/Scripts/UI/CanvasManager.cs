using Coffee_Rush.UI.BaseSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace Coffee_Rush.UI
{
    public class CanvasManager : MonoSingleton<CanvasManager>
    {
        [Header("Self References")]
        private IPage[] pages;

        protected override void Awake()
        {
            base.Awake();

            pages = GetComponentsInChildren<IPage>();
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            CurPage = ePageType.MainMenu;
        }

        private ePageType curPage;
        public ePageType CurPage
        {
            get => curPage;
            set
            {
                curPage = value;
                OnPageChanged();
            }
        }

        private void OnPageChanged()
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if((byte)curPage == i) pages[i].Show();
                else pages[i].Hide();
            }
        }
    }
}