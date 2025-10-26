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
        
        [Header("Raycast Config")]
        [SerializeField] private LayerMask pieceLayer;
        private RaycastHit[] hits = new RaycastHit[1];
        [SerializeField] private HexCellController targetCell;

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
            int hitCount = Physics.RaycastNonAlloc(ray, hits, 100f, pieceLayer);
            if (hitCount > 0)
            {
                currStack = hits[0].transform.GetComponentInParent<HexStackController>();
            }
        }

        // TODO : Move to SDK + Optimize this method to avoid ScreenToWorldPoint call every frame
        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            // Avoid z-fitting issue on ScreenToWorldPoint
            mousePos.z = gameplayCam.WorldToScreenPoint(currStack.transform.position).z + 1.2f;
            return gameplayCam.ScreenToWorldPoint(mousePos).With(z: ConstantKey.STACK_LIFTING_OFFSET_Z);
        }
        
        private void HandleDragging()
        {
            if (currStack == null) return;
            
            Vector3 targetPos = GetMouseWorldPos();
            currStack.OnDragged(targetPos);
            
            // TODO : Use Raycast to catch target cell
        }

        private void HandleDropping()
        {
            if (currStack == null) return;

            currStack?.OnDropped(targetCell);
        }
    }
}