using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading.BaseSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.UI.Loading
{
    public class CanvasManager : MonoSingleton<CanvasManager>
    {
        // Self References
        private IPage[] pages;
        private eScreenType curScreen;
        
        public eScreenType CurScreen
        {
            get => curScreen;
            set
            {
                curScreen = value;
                OnPageChanged();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            pages = GetComponentsInChildren<IPage>();
            CurScreen = eScreenType.MainMenu;
        }

        private void OnPageChanged()
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if((byte)curScreen == i) pages[i].Show();
                else pages[i].Hide();
            }
        }
    }
}