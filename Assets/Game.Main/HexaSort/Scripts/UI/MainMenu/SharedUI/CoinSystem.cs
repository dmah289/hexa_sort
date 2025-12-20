using System;
using Framework;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.BaseSystem;
using HexaSort.UI.Loading.MainMenu.Footer;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.Loading.MainMenu.SharedUI
{
    public enum eResourceType
    {
        Coin,
        Life
    }
    public struct ResourceChangedEventDTO : IEventDTO
    {
        public eResourceType ResourceType;
        public float Amount;
        
        public ResourceChangedEventDTO(eResourceType resourceType, float amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }
    
    public class CoinSystem : MonoBehaviour
    {
        [SerializeField] private Text counterTxt;
        [SerializeField] private FooterManager footer;

        #region Unity APIs

        private void Awake()
        {
            EventBus<ResourceChangedEventDTO>.Register(onEventWithArgs: OnCoinChanged);
        }

        private void OnEnable()
        {
            counterTxt.text = $"{LocalDataManager.CoinAmount}";
        }

        #endregion
        
        #region Class Methods

        private void OnCoinChanged(ResourceChangedEventDTO data)
        {
            if(data.ResourceType == eResourceType.Coin)
                counterTxt.text = $"{data.Amount}";
        }
        
        public void OnCoinBtnClicked()
        {
            if (CanvasManager.Instance.CurScreen == eScreenType.MainMenu)
            {
                footer.ShowMenuShop();
            }
        }

        #endregion

        
    }
}