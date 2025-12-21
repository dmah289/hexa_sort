using System;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using UnityEngine;

namespace HexaSort.UI.MainMenu.Shop
{
    public enum eItemInBundleType
    {
        Lives,
        Respawn,
        DestroyStack
    }
    
    [Serializable]
    public class ItemInBundleData
    {
        public eItemInBundleType itemInBundleType;
        public Sprite sprite;
        public int amount;
    }
    
    public enum eShopBundleType
    {
        Life,
        Small,
        Big,
        Ultimate
    }
    
    [Serializable]
    public class ShopBundleData
    {
        public eShopBundleType bundleType;
        public string title;
        public int price;
        public ItemInBundleData[] items;
        
        public bool HasRespawnItem()
        {
            foreach (var item in items)
            {
                if (item.itemInBundleType == eItemInBundleType.Respawn)
                    return true;
            }
            return false;
        }

        public bool HasDestroyItem()
        {
            foreach (var item in items)
            {
                if (item.itemInBundleType == eItemInBundleType.DestroyStack)
                    return true;
            }
            return false;
        }

        public bool CanShow()
        {
            if(HasRespawnItem() && LocalDataManager.LevelIndex < ConstantKey.BoosterUnlockLevel[eBoosterType.Respawn])
                return false;
            
            if(HasDestroyItem() && LocalDataManager.LevelIndex < ConstantKey.BoosterUnlockLevel[eBoosterType.DestroyStack])
                return false;

            return true;
        }
    }
    
    [CreateAssetMenu(fileName = "shop_bundles_data", menuName = "Shop/Shop Bundles Data")]
    public class ShopBundlesData : ScriptableObject
    {
        public ShopBundleData[] bundles;
    }
}