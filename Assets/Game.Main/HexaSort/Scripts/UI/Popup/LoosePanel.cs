using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
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
        [SerializeField] private GameObject revievePanel;
        [SerializeField] public SightingTarget[] sightingTargets;
        [SerializeField] private RectTransform revieveCoinBtn;
        [SerializeField] private RectTransform bottomRevivePanel;

        private void OnEnable()
        {
            revieveCoinBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{ConstantKey.RevivePrice}";
            revieveCoinBtn.localScale = Vector3.zero;
            bottomRevivePanel.anchoredPosition = new Vector2(0, ConstantKey.BottomRevivePanelOffsetY);
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
            revievePanel.SetActive(true);
            failLevelPanel.gameObject.SetActive(false);
        }

        public void OnCloseRevivePopupClicked()
        {
            LevelManager.Instance.CurrentLevelState = eLevelState.Failed;
            EventBus<LifeChangedEventDTO>.Raise(new LifeChangedEventDTO(-1));

            revievePanel.SetActive(false);
            failLevelPanel.gameObject.SetActive(true);

            failLevelPanel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            failLevelPanel.DOScale(Vector3.one, 0.2f);
        }

        public void OnContinueBtnFailClicked()
        {
            LevelManager.Instance.CleanUpLevel().Forget();
            gameObject.SetActive(false);
            LevelManager.Instance.CurrentLevelState = eLevelState.None;
        }

        public void OnReviveByCoinBtnClicked()
        {
            OnReviveByCoinBtnClickedAsync().Forget();
        }

        public async UniTask OnReviveByCoinBtnClickedAsync()
        {
            for(int i = 0; i < sightingTargets.Length; i++)
            {
                sightingTargets[i].ShootArrowToTarget();
            }
            
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