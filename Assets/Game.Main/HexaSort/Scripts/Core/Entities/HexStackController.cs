using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Audio;
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
        
        [Header("Managers")]
        [SerializeField] private List<HexPieceController> pieces = new();
        [SerializeField] private HexCell parentCell;
        [SerializeField] private bool isOnGrid;
        [SerializeField] private int idxOnTray;
        [SerializeField] private bool sfxSpawnedPlayed;
        
        // Cache for smooth dragging - avoid allocation
        private Vector3 _dragVelocity;
        private const float DRAG_SMOOTH_TIME = 0.05f; // Điều chỉnh để mượt hơn (0.05-0.1f)
        
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
        
        #endregion
        

        #region Unity Callbacks

        private void Awake()
        {
            selfTransform = transform;
        }

        #endregion

        #region Spawning Methods

        public async UniTask OnSpawningOnTray(int idx, Vector2 spawnMidStackPos, bool allowWaitingSliding = false)
        {
            idxOnTray = idx;
            int pieceAmount = Random.Range(3, 8);
            for(int i = 0; i < pieceAmount; i++)
            {
                HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(PoolingType.HexPiece,
                    destroyCancellationToken,
                    selfTransform
                );
                
                int colorIdx = Random.Range(2, 4);
                piece.ColorType = (ColorType)colorIdx;

                Vector3 spawnedPos = (i * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back).Add(y: i * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
                piece.transform.localPosition = spawnedPos;
                
                pieces.Add(piece);
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
            _dragVelocity = Vector3.zero; // Reset velocity
            
            selfTransform.Reset();
        }

        public void OnReturnToPool()
        {
            idxOnTray = -1;
            parentCell = null;
            isOnGrid = false;
            _dragVelocity = Vector3.zero; // Reset velocity
            
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
                ref _dragVelocity, 
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