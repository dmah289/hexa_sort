using System;
using Framework;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
{
    public struct CoinChangedEventDTO : IEventDTO
    {
        public float amount;
        public CoinChangedEventDTO(float amount)
        {
            this.amount = amount;
        }
    }
    
    public class CoinSystem : MonoBehaviour, IEventBusListener
    {
        [SerializeField] private Text counterTxt;
        

        [SerializeField] private float coinCount;
        public float CoinCounter
        {
            get => coinCount;
            set
            {
                coinCount = value;
                LocalDataManager.CoinAmount = coinCount;
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