using TMPro;
using UnityEngine;

namespace HexaSort.UI.MainMenu.Shop
{
    public class ShopBundleVisual : MonoBehaviour
    {
        [SerializeField] private eShopBundleType bundleType;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleTxt;
        [SerializeField] private TextMeshProUGUI priceTxt;
        [SerializeField] private ItemInBundleVisual[] itemVisuals;
        
        public void Setup(ShopBundleData shopBundleData)
        {
            bundleType = shopBundleData.bundleType;
            titleTxt.text = shopBundleData.title;
            priceTxt.text = $"{shopBundleData.price}";
            
            for(int i = 0; i < itemVisuals.Length; i++)
                itemVisuals[i].gameObject.SetActive(false);

            for (int i = 0; i < shopBundleData.items.Length; i++)
            {
                itemVisuals[i].gameObject.SetActive(true);
                itemVisuals[i].Setup(shopBundleData.items[i]);
            }
        }
    }
}