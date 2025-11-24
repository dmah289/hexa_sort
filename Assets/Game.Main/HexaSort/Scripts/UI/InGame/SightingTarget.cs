using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HexaSort.Core.Entities.Grid;
using HexaSort.Managers.Level;
using UnityEngine;

namespace HexaSort.UI.Loading.InGame
{
    public class SightingTarget : MonoBehaviour
    {
        [SerializeField] private RectTransform arrow;
        [SerializeField] private HexCell parentCell;

        public HexCell ParentCell
        {
            set => parentCell = value;
        }

        private void OnEnable()
        {
            arrow.gameObject.SetActive(false);
        }

        public async UniTask ShootArrowToTarget()
        {
            arrow.gameObject.SetActive(true);
            arrow.anchoredPosition = new Vector2(0, 300);
            
            await arrow.DOAnchorPos(new Vector2(0, 120), 0.2f)
                .SetDelay(0.1f)
                .SetEase(Ease.InBack);
            
            await parentCell.CollectAllPieces();
            gameObject.SetActive(false);
        }
    }
}