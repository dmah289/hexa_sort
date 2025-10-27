using System;
using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace HexaSort.Scripts.Core.Mechanics
{
    public class SelectionController : MonoSingleton<SelectionController>
    {
        [Header("Managers")]
        [SerializeField] private HexStackController currStack;
        
        [Header("References")]
        [SerializeField] private Camera gameplayCam;
        
        [Header("Raycast Picking Config")]
        [SerializeField] private LayerMask pieceLayer;
        private RaycastHit[] pickingHits = new RaycastHit[1];
        [SerializeField] private HexCellController currTargetCell;
        
        [Header("Raycast Dragging Config")]
        private Ray catchingRay;
        [SerializeField] private LayerMask cellLayer;
        private RaycastHit[] cellHits = new RaycastHit[1];
        public Color selectedCellColor, normalCellColor;

        protected override void Awake()
        {
            base.Awake();
            
            catchingRay = new Ray(Vector3.zero, gameplayCam.transform.forward.normalized);
            // Debug.DrawRay(catchingRay.origin, catchingRay.direction, Color.red, 100f);
        }

        private void Update()
        {
#if UNITY_EDITOR
            HandleEditorSelection();
#else
            HandleMobileSelection();
#endif
        }

        private void HandleEditorSelection()
        {
            if(Input.GetMouseButtonDown(0))
                HandlePicking();
            else if(Input.GetMouseButton(0))
                HandleDragging();
            else if(Input.GetMouseButtonUp(0))
                HandleDropping();
        }

        private void HandleMobileSelection()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        HandlePicking();
                        break;
                    
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        HandleDragging();
                        break;
                    
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        HandleDropping();
                        break;
                }
            }
        }
        
        private void HandlePicking()
        {
            Vector3 screenTouchPos = Input.touchCount > 0 
                ? Input.GetTouch(0).position
                : Input.mousePosition;
            
            Ray ray = gameplayCam.ScreenPointToRay(screenTouchPos);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            int hitCount = Physics.RaycastNonAlloc(ray, pickingHits, 100f, pieceLayer);
            if (hitCount > 0)
            {
                currStack = pickingHits[0].transform.GetComponentInParent<HexStackController>();
                catchingRay.origin = currStack.transform.position;
            }
        }

        private Vector3 GetDraggingMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            // Avoid z-fitting issue on ScreenToWorldPoint
            mousePos.z = gameplayCam.WorldToScreenPoint(currStack.transform.position).z + 1.2f;
            return gameplayCam.ScreenToWorldPoint(mousePos).With(z: ConstantKey.STACK_LIFTING_OFFSET_Z);
        }
        
        private void HandleDragging()
        {
            if (currStack == null) return;
            
            Vector3 targetPos = GetDraggingMouseWorldPos();
            currStack.OnDragged(targetPos);

            HandleCatchingCellOnDragging();
        }

        private void HandleCatchingCellOnDragging()
        {
            catchingRay.origin = currStack.selfTransform.position;
            Debug.DrawRay(catchingRay.origin, catchingRay.direction * 500, Color.red);

            int hitCount = Physics.RaycastNonAlloc(catchingRay, cellHits, 500, cellLayer);
            if (hitCount > 0)
            {
                HexCellController newTargetCell = cellHits[0].collider.GetComponentInParent<HexCellController>();
                if (currTargetCell != newTargetCell)
                {
                    currTargetCell?.SetSelectedState(normalCellColor);
                    currTargetCell = newTargetCell;
                    currTargetCell.SetSelectedState(selectedCellColor);
                }
            }
            else
            {
                if (currTargetCell)
                {
                    currTargetCell.SetSelectedState(normalCellColor);
                    currTargetCell = null;
                }
            }
        }

        private void HandleDropping()
        {
            if (currStack == null) return;

            currTargetCell?.SetSelectedState(normalCellColor);
            currStack?.OnDropped(currTargetCell);
        }
    }
}