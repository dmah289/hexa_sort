using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.MainMenu.Home;
using HexaSort.UI.Loading.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.UI.MainMenu.SharedUI
{
    public class GetMoreLivePopup : APopup
    {
        [SerializeField] private LifeSystem lifeSystem;
        
        public void OnBuyLiveBtnClicked()
        {
            if (LocalDataManager.CoinAmount >= ConstantKey.LIVE_PRICE)
            {
                EventBus<CoinChangedEventDTO>.Raise(
                    new CoinChangedEventDTO(-ConstantKey.LIVE_PRICE));
                EventBus<LifeChangedEventDTO>.Raise(
                    new LifeChangedEventDTO(1));
                
                // TODO : Spawn live then fly to life panel
                
                HidePopup();
            }
            else
            {
                // TODO : Show insufficient coin toast
            }
        }
    }
}