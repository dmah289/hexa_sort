using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Main.LevelEditor.Scripts.LevelData;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Core.Entities.Piece;
using HexaSort.UI.Gameplay.Goals;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeSequenceExecutor : MonoBehaviour
    {
        private const int MergeDelayBeforeNewExecution = 300;
        private const int MergeDelayBetween2PairMerge = 500;
        private const int CheckCollectingDelay = 300;
        
        [Header("References")]
        [SerializeField] private RectTransform piecesGoalTargetPanel;
        [SerializeField] private Camera mainCam;
        
        [Header("Merge Tracking")]
        [SerializeField] private bool isCheckingMerging;
        private List<HexCell> waitingMergableCells = new();
        
        [SerializeField] private bool isPairMerging;
        [SerializeField] private bool isCheckingCollecting;
        [SerializeField] private bool newStackLaidDown;
        
        // TODO : dynammic threshold base on level progress
        [SerializeField] private int dynamicThresholdForCollectingPieces = 10;
        
        
        public List<HexCell> WaitingMergableCells => waitingMergableCells;
        public bool IsBusy => isPairMerging || isCheckingCollecting;
        

        public bool NewStackLaidDown
        {
            get => newStackLaidDown;
            set => newStackLaidDown = value;
        }

        public async UniTask ExecuteMergeSequence(List<HexCell> connectedCells, HexCell[,] parents)
        {
            await UniTask.Delay(MergeDelayBeforeNewExecution);
            
            if (connectedCells.Count <= 1)
                return;
            
            // Merge from the leaf nodes to the root node
            for (int i = connectedCells.Count - 1; i > 0; i--)
            {
                // Check new block placed during merging
                if (newStackLaidDown)
                {
                    newStackLaidDown = false;
                    waitingMergableCells.Add(connectedCells[i]);
                    await UniTask.Delay(MergeDelayBeforeNewExecution);
                    return;
                }
                
                HexCell parent = parents[connectedCells[i].GridPos.row, connectedCells[i].GridPos.col];
                await DoPairMerge(connectedCells[i], parent);
                
                await UniTask.Delay(MergeDelayBetween2PairMerge);
            }
            
            await UniTask.Delay(CheckCollectingDelay);
            
            // Check new block placed during merging
            if (newStackLaidDown)
            {
                newStackLaidDown = false;
                waitingMergableCells.Add(connectedCells[0]);
                await UniTask.Delay(MergeDelayBeforeNewExecution);
                return;
            }

            await CheckCollectingPieces(connectedCells[0]);
        }

        private async UniTask DoPairMerge(HexCell startCell, HexCell endCell)
        {
            isPairMerging = true;
            
            await UniTask.WaitUntil(() => startCell.CurrentStack.IsOnGrid);

            ColorType sharedColor = endCell.ColorOnTop;

            while (startCell.CurrentStack.Pieces.Count > 0 && startCell.ColorOnTop == sharedColor)
            {
                Vector3 overturnDir = (endCell.selfTransform.position - startCell.selfTransform.position).normalized;
                
                float maxHeight = Mathf.Max(startCell.CurrentStack.Height, endCell.CurrentStack.Height);

                endCell.CurrentStack.AttractPiece(startCell.CurrentStack.Pieces.RemoveLast(), overturnDir, maxHeight);
                await UniTask.Delay((int)(0.07f * HexPieceController.OverturnDuration * 1000f));
            }

            CheckIfCanContinueMerging(startCell);
            
            isPairMerging = false;
        }

        private void CheckIfCanContinueMerging(HexCell cell)
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
            isCheckingCollecting = true;

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
                    
                    if(i == sameColorCount-2)
                        await PlayVFXToPieceGoalPanel(cell, sameColorCount);
                    
                    await UniTask.Delay((int)(HexPieceController.ScaleDuration * 0.2f * 1000f));
                }
                
                CheckIfCanContinueMerging(cell);
            }
            
            isCheckingCollecting = false;
        }

        private async UniTask PlayVFXToPieceGoalPanel(HexCell cell, int sameColorCount)
        {
            RectTransform starTrail = await ObjectPooler.GetFromPool<RectTransform>(PoolingType.StarTrail, destroyCancellationToken, piecesGoalTargetPanel);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(piecesGoalTargetPanel,
                mainCam.WorldToScreenPoint(cell.selfTransform.position),
                null,
                out Vector2 localPos);
            starTrail.anchoredPosition = localPos;

            eGoalCollectedDTO piecesCollectedDTO = new eGoalCollectedDTO(eLevelGoalType.Piece, sameColorCount);
                
            for (int i = 0; i < starTrail.childCount; i++)
            {
                Image image = starTrail.GetChild(i).GetComponent<Image>();
                if (image != null)
                    image.enabled = i == (int)piecesCollectedDTO.goalType;
            }
                
            starTrail.DOAnchorPos(Vector2.zero, 0.75f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                ObjectPooler.ReturnToPool(PoolingType.StarTrail, starTrail, destroyCancellationToken);
                // EventBus<eGoalCollectedDTO>.Raise(piecesCollectedDTO);
            });
        }
    }
}