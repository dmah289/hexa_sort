using HexaSort.UI.Loading.BaseSystem;
using Framework;
using HexaSort.UI.BaseSystem;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.InGame
{
    public class InGamePage : MonoBehaviour, IPage
    {
        [SerializeField] private Text levelIndex; 
        
        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);

                levelIndex.text = $"LEVEL {PlayerPrefs.GetInt(ConstantKey.LevelIndexKey) + 1,0}";
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