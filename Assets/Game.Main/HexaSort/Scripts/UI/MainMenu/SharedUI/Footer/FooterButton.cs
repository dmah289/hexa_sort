using UnityEngine;
using UnityEngine.UI;

namespace Coffee_Rush.UI.MainMenu.Footer
{
    public class FooterButton : MonoBehaviour
    {
        public static Vector3 minTitleScale = new (0.5f, 0.5f, 0.5f);
        
        [SerializeField] private RectTransform iconRect;
        [SerializeField] private Text titleTxt;
        [SerializeField] private RectTransform titleRect;
        
        [SerializeField] private float selfRatio;
        private Vector2 initPos, targetPos;

        private void Awake()
        {
            initPos = new Vector2(iconRect.anchoredPosition.x, -15f);
            targetPos = new Vector2(iconRect.anchoredPosition.x, 30f);
        }

        public void OnLerpRatioChanged(float ratio)
        {
            iconRect.anchoredPosition = Vector2.Lerp(initPos, targetPos, 1 - Mathf.Abs(ratio - selfRatio) * 2);
            
            float textRatio = Mathf.Lerp(0.5f, 1, 1 - Mathf.Abs(ratio - selfRatio) * 4);
            
            if(Mathf.Approximately(textRatio, 0.5f)) titleTxt.gameObject.SetActive(false);
            else titleTxt.gameObject.SetActive(true);
            
            //titleTxt.SetAlpha(textRatio);
            titleRect.localScale = Vector3.Lerp(minTitleScale, Vector3.one, textRatio);
        }
    }
}