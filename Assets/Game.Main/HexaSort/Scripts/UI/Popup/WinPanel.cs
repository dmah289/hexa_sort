using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Managers.Level;
using HexaSort.UI.Loading.BaseSystem;
using HexaSort.UI.Loading.MainMenu.SharedUI;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HexaSort.UI.Loading.InGame
{
    public class WinPanel : MonoBehaviour
    {
        [Header("----- UI Elements -----")]
        [SerializeField] private RectTransform selfRect;
        [SerializeField] private RectTransform visualRect;
        [SerializeField] private RectTransform target;
        [SerializeField] private ParticleSystem[] confetti;
        [SerializeField] private GameObject containerGroup;
        
        
        [Header("----- Claim Button Elements -----")]
        [SerializeField] private Text coinAmountTxt;
        [SerializeField] private RectTransform coinIcon;
        [SerializeField] private RectTransform claimBtn;
        
        [Header("----- Manager -----")]
        [SerializeField] private RectTransform coinPrefabs;
        [SerializeField] private RectTransform[] coins;

        private void Awake()
        {
            coins = new RectTransform[7];
            for (int i = 0; i < coins.Length; i++)
            {
                coins[i] = Instantiate(coinPrefabs);
                coins[i].transform.SetParent(claimBtn);
                coins[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
                Show().Forget();
        }

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            containerGroup.SetActive(false);
            
            await UniTask.Delay(300);

            for (int i = 0; i < confetti.Length; i++)
            {
                confetti[i].Play();
            }
            
            containerGroup.SetActive(true);
            coinAmountTxt.text = $"{ConstantKey.WinCoinReward}";
            visualRect.localScale = Vector3.zero;
            visualRect.DOScale(Vector3.one, 1f)
                .SetEase(Ease.OutBack);
        }
        
        public void OnClaimBtnClicked() => OnClaimBtnClickedAsync().Forget();
        
        public async UniTask OnClaimBtnClickedAsync()
        {
            EventBus<CoinChangedEventDTO>.Raise(new CoinChangedEventDTO(ConstantKey.WinCoinReward));
            
            SpawnCoins();
            await CollectCoins();
            LocalDataManager.LevelIndex++;
            
            await UniTask.Delay(1000);
            
            LevelManager.Instance.CleanUpLevel();
            CanvasManager.Instance.ShowLoadingScreen(eScreenType.InGame);
            
            Hide();
        }

        private void Hide()
        {
            containerGroup.SetActive(false);
            gameObject.SetActive(false);
        }

        private async UniTask CollectCoins()
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < coins.Length; i++)
            {
                sequence.Join(coins[i].DOMove(target.position, 1f)
                    .SetEase(Ease.InBack)
                    .SetDelay(0.03f*i));
            }

            await sequence.AsyncWaitForCompletion();
    
            for (int i = 0; i < coins.Length; i++)
                coins[i].gameObject.SetActive(false);
            
        }

        private void SpawnCoins()
        {
            for (int i = 0; i < coins.Length; i++)
            {
                coins[i].gameObject.SetActive(true);
                Vector2 offset = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
                coins[i].anchoredPosition = Vector3.zero;
                coins[i].DOAnchorPos(offset, 0.1f)
                    .SetDelay(0.01f * i)
                    .SetEase(Ease.OutBack);
            }
        }
    }
}