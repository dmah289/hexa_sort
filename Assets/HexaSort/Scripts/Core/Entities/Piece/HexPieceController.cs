using System;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities.Piece
{
    public class HexPieceController : MonoBehaviour, IPoolableObject
    {
        [SerializeField] private Collider selfCollider;

        public bool Selectable
        {
            get => selfCollider.enabled;
            set => selfCollider.enabled = value;
        }

        private void Awake()
        {
            // selfCollider = GetComponentInChildren<Collider>();
        }

        public void OnGetFromPool()
        {
            transform.Reset();
            Selectable = false;
        }
    }
}