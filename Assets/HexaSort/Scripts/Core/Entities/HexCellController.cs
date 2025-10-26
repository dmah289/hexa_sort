using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class HexCellController : MonoBehaviour, IPoolableObject
    {
        public void OnGetFromPool()
        {
            transform.Reset();
        }
    }
}