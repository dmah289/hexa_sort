using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities;
using HexaSort.Scripts.Core.Controllers;
using HexaSort.Scripts.Core.Entities.Piece;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
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
        [SerializeField] private CellLock cellLock;
        [SerializeField] private CellWood cellWood;
        
        public ColorType ColorOnTop => IsOccupied ? currStack.ColorOnTop : default;
        
        public bool IsOccupied => currStack != null;
        
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
        
        public int PiecesCount => IsOccupied ? currStack.PiecesCount : 0;

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

        // TODO : Show sighting target effect on top of stack
        public void ShowSightingTarget()
        {
            
        }
        
        #endregion

        #region Spawn Objects

        // TODO : Spawn Objects
        public void SpawnObjects(CellData cellData)
        {
            if (cellData.HasWood)
                SpawnWoods();
            else if (cellData.UnlockValue > 0) 
                SpawnLock(cellData.UnlockValue).Forget();
            else if(cellData.LockedStack.IsValid())
                SpawnLockedStack(cellData.LockedStack);
        }

        private void SpawnLockedStack(LockedStackData lockedStackData)
        {
            
        }

        private async UniTask SpawnLock(int unlockValue)
        {
            // cellLock = await ObjectPooler.GetFromPool<CellLock>(PoolingType.CellLock, destroyCancellationToken, selfTransform);
            // cellLock.Setup(unlockValue, this);
        }

        private void SpawnWoods()
        {
            
        }

        #endregion

        
    }
}