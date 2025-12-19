using Game.Main.HexaSort.Scripts.UI.Popup;
using HexaSort.UI.BaseSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class InGamePage : MonoSingleton<InGamePage>, IPage
    {
        [SerializeField] private BoosterManager boosterManager;
        
        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public void CheckUnlockBoosters()
        {
            PopupManager.Instance.CheckShowUnlockBoosterPopup();
            boosterManager.CheckUnlockBoosterButtons();
        }

        public void HideBoosterButtons()
        {
            boosterManager.HideBoosterButtons();
        }


        public void ShowBoosterButtons()
        {
            boosterManager.ShowBoosterButtons();
        }
    }
}