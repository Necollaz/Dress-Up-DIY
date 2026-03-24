using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Project.Gameplay
{
    public sealed class MakeupPointerInput
    {
        private readonly Camera _mainCamera;
        private readonly MakeupBookView _makeupBookView;
        private readonly BlushMakeupConfig _blushConfig;
        private readonly LipstickMakeupConfig _lipstickConfig;
        private readonly EyeshadowMakeupConfig _eyeshadowConfig;

        public MakeupPointerInput(
            Camera mainCamera,
            MakeupBookView makeupBookView,
            BlushMakeupConfig blushConfig,
            LipstickMakeupConfig lipstickConfig,
            EyeshadowMakeupConfig eyeshadowConfig)
        {
            _mainCamera = mainCamera;
            _makeupBookView = makeupBookView;
            _blushConfig = blushConfig;
            _lipstickConfig = lipstickConfig;
            _eyeshadowConfig = eyeshadowConfig;
        }

        public bool IsLeftMousePressedThisFrame() => 
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        public bool IsLeftMouseHeld() => Mouse.current != null && Mouse.current.leftButton.isPressed;

        public bool IsLeftMouseReleasedThisFrame() => 
            Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

        public bool IsPointerBlockedByUi() => 
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

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

        public bool TryGetBookTab(out MakeupBookTabView selectedTabView)
        {
            selectedTabView = null;

            if (_makeupBookView == null)
                return false;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            return _makeupBookView.TryGetTab(pointerWorldPosition, out selectedTabView);
        }

        public bool TryGetPaletteColor(out BlushPaletteColorView selectedColorView, out int selectedColorIndex)
        {
            selectedColorView = null;
            selectedColorIndex = -1;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);
            BlushPaletteColorView[] paletteColors = _blushConfig.PaletteColors;

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int colorIndex = 0; colorIndex < paletteColors.Length; colorIndex++)
                {
                    BlushPaletteColorView colorView = paletteColors[colorIndex];

                    if (colorView == null)
                        continue;

                    if (colorView.TapZone == currentCollider)
                    {
                        selectedColorView = colorView;
                        selectedColorIndex = colorIndex;
                        
                        return true;
                    }
                }
            }

            return false;
        }
        
        public bool TryGetLipstickSample(
            out LipstickPaletteColorView selectedLipstickView,
            out int selectedLipstickIndex)
        {
            selectedLipstickView = null;
            selectedLipstickIndex = -1;

            if (_lipstickConfig == null)
                return false;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);
            LipstickPaletteColorView[] paletteColors = _lipstickConfig.PaletteColors;

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int colorIndex = 0; colorIndex < paletteColors.Length; colorIndex++)
                {
                    LipstickPaletteColorView colorView = paletteColors[colorIndex];

                    if (colorView == null)
                        continue;

                    if (colorView.TapZone == currentCollider)
                    {
                        selectedLipstickView = colorView;
                        selectedLipstickIndex = colorIndex;
                        
                        return true;
                    }
                }
            }

            return false;
        }
        
        public bool TryGetEyeshadowColor(out EyeshadowPaletteColorView selectedColorView, out int selectedColorIndex)
        {
            selectedColorView = null;
            selectedColorIndex = -1;

            if (_eyeshadowConfig == null)
                return false;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return false;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);
            EyeshadowPaletteColorView[] paletteColors = _eyeshadowConfig.PaletteColors;

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int colorIndex = 0; colorIndex < paletteColors.Length; colorIndex++)
                {
                    EyeshadowPaletteColorView colorView = paletteColors[colorIndex];

                    if (colorView == null)
                        continue;

                    if (colorView.TapZone == currentCollider)
                    {
                        selectedColorView = colorView;
                        selectedColorIndex = colorIndex;
                        
                        return true;
                    }
                }
            }

            return false;
        }
    }
}