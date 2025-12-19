using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.InGame;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.UITools.Toast;
using TMPro;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class BuyBoosterPopup : APopup
    {
        [SerializeField] private eBoosterType _boosterType;
        
        [SerializeField] private GameObject respawnGroup;
        [SerializeField] private GameObject destroyStackGroup;
        [SerializeField] private TextMeshProUGUI titleName;
        [SerializeField] private TextMeshProUGUI boosterPriceText;

        protected override void Awake()
        {
            base.Awake();

            _boosterType = eBoosterType.None;
        }

        public void SetupBoosterType(eBoosterType boosterType)
        {
            _boosterType = boosterType;
            
            respawnGroup.SetActive(false);
            destroyStackGroup.SetActive(false);

            switch (boosterType)
            {
                case eBoosterType.Respawn:
                    respawnGroup.SetActive(true);
                    titleName.text = "Respawn";
                    boosterPriceText.text = ConstantKey.BoosterPrices[eBoosterType.Respawn].ToString();
                    break;
                case eBoosterType.DestroyStack:
                    destroyStackGroup.SetActive(true);
                    titleName.text = "Destroy Stack";
                    boosterPriceText.text = ConstantKey.BoosterPrices[eBoosterType.DestroyStack].ToString();
                    break;
            }
        }

        public void OnBuyButtonClicked()
        {
            if(LocalDataManager.CoinAmount >= ConstantKey.BoosterPrices[_boosterType])
            {
                switch (_boosterType)
                {
                    case eBoosterType.Respawn:
                        LocalDataManager.BoosterRespawnAmount++;
                        break;
                    case eBoosterType.DestroyStack:
                        LocalDataManager.BoosterDestroyStackAmount++;
                        break;
                }
                LocalDataManager.CoinAmount -= ConstantKey.BoosterPrices[_boosterType];
            }
            else ToastManager.Instance.Show(ConstantKey.Toast_InsufficentCoins);

            _boosterType = eBoosterType.None;
        }
    }
}