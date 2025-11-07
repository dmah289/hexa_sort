using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities.Piece
{
    public enum ColorType : byte
    {
        Cyan,
        Black,
        Orange,
        Green,
        Pink,
        Blue,
        Purple,
        Red,
        Yellow,
        White
    }
    
    public class HexPieceController : MonoBehaviour, IPoolableObject
    {
        public static float ScaleDuration = 0.25f;
        
        [Header("Self Components")]
        [SerializeField] private Collider selfCollider;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        public Transform selfTransform;
        
        [Header("Config")]
        [SerializeField] private ColorType colorType;

        public bool Selectable
        {
            get => selfCollider.enabled;
            set => selfCollider.enabled = value;
        }

        public ColorType ColorType
        {
            get => colorType;
            set
            {
                colorType = value;
                selfMeshRenderer.SetOffsetTexture(value);
            }
        }

        #region Object Pooling Callbacks

        public void OnGetFromPool()
        {
            // Not all pieces are selectable when spawned
            selfTransform.localScale = Vector3.one * ConstantKey.INITIAL_PIECE_SCALE;
            Selectable = false;
        }

        public void OnReturnToPool() { }

        #endregion

        // TODO : Using DOTS + dynamic parabola height base on max height of 2 stacks
        public void OverturnToLocalPos(Vector3 targetLocalPos, Vector3 overturnDirection)
        {
            selfTransform.DOKill();
    
            Sequence sequence = DOTween.Sequence();

            Vector3 start = selfTransform.localPosition;
            float duration = Vector3.Distance(start, targetLocalPos) / ConstantKey.ATTRACTION_VELOCITY;
            if (duration <= 0f)
            {
                selfTransform.localPosition = targetLocalPos;
                return;
            }

            Vector3 rotationAxis = Vector3.Cross(Vector3.back, overturnDirection).normalized;

            float jumpHeight = 2.5f;

            sequence.Join(DOTween.To(() => 0f, t =>
            {
                Vector3 basePos = Vector3.Lerp(start, targetLocalPos, t);
                
                // Parabola formula: 4h * t * (1 - t) - max at height h when t = 0.5
                float parabola = 4f * jumpHeight * t * (1f - t);
                selfTransform.localPosition = basePos + Vector3.back * parabola;
            }, 1f, duration).SetEase(Ease.Linear));

            sequence.Join(DOTween.To(() => 0f, angle =>
            {
                selfTransform.localRotation = Quaternion.AngleAxis(angle, rotationAxis);
            }, 180f, duration).SetEase(Ease.Linear));

            void SetEndState()
            {
                selfTransform.localPosition = targetLocalPos;
                selfTransform.localRotation = Quaternion.identity;
            }
            sequence.OnComplete(SetEndState).OnKill(SetEndState);
        }

        
        public void OnCollected()
        {
            selfTransform.DOKill();
            selfTransform.DOScale(Vector3.one * 0.1f, ScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => ObjectPooler.ReturnToPool(PoolingType.HexPiece, this, this.GetCancellationTokenOnDestroy()));
        }
    }
}