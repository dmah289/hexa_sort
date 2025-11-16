using System;
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Entities;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Scripts.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelCurveVersion currLevelCurveVersion;
        [SerializeField] private GridLayoutVersion currGridLayoutVersion;
        
        public async UniTask<LevelDataSO> GetCurrLevelData()
        {
            if (currLevelCurveVersion == null)
            {
                currLevelCurveVersion = await Addressables.LoadAssetAsync<LevelCurveVersion>(ConstantKey.LevelCurveVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            return currLevelCurveVersion.levelDatas[LocalDataManager.LevelIndex];
        }
        
        public async UniTask<GridLayoutSO> GetCurrGridLayout()
        {
            if (currGridLayoutVersion == null)
            {
                currGridLayoutVersion = await Addressables.LoadAssetAsync<GridLayoutVersion>(ConstantKey.GridLayoutVersion)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            LevelDataSO currLevelData = await GetCurrLevelData();
            GridLayoutSO currLayout = currGridLayoutVersion.gridLayouts[currLevelData.GridLayoutID];
            
            return currLayout;
        }
        
        public async UniTask SetupLevel(GridController grid)
        {
            LevelDataSO currLevelData = await GetCurrLevelData();
            GridLayoutSO currGridLayout = await GetCurrGridLayout();
            grid.SetupLevel(currLevelData, currGridLayout);
        }

        public void CleanUpLevel(GridController grid, TrayController tray)
        {
            grid.CleanUp();
            tray.CleanUp();
        }
    }
}