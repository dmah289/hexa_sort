using Cysharp.Threading.Tasks;
using HexaSort.Audio;
using HexaSort.Managers.Level;
using HexaSort.Core.Entities;
using HexaSort.Core.Entities.Grid;
using HexaSort.UI.Loading.InGame;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HexaSort.Scripts.Core.Controllers
{
    public class SelectionController : MonoSingleton<SelectionController>
    {
        [Header("Managers")]
        [SerializeField] private HexStackController currStack;
        
        [Header("References")]
        [SerializeField] private Camera gameplayCam;
        [SerializeField] private GridController grid;
        
        [Header("Raycast Picking Config")]
        [SerializeField] private LayerMask pieceLayer;
        private RaycastHit[] pickingHits = new RaycastHit[1];
        [SerializeField] private HexCell currTargetCell;
        
        [Header("Raycast Dragging Config")]
        private Ray catchingRay;
        [SerializeField] private LayerMask cellLayer;
        private RaycastHit[] cellHits = new RaycastHit[1];
        public Color selectedCellColor, normalCellColor;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            
            catchingRay = new Ray(Vector3.zero, gameplayCam.transform.forward.normalized);
            // Debug.DrawRay(catchingRay.origin, catchingRay.direction, Color.red, 100f);
        }

        private void Update()
        {
            if (LevelManager.Instance.CurrentLevelState == eLevelState.Playing
                || LevelManager.Instance.CurrentLevelState == eLevelState.IsUsingDestroyStackBooster)
            {
#if UNITY_EDITOR
                HandleEditorSelection();
#else
                HandleMobileSelection();
#endif
            }
        }

        #endregion

        #region Input Handlers

        private void HandleEditorSelection()
        {
            if(Input.GetMouseButtonDown(0))
                HandlePicking();
            else if(Input.GetMouseButton(0))
                HandleDragging();
            else if(Input.GetMouseButtonUp(0))
                HandleDropping();
        }

        private void HandleMobileSelection()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandlePicking();
                        break;
                    
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        HandleDragging();
                        break;
                    
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        HandleDropping();
                        break;
                }
            }
        }

        #endregion

        #region Pick

        private async UniTask HandlePicking()
        {
            Vector3 screenTouchPos = Input.touchCount > 0 
                ? Input.GetTouch(0).position
                : Input.mousePosition;
            
            Ray ray = gameplayCam.ScreenPointToRay(screenTouchPos);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            int hitCount = Physics.RaycastNonAlloc(ray, pickingHits, 100f, pieceLayer);
            if (hitCount > 0)
            {
                if (LevelManager.Instance.CurrentLevelState == eLevelState.Playing)
                {
                    AudioManager.Instance.PlaySfx(ConstantKey.SFX_BLOCK_PICKED);
                    currStack = pickingHits[0].transform.GetComponentInParent<HexStackController>();
                    catchingRay.origin = currStack.transform.position;
                }
                else if (LevelManager.Instance.CurrentLevelState == eLevelState.IsUsingDestroyStackBooster)
                {
                    HexCell cell = pickingHits[0].transform.GetComponentInParent<HexCell>();

                    if(cell) OnDestroyStackBoosterUsed(cell).Forget();
                }
            }
        }

        #endregion

        #region Drag

        private Vector3 GetDraggingMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            // Avoid z-fitting issue on ScreenToWorldPoint
            mousePos.z = gameplayCam.WorldToScreenPoint(currStack.transform.position).z + 1.5f;
            return gameplayCam.ScreenToWorldPoint(mousePos).With(z: -ConstantKey.STACK_LIFTING_OFFSET_Z);
        }
        
        private void HandleCatchingCellOnDragging()
        {
            catchingRay.origin = currStack.selfTransform.position;
            // Debug.DrawRay(catchingRay.origin, catchingRay.direction * 500, Color.red);

            int hitCount = Physics.RaycastNonAlloc(catchingRay, cellHits, 500, cellLayer);
            if (hitCount > 0)
            {
                HexCell newTargetCell = cellHits[0].collider.GetComponentInParent<HexCell>();
                if (currTargetCell != newTargetCell)
                {
                    currTargetCell?.SetMaterialState(normalCellColor);
                    currTargetCell = newTargetCell;
                    currTargetCell.SetMaterialState(selectedCellColor);
                }
            }
            else
            {
                if (currTargetCell)
                {
                    currTargetCell.SetMaterialState(normalCellColor);
                    currTargetCell = null;
                }
            }
        }
        
        private void HandleDragging()
        {
            if (currStack == null) return;
            
            Vector3 targetPos = GetDraggingMouseWorldPos();
            currStack.OnDragged(targetPos);

            HandleCatchingCellOnDragging();
        }

        #endregion

        #region Drop

        private void HandleDropping()
        {
            if (!currStack) return;
            
            currStack.OnDropped(currTargetCell);

            if (currTargetCell)
            {
                currTargetCell.SetMaterialState(normalCellColor);
                EventBus<LaidDownStackDTO>.RaiseBoth(new LaidDownStackDTO(currTargetCell));
            }
            
            currStack = null;
            currTargetCell = null;
        }
        
        public void ReturnSelectedStackToTray()
        {
            currTargetCell = null;
            currStack?.OnDropped(null);
            currStack = null;
        }

        #endregion

        #region Destroy Stack Booster

        private async UniTask OnDestroyStackBoosterUsed(HexCell cell)
        {
            await SpawnMagicWand(cell);
            
            await UniTask.Delay(1000);

            cell.CollectAllPieces().Forget();
                    
            LevelManager.Instance.SetLevelState(eLevelState.Playing);
            InGamePage.Instance.ShowBoosterButtons();
            grid.SetStacksOnGridSelectableState(false);
        }

        private async UniTask SpawnMagicWand(HexCell cell)
        {
            MagicWand magicWand = await ObjectPooler.GetFromPool<MagicWand>
                (PoolingType.MagicWand, destroyCancellationToken,InGamePage.Instance.selfRT);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                InGamePage.Instance.selfRT,
                gameplayCam.WorldToScreenPoint(cell.CurrentStack.TopPiece.selfTransform.position),
                null,
                out Vector2 wandSpawnedPos);
            magicWand.selfRT.anchoredPosition = wandSpawnedPos.Add(x:50, y:50);
            magicWand.PlayAnim();
            
            Debug.Break();
        }

        #endregion
    }
}