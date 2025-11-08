using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.SharedUI
{
    public class CoinSystem : MonoBehaviour
    {
        [SerializeField] private Text counterTxt;
        

        [SerializeField] private int coinCount;
        public int CoinCounter
        {
            get => coinCount;
            set
            {
                coinCount = value;
                PlayerPrefs.SetInt(KeySave.CoinCountKey, coinCount);
                counterTxt.text = $"{coinCount}";
            }
        }

        private void OnEnable()
        {
            CoinCounter = PlayerPrefs.GetInt(KeySave.CoinCountKey, 0);
        }
    }
}