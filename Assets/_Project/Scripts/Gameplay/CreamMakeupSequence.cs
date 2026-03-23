using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay
{
    public sealed class CreamMakeupSequence : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;

        [Header("Hand")]
        [SerializeField] private Transform _handRoot;
        [SerializeField] private Collider2D _handDragZone;
        [SerializeField] private GameObject _creamInHandVisual;

        [Header("Cream")]
        [SerializeField] private Collider2D _creamTapZone;
        [SerializeField] private GameObject _creamStandVisual;

        [Header("Face")]
        [SerializeField] private Collider2D _faceZone;
        [SerializeField] private PlayerFaceStateRenderer _playerFaceStateRenderer;

        [Header("Points")]
        [SerializeField] private Transform _handDefaultPoint;
        [SerializeField] private Transform _creamPickupPoint;
        [SerializeField] private Transform _creamDragStartPoint;
        [SerializeField] private Transform _faceApplyPoint;

        [Header("Timing")]
        [SerializeField] private float _automaticMoveDuration = 0.35f;
        [SerializeField] private float _dragSmoothTime = 0.04f;
        [SerializeField] private float _creamApplyDuration = 0.35f;
        [SerializeField] private float _pauseAfterApplyDuration = 0.1f;

        [Header("Curves")]
        [SerializeField] private AnimationCurve _automaticMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private MakeupInteractionStageType _interactionStageType;
        private Vector3 _dragVelocity;

        private void Awake()
        {
            _interactionStageType = MakeupInteractionStageType.Idle;

            if (_handRoot != null && _handDefaultPoint != null)
                _handRoot.position = _handDefaultPoint.position;

            _creamInHandVisual?.SetActive(false);
            _creamStandVisual?.SetActive(true);
            _playerFaceStateRenderer?.ResetState();
        }

        private void Update()
        {
            switch (_interactionStageType)
            {
                case MakeupInteractionStageType.Idle:
                    ProcessCreamSelectionInput();
                    break;

                case MakeupInteractionStageType.WaitingForHandDragStart:
                    ProcessHandDragStartInput();
                    break;

                case MakeupInteractionStageType.DraggingHandToFace:
                    ProcessHandDragging();
                    break;
            }
        }

        private void ProcessCreamSelectionInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            Collider2D pointerCollider = GetColliderUnderPointer();

            if (pointerCollider == _creamTapZone)
                StartCreamSequenceAsync().Forget();
        }

        private void ProcessHandDragStartInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            Collider2D pointerCollider = GetColliderUnderPointer();

            if (pointerCollider == _handDragZone)
            {
                _interactionStageType = MakeupInteractionStageType.DraggingHandToFace;
                _dragVelocity = Vector3.zero;
            }
        }

        private void ProcessHandDragging()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                    return;

                Vector3 targetPosition = new Vector3(
                    pointerWorldPosition.x, 
                    pointerWorldPosition.y, 
                    _handRoot.position.z);

                _handRoot.position = Vector3.SmoothDamp(
                    _handRoot.position,
                    targetPosition, 
                    ref _dragVelocity,
                    _dragSmoothTime);
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                {
                    _interactionStageType = MakeupInteractionStageType.WaitingForHandDragStart;
                    
                    return;
                }

                bool isReleasedInsideFaceZone = _faceZone != null && _faceZone.OverlapPoint(pointerWorldPosition);

                if (isReleasedInsideFaceZone)
                {
                    ApplyCreamAsync().Forget();
                    
                    return;
                }

                _interactionStageType = MakeupInteractionStageType.WaitingForHandDragStart;
            }
        }

        private async UniTaskVoid StartCreamSequenceAsync()
        {
            if (_interactionStageType != MakeupInteractionStageType.Idle)
                return;

            _interactionStageType = MakeupInteractionStageType.MovingHandToCream;

            await MoveHandToAsync(_creamPickupPoint.position, _automaticMoveDuration);

            _creamStandVisual?.SetActive(false);
            _creamInHandVisual?.SetActive(true);

            _interactionStageType = MakeupInteractionStageType.MovingHandToCreamDragStartPoint;

            await MoveHandToAsync(_creamDragStartPoint.position, _automaticMoveDuration);

            _interactionStageType = MakeupInteractionStageType.WaitingForHandDragStart;
        }

        private async UniTaskVoid ApplyCreamAsync()
        {
            if (_interactionStageType != MakeupInteractionStageType.DraggingHandToFace)
                return;

            _interactionStageType = MakeupInteractionStageType.ApplyingCream;

            await MoveHandToAsync(_faceApplyPoint.position, _automaticMoveDuration * 0.5f);

            if (_playerFaceStateRenderer != null)
                await _playerFaceStateRenderer.HideAcneAsync(_creamApplyDuration);

            await UniTask.Delay((int)(_pauseAfterApplyDuration * 1000f));

            _interactionStageType = MakeupInteractionStageType.ReturningHandWithCream;

            await MoveHandToAsync(_creamPickupPoint.position, _automaticMoveDuration);

            _creamInHandVisual?.SetActive(false);
            _creamStandVisual?.SetActive(true);

            await MoveHandToAsync(_handDefaultPoint.position, _automaticMoveDuration);

            _interactionStageType = MakeupInteractionStageType.Idle;
        }

        private async UniTask MoveHandToAsync(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = _handRoot.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float easedProgress = _automaticMoveCurve.Evaluate(progress);

                _handRoot.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedProgress);

                await UniTask.Yield();
            }

            _handRoot.position = targetPosition;
        }

        private Collider2D GetColliderUnderPointer()
        {
            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return null;

            return Physics2D.OverlapPoint(pointerWorldPosition);
        }

        private bool TryGetPointerWorldPosition(out Vector3 pointerWorldPosition)
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

        private static bool IsPointerBlockedByUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}