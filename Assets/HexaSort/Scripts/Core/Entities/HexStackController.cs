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
        [SerializeField] private Transform selfTransform;

        private void Awake()
        {
            selfTransform = transform;
        }

        public async UniTaskVoid Setup(int idx, Vector2 spawnMidStackPos)
        {
            int pieceAmount = Random.Range(3, 8);
            for(int i = 0; i < pieceAmount; i++)
            {
                HexPieceController piece = await ObjectPooler.GetFromPool<HexPieceController>(PoolingType.HexPiece,
                    destroyCancellationToken,
                    selfTransform
                );
                
                piece.transform.localPosition = i * ConstantKey.HEX_PIECE_THICKNESS * Vector3.back;
            }
            
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

        public void OnDropped(HexCellController targetCell)
        {
            if (!targetCell)
            {
                selfTransform.DOKill();
                float duration = selfTransform.localPosition.magnitude / ConstantKey.BACK_TO_HOLDER_VELOCITY;
                selfTransform.DOLocalMove(Vector3.zero, duration)
                    .SetEase(Ease.OutFlash)
                    .OnKill(() => selfTransform.localPosition = Vector3.zero);

                return;
            }
            
            // TODO : Drop to cell
        }
    }
}