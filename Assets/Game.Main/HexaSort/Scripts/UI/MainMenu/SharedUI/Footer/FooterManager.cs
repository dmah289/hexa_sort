using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HexaSort.UI.Loading.MainMenu.Footer
{
    public class FooterManager : MonoBehaviour
    {
        [SerializeField] private RectTransform selection;
        [SerializeField] private FooterButton[] footerButtons;
        [SerializeField] private RectTransform tabsParent;
        
        private float minSelectionPosX;
        private float maxSelectionPosX;
        private float speed = 4f;
        private float targetRatio;
        private float normDirection;

        [SerializeField] private Vector2 upperMinMaxAnchorX;
        [SerializeField] private Vector2 lowerMinMaxAnchorX;
        [SerializeField] private float curRatio;
        
        
        public float LerpRatio
        {
            get => curRatio;
            set
            {
                curRatio = value;
                UpdateSelectionPosition();
                UpdateAllTabsAnchor();
                for (int i = 0; i < footerButtons.Length; i++)
                {
                    footerButtons[i].OnLerpRatioChanged(curRatio);
                }
            }
        }

        private async void Start()
        {
            await UniTask.DelayFrame(1);
            
            minSelectionPosX = footerButtons[0].GetComponent<RectTransform>().anchoredPosition.x;
            maxSelectionPosX = footerButtons[2].GetComponent<RectTransform>().anchoredPosition.x;
            LerpRatio = 0.5f;
        }

        public void OnFooterButtonClicked(int index)
        {
            targetRatio = (float)index / 2;
            normDirection = Mathf.Sign(targetRatio - curRatio);
        }

        private void Update()
        {
            if (Mathf.Abs(curRatio - targetRatio) < 0.05f && !Mathf.Approximately(targetRatio, curRatio))
                LerpRatio = targetRatio;
            else if (Mathf.Abs(curRatio - targetRatio) >= 0.05f)
                LerpRatio += speed * normDirection * Time.deltaTime;
                
        }

        private void UpdateSelectionPosition()
        {
            Vector2 curSelectionPos = selection.anchoredPosition;
            curSelectionPos.x = Mathf.Lerp(minSelectionPosX, maxSelectionPosX, curRatio);
            selection.anchoredPosition = curSelectionPos;
        }
        
        private void UpdateAllTabsAnchor()
        {
            Vector2 res = Vector2.Lerp(lowerMinMaxAnchorX, upperMinMaxAnchorX, 1-curRatio);
            
            tabsParent.anchorMin = new Vector2(res.x, 0);
            tabsParent.anchorMax = new Vector2(res.y, 1);
        }
    }
}