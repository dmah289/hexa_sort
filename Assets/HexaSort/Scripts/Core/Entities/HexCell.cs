using System;
using HexaSort.Scripts.Core.Controllers;
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
        [SerializeField] private Vector2Int gridPos;
        
        public bool IsOccupied => currStack != null;
        
        public Vector2Int GridPos
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
                collider.enabled = !currStack;
            }
        }

        private void Awake()
        {
            selfTransform = transform;
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public void OnGetFromPool()
        {
            transform.Reset();
            meshRenderer.SetVertexLitColor(SelectionController.Instance.normalCellColor);
        }

        public void SetMaterialState(Color color)
        {
            meshRenderer.SetVertexLitColor(color);
        }
    }
}