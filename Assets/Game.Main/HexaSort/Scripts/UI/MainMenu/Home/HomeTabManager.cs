using System;
using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.HexaSort.Scripts.UI.Popup;
using HexaSort.Managers.Level;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.BaseSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.MainMenu.Home
{
    public class HomeTabManager : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Text[] levelIndexTexts;
        
        [Header("References")]
        [SerializeField] private LifeSystem lifeSystem;
        
        private void OnEnable()
        {
            SetupLevelPath();
        }

        private void SetupLevelPath()
        {
            int curLevelIndex = LocalDataManager.LevelIndex;
            for (int i = 0; i < levelIndexTexts.Length; i++)
            {
                levelIndexTexts[i].text = $"{curLevelIndex + i + 1}";
            }
        }
        
        public void OnPlayBtnClicked()
        {
            if (LocalDataManager.CanPlay)
            {
                CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
            }
            else
            {
                lifeSystem.FlashOnOutOfLife();
                PopupManager.Instance.ShowGetMoreLifePopup();
            }
        }
    }
}