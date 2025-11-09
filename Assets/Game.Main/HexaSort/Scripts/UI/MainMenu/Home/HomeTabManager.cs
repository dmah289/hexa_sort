using Framework;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.BaseSystem;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
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

        private void OnEnable()
        {
            SetupLevelPath();
        }

        private void SetupLevelPath()
        {
            int curLevelIndex = PlayerPrefs.GetInt(ConstantKey.LevelIndexKey, 0);
            for (int i = 0; i < levelIndexTexts.Length; i++)
            {
                levelIndexTexts[i].text = $"{curLevelIndex + i + 1}";
            }
        }
        
        public void OnPlayBtnClicked()
        {
            if (lifeSystem.CanPlay)
            {
                loadingLevel.NextPage = ePageType.InGame;
                CanvasManager.Instance.CurPage = ePageType.LoadingLevel;
            }
            else lifeSystem.FlashOnOutOfLife();
        }
    }
}