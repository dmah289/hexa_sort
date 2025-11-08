using System;
using Coffee_Rush.UI.BaseSystem;
using DG.Tweening;
using UnityEngine;

namespace Coffee_Rush.UI.MainMenu
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