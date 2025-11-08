using System;
using Cysharp.Threading.Tasks;
using manhnd_sdk.Scripts.ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.Scripts.UI
{
    public abstract class ALoadingScreen : MonoBehaviour
    {
        [Header("Shared Fields")]
        [SerializeField] private RectTransform fillRectTransform;
        [SerializeField] private float fillDuration = 3.5f;
        [SerializeField] private float maxFillWidth = 825f;
        [SerializeField] private float fillHeight = 55f;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        protected abstract UniTask OnFullFillAmount();

        // Loading using sizeDelta to simulate fill amount
        protected async UniTask DoLoading()
        {
            float timer = 0f;
            float curWidth = 0;
            fillRectTransform.sizeDelta = new Vector2(curWidth, fillHeight);

            while (timer < fillDuration)
            {
                timer += Time.deltaTime;

                curWidth = (timer / fillDuration) * maxFillWidth;
                fillRectTransform.sizeDelta = new Vector2(curWidth, fillHeight);
                
                await UniTask.Yield();
            }
            
            fillRectTransform.sizeDelta = new Vector2(maxFillWidth, fillHeight);
            
            await OnFullFillAmount();
        }
        
    }
}