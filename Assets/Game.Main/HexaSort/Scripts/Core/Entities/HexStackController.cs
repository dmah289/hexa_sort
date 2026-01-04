using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Audio;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.Core.Entities.Grid;
using HexaSort.Core.Entities.Grid.Piece;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace HexaSort.Core.Entities
{
    public class HexStackController : MonoBehaviour, IPoolableObject
    {
        [Header("Self Components")]
        public Transform selfTransform;
        
        [Header("----- Managers -----")]
        [SerializeField] private List<HexPieceController> pieces = new();
        [SerializeField] private HexCell parentCell;
        [SerializeField] private bool isOnGrid;
        [SerializeField] private int idxOnTray;
        [SerializeField] private bool sfxSpawnedPlayed;
        [SerializeField] private int[] colorDistribution;
        
        // Cache for smooth dragging - avoid allocation
        private Vector3 dragVelocity;
        private const float DRAG_SMOOTH_TIME = 0.05f;
        
        #region Properties
        
        public ColorType ColorOnTop => pieces.Count > 0 ? pieces[^1].ColorType : default;
        public List<HexPieceController> Pieces => pieces;
        public HexPieceController TopPiece => pieces[^1];
        public bool IsOnGrid => isOnGrid;
        public float Height => pieces.Count * ConstantKey.HEX_PIECE_THICKNESS;
        public int PiecesCount => pieces.Count;
        public int IdxOnTray
        {
            get => idxOnTray;
            set => idxOnTray = value;
        }
        public bool Selectable
        {
            get => pieces[0].Selectable && pieces[^1].Selectable && pieces[pieces.Count/2];
            set
            {
                pieces[0].Selectable = value;
                pieces[^1].Selectable = value;
                pieces[pieces.Count/2].Selectable = value;
            }
        }
        public int TopColorAmount
        {
            get
            {
                if (pieces.Count == 0) return 0;
                
                ColorType topColor = ColorOnTop;
                int count = 0;
                for (int i = pieces.Count - 1; i >= 0; i--)
                {
                    if (pieces[i].ColorType == topColor)
                        count++;
                    else break;
                }
                return count;
            }
        }
        
        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            selfTransform = transform;
        }

        #endregion

        #region Spawning Methods

        /// <summary>
        /// Spawn stack on tray with random piece's amount per colors
        /// </summary>
        /// <param name="idx">Stack order on tray</param>
        /// <param name="spawnMidStackPos">Center of the stacks on tray</param>
        /// <param name="chosenColors">Max 3 colors randomly or sorted by descending pieces amount per colors on board</param>
        public async UniTask OnSpawningOnTray(int idx,
            Vector2 spawnMidStackPos,
            List<ColorType> chosenColors,
            bool allowWaitingSliding = false)
        {
            idxOnTray = idx;
            
            int totalPieces = Random.Range(chosenColors.Count, 7);
            
            colorDistribution = DistributeColorsToLayers(totalPieces, chosenColors.Count);
                
            // spawn layer by layer
            int currentPieceIndex = 0;
            for (int colorLayerIdx = 0; colorLayerIdx < chosenColors.Count; colorLayerIdx++)
            {
                ColorType currentColor = chosenColors[colorLayerIdx];
                int piecesForThisColor = colorDistribution[colorLayerIdx];
                    
                // Sinh các mảnh cùng màu liên tiếp
                for (int j = 0; j < piecesForThisColor; j++)
                {
                    HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(
                        PoolingType.HexPiece,
                        destroyCancellationToken,
                        selfTransform
                    );

                    piece.ColorType = currentColor;

                    Vector3 spawnedPos = (currentPieceIndex * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back)
                        .Add(y: currentPieceIndex * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
                    piece.transform.localPosition = spawnedPos;

                    pieces.Add(piece);
                    currentPieceIndex++;
                }
            }
            
            Selectable = true;

            selfTransform.position = spawnMidStackPos + (idx-1) * new Vector2(ConstantKey.HEX_STACK_SPACING, 0);

            selfTransform.DOKill();
            float duration = selfTransform.localPosition.x / ConstantKey.SLIDE_IN_VELOCITY;
            UniTask slidingUniTask =  selfTransform.DOLocalMove(Vector3.zero, duration)
                .SetEase(Ease.OutFlash)
                .SetDelay(idx * 0.3f)
                .OnKill(() => selfTransform.localPosition = Vector3.zero)
                .OnUpdate(() =>
                {
                    if(!sfxSpawnedPlayed && Vector3.Distance(selfTransform.localPosition, Vector3.zero) < 2f)
                    {
                        sfxSpawnedPlayed = true;
                        AudioManager.Instance.PlaySfx(ConstantKey.SFX_BLOCK_SPAWNED);
                    }
                }).ToUniTask();

            if (allowWaitingSliding)
                await slidingUniTask;
            else slidingUniTask.Forget();
        }
        
        /// <summary>
        /// Spawn stack on tray with random piece's amount per colors
        /// </summary>
        /// <param name="idx">Stack order on tray</param>
        /// <param name="spawnMidStackPos">Center of the stacks on tray</param>
        /// <param name="chosenColors">Max 3 colors randomly or sorted by descending pieces amount per colors on board</param>
        public async UniTask OnSpawningOnTray2(int idx,
            Vector2 spawnMidStackPos,
            List<ColorType> chosenColors,
            int[] colorDistribution,
            bool allowWaitingSliding = false)
        {
            idxOnTray = idx;
            
            int totalPieces = Random.Range(chosenColors.Count, 7);
            
            colorDistribution = DistributeColorsToLayers(totalPieces, chosenColors.Count);
                
            // spawn layer by layer
            int currentPieceIndex = 0;
            for (int colorLayerIdx = 0; colorLayerIdx < chosenColors.Count; colorLayerIdx++)
            {
                ColorType currentColor = chosenColors[colorLayerIdx];
                int piecesForThisColor = colorDistribution[colorLayerIdx];
                    
                // Sinh các mảnh cùng màu liên tiếp
                for (int j = 0; j < piecesForThisColor; j++)
                {
                    HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(
                        PoolingType.HexPiece,
                        destroyCancellationToken,
                        selfTransform
                    );

                    piece.ColorType = currentColor;

                    Vector3 spawnedPos = (currentPieceIndex * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back)
                        .Add(y: currentPieceIndex * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
                    piece.transform.localPosition = spawnedPos;

                    pieces.Add(piece);
                    currentPieceIndex++;
                }
            }
            
            Selectable = true;

            selfTransform.position = spawnMidStackPos + (idx-1) * new Vector2(ConstantKey.HEX_STACK_SPACING, 0);

            selfTransform.DOKill();
            float duration = selfTransform.localPosition.x / ConstantKey.SLIDE_IN_VELOCITY;
            UniTask slidingUniTask =  selfTransform.DOLocalMove(Vector3.zero, duration)
                .SetEase(Ease.OutFlash)
                .SetDelay(idx * 0.3f)
                .OnKill(() => selfTransform.localPosition = Vector3.zero)
                .OnUpdate(() =>
                {
                    if(!sfxSpawnedPlayed && Vector3.Distance(selfTransform.localPosition, Vector3.zero) < 2f)
                    {
                        sfxSpawnedPlayed = true;
                        AudioManager.Instance.PlaySfx(ConstantKey.SFX_BLOCK_SPAWNED);
                    }
                }).ToUniTask();

            if (allowWaitingSliding)
                await slidingUniTask;
            else slidingUniTask.Forget();
        }
        
        /// <summary>
        /// Allocate number of pieces per color layer based on weights
        /// </summary>
        private int[] DistributeColorsToLayers(int totalPieces, int colorCount)
        {
            int[] distribution = new int[colorCount];
            
            if (colorCount == 1)
            {
                distribution[0] = totalPieces;
                return distribution;
            }
            
            // Allocate min one piece per layer
            int minPerColor = 1;
            int remaining = totalPieces - (colorCount * minPerColor);
            for (int i = 0; i < colorCount; i++)
            {
                distribution[i] = minPerColor;
            }

            // allocate remaining pieces based on weights
            // color with higher index (less pieces on board) has higher chance to get more spawned pieces
            while (remaining > 0)
            {
                int randomValue = Random.Range(0, GameDifficultyController.Instance.TotalWeight);
                int selectedColorIdx = colorCount - 1;
                
                for (int i = 0; i < colorCount; i++)
                {
                    if (randomValue < GameDifficultyController.Instance.CumulativeWeights[i])
                    {
                        selectedColorIdx = i;
                        break;
                    }
                }
                
                distribution[selectedColorIdx]++;
                remaining--;
            }
            
            return distribution;
        }

        public async UniTask OnSpawningOnCell(HexCell targetCell, PackedStackData packedStackData)
        {
            parentCell = targetCell;
            targetCell.CurrentStack = this;

            int currPieceAmount = 0;
            for (int i = 0; i < packedStackData.ColorLayers.Length; i++)
            {
                for(int j = 0; j < packedStackData.ColorLayers[i].amount; j++)
                {
                    HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(PoolingType.HexPiece,
                        destroyCancellationToken,
                        selfTransform
                    );
                    piece.ColorType = packedStackData.ColorLayers[i].colorType;

                    Vector3 spawnedPos = (currPieceAmount * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back).Add(y: currPieceAmount * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
                    piece.transform.localPosition = spawnedPos;
                    
                    pieces.Add(piece);
                    currPieceAmount++;
                }
            }
            Selectable = false;
            
            selfTransform.localPosition = ConstantKey.STACK_LOCAL_POS_ON_CELL;
            isOnGrid = true;
        }

        #endregion

        #region Object Pooling Callbacks

        public void OnGetFromPool()
        {
            idxOnTray = -1;
            parentCell = null;
            isOnGrid = false;
            sfxSpawnedPlayed = false;
            dragVelocity = Vector3.zero;
            
            selfTransform.Reset();
        }

        public void OnReturnToPool()
        {
            idxOnTray = -1;
            parentCell = null;
            isOnGrid = false;
            dragVelocity = Vector3.zero;
            
            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexPiece, pieces[i], destroyCancellationToken);
                pieces.RemoveAt(i);
            }
        }

        #endregion

        #region Interaction Callbacks

        public void OnDragged(Vector3 targetPos)
        {
            selfTransform.position = Vector3.SmoothDamp(
                selfTransform.position, 
                targetPos, 
                ref dragVelocity, 
                DRAG_SMOOTH_TIME
            );
        }

        public void OnDropped(HexCell targetCell)
        {
            Vector3 targetLocalPos;
            if (targetCell)
            {
                AudioManager.Instance.PlaySfx(ConstantKey.SFX_BLOCK_DROPPED);
                selfTransform.SetParent(targetCell.selfTransform);
                parentCell = targetCell;
                
                targetLocalPos = ConstantKey.STACK_LOCAL_POS_ON_CELL;
                targetCell.CurrentStack = this;

                Selectable = false;
            }
            else targetLocalPos = Vector3.zero;
            
            selfTransform.DOKill();
            float duration = selfTransform.localPosition.magnitude / ConstantKey.SNAP_TO_TARGET_VELOCITY;
            selfTransform.DOLocalMove(targetLocalPos, duration)
                .SetEase(Ease.OutSine)
                .OnKill(() => selfTransform.localPosition = targetLocalPos)
                .OnComplete(() =>
                {
                    if (targetCell) isOnGrid = true;
                });
        }

        #endregion

        #region Merge Anim

        public void AttractPiece(HexPieceController newPiece, Vector3 overturnDir, float maxHeight)
        {
            pieces.Add(newPiece);
            newPiece.selfTransform.SetParent(selfTransform);
            
            Vector3 targetLocalPos = ((pieces.Count - 1) * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back)
                .Add(y: (pieces.Count - 1) * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
            
            newPiece.OverturnToLocalPos(targetLocalPos, overturnDir, maxHeight);
        }

        public void CollectLastPiece()
        {
            pieces.RemoveLast().OnCollected();
        }

        #endregion
    }
}
