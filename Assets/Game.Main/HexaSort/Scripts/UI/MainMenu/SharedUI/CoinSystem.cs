using Framework;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
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
                PlayerPrefs.SetInt(ConstantKey.CoinCountKey, coinCount);
                counterTxt.text = $"{coinCount}";
            }
        }

        private void OnEnable()
        {
            CoinCounter = PlayerPrefs.GetInt(ConstantKey.CoinCountKey, 0);
        }
    }
}