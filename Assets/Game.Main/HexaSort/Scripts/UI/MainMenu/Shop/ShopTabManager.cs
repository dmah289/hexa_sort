using UnityEngine;

namespace HexaSort.UI.MainMenu.Shop
{
    public class ShopTabManager : MonoBehaviour
    {
        [SerializeField] private ShopBundleVisual[] shopBundleVisuals;
        
        [SerializeField] private ShopBundlesData shopBundlesData;

        private void OnEnable()
        {
            SetupShopBundles();
        }

        private void SetupShopBundles()
        {
            for (int i = 0; i < shopBundleVisuals.Length; i++)
                shopBundleVisuals[i].gameObject.SetActive(false);
            
            for (int i = 0; i < shopBundlesData.bundles.Length; i++)
            {
                if (shopBundlesData.bundles[i].CanShow())
                {
                    shopBundleVisuals[i].gameObject.SetActive(true);
                    shopBundleVisuals[i].Setup(shopBundlesData.bundles[i]);
                }
            }
        }
    }
}