using HexaSort.UI.Loading.BaseSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI;
using HexaSort.Managers.Level;
using HexaSort.UI.MainMenu.SharedUI;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using Unity.VisualScripting;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    
    public class LoosePanel : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private RectTransform selfRect;
        
        [Header("Panels")]
        [SerializeField] private GameObject revievePanel;
        [SerializeField] private RectTransform failLevelPanel;
        
        [Header("References")]
        [SerializeField] private ScaleAnimButton pauseBtn;
        

        public async UniTaskVoid ShowRevivePanel()
        {
            await UniTask.Delay(500);
            
            gameObject.SetActive(true);
            revievePanel.SetActive(true);
            failLevelPanel.gameObject.SetActive(false);
        }

        public void OnCloseRevivePopupClicked()
        {
            LevelManager.Instance.CurrentLevelState = eLevelState.Failed;
            EventBus<LifeChangedEventDTO>.Raise(new  LifeChangedEventDTO(-1));
            
            revievePanel.SetActive(false);
            failLevelPanel.gameObject.SetActive(true);
            
            failLevelPanel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            failLevelPanel.DOScale(Vector3.one, 0.2f);
        }

        public void OnContinueBtnFailClicked()
        {
            //LevelManager.Instance.FailLevel().Forget();
            gameObject.SetActive(false);
            LevelManager.Instance.CurrentLevelState = eLevelState.None;
        }
    }
}