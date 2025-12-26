using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Main.HexaSort.Scripts.Managers;
using Game.Main.HexaSort.Scripts.UI.Popup;
using HexaSort.Audio;
using HexaSort.Core.Entities.Grid;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using manhnd_sdk.UITools.Toast;
using UnityEngine;

namespace HexaSort.Managers.Level
{
    public enum eLevelState : byte
    {
        None = 0,
        Playing = 1,
        OutOfSpace = 2,
        Win = 3,
        Failed = 4,
        IsUsingDestroyStackBooster = 6
    }
    
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [Header("----- Self Components -----")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("----- References -----")]
        [SerializeField] private Camera gameplayCam;
        [SerializeField] private GridController grid;
        [SerializeField] private TrayController tray;
        [SerializeField] private LoosePanel loosePanel;
        [SerializeField] private WinPanel winPanel;
        
        [Header("----- State Management -----")]
        [SerializeField] private eLevelState currentLevelState;
        
        public eLevelState CurrentLevelState
        {
            get => currentLevelState;
            set
            {
                if (currentLevelState == value) return;
                SetLevelState(value);
                OnLevelStateChanged();
            }
        }
        
        public List<ColorType> SpawnableColors => levelLoader.SpawnableColors;
        
        public void SetLevelState(eLevelState value)
        {
            currentLevelState = value;
        }

        private void OnLevelStateChanged()
        {
            switch (currentLevelState)
            {
                case eLevelState.Playing:
                    InGamePage.Instance.ShowBoosterButtons();
                    levelLoader.SetupLevel(grid).Forget();
                    break;
                case eLevelState.OutOfSpace:
                    ToastManager.Instance.Show(ConstantKey.Toast_OutOfSpace);
                    AudioManager.Instance.PlaySfx(ConstantKey.SFX_OUT_OF_SPACE);
                    InGamePage.Instance.HideBoosterButtons();
                    grid.FindDestroyableStacks(loosePanel).Forget();
                    loosePanel.ShowRevivePanel().Forget();
                    SelectionController.Instance.ReturnSelectedStackToTray();
                    break;
                case eLevelState.Win:
                    AudioManager.Instance.PlaySfx(ConstantKey.SFX_LEVEL_COMPLETED);
                    winPanel.Show().Forget();
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            levelLoader = GetComponent<LevelLoader>();
            CurrentLevelState = eLevelState.None;
        }

        public void EnterGameplay()
        {
            InGamePage.Instance.CheckUnlockBoosters();
            CurrentLevelState = eLevelState.Playing;
        }

        public void ZoomInCamera(int width, Vector2 centerPos)
        {
            gameplayCam.transform.position = gameplayCam.transform.position
                .With(x: centerPos.x, y: centerPos.y - ConstantKey.GAMEPLAY_CAM_Y_OFFSET);

            float targetOrthoSize = 0.893f * width + 0.286f;
            
            gameplayCam.orthographicSize = targetOrthoSize + ConstantKey.ZOOM_IN_OFFSET_ORTHO;
            gameplayCam.DOOrthoSize(targetOrthoSize, 1.5f)
                .SetEase(Ease.OutSine)
                .OnKill(() => gameplayCam.orthographicSize = targetOrthoSize);
        }

        public void CleanUpLevel()
        {
            CurrentLevelState = eLevelState.None;
            levelLoader.CleanUpLevel(grid, tray);
        }
    }
}