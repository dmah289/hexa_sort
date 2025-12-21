using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.UITools.Toast;
using TMPro;
using UnityEngine;

namespace HexaSort.UI.MainMenu.Shop
{
    public class ShopBundleVisual : MonoBehaviour
    {
        [SerializeField] private ShopBundleData m_shopBundleData;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleTxt;
        [SerializeField] private TextMeshProUGUI priceTxt;
        [SerializeField] private ItemInBundleVisual[] itemVisuals;
        
        public void Setup(ShopBundleData shopBundleData)
        {
            m_shopBundleData = shopBundleData;
            titleTxt.text = m_shopBundleData.title;
            priceTxt.text = $"{m_shopBundleData.price}";
            
            for(int i = 0; i < itemVisuals.Length; i++)
                itemVisuals[i].gameObject.SetActive(false);

            for (int i = 0; i < shopBundleData.items.Length; i++)
            {
                itemVisuals[i].gameObject.SetActive(true);
                itemVisuals[i].Setup(shopBundleData.items[i]);
            }
        }
        
        public void OnBuyButtonClicked()
        {
            OnBuyButtonClickedAsync().Forget();
        }

        public async UniTask OnBuyButtonClickedAsync()
        {
            if (LocalDataManager.CoinAmount >= m_shopBundleData.price)
            {
                LocalDataManager.CoinAmount -= m_shopBundleData.price;
                for (int i = 0; i < m_shopBundleData.items.Length; i++)
                {
                    var item = m_shopBundleData.items[i];
                    switch (item.itemInBundleType)
                    {
                        case eItemInBundleType.Lives:
                            LocalDataManager.CurrentLife += item.amount;
                            break;
                        case eItemInBundleType.Respawn:
                            LocalDataManager.BoosterRespawnAmount += item.amount;
                            break;
                        case eItemInBundleType.DestroyStack:
                            LocalDataManager.BoosterDestroyStackAmount += item.amount;
                            break;
                    }
                }
                
                for (int i = 0; i < m_shopBundleData.items.Length; i++)
                {
                    var item = m_shopBundleData.items[i];
                    switch (item.itemInBundleType)
                    {
                        case eItemInBundleType.Lives:
                            ToastManager.Instance.Show($"Bought {item.amount} lives!");
                            await UniTask.Delay(1100);
                            break;
                        case eItemInBundleType.Respawn:
                            ToastManager.Instance.Show($"Bought {item.amount} Respawn booster!");
                            await UniTask.Delay(1100);
                            break;
                        case eItemInBundleType.DestroyStack:
                            ToastManager.Instance.Show($"Bought {item.amount} Destroy Stack booster!");
                            await UniTask.Delay(1100);
                            break;
                    }
                }
            }
            else ToastManager.Instance.Show(ConstantKey.Toast_InsufficentCoins);
        }
    }
}