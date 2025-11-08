using Coffee_Rush.UI.BaseSystem;
using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.InGame
{
    public class InGamePage : MonoBehaviour, IPage
    {
        [SerializeField] private Text levelIndex; 
        
        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);

                levelIndex.text = $"LEVEL {PlayerPrefs.GetInt(KeySave.LevelIndexKey) + 1,0}";
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