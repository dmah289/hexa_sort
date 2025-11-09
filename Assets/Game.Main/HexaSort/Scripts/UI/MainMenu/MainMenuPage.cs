using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading.MainMenu.Footer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HexaSort.UI.MainMenu
{
    public class MainMenuPage : MonoBehaviour, IPage, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform tabsParent;
        [SerializeField] private FooterManager footerManager;
        [SerializeField] private Vector2 upperMinMaxAnchorX;
        [SerializeField] private Vector2 lowerMinMaxAnchorX;
        
        private float dragStartX;
        private Vector2 startAnchorMin;
        private Vector2 startAnchorMax;
        private bool isDragging;
        
        public bool horizontal => true;

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            dragStartX = eventData.position.x;
            startAnchorMin = tabsParent.anchorMin;
            startAnchorMax = tabsParent.anchorMax;
            footerManager.StopAnimation(); // Dừng animation khi bắt đầu drag
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            float dragDelta = (eventData.position.x - dragStartX) / Screen.width;
            float newMinX = Mathf.Clamp(startAnchorMin.x + dragDelta, lowerMinMaxAnchorX.x, upperMinMaxAnchorX.x);
            float newMaxX = Mathf.Clamp(startAnchorMax.x + dragDelta, lowerMinMaxAnchorX.y, upperMinMaxAnchorX.y);

            tabsParent.anchorMin = new Vector2(newMinX, 0);
            tabsParent.anchorMax = new Vector2(newMaxX, 1);

            // Tính ratio và đồng bộ xuống footer (không snap)
            float ratio = 1 - Mathf.InverseLerp(lowerMinMaxAnchorX.x, upperMinMaxAnchorX.x, newMinX);
            footerManager.SyncFromPage(ratio);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            // Snap về tab gần nhất khi thả tay
            float currentRatio = 1 - Mathf.InverseLerp(lowerMinMaxAnchorX.x, upperMinMaxAnchorX.x, tabsParent.anchorMin.x);
            int nearestTab = Mathf.RoundToInt(currentRatio * 2);
            footerManager.SnapToTab(nearestTab);
        }
    }
}