using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.BaseSystem;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.MainMenu.Home
{
    public class HomeTabManager : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private Text[] levelIndexTexts;
        
        [Header("References")]
        [SerializeField] private LoadingLevel loadingLevel;
        [SerializeField] private LifeSystem lifeSystem;
        [SerializeField] private BackgroundClickHandler getMoreLifePanel;

        private void OnEnable()
        {
            SetupLevelPath();
            DOVirtual.DelayedCall(1f,() =>
            {
                LocalDataManager.CurrentLife = 0;
                LocalDataManager.CoinAmount = 1000;
                lifeSystem.LoadLifeData();
            });
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
            if (lifeSystem.CanPlay)
            {
                loadingLevel.NextScreen = eScreenType.InGame;
                CanvasManager.Instance.CurScreen = eScreenType.LoadingLevel;
            }
            else
            {
                lifeSystem.FlashOnOutOfLife();
                getMoreLifePanel.ShowBackground();
            }
        }
    }
}