using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Project.Gameplay
{
    public sealed class MakeupPointerInput
    {
        private readonly Camera _mainCamera;
        private readonly MakeupBookViewNew _makeupBookViewNew;
        private readonly BlushMakeupConfig _blushConfig;

        public MakeupPointerInput(
            Camera mainCamera,
            MakeupBookViewNew makeupBookViewNew,
            BlushMakeupConfig blushConfig)
        {
            _mainCamera = mainCamera;
            _makeupBookViewNew = makeupBookViewNew;
            _blushConfig = blushConfig;
        }

        public bool IsLeftMousePressedThisFrame() => 
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        public bool IsLeftMouseHeld() => Mouse.current != null && Mouse.current.leftButton.isPressed;

        public bool IsLeftMouseReleasedThisFrame() => 
            Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

        public bool IsPointerBlockedByUi() => 
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        public bool TryGetPointerWorldPosition(out Vector3 pointerWorldPosition)
        {
            if (_mainCamera == null || Mouse.current == null)
            {
                pointerWorldPosition = Vector3.zero;
                
                return false;
            }

            Vector2 pointerScreenPosition = Mouse.current.position.ReadValue();
            Vector3 screenPosition = new Vector3(
                pointerScreenPosition.x,
                pointerScreenPosition.y,
                Mathf.Abs(_mainCamera.transform.position.z));

            pointerWorldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            pointerWorldPosition.z = 0f;

            return true;
        }

        public Collider2D GetFirstColliderUnderPointer()
        {
            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return null;

            return Physics2D.OverlapPoint(pointerWorldPosition);
        }

        public Collider2D[] GetCollidersUnderPointer()
        {
            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return Array.Empty<Collider2D>();

            return Physics2D.OverlapPointAll(pointerWorldPosition);
        }

        public bool TryGetBookTab(out MakeupBookTabViewNew selectedTabView)
        {
            selectedTabView = null;

            if (_makeupBookViewNew == null)
                return false;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            return _makeupBookViewNew.TryGetTab(pointerWorldPosition, out selectedTabView);
        }

        public bool TryGetPaletteColor(out BlushPaletteColorViewNew selectedColorView, out int selectedColorIndex)
        {
            selectedColorView = null;
            selectedColorIndex = -1;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);
            BlushPaletteColorViewNew[] paletteColors = _blushConfig.PaletteColors;

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int colorIndex = 0; colorIndex < paletteColors.Length; colorIndex++)
                {
                    BlushPaletteColorViewNew colorViewNew = paletteColors[colorIndex];

                    if (colorViewNew == null)
                        continue;

                    if (colorViewNew.TapZone == currentCollider)
                    {
                        selectedColorView = colorViewNew;
                        selectedColorIndex = colorIndex;
                        
                        return true;
                    }
                }
            }

            return false;
        }
    }
}