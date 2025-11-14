using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Scripts.Core.Entities;
using HexaSort.Scripts.Core.Entities.Piece;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;
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
        
        public ColorType ColorOnTop => pieces.Count > 0 ? pieces[^1].ColorType : default;
        public List<HexPieceController> Pieces => pieces;
        public bool IsOnGrid => isOnGrid;
        public float Height => pieces.Count * ConstantKey.HEX_PIECE_THICKNESS;
        public int PiecesCount => pieces.Count;

        private void Awake()
        {
            selfTransform = transform;
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

        public async UniTaskVoid OnSpawningOnTray(int idx, Vector2 spawnMidStackPos)
        {
            int pieceAmount = Random.Range(3, 8);
            for(int i = 0; i < pieceAmount; i++)
            {
                HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(PoolingType.HexPiece,
                    destroyCancellationToken,
                    selfTransform
                );
                
                int colorIdx = Random.Range(0, 3);
                piece.ColorType = (ColorType)colorIdx;

                Vector3 spawnedPos = (i * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back).Add(y: i * ConstantKey.BACKWARD_PIECE_OFFSET_Y);
                piece.transform.localPosition = spawnedPos;
                
                pieces.Add(piece);
            }
            Selectable = true;
            
            selfTransform.position = spawnMidStackPos + (idx-1) * new Vector2(ConstantKey.HEX_STACK_SPACING, 0);

            selfTransform.DOKill();
            float duration = selfTransform.localPosition.x / ConstantKey.SLIDE_IN_VELOCITY;
            selfTransform.DOLocalMove(Vector3.zero, duration)
                .SetEase(Ease.OutFlash)
                .SetDelay(idx * 0.3f)
                .OnKill(() => selfTransform.localPosition = Vector3.zero);
        }

        #region Object Pooling Callbacks

        public void OnGetFromPool()
        {
            parentCell = null;
            isOnGrid = false;
            selfTransform.Reset();
        }

        public void OnReturnToPool()
        {
            parentCell = null;
            isOnGrid = false;
            
            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                ObjectPooler.ReturnToPool(PoolingType.HexPiece, pieces[i], destroyCancellationToken);
                pieces.RemoveAt(i);
            }
        }

        #endregion

        public void OnDragged(Vector3 targetPos)
        {
            selfTransform.position = Vector3.Lerp(selfTransform.position, targetPos, Time.deltaTime * 20);
        }

        public void OnDropped(HexCell targetCell)
        {
            Vector3 targetLocalPos;
            if (targetCell)
            {
                selfTransform.SetParent(targetCell.selfTransform);
                parentCell = targetCell;
                
                targetLocalPos = ConstantKey.STACK_LOCAL_POS_ON_CELL;
                Selectable = false;
                targetCell.CurrentStack = this;
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
    }
}