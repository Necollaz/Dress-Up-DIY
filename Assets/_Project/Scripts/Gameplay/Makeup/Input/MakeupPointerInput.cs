using UnityEngine;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Input
{
    public sealed class MakeupPointerInput
    {
        private readonly MakeupPointerInputSource _pointerInputSource;
        private readonly MakeupPointerWorldHit _pointerWorldHit;
        private readonly MakeupPointerSelection _pointerSelection;

        private MakeupPointerSnapshot _pointerSnapshot;

        public MakeupPointerInput(
            MakeupPointerInputSource pointerInputSource,
            MakeupPointerWorldHit pointerWorldHit,
            MakeupPointerSelection pointerSelection)
        {
            _pointerInputSource = pointerInputSource;
            _pointerWorldHit = pointerWorldHit;
            _pointerSelection = pointerSelection;
        }
        
        public bool IsPointerHeld() => _pointerSnapshot.IsHeld;

        public bool IsPointerReleasedThisFrame() => _pointerSnapshot.IsReleasedThisFrame;

        public bool IsPointerPressAllowedThisFrame() =>
            _pointerSnapshot.IsPressedThisFrame &&
            _pointerSnapshot.IsPointerOverUi == false;

        public bool TryGetPointerWorldPosition(out Vector3 pointerWorldPosition)
        {
            pointerWorldPosition = _pointerSnapshot.WorldPosition;

            return _pointerSnapshot.HasWorldPosition;
        }

        public bool ContainsColliderUnderPointer(Collider2D targetCollider) =>
            _pointerWorldHit.ContainsCollider(targetCollider);

        public bool TryGetBookTab(out MakeupBookTabView selectedTabView)
        {
            return _pointerSelection.TryGetBookTab(
                _pointerWorldHit.CollidersUnderPointerBuffer,
                _pointerWorldHit.CollidersUnderPointerCount,
                out selectedTabView);
        }

        public bool TryGetPaletteColor(out BlushPaletteColorView selectedColorView, out int selectedColorIndex)
        {
            return _pointerSelection.TryGetBlushPaletteColor(
                _pointerWorldHit.CollidersUnderPointerBuffer,
                _pointerWorldHit.CollidersUnderPointerCount,
                out selectedColorView,
                out selectedColorIndex);
        }

        public bool TryGetLipstickSample(
            out LipstickPaletteColorView selectedLipstickView, 
            out int selectedLipstickIndex)
        {
            return _pointerSelection.TryGetLipstickSample(
                _pointerWorldHit.CollidersUnderPointerBuffer,
                _pointerWorldHit.CollidersUnderPointerCount,
                out selectedLipstickView,
                out selectedLipstickIndex);
        }

        public bool TryGetEyeshadowColor(out EyeshadowPaletteColorView selectedColorView, out int selectedColorIndex)
        {
            return _pointerSelection.TryGetEyeshadowColor(
                _pointerWorldHit.CollidersUnderPointerBuffer,
                _pointerWorldHit.CollidersUnderPointerCount,
                out selectedColorView,
                out selectedColorIndex);
        }
        
        public void BeginFrame()
        {
            _pointerSnapshot = _pointerInputSource.ReadSnapshot();

            if (_pointerSnapshot.HasWorldPosition == false)
            {
                _pointerWorldHit.Clear();

                return;
            }

            _pointerWorldHit.Update(_pointerSnapshot.WorldPosition);
        }
    }
}