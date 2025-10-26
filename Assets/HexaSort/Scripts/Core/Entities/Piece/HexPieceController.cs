using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities.Piece
{
    public class HexPieceController : MonoBehaviour, IPoolableObject
    {
        
        
        public void OnGetFromPool()
        {
            transform.Reset();
        }
    }
}