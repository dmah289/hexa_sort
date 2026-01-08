using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.UI.BaseSystem;
using HexaSort.UI.Loading;
using HexaSort.UI.Loading.InGame;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using TMPro;
using UnityEngine;

namespace HexaSort.Core.Entities.Grid
{
    public class HexCell : MonoBehaviour, IPoolableObject
    {
        [Header("----- Self Components -----")]
        public Transform selfTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider collider;
        
        [Header("----- Managers -----")]
        [SerializeField] private HexStackController currStack;
        private (int row, int col) gridPos;
        
        [Header("----- Mechanics -----")]
        [SerializeField] private WoodCell woodCell;
        [SerializeField] private PackedCell packedCell;
        
        [SerializeField] private TextMeshPro idxTxt;

        #region Properties
        
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
        
        #endregion

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
            DisableAllMechanics();
            selfTransform.Reset();
            meshRenderer.SetVertexLitColor(SelectionController.Instance.normalCellColor);
            Selectable = true;
        }

        public void OnReturnToPool()
        {
            DisableAllMechanics();
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

        public bool HasAtLeastOneUnoccupiedNeighbor(GridController grid)
        {
            int startIdx = (gridPos.col & 1) == 1 ? 0 : 6;

            for (int i = startIdx; i < startIdx + 6; i++)
            {
                int newCol = gridPos.col + PathFinder.colOffsets[i % 6];
                int newRow = gridPos.row + PathFinder.rowOffsets[i];
                
                if (newCol < 0 || newCol >= grid.GridSize.width || newRow < 0 || newRow >= grid.GridSize.height)
                    continue;
                
                HexCell neighbor = grid.GridCells[newRow, newCol];
                
                if (neighbor && !neighbor.IsOccupied)
                {
                    return true;
                }
            }

            return false;
        }
        
        public async UniTask CollectAllPieces(bool withCollectingGoal = true)
        {
            if (!IsOccupied) return;
            
            int totalPieces = PiecesCount;
            for (int i = 0; i < totalPieces; i++)
            {
                CurrentStack.CollectLastPiece();

                int idxToPlayAnim = Mathf.Max(totalPieces - 2, 0);
                if(i == idxToPlayAnim && !CanvasManager.Instance.pieceTrackerPanel.IsCompleted)
                    await VFXManager.Instance.PlayVFXToGoalPanel(this,
                        eLevelGoalType.Piece,
                        totalPieces,
                        destroyCancellationToken,
                        withCollectingGoal);
                    
                await UniTask.Delay((int)(HexPieceController.ScaleDuration * 0.2f * 1000f));
            }
            
            if (PiecesCount == 0)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexStack, CurrentStack, destroyCancellationToken);
                CurrentStack = null;
            }
        }
        
        #endregion

        #region Spawn Objects
        
        public void SetupMechanics(CellData cellData)
        {
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

        #endregion

        public void SetIdxTxt(int i, int j)
        {
            idxTxt.text = $"[{i},{j}]";
        }
    }
}