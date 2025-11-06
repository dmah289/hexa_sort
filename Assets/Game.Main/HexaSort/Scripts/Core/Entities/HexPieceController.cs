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

        public void OnGetFromPool()
        {
            // Not all pieces are selectable when spawned
            selfTransform.localScale = Vector3.one * ConstantKey.INITIAL_PIECE_SCALE;
            Selectable = false;
        }

        public async UniTask OverturnToLocalPos(Vector3 targetLocalPos)
        {
            selfTransform.DOKill();
            Sequence sequence = DOTween.Sequence();
            
            float duration = Vector3.Distance(selfTransform.localPosition, targetLocalPos) / ConstantKey.ATTRACTION_VELOCITY;
            
            sequence.Join(transform.DOLocalJump(targetLocalPos, 1f, 1, duration)
                .SetEase(Ease.Linear));
        
            Vector3 direction = (targetLocalPos - selfTransform.localPosition).normalized;
            Vector3 rotationAxis = Vector3.Cross(Vector3.back, direction);
        
            Quaternion correctedRotation = Quaternion.Euler(selfTransform.localRotation.eulerAngles.With(z: 0));
        
            sequence.Join(DOTween.To(() => 0f, value => {
                selfTransform.localRotation = correctedRotation * Quaternion.AngleAxis(value, rotationAxis);
            }, 180f, duration).SetEase(Ease.OutFlash));
        
            // sequence.OnComplete(() => selfTransform.localRotation = Quaternion.identity)
            //     .OnKill(() => selfTransform.localRotation = Quaternion.identity);
            //
            // await UniTask.WaitUntil(() => !sequence.IsActive() || sequence.IsComplete());
            
            var tcs = new UniTaskCompletionSource();
            void SetEndState()
            {
                selfTransform.localRotation = Quaternion.identity;
                selfTransform.localPosition = targetLocalPos;
                tcs.TrySetResult();
            }
            sequence.OnComplete(SetEndState).OnKill(SetEndState);
        
            await tcs.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
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