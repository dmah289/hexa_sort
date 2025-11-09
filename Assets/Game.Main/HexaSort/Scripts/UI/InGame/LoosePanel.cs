using HexaSort.UI.Loading.BaseSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using Unity.VisualScripting;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public enum eLooseReason
    {
        OutOfSpace
    }
    
    public class LoosePanel : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private RectTransform selfRect;
        
        [Header("Panels")]
        [SerializeField] private GameObject revievePanel;
        [SerializeField] private RectTransform failLevelPanel;
        

        public async UniTaskVoid Show(eLooseReason reason)
        {
            // LevelManager.Instance.StopGameplay();

            await UniTask.Delay(500);
            
            gameObject.SetActive(true);
            
            revievePanel.SetActive(reason == eLooseReason.OutOfSpace);
            failLevelPanel.gameObject.SetActive(reason != eLooseReason.OutOfSpace);
        }

        public void OnNoBtnClicked()
        {
            revievePanel.SetActive(false);
            failLevelPanel.gameObject.SetActive(true);
            
            failLevelPanel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            failLevelPanel.DOScale(Vector3.one, 0.2f);
        }

        public void OnContinueBtnClicked()
        {
            // TODO : review this logic
            EventBus<LifeChangedEventDTO>.Raise(new  LifeChangedEventDTO(-1));
            //LevelManager.Instance.FailLevel().Forget();
            gameObject.SetActive(false);
        }
    }
}