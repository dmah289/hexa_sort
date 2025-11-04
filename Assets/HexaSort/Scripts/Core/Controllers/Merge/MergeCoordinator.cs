using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Core.Entities;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeCoordinator : MonoBehaviour
    {
        [Header("Merge Tracking")]
        [SerializeField] private bool isCheckingMerging;
        private List<HexCell> waitingMergableCells = new();
        
        public List<HexCell> WaitingMergableCells => waitingMergableCells;
        
        
    }
}