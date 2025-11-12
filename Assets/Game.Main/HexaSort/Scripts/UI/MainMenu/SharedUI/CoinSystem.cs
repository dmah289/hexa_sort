using System;
using Framework;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
{
    public struct CoinChangedEventDTO : IEventDTO
    {
        public int amount;
        public CoinChangedEventDTO(int amount)
        {
            this.amount = amount;
        }
    }
    
    public class CoinSystem : MonoBehaviour, IEventBusListener
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

        #region Unity APIs

        private void Awake()
        {
            RegisterCallbacks();
        }

        // private void OnEnable()
        // {
        //     CoinCounter = PlayerPrefs.GetInt(ConstantKey.CoinCountKey, 0);
        // }

        #endregion

        

        #region Coin Change Event

        public void RegisterCallbacks()
        {
            EventBus<CoinChangedEventDTO>.Register(onEventWithArgs: OnCoinChanged);
        }

        private void OnCoinChanged(CoinChangedEventDTO data)
        {
            CoinCounter += data.amount;
        }

        public void DeregisterCallbacks()
        {
            EventBus<CoinChangedEventDTO>.Deregister(onEventWithArgs: OnCoinChanged);
        }

        #endregion
    }
}