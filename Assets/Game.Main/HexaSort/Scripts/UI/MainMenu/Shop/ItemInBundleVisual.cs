using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.MainMenu.Shop
{
    public class ItemInBundleVisual : MonoBehaviour
    {
        [SerializeField] private eItemInBundleType itemInBundleType;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        public void Setup(ItemInBundleData itemData)
        {
            itemIcon.sprite = itemData.sprite;
            itemIcon.SetNativeSize();
            itemAmountText.text = $"x{itemData.amount}";
        }
    }
}