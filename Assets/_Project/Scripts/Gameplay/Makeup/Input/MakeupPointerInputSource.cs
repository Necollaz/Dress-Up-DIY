using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _Project.Gameplay.Makeup.Input
{
    public sealed class MakeupPointerInputSource : IDisposable
    {
        private readonly Camera _mainCamera;
        private readonly InputSystem_Actions _inputActions;

        public MakeupPointerInputSource(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
        }

        public void Dispose()
        {
            _inputActions.Player.Disable();
            _inputActions.Dispose();
        }

        public MakeupPointerSnapshot ReadSnapshot()
        {
            Vector2 screenPosition = _inputActions.Player.Point.ReadValue<Vector2>();

            bool isPressedThisFrame = _inputActions.Player.Press.WasPressedThisFrame();
            bool isHeld = _inputActions.Player.Press.IsPressed();
            bool isReleasedThisFrame = _inputActions.Player.Press.WasReleasedThisFrame();

            int pointerId = ReadPointerId();
            bool hasWorldPosition = TryConvertScreenToWorldPosition(screenPosition, out Vector3 worldPosition);
            bool isPointerOverUi = IsPointerOverUi(pointerId);

            return new MakeupPointerSnapshot(
                worldPosition,
                isPressedThisFrame,
                isHeld,
                isReleasedThisFrame,
                isPointerOverUi,
                hasWorldPosition);
        }

        private int ReadPointerId()
        {
            if (Touchscreen.current == null)
                return PointerInputModule.kMouseLeftId;

            if (Touchscreen.current.primaryTouch.press.isPressed == false)
                return PointerInputModule.kMouseLeftId;

            return _inputActions.Player.PointerId.ReadValue<int>();
        }

        private bool IsPointerOverUi(int pointerId)
        {
            if (EventSystem.current == null)
                return false;

            bool isTouchActive =
                Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.isPressed;

            if (isTouchActive)
                return EventSystem.current.IsPointerOverGameObject(pointerId);

            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool TryConvertScreenToWorldPosition(
            Vector2 screenPosition,
            out Vector3 worldPosition)
        {
            if (_mainCamera == null)
            {
                worldPosition = Vector3.zero;
                
                return false;
            }

            Vector3 screenPoint = new Vector3(
                screenPosition.x,
                screenPosition.y, 
                Mathf.Abs(_mainCamera.transform.position.z));

            worldPosition = _mainCamera.ScreenToWorldPoint(screenPoint);
            worldPosition.z = 0f;

            return true;
        }
    }
}