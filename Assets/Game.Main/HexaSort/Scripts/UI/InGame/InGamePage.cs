using HexaSort.UI.BaseSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class InGamePage : MonoBehaviour, IPage
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