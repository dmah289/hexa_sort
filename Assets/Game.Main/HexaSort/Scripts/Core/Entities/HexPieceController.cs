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
        public static float ScaleDuration = 0.3f;
        
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

        // public async UniTask OverturnToLocalPos(Vector3 targetLocalPos)
        // {
        //     selfTransform.DOKill();
        //     Sequence sequence = DOTween.Sequence();
        //     
        //     float duration = Vector3.Distance(selfTransform.localPosition, targetLocalPos) / ConstantKey.ATTRACTION_VELOCITY;
        //     
        //     sequence.Join(transform.DOLocalJump(targetLocalPos, 1f, 1, duration)
        //         .SetEase(Ease.Linear));
        //
        //     Vector3 direction = (targetLocalPos - selfTransform.localPosition).With(z:0).normalized;
        //     Vector3 rotationAxis = Vector3.Cross(Vector3.forward, direction);
        //
        //     Quaternion correctedRotation = Quaternion.Euler(selfTransform.localRotation.eulerAngles.With(z: 0));
        //
        //     sequence.Join(DOTween.To(() => 0f, value => {
        //         selfTransform.localRotation = correctedRotation * Quaternion.AngleAxis(value, rotationAxis);
        //     }, 180f, duration).SetEase(Ease.OutFlash));
        //
        //     // sequence.OnComplete(() => selfTransform.localRotation = Quaternion.identity)
        //     //     .OnKill(() => selfTransform.localRotation = Quaternion.identity);
        //     //
        //     // await UniTask.WaitUntil(() => !sequence.IsActive() || sequence.IsComplete());
        //     
        //     var tcs = new UniTaskCompletionSource();
        //     void SetEndState()
        //     {
        //         selfTransform.localRotation = Quaternion.identity;
        //         selfTransform.localPosition = targetLocalPos;
        //         tcs.TrySetResult();
        //     }
        //     sequence.OnComplete(SetEndState).OnKill(SetEndState);
        //
        //     await tcs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        // }

        // TODO : Using DOTS + dynamic parabola height base on max height of 2 stacks
        public void OverturnToLocalPos(Vector3 targetLocalPos, Vector3 _direction)
        {
            selfTransform.DOKill();
    
            // Force reset to identity before starting new tween to avoid accumulation
            selfTransform.localRotation = Quaternion.identity;
    
            Sequence sequence = DOTween.Sequence();

            Vector3 start = selfTransform.localPosition;
            float duration = Vector3.Distance(start, targetLocalPos) / ConstantKey.ATTRACTION_VELOCITY;
            if (duration <= 0f)
            {
                selfTransform.localPosition = targetLocalPos;
                return;
            }

            // Direction on X-Y plane
            Vector3 direction = _direction;
            if (direction.sqrMagnitude < 0.001f) direction = Vector3.right;
            direction.Normalize();

            // Rotation axis perpendicular to both movement direction and -Z
            Vector3 rotationAxis = Vector3.Cross(Vector3.back, direction).normalized;

            float jumpHeight = 1.8f;

            // Position tween with parabola on -Z
            sequence.Join(DOTween.To(() => 0f, t =>
            {
                Vector3 basePos = Vector3.Lerp(start, targetLocalPos, t);
                float parabola = 4f * jumpHeight * t * (1f - t);
                selfTransform.localPosition = basePos + Vector3.back * parabola;
            }, 1f, duration).SetEase(Ease.Linear));

            // Rotation tween: clean 0->180 flip around computed axis
            sequence.Join(DOTween.To(() => 0f, angle =>
            {
                selfTransform.localRotation = Quaternion.AngleAxis(angle, rotationAxis);
            }, 180f, duration).SetEase(Ease.Linear));

            // var tcs = new UniTaskCompletionSource();
            void SetEndState()
            {
                selfTransform.localPosition = targetLocalPos;
                selfTransform.localRotation = Quaternion.identity;
                // tcs.TrySetResult();
            }
            sequence.OnComplete(SetEndState).OnKill(SetEndState);
            sequence.Play();

            // await tcs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }

        
        public void OnCollected()
        {
            selfTransform.DOKill();
            selfTransform.DOScale(Vector3.zero, ScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => ObjectPooler.ReturnToPool(PoolingType.HexPiece, this, this.GetCancellationTokenOnDestroy()));
        }
    }
}