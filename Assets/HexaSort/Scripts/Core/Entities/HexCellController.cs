using System;
using HexaSort.Scripts.Core.Controllers;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class HexCellController : MonoBehaviour, IPoolableObject
    {
        [Header("Self Components")]
        public Transform selfTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider collider;
        
        [Header("Managers")]
        [SerializeField] private HexStackController currStack;
        
        public bool IsOccupied => currStack != null;

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

        public void SetSelectedState(Color color)
        {
            meshRenderer.SetVertexLitColor(color);
        }
    }
}