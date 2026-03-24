using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class EyeshadowMakeupFlow
    {
        private const float HALF_MOVE_DURATION_MULTIPLIER = 0.5f;

        private readonly EyeshadowMakeupConfig _eyeshadowConfig;
        private readonly MakeupMotionConfig _motionConfig;
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupVisualState _visualState;
        private readonly MakeupHandMotion _handMotion;

        public EyeshadowMakeupFlow(
            EyeshadowMakeupConfig eyeshadowConfig,
            MakeupMotionConfig motionConfig,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            MakeupHandMotion handMotion)
        {
            _eyeshadowConfig = eyeshadowConfig;
            _motionConfig = motionConfig;
            _playerFaceStateView = playerFaceStateView;
            _runtimeState = runtimeState;
            _visualState = visualState;
            _handMotion = handMotion;
        }

        public async UniTask TakeBrushAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.WaitingForEyeshadowBrushSelection)
                return;

            if (_eyeshadowConfig.BrushPickupPoint == null)
                return;

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingHandToEyeshadowBrush;

            await _handMotion.MoveHandToAsync(
                _eyeshadowConfig.BrushPickupPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _visualState.SetEyeshadowBrushInHandVisible(true);
            _visualState.SetEyeshadowBrushStandVisible(false);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowColorSelection;
        }

        public async UniTask SelectEyeshadowColorAsync(EyeshadowPaletteColorView colorView, int colorIndex)
        {
            bool canSelectColor =
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForEyeshadowColorSelection ||
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForEyeshadowBrushDragStart;

            if (canSelectColor == false || colorView == null || colorView.BrushDipPoint == null)
                return;

            _runtimeState.ActiveSequence?.Kill();
            _runtimeState.DragVelocity = Vector3.zero;
            _runtimeState.SelectedEyeshadowColorIndex = colorIndex;
            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingEyeshadowBrushToColor;

            await _handMotion.MoveEyeshadowBrushTipToAsync(
                colorView.BrushDipPoint.position,
                _motionConfig.AutomaticMoveDuration);

            await _handMotion.PlayBrushDipAnimationAsync();

            _visualState.ApplyEyeshadowBrushTipColor(colorView);

            if (_eyeshadowConfig.BrushChestHoldPoint == null)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowBrushDragStart;
                
                return;
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingEyeshadowBrushToChestHoldPoint;

            await _handMotion.MoveHandToAsync(
                _eyeshadowConfig.BrushChestHoldPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowBrushDragStart;
        }

        public async UniTask ApplyEyeshadowAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.DraggingEyeshadowBrushToFace)
                return;

            if (_runtimeState.SelectedEyeshadowColorIndex < 0)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowColorSelection;
                
                return;
            }

            if (_eyeshadowConfig.LeftEyeApplyPoint == null ||
                _eyeshadowConfig.RightEyeApplyPoint == null ||
                _eyeshadowConfig.BrushPickupPoint == null)
            {
                return;
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.ApplyingEyeshadow;

            await _handMotion.MoveEyeshadowBrushTipToAsync(
                _eyeshadowConfig.LeftEyeApplyPoint.position,
                _motionConfig.AutomaticMoveDuration * HALF_MOVE_DURATION_MULTIPLIER);
            await _handMotion.PlayBrushDipAnimationAsync();

            if (_playerFaceStateView != null)
            {
                await _playerFaceStateView.ShowLeftEyeshadowAsync(
                    _runtimeState.SelectedEyeshadowColorIndex,
                    _motionConfig.ApplyDuration);
            }

            await _handMotion.MoveEyeshadowBrushTipToAsync(
                _eyeshadowConfig.RightEyeApplyPoint.position,
                _motionConfig.AutomaticMoveDuration * HALF_MOVE_DURATION_MULTIPLIER);
            await _handMotion.PlayBrushDipAnimationAsync();

            if (_playerFaceStateView != null)
            {
                await _playerFaceStateView.ShowRightEyeshadowAsync(
                    _runtimeState.SelectedEyeshadowColorIndex,
                    _motionConfig.ApplyDuration);
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningEyeshadowBrush;

            await _handMotion.MoveHandToAsync(
                _eyeshadowConfig.BrushPickupPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _visualState.SetEyeshadowBrushInHandVisible(false);
            _visualState.ResetEyeshadowBrushTipColor();
            _visualState.SetEyeshadowBrushStandVisible(true);

            await _handMotion.MoveHandToDefaultPointAsync();

            _runtimeState.SelectedEyeshadowColorIndex = -1;
            _runtimeState.DragVelocity = Vector3.zero;
            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowBrushSelection;
        }
    }
}