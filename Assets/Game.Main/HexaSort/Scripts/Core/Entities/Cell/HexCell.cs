using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.Scripts.Core.Controllers;
using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Core.Entities.Grid
{
    public class HexCell : MonoBehaviour, IPoolableObject
    {
        [Header("Self Components")]
        public Transform selfTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider collider;
        
        [Header("Managers")]
        [SerializeField] private HexStackController currStack;
        private (int row, int col) gridPos;
        
        [Header("Mechanics")]
        [SerializeField] private WoodCell woodCell;
        [SerializeField] private PackedCell packedCell;

        
        public bool IsMergable => IsOccupied && !woodCell.gameObject.activeSelf
                                             && !packedCell.gameObject.activeSelf;
        
        public ColorType ColorOnTop => IsOccupied ? currStack.ColorOnTop : default;
        
        public bool IsOccupied => currStack != null
                                  || woodCell.gameObject.activeSelf
                                  || packedCell.gameObject.activeSelf;
        
        public (int row, int col) GridPos
        {
            get => gridPos;
            set => gridPos = value;
        }

        public HexStackController CurrentStack
        {
            get => currStack;
            set
            {
                currStack = value;
                Selectable = !currStack;
            }
        }

        public bool Selectable
        {
            get => collider.enabled;
            set => collider.enabled = value;
        }
        
        public int PiecesCount => currStack ? currStack.PiecesCount : 0;

        #region Unity APIs

        private void Awake()
        {
            selfTransform = transform;
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        #endregion
        
        #region Object Pooling Callbacks

        public void OnGetFromPool()
        {
            transform.Reset();
            meshRenderer.SetVertexLitColor(SelectionController.Instance.normalCellColor);
            Selectable = true;
        }

        public void OnReturnToPool()
        {
            if (currStack)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexStack, currStack, destroyCancellationToken);
                CurrentStack = null;
            }
        }

        #endregion

        #region Class Methods

        public void SetMaterialState(Color color)
        {
            meshRenderer.SetVertexLitColor(color);
        }
        
        public bool IsNeighborOf((int row, int col) sourceCellGridPos)
        {
            int startIdx = (gridPos.col & 1) == 1 ? 0 : 6;

            for (int i = startIdx; i < startIdx + 6; i++)
            {
                int newCol = gridPos.col + PathFinder.colOffsets[i % 6];
                int newRow = gridPos.row + PathFinder.rowOffsets[i];

                if (newCol == sourceCellGridPos.col && newRow == sourceCellGridPos.row)
                {
                    return true;
                }
            }

            return false;
        }
        
        public async UniTask CollectAllPieces(bool withCollectingGoal = true)
        {
            if (!IsOccupied) return;
            
            Debug.Log($"{gameObject.name} - {PiecesCount}");
            for (int i = 0; i < PiecesCount; i++)
            {
                CurrentStack.CollectLastPiece();
                
                if(i == PiecesCount-2 && !CanvasManager.Instance.pieceTrackerPanel.IsCompleted)
                    await VFXManager.Instance.PlayVFXToPieceGoalPanel(this,
                        eLevelGoalType.Piece,
                        PiecesCount,
                        destroyCancellationToken,
                        withCollectingGoal);
                    
                await UniTask.Delay((int)(HexPieceController.ScaleDuration * 0.2f * 1000f));
            }
            
            if (CurrentStack.Pieces.Count == 0)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexStack, CurrentStack, destroyCancellationToken);
                CurrentStack = null;
            }
            
            
        }
        
        #endregion

        #region Spawn Objects
        
        public void SpawnObjects(CellData cellData)
        {
            DisableAllMechanics();

            if (cellData.MechanicsType == eMechanicsType.Wood && cellData.HasWood)
                woodCell.Setup();
            else if(cellData.MechanicsType == eMechanicsType.Packed && cellData.packedStack.IsValid())
                packedCell.Setup(cellData.packedStack).Forget();
        }

        private void DisableAllMechanics()
        {
            woodCell.gameObject.SetActive(false);
            packedCell.gameObject.SetActive(false);
        }
        
        public async UniTask<SightingTarget> SpawnSightingTarget(RectTransform loosePanel, Camera mainCam)
        {
            SightingTarget sightingTarget = await ObjectPooler.GetFromPool<SightingTarget>
                (PoolingType.SightingTarget, destroyCancellationToken, loosePanel);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                loosePanel,
                mainCam.WorldToScreenPoint(CurrentStack.TopPiece.selfTransform.position),
                null,
                out Vector2 localPos);

            sightingTarget.ParentCell = this;
            sightingTarget.GetComponent<RectTransform>().anchoredPosition = localPos;

            return sightingTarget;
        }

        #endregion
    }
}