using System;
using Cysharp.Threading.Tasks;
using HexaSort.UI.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace HexaSort.Scripts.UI
{
    public class LoadingOpenGame : ALoadingScreen
    {
        [Header("Configs")]
        [SerializeField] private string newSceneName = "gameplay_scene";
        
        private void OnEnable()
        {
            DoLoading().Forget();
        }

        protected override async UniTask OnFullFillAmount()
        {
            AsyncOperationHandle<SceneInstance> sceneLoadHandle = 
                Addressables.LoadSceneAsync(newSceneName, LoadSceneMode.Single, false);
            
            await UniTask.WaitUntil(() => sceneLoadHandle.IsDone);
            await UniTask.Delay(1000);
            
            await sceneLoadHandle.Result.ActivateAsync();
        }
    }
}