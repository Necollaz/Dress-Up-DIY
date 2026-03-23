using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class MakeupGameplay : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private MakeupBookViewNew _makeupBookViewNew;
        [SerializeField] private PlayerFaceStateView _playerFaceStateView;

        [Header("Configs")]
        [SerializeField] private MakeupHandConfig _handConfig;
        [SerializeField] private MakeupMotionConfig _motionConfig;
        [SerializeField] private CreamMakeupConfig _creamConfig;
        [SerializeField] private BlushMakeupConfig _blushConfig;

        private MakeupRuntimeState _runtimeState;
        private MakeupPointerInput _pointerInput;
        private MakeupVisualState _visualState;
        private MakeupHandMotion _handMotion;
        private CreamMakeupFlow _creamMakeupFlow;
        private BlushMakeupFlow _blushMakeupFlow;

        private void Awake()
        {
            _runtimeState = new MakeupRuntimeState();
            _pointerInput = new MakeupPointerInput(_mainCamera, _makeupBookViewNew, _blushConfig);
            _visualState = new MakeupVisualState(_handConfig, _creamConfig, _blushConfig, _runtimeState);
            _handMotion = new MakeupHandMotion(_handConfig, _motionConfig, _runtimeState);
            _creamMakeupFlow = new CreamMakeupFlow(
                _creamConfig,
                _motionConfig,
                _playerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotion);
            _blushMakeupFlow = new BlushMakeupFlow(
                _blushConfig,
                _motionConfig,
                _playerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotion);

            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
            _runtimeState.IsBlushPageOpened = false;

            _visualState.ResetActiveToolState();
            _playerFaceStateView?.ResetFaceState();
        }

        private void OnDestroy()
        {
            _runtimeState.ActiveSequence?.Kill();
        }

        private void Update()
        {
            ProcessBookTabInput();
            ProcessCreamSelectionInput();

            switch (_runtimeState.ProcessStageType)
            {
                case MakeupProcessStageType.WaitingForCreamDragStart:
                    ProcessCreamDragStartInput();
                    break;

                case MakeupProcessStageType.DraggingCreamToFace:
                    ProcessHandDragging(
                        _creamConfig.FaceZone,
                        MakeupProcessStageType.WaitingForCreamDragStart,
                        _creamMakeupFlow.ApplyCreamAsync).Forget();
                    break;

                case MakeupProcessStageType.WaitingForBrushSelection:
                    ProcessBrushSelectionInput();
                    break;

                case MakeupProcessStageType.WaitingForBlushColorSelection:
                    ProcessBlushPaletteColorInput();
                    break;

                case MakeupProcessStageType.WaitingForBrushDragStart:
                    ProcessBlushPaletteColorInput();
                    ProcessBrushDragStartInput();
                    break;

                case MakeupProcessStageType.DraggingBrushToFace:
                    ProcessHandDragging(
                        _blushConfig.FaceZone,
                        MakeupProcessStageType.WaitingForBrushDragStart,
                        _blushMakeupFlow.ApplyBlushAsync).Forget();
                    break;
            }
        }

        private void ProcessBookTabInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false || _pointerInput.IsPointerBlockedByUi())
                return;

            if (_pointerInput.TryGetBookTab(out MakeupBookTabViewNew selectedTabView) == false)
                return;

            if (selectedTabView.PageType == MakeupBookPageType.Blush)
            {
                _makeupBookViewNew.SelectPage(MakeupBookPageType.Blush);

                if (_runtimeState.IsBlushPageOpened == false)
                    OpenBlushPageAsync().Forget();

                return;
            }

            _makeupBookViewNew.SelectPage(selectedTabView.PageType);
            
            CloseBlushPageAsync().Forget();
        }

        private void ProcessCreamSelectionInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false ||
                _pointerInput.IsPointerBlockedByUi() ||
                _creamMakeupFlow.CanSwitchToCream() == false)
            {
                return;
            }

            if (_pointerInput.GetFirstColliderUnderPointer() != _creamConfig.CreamTapZone)
                return;

            SwitchToCreamAsync().Forget();
        }

        private void ProcessCreamDragStartInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false || _pointerInput.IsPointerBlockedByUi())
                return;

            if (_pointerInput.GetFirstColliderUnderPointer() == _handConfig.HandDragZone)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.DraggingCreamToFace;
                _runtimeState.DragVelocity = Vector3.zero;
            }
        }

        private void ProcessBrushSelectionInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false || _pointerInput.IsPointerBlockedByUi())
                return;

            Collider2D[] collidersUnderPointer = _pointerInput.GetCollidersUnderPointer();

            for (int index = 0; index < collidersUnderPointer.Length; index++)
            {
                if (collidersUnderPointer[index] == _blushConfig.BrushStandTapZone)
                {
                    _blushMakeupFlow.TakeBrushAsync().Forget();
                    
                    return;
                }
            }
        }

        private void ProcessBlushPaletteColorInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false || _pointerInput.IsPointerBlockedByUi())
                return;

            if (_pointerInput.TryGetPaletteColor(
                    out BlushPaletteColorViewNew selectedColorView,
                    out int selectedColorIndex))
            {
                _blushMakeupFlow.SelectBlushColorAsync(selectedColorView, selectedColorIndex).Forget();
            }
        }

        private void ProcessBrushDragStartInput()
        {
            if (_pointerInput.IsLeftMousePressedThisFrame() == false || _pointerInput.IsPointerBlockedByUi())
                return;

            Collider2D[] collidersUnderPointer = _pointerInput.GetCollidersUnderPointer();

            for (int index = 0; index < collidersUnderPointer.Length; index++)
            {
                if (collidersUnderPointer[index] == _handConfig.HandDragZone)
                {
                    _runtimeState.ProcessStageType = MakeupProcessStageType.DraggingBrushToFace;
                    _runtimeState.DragVelocity = Vector3.zero;
                    
                    return;
                }
            }
        }

        private async UniTaskVoid OpenBlushPageAsync()
        {
            _runtimeState.IsBlushPageOpened = true;
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            await ReturnActiveToolToStandAsync();

            _visualState.SetBrushStandVisible(true);
            
            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBrushSelection;
        }

        private async UniTaskVoid CloseBlushPageAsync()
        {
            _runtimeState.IsBlushPageOpened = false;
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            await ReturnActiveToolToStandAsync();

            _visualState.SetBrushStandVisible(false);
            
            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
        }

        private async UniTaskVoid SwitchToCreamAsync()
        {
            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningToolBeforeSwitch;

            if (_runtimeState.IsBlushPageOpened)
            {
                _makeupBookViewNew?.SelectPage(MakeupBookPageType.None);
                
                _runtimeState.IsBlushPageOpened = false;
            }

            _visualState.SetBrushStandVisible(false);
            _visualState.ResetBrushTipColor();

            await ReturnActiveToolToStandAsync();
            await _creamMakeupFlow.StartCreamSequenceAsync();
        }

        private async UniTask ReturnActiveToolToStandAsync()
        {
            _runtimeState.ActiveSequence?.Kill();

            if (_visualState.IsCreamInHandVisible())
            {
                if (_creamConfig.CreamPickupPoint != null)
                {
                    await _handMotion.MoveHandToAsync(
                        _creamConfig.CreamPickupPoint.position,
                        _motionConfig.AutomaticMoveDuration);
                }

                _visualState.SetCreamInHandVisible(false);
                _visualState.SetCreamStandVisible(true);
                
                await _handMotion.MoveHandToDefaultPointAsync();
            }
            else if (_visualState.IsBrushInHandVisible())
            {
                if (_blushConfig.BrushPickupPoint != null)
                {
                    await _handMotion.MoveHandToAsync(
                        _blushConfig.BrushPickupPoint.position,
                        _motionConfig.AutomaticMoveDuration);
                }

                _visualState.SetBrushInHandVisible(false);
                _visualState.ResetBrushTipColor();
                _visualState.SetBrushStandVisible(_runtimeState.IsBlushPageOpened);
                
                await _handMotion.MoveHandToDefaultPointAsync();
            }
            else
            {
                _visualState.SetCreamInHandVisible(false);
                _visualState.SetCreamStandVisible(true);
                _visualState.SetBrushInHandVisible(false);
                _visualState.SetBrushStandVisible(false);
                _visualState.ResetBrushTipColor();
                _visualState.MoveHandToDefaultPointImmediately();
            }

            _runtimeState.SelectedBlushColorIndex = -1;
            _runtimeState.DragVelocity = Vector3.zero;
        }

        private async UniTaskVoid ProcessHandDragging(
            Collider2D faceZone,
            MakeupProcessStageType fallbackStageType,
            Func<UniTask> applyAction)
        {
            if (_pointerInput.IsLeftMouseHeld())
            {
                if (_pointerInput.TryGetPointerWorldPosition(out Vector3 pointerWorldPosition))
                    _handMotion.UpdateDraggedHandPosition(pointerWorldPosition);
            }

            if (_pointerInput.IsLeftMouseReleasedThisFrame() == false)
                return;

            if (_pointerInput.TryGetPointerWorldPosition(out Vector3 releaseWorldPosition) == false)
            {
                _runtimeState.ProcessStageType = fallbackStageType;
                
                return;
            }

            bool isReleasedInsideFaceZone = faceZone != null && faceZone.OverlapPoint(releaseWorldPosition);

            if (isReleasedInsideFaceZone)
            {
                await applyAction();
                
                return;
            }

            _runtimeState.ProcessStageType = fallbackStageType;
        }
    }
}