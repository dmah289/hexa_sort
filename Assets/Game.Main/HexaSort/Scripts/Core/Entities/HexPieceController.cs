using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Audio;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using UnityEngine;

namespace HexaSort.Core.Entities.Grid.Piece
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
        White,
        None
    }
    
    // Cache animation state
    public struct OverturnAnimationState
    {
        public Vector3 startPos;
        public Vector3 targetPos;
        public Vector3 rotationAxis;
        public float jumpHeight;
        public float elapsedTime;
        public float moveDuration;
        public float rotationDuration;
        public float rotationDelay;
    }
    
    public class HexPieceController : MonoBehaviour, IPoolableObject
    {
        public static float ScaleDuration = 0.2f;
        private const float JumpOffset = 1f;
        public const float OverturnDuration = 0.7f;
        
        [Header("Self Components")]
        [SerializeField] private Collider selfCollider;
        [SerializeField] private MeshRenderer selfMeshRenderer;
        public Transform selfTransform;
        
        [Header("Config")]
        [SerializeField] private ColorType colorType;
        
        [Header("----- Anim State -----")]
        [SerializeField] private bool _isOverturnAnimating;
        private OverturnAnimationState _overturnState;

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

            _isOverturnAnimating = false;
        }

        public void OnReturnToPool()
        {
            _isOverturnAnimating = false;
        }

        #endregion
        
        private static float EaseOutFlash(float t)
        {
            // Tương tự Ease.OutFlash của DOTween
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }
        
        public void OverturnToLocalPos(Vector3 targetLocalPos, Vector3 overturnDirection, float maxHeight)
        {
            _overturnState.startPos = selfTransform.localPosition;
            _overturnState.targetPos = targetLocalPos;
            _overturnState.rotationAxis = Vector3.Cross(Vector3.back, overturnDirection).normalized;
            _overturnState.jumpHeight = maxHeight + JumpOffset;
            _overturnState.elapsedTime = 0f;
            _overturnState.moveDuration = OverturnDuration;
            _overturnState.rotationDuration = 0.9f * OverturnDuration;
            _overturnState.rotationDelay = 0.1f * OverturnDuration;

            _isOverturnAnimating = true;
        }
        
        private void Update()
        {
            if (!_isOverturnAnimating) return;

            OverturnAnimationState state = _overturnState;
            state.elapsedTime += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(state.elapsedTime / state.moveDuration);
            float easedT = EaseOutFlash(normalizedTime);

            // Optimized interpolation: manual per-component lerp to avoid extra Mathf calls inside Vector3.Lerp
            // Cache locals to reduce struct field access
            Vector3 start = state.startPos;
            Vector3 target = state.targetPos;
            Vector3 delta = target - start;
            Vector3 currentPos;
            currentPos.x = start.x + delta.x * easedT;
            currentPos.y = start.y + delta.y * easedT;
            currentPos.z = start.z + delta.z * easedT;

            float parabola = 4f * state.jumpHeight * easedT * (1f - easedT);
            currentPos += Vector3.back * parabola;
            selfTransform.localPosition = currentPos;

            if (state.elapsedTime > state.rotationDelay)
            {
                float rotationNormalizedTime = Mathf.Clamp01((state.elapsedTime - state.rotationDelay) / state.rotationDuration);
                float angle = EaseOutFlash(rotationNormalizedTime) * 180f;
                Quaternion currentRot = Quaternion.AngleAxis(angle, state.rotationAxis);
                selfTransform.localRotation = currentRot;
            }

            if (state.elapsedTime >= state.moveDuration)
            {
                selfTransform.localPosition = state.targetPos;
                selfTransform.localRotation = Quaternion.identity;
                _isOverturnAnimating = false;
            }

            _overturnState = state;
        }
        
        public void OnCollected()
        {
            _isOverturnAnimating = false;
            
            AudioManager.Instance.PlaySfx(ConstantKey.SFX_PIECE_COLLECTED);
            selfTransform.DOKill();
            selfTransform.DOScale(Vector3.one * 0.2f, ScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => ObjectPooler.ReturnToPool(PoolingType.HexPiece, this, this.GetCancellationTokenOnDestroy()));
        }
    }
}

