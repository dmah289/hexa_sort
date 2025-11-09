using HexaSort.UI.MainMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexaSort.UI.MainMenu
{
    public class SingleScrollRect : ScrollRect
    {
        [HideInInspector]
        public MainMenuPage mOutterScrollRect;
        private bool draggingSide;

        public bool IsDraggingItself
            => !draggingSide;
        
        protected void CheckMainScroll()
        {
            if (mOutterScrollRect != null)
                return;
            mOutterScrollRect = GetComponentInParent<MainMenuPage>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            CheckMainScroll();
            bool shouldDragSide = (mOutterScrollRect.horizontal)
                                ? Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)
                                : Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x);
            if (shouldDragSide)
            {
                Vector2 newdelta = (mOutterScrollRect.horizontal)
                                 ? new Vector2(eventData.delta.x, 0)
                                 : new Vector2(0, eventData.delta.y);
                eventData.delta = newdelta;
                mOutterScrollRect?.OnBeginDrag(eventData);
                draggingSide = true;
            }
            else base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            CheckMainScroll();
            if (draggingSide)
            {
                Vector2 newdelta = (mOutterScrollRect.horizontal)
                                 ? new Vector2(eventData.delta.x, 0)
                                 : new Vector2(0, eventData.delta.y);
                eventData.delta = newdelta;
                mOutterScrollRect?.OnDrag(eventData);
            }
            else base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            CheckMainScroll();
            if (draggingSide)
            {
                mOutterScrollRect?.OnEndDrag(eventData);
                draggingSide = false;
            }
            else base.OnEndDrag(eventData);
        }
    }
}
