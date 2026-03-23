using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class BlushMakeupSequence : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private MakeupBookView _makeupBookView;

        [Header("Palette")]
        [SerializeField] private BlushPaletteColorView[] _blushPaletteColors;
        [SerializeField] private Collider2D _brushStandTapZone;
        [SerializeField] private SpriteRenderer _brushStandVisual;

        [Header("Hand")]
        [SerializeField] private Transform _handRoot;
        [SerializeField] private Transform _brushTipPoint;
        [SerializeField] private Collider2D _handDragZone;
        [SerializeField] private SpriteRenderer _brushInHandVisual;
        [SerializeField] private SpriteRenderer _brushTipRenderer;

        [Header("Points")]
        [SerializeField] private Transform _handDefaultPoint;
        [SerializeField] private Transform _brushPickupPoint;
        [SerializeField] private Transform _brushChestHoldPoint;
        [SerializeField] private Transform _faceApplyPoint;

        [Header("Face")]
        [SerializeField] private Collider2D _faceZone;
        [SerializeField] private PlayerBlushStateRenderer _playerBlushStateRenderer;

        [Header("Timing")]
        [SerializeField] private float _automaticMoveDuration = 0.35f;
        [SerializeField] private float _dragSmoothTime = 0.04f;
        [SerializeField] private float _blushApplyDuration = 0.35f;
        [SerializeField] private float _brushDipAnimationStepDuration = 0.08f;
        [SerializeField] private float _brushDipOffset = 0.12f;

        [Header("Brush Tip")]
        [SerializeField] private Color _defaultBrushTipColor = Color.white;

        private Sequence _activeSequence;

        private BlushInteractionStageType _interactionStageType;
        private Vector3 _dragVelocity;
        
        private int _selectedColorIndex = -1;
        private bool _isBlushPageOpened;

        private void Awake()
        {
            _interactionStageType = BlushInteractionStageType.Idle;

            if (_handRoot != null && _handDefaultPoint != null)
                _handRoot.position = _handDefaultPoint.position;

            _brushInHandVisual?.gameObject.SetActive(false);
            _brushStandVisual?.gameObject.SetActive(false);

            if (_brushTipRenderer != null)
                _brushTipRenderer.color = _defaultBrushTipColor;
        }

        private void OnDestroy()
        {
            _activeSequence?.Kill();
        }

        private void Update()
        {
            ProcessBookTabInput();

            switch (_interactionStageType)
            {
                case BlushInteractionStageType.WaitingForBrushSelection:
                    ProcessBrushSelectionInput();
                    break;

                case BlushInteractionStageType.WaitingForColorSelection:
                    ProcessPaletteColorInput();
                    break;

                case BlushInteractionStageType.WaitingForHandDragStart:
                    ProcessPaletteColorInput();
                    ProcessHandDragStartInput();
                    break;

                case BlushInteractionStageType.DraggingHandToFace:
                    ProcessHandDragging();
                    break;
            }
        }

        private void ProcessBookTabInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return;

            if (_makeupBookView == null)
                return;

            if (_makeupBookView.TryGetTabByWorldPoint(
                    pointerWorldPosition,
                    out MakeupBookTabView selectedBookTab) == false)
            {
                return;
            }

            if (selectedBookTab.PageType == MakeupBookPageType.Blush)
            {
                _makeupBookView.SelectPage(MakeupBookPageType.Blush);

                if (_isBlushPageOpened == false)
                    OpenBlushPage();

                return;
            }

            _makeupBookView.SelectPage(selectedBookTab.PageType);
            
            CloseBlushPageState();
        }

        private void ProcessBrushSelectionInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);

            for (int index = 0; index < collidersUnderPointer.Length; index++)
            {
                if (collidersUnderPointer[index] == _brushStandTapZone)
                {
                    TakeBrushAsync().Forget();
                    
                    return;
                }
            }
        }

        private void ProcessPaletteColorInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return;

            if (TryGetPaletteColorUnderPointer(
                    pointerWorldPosition,
                    out BlushPaletteColorView blushPaletteColor,
                    out int paletteColorIndex) == false)
            {
                return;
            }

            SelectColorAsync(blushPaletteColor, paletteColorIndex).Forget();
        }

        private void ProcessHandDragStartInput()
        {
            if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false)
                return;

            if (IsPointerBlockedByUi())
                return;

            if (TryGetPointerWorldPosition(out Vector3 pointerWorldPosition) == false)
                return;

            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(pointerWorldPosition);

            for (int index = 0; index < collidersUnderPointer.Length; index++)
            {
                if (collidersUnderPointer[index] == _handDragZone)
                {
                    _interactionStageType = BlushInteractionStageType.DraggingHandToFace;
                    _dragVelocity = Vector3.zero;
                    
                    return;
                }
            }
        }

        private void ProcessHandDragging()
        {
            if (_handRoot == null)
                return;

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
                    _interactionStageType = BlushInteractionStageType.WaitingForHandDragStart;
                    
                    return;
                }

                bool isReleasedInsideFaceZone = _faceZone != null && _faceZone.OverlapPoint(pointerWorldPosition);

                if (isReleasedInsideFaceZone)
                {
                    ApplyBlushAsync().Forget();
                    
                    return;
                }

                _interactionStageType = BlushInteractionStageType.WaitingForHandDragStart;
            }
        }

        private void OpenBlushPage()
        {
            _isBlushPageOpened = true;

            if (_interactionStageType != BlushInteractionStageType.Idle)
                return;

            _brushStandVisual?.gameObject.SetActive(true);
            _brushInHandVisual?.gameObject.SetActive(false);

            if (_brushTipRenderer != null)
                _brushTipRenderer.color = _defaultBrushTipColor;

            if (_handRoot != null && _handDefaultPoint != null)
                _handRoot.position = _handDefaultPoint.position;

            _selectedColorIndex = -1;
            _dragVelocity = Vector3.zero;
            _interactionStageType = BlushInteractionStageType.WaitingForBrushSelection;
        }

        private void CloseBlushPageState()
        {
            _activeSequence?.Kill();
            _isBlushPageOpened = false;

            _brushStandVisual?.gameObject.SetActive(false);
            _brushInHandVisual?.gameObject.SetActive(false);

            if (_brushTipRenderer != null)
                _brushTipRenderer.color = _defaultBrushTipColor;

            if (_handRoot != null && _handDefaultPoint != null)
                _handRoot.position = _handDefaultPoint.position;

            _selectedColorIndex = -1;
            _dragVelocity = Vector3.zero;
            _interactionStageType = BlushInteractionStageType.Idle;
        }

        private async UniTaskVoid TakeBrushAsync()
        {
            if (_interactionStageType != BlushInteractionStageType.WaitingForBrushSelection)
                return;

            if (_brushPickupPoint == null)
                return;

            _interactionStageType = BlushInteractionStageType.MovingHandToBrush;

            await MoveHandToAsync(_brushPickupPoint.position, _automaticMoveDuration);

            _brushStandVisual?.gameObject.SetActive(false);
            _brushInHandVisual?.gameObject.SetActive(true);

            _interactionStageType = BlushInteractionStageType.WaitingForColorSelection;
        }

        private async UniTaskVoid SelectColorAsync(BlushPaletteColorView blushPaletteColor, int paletteColorIndex)
        {
            bool canSelectColor =
                _interactionStageType == BlushInteractionStageType.WaitingForColorSelection ||
                _interactionStageType == BlushInteractionStageType.WaitingForHandDragStart;

            if (canSelectColor == false)
                return;

            if (blushPaletteColor == null || blushPaletteColor.BrushDipPoint == null)
                return;

            _activeSequence?.Kill();
            _dragVelocity = Vector3.zero;
            _interactionStageType = BlushInteractionStageType.MovingHandToColor;

            _selectedColorIndex = paletteColorIndex;

            await MoveBrushTipToAsync(blushPaletteColor.BrushDipPoint.position, _automaticMoveDuration);
            await PlayBrushDipAnimationAsync();

            ApplyBrushTipColor(blushPaletteColor);

            if (_brushChestHoldPoint == null)
            {
                _interactionStageType = BlushInteractionStageType.WaitingForHandDragStart;
                
                return;
            }

            _interactionStageType = BlushInteractionStageType.MovingHandToChestHoldPoint;

            await MoveHandToAsync(_brushChestHoldPoint.position, _automaticMoveDuration);

            _interactionStageType = BlushInteractionStageType.WaitingForHandDragStart;
        }

        private async UniTaskVoid ApplyBlushAsync()
        {
            if (_interactionStageType != BlushInteractionStageType.DraggingHandToFace)
                return;

            if (_selectedColorIndex < 0)
            {
                _interactionStageType = BlushInteractionStageType.WaitingForColorSelection;
                
                return;
            }

            if (_faceApplyPoint == null || _brushPickupPoint == null || _handDefaultPoint == null)
            {
                _interactionStageType = BlushInteractionStageType.WaitingForBrushSelection;
                
                return;
            }

            _interactionStageType = BlushInteractionStageType.ApplyingBlush;

            await MoveBrushTipToAsync(_faceApplyPoint.position, _automaticMoveDuration * 0.5f);
            await PlayBrushDipAnimationAsync();

            if (_playerBlushStateRenderer != null)
                await _playerBlushStateRenderer.ShowBlushAsync(_selectedColorIndex, _blushApplyDuration);

            _interactionStageType = BlushInteractionStageType.ReturningBrush;

            await MoveHandToAsync(_brushPickupPoint.position, _automaticMoveDuration);

            _brushInHandVisual?.gameObject.SetActive(false);
            _brushStandVisual?.gameObject.SetActive(true);

            if (_brushTipRenderer != null)
                _brushTipRenderer.color = _defaultBrushTipColor;

            await MoveHandToAsync(_handDefaultPoint.position, _automaticMoveDuration);

            _selectedColorIndex = -1;
            _interactionStageType = BlushInteractionStageType.WaitingForBrushSelection;
        }

        private async UniTask MoveBrushTipToAsync(Vector3 targetBrushTipPosition, float duration)
        {
            if (_handRoot == null || _brushTipPoint == null)
            {
                await MoveHandToAsync(targetBrushTipPosition, duration);
                
                return;
            }

            Vector3 brushTipOffset = _brushTipPoint.position - _handRoot.position;
            Vector3 targetHandRootPosition = targetBrushTipPosition - brushTipOffset;

            await MoveHandToAsync(targetHandRootPosition, duration);
        }

        private async UniTask PlayBrushDipAnimationAsync()
        {
            if (_handRoot == null)
                return;

            _activeSequence?.Kill();

            Vector3 startPosition = _handRoot.position;
            Vector3 leftPosition = startPosition + Vector3.left * _brushDipOffset;
            Vector3 rightPosition = startPosition + Vector3.right * _brushDipOffset;

            _activeSequence = DOTween.Sequence();
            _activeSequence.Append(_handRoot.DOMove(leftPosition, _brushDipAnimationStepDuration));
            _activeSequence.Append(_handRoot.DOMove(rightPosition, _brushDipAnimationStepDuration));
            _activeSequence.Append(_handRoot.DOMove(leftPosition, _brushDipAnimationStepDuration));
            _activeSequence.Append(_handRoot.DOMove(startPosition, _brushDipAnimationStepDuration));

            await _activeSequence.AsyncWaitForCompletion();
        }

        private async UniTask MoveHandToAsync(Vector3 targetPosition, float duration)
        {
            if (_handRoot == null)
                return;

            if (duration <= 0f)
            {
                _handRoot.position = targetPosition;
                
                return;
            }

            Vector3 startPosition = _handRoot.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

                _handRoot.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);

                await UniTask.Yield();
            }

            _handRoot.position = targetPosition;
        }

        private bool TryGetPaletteColorUnderPointer(
            Vector3 worldPoint,
            out BlushPaletteColorView selectedColor,
            out int selectedColorIndex)
        {
            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(worldPoint);

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int colorIndex = 0; colorIndex < _blushPaletteColors.Length; colorIndex++)
                {
                    BlushPaletteColorView blushPaletteColor = _blushPaletteColors[colorIndex];

                    if (blushPaletteColor == null)
                        continue;

                    if (blushPaletteColor.TapZone == currentCollider)
                    {
                        selectedColor = blushPaletteColor;
                        selectedColorIndex = colorIndex;
                        
                        return true;
                    }
                }
            }

            selectedColor = null;
            selectedColorIndex = -1;
            
            return false;
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

        private bool IsPointerBlockedByUi() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        
        private void ApplyBrushTipColor(BlushPaletteColorView blushPaletteColor)
        {
            if (_brushTipRenderer == null || blushPaletteColor == null)
                return;

            Color brushTipColor = blushPaletteColor.BrushTipColor;
            brushTipColor.a = 1f;
            _brushTipRenderer.color = brushTipColor;
        }
    }
}