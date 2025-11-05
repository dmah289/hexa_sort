using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace HexaSort.Scripts.UI
{
    public class LoadingScreen : ALoadingScreen
    {
        [Header("Configs")]
        [SerializeField] private string newSceneName = "Gameplay scene";
        
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