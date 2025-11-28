using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
using HexaSort.UI.Loading.BaseSystem;
using HexaSort.UI.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using manhnd_sdk.UITools.Toast;
using TMPro;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{

    public class LoosePanel : MonoBehaviour
    {
        [Header("Self Components")] 
        [SerializeField] private RectTransform selfRect;

        [Header("Panels")] 
        [SerializeField] private RectTransform failLevelPanel;

        [Header("Revive Panel References")]
        [SerializeField] private GameObject revivePanel;
        [SerializeField] public SightingTarget[] sightingTargets;
        [SerializeField] private RectTransform reviveCoinBtn;
        [SerializeField] private RectTransform bottomRevivePanel;

        private void Awake()
        {
            reviveCoinBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{ConstantKey.RevivePrice}";
        }

        private void OnApplicationQuit()
        {
            if (LevelManager.Instance.CurrentLevelState == eLevelState.OutOfSpace)
                EventBus<LifeChangedEventDTO>.Raise(new LifeChangedEventDTO(-1));
        }

        public async UniTaskVoid ShowRevivePanel()
        {
            await UniTask.Delay(500);

            gameObject.SetActive(true);
            revivePanel.SetActive(true);
            failLevelPanel.gameObject.SetActive(false);
            
            reviveCoinBtn.localScale = Vector3.zero;
            bottomRevivePanel.anchoredPosition = new Vector2(0, ConstantKey.BottomRevivePanelOffsetY);
            
            reviveCoinBtn.DOScale(1f, 0.4f)
                .SetEase(Ease.OutQuad);
            bottomRevivePanel.DOAnchorPos(Vector2.zero, 0.4f)
                .SetEase(Ease.OutQuad);
        }

        public void OnCloseRevivePopupClicked()
        {
            LevelManager.Instance.CurrentLevelState = eLevelState.Failed;
            EventBus<LifeChangedEventDTO>.Raise(new LifeChangedEventDTO(-1));

            revivePanel.SetActive(false);
            failLevelPanel.gameObject.SetActive(true);

            failLevelPanel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            failLevelPanel.DOScale(Vector3.one, 0.2f);
        }

        public void OnContinueBtnFailClicked()
        {
            LevelManager.Instance.CleanUpLevel();
            LevelManager.Instance.CurrentLevelState = eLevelState.None;
            
            CanvasManager.Instance.ShowLoadingScreen(eScreenType.MainMenu);
            gameObject.SetActive(false);
        }

        public void OnReviveByCoinBtnClicked()
        {
            OnReviveByCoinBtnClickedAsync().Forget();
        }

        public async UniTask OnReviveByCoinBtnClickedAsync()
        {
            UniTask[] shootTasks = new UniTask[sightingTargets.Length];
            for(int i = 0; i < sightingTargets.Length; i++)
            {
                shootTasks[i] = sightingTargets[i].ShootArrowToTarget();
            }
            await UniTask.WhenAll(shootTasks);
            
            LevelManager.Instance.CurrentLevelState = eLevelState.Playing;
            
            await UniTask.Delay(500);
            
            reviveCoinBtn.localScale = Vector3.one;
            bottomRevivePanel.anchoredPosition = Vector2.zero;
            
            
            reviveCoinBtn.DOScale(0f, 0.4f)
                .SetEase(Ease.OutQuad);
            bottomRevivePanel.DOAnchorPos(new Vector2(0, ConstantKey.BottomRevivePanelOffsetY), 0.5f)
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    CanvasManager.Instance.loosePanel.gameObject.SetActive(false);
                });
            
            // if (LocalDataManager.CoinAmount >= ConstantKey.RevivePrice)
            // {
            //     LocalDataManager.CoinAmount -= ConstantKey.RevivePrice;
            //     
            //     for(int i = 0; i < sightingTargets.Length; i++)
            //     {
            //         sightingTargets[i].ShootArrowToTarget();
            //     }
            // }
            // else ToastManager.Instance.Show(ConstantKey.Toast_InsufficentCoins);
        }
    }
}