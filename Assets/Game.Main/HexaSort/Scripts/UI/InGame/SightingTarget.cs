using System;
using Cysharp.Threading.Tasks;
using HexaSort.Core.Entities.Grid;
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

        public void ShootArrowToTarget()
        {
            Debug.Log("Arrow Shooted");
            arrow.gameObject.SetActive(true);
        }

        public void OnArrowShooted()
        {
            gameObject.SetActive(false);
            parentCell.CollectAllPieces().Forget();
            CanvasManager.Instance.loosePanel.gameObject.SetActive(false);
        }
    }
}