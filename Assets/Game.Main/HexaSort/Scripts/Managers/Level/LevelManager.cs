using System;
using DG.Tweening;
using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.Scripts.Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [Header("Self Components")]
        [SerializeField] private LevelLoader levelLoader;
        
        [Header("References")]
        [SerializeField] private Camera gameplayCam;
        [SerializeField] private GridController grid;
        
        [Header("State Management")]
        [SerializeField] private eLevelState currentLevelState;
        
        public eLevelState CurrentLevelState
        {
            get => currentLevelState;
            set
            {
                if (currentLevelState == value) return;
                currentLevelState = value;

                // switch (currentLevelState)
                // {
                //     case LevelState.OutOfSpace:
                //         EventBus<LevelOutOfSpaceDTO>.Raise();
                //         break;
                // }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            levelLoader = GetComponent<LevelLoader>();
        }

        public void EnterGameplay()
        {
            levelLoader.SetupLevel(grid);
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
    }
}