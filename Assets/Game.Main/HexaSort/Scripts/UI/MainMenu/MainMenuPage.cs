using HexaSort.UI.BaseSystem;
using UnityEngine;

namespace HexaSort.UI.MainMenu
{
    public class MainMenuPage : MonoBehaviour, IPage
    {
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
    }
}