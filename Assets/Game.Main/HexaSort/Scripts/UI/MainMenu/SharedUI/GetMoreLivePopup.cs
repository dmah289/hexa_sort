using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Main.HexaSort.Scripts.Managers;
using manhnd_sdk.UITools.Toast;
using HexaSort.UI.Loading.MainMenu.Home;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.UI.MainMenu.SharedUI
{
    public class GetMoreLivePopup : APopup
    {
        [Header("Self References")]
        [SerializeField] private RectTransform lifeIcon;
        
        [Header("References")]
        [SerializeField] private LifeSystem lifeSystem;
        
        
        public void OnBuyLiveBtnClicked()
        {
            if (LocalDataManager.CoinAmount >= ConstantKey.BuyLivePrice)
            {
                LocalDataManager.CoinAmount -= ConstantKey.BuyLivePrice;
                LocalDataManager.CurrentLife++;
                
                HidePopup();
                
                AnimateLifeFlight().Forget();
            }
            else
            {
                ToastManager.Instance.Show(ConstantKey.Toast_InsufficentCoins);
            }
        }

        private async UniTaskVoid AnimateLifeFlight()
        {
            RectTransform life = await ObjectPooler.GetFromPool<RectTransform>(
                PoolingType.LifeFly, destroyCancellationToken, lifeSystem.TargetRectTransform);
            life.position = lifeIcon.position;
            
            await UniTask.Delay(500);

            life.DOScale(0.35f, 0.75f);
            life.DOAnchorPos(Vector2.zero, 0.75f).OnComplete(() =>
            {
                ObjectPooler.ReturnToPool(PoolingType.LifeFly, life, destroyCancellationToken);
                lifeSystem.PlayCoinHitEffect();
            });
            
        }
    }
}