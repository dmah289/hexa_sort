using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Scripts.Core.Entities.Piece;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexaSort.Scripts.Core.Entities
{
    public class HexStackController : MonoBehaviour, IPoolableObject
    {
        [Header("Self Components")]
        public Transform selfTransform;
        
        [Header("Managers")]
        [SerializeField] private List<HexPieceController> pieces = new();
        
        public ColorType ColorOnTop => pieces.Count > 0 ? pieces[^1].ColorType : default;

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

        public async UniTaskVoid OnSpawned(int idx, Vector2 spawnMidStackPos)
        {
            int pieceAmount = Random.Range(3, 8);
            for(int i = 0; i < pieceAmount; i++)
            {
                HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(PoolingType.HexPiece,
                    destroyCancellationToken,
                    selfTransform
                );
                
                int colorIdx = Random.Range(0, System.Enum.GetValues(typeof(ColorType)).Length);
                piece.ColorType = (ColorType)colorIdx;

                Vector3 spawnedPos = (i * (ConstantKey.HEX_PIECE_THICKNESS + 0.01f) * Vector3.back).Add(y: i*0.02f);
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

        public void OnGetFromPool()
        {
            selfTransform.Reset();
        }

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
                targetLocalPos = ConstantKey.STACK_LOCAL_POS_ON_CELL;
                Selectable = false;
                targetCell.CurrentStack = this;
            }
            else targetLocalPos = Vector3.zero;
            
            selfTransform.DOKill();
            float duration = selfTransform.localPosition.magnitude / ConstantKey.SNAP_TO_TARGET_VELOCITY;
            selfTransform.DOLocalMove(targetLocalPos, duration)
                .SetEase(Ease.OutSine)
                .OnKill(() => selfTransform.localPosition = targetLocalPos);
        }
    }
}