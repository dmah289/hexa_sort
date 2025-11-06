using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Core.Entities.Piece;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeSequenceExecutor : MonoBehaviour
    {
        [Header("Merge Tracking")]
        [SerializeField] private bool isCheckingMerging;
        private List<HexCell> waitingMergableCells = new();
        
        [SerializeField] private bool isMerging;
        [SerializeField] private bool isCollectingPieces;
        [SerializeField] private bool newStackLaidDown;
        
        // TODO : dynammic threshold base on level progress
        [SerializeField] private int dynamicThresholdForCollectingPieces = 10;
        
        
        public List<HexCell> WaitingMergableCells => waitingMergableCells;
        public bool IsMerging => isMerging;
        public bool IsCollectingPieces => isCollectingPieces;

        public bool NewStackLaidDown
        {
            get => newStackLaidDown;
            set => newStackLaidDown = value;
        }

        public async UniTask ExecuteMergeSequence(List<HexCell> connectedCells, HexCell[,] parents)
        {
            // Merge from the leaf nodes to the root node
            for (int i = connectedCells.Count - 1; i > 0; i--)
            {
                // Check new block placed during merging
                if (newStackLaidDown)
                {
                    newStackLaidDown = false;
                    waitingMergableCells.Add(connectedCells[i]);
                    return;
                }
                
                HexCell parent = parents[connectedCells[i].GridPos.row, connectedCells[i].GridPos.col];
                await DoPairMerge(connectedCells[i], parent);
                
                await UniTask.Yield();
            }
            
            await UniTask.Delay(100);
            
            // Check new block placed during merging
            if (newStackLaidDown)
            {
                newStackLaidDown = false;
                waitingMergableCells.Add(connectedCells[0]);
                return;
            }

            await CheckCollectingPieces(connectedCells[0]);
        }

        private async UniTask DoPairMerge(HexCell startCell, HexCell endCell)
        {
            isMerging = true;

            ColorType sharedColor = endCell.ColorOnTop;

            while (startCell.CurrentStack.Pieces.Count > 0 && startCell.ColorOnTop == sharedColor)
            {
                await endCell.CurrentStack.AttractPiece(startCell.CurrentStack.Pieces.RemoveLast());
                await UniTask.Yield();
            }

            CheckIfCanContinueCollecting(startCell);
            
            isMerging = false;
        }

        private void CheckIfCanContinueCollecting(HexCell cell)
        {
            if (cell.CurrentStack.Pieces.Count == 0)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexStack, cell.CurrentStack, destroyCancellationToken);
                cell.CurrentStack = null;
            }
            else waitingMergableCells.Add(cell);
        } 
        
        private async UniTask CheckCollectingPieces(HexCell cell)
        {
            isCollectingPieces = true;

            int sameColorCount = 0;
            ColorType targetColor = cell.ColorOnTop;
            for (int i = cell.CurrentStack.Pieces.Count - 1; i >= 0; i--)
            {
                if (cell.CurrentStack.Pieces[i].ColorType == targetColor) sameColorCount++;
                else break;
            }

            if (sameColorCount >= dynamicThresholdForCollectingPieces)
            {
                for (int i = 0; i < sameColorCount; i++)
                {
                    cell.CurrentStack.CollectLastPiece();
                    await UniTask.Delay((int)(HexPieceController.ScaleDuration * 0.25f * 1000f));
                }
            }
            
            // TODO : Raise event piece collected
            
            CheckIfCanContinueCollecting(cell);
            
            isCollectingPieces = false;
        }
    }
}