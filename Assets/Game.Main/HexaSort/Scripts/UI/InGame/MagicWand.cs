using System;
using DG.Tweening;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class MagicWand : MonoBehaviour, IPoolableObject
    {
        public RectTransform selfRT;
        
        [SerializeField] private RectTransform visual;

        private void Awake()
        {
            selfRT = GetComponent<RectTransform>();
        }

        public void OnGetFromPool() { }

        public void OnReturnToPool() { }
        
        public void PlayAnim()
        {
            visual.localScale = Vector3.zero;
            visual.eulerAngles = Vector3.zero;
            visual.anchoredPosition = Vector2.zero;
            
            visual.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                visual.DOAnchorPos(new Vector2(0, -50f), 0.5f)
                    .SetEase(Ease.OutBack);
                visual.DORotate(new Vector3(0,0,30), 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => ObjectPooler.ReturnToPool(PoolingType.MagicWand, this, destroyCancellationToken));
            });
        }
    }
}