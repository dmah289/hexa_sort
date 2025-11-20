using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Core.Entities.Grid;
using HexaSort.UI.Gameplay.Goals;
using LevelEditor.LevelData;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace HexaSort.UI.BaseSystem
{
    public class VFXManager : MonoSingleton<VFXManager>
    {
        [Header("Goal VFX")]
        [SerializeField] private Camera mainCam;
        [SerializeField] private RectTransform[] goalVFXTargetPanels;
        
        public async UniTask PlayVFXToPieceGoalPanel(HexCell cell, 
            eLevelGoalType goalType,
            int amount,
            CancellationToken destroyCancellationToken)
        {
            RectTransform starTrail = await ObjectPooler.GetFromPool<RectTransform>
                (PoolingType.StarTrail, destroyCancellationToken, goalVFXTargetPanels[(int)goalType]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(goalVFXTargetPanels[(int)goalType],
                mainCam.WorldToScreenPoint(cell.selfTransform.position),
                null,
                out Vector2 localPos);
            starTrail.anchoredPosition = localPos;

            GoalCollectedDTO piecesCollectedDTO = new GoalCollectedDTO(goalType, amount, cell.GridPos);
                
            for (int i = 0; i < starTrail.childCount; i++)
            {
                Image image = starTrail.GetChild(i).GetComponent<Image>();
                if (image != null)
                    image.enabled = i == (int)piecesCollectedDTO.GoalType;
            }

            await UniTask.Delay(155);
                
            starTrail.DOAnchorPos(Vector2.zero, 0.7f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                ObjectPooler.ReturnToPool(PoolingType.StarTrail, starTrail, destroyCancellationToken);
                EventBus<GoalCollectedDTO>.Raise(piecesCollectedDTO);
            });
        }
    }
}