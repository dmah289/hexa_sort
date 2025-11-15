using HexaSort.UI.Loading.BaseSystem;
using Framework;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.BaseSystem;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using UnityEngine.UI;

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