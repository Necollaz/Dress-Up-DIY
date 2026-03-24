using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class BlushMakeupFlow
    {
        private const float HALF_MOVE_DURATION_MULTIPLIER = 0.5f;

        private readonly BlushMakeupConfig _blushConfig;
        private readonly MakeupMotionConfig _motionConfig;
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupVisualState _visualState;
        private readonly MakeupHandMotion _handMotion;

        public BlushMakeupFlow(
            BlushMakeupConfig blushConfig,
            MakeupMotionConfig motionConfig,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            MakeupHandMotion handMotion)
        {
            _blushConfig = blushConfig;
            _motionConfig = motionConfig;
            _playerFaceStateView = playerFaceStateView;
            _runtimeState = runtimeState;
            _visualState = visualState;
            _handMotion = handMotion;
        }

        public async UniTask TakeBrushAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.WaitingForBrushSelection)
                return;

            if (_blushConfig.BrushPickupPoint == null)
                return;

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingHandToBrush;

            await _handMotion.MoveHandToAsync(
                _blushConfig.BrushPickupPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _visualState.SetBrushStandVisible(false);
            _visualState.SetBrushInHandVisible(true);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
        }

        public async UniTask SelectBlushColorAsync(BlushPaletteColorView colorView, int colorIndex)
        {
            bool canSelectColor =
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForBlushColorSelection ||
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForBrushDragStart;

            if (canSelectColor == false || colorView == null || colorView.BrushDipPoint == null)
                return;

            _runtimeState.ActiveSequence?.Kill();
            _runtimeState.DragVelocity = UnityEngine.Vector3.zero;
            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingBrushToColor;
            _runtimeState.SelectedBlushColorIndex = colorIndex;

            await _handMotion.MoveBrushTipToAsync(
                colorView.BrushDipPoint.position,
                _motionConfig.AutomaticMoveDuration);
            await _handMotion.PlayBrushDipAnimationAsync();

            _visualState.ApplyBrushTipColor(colorView);

            if (_blushConfig.BrushChestHoldPoint == null)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBrushDragStart;
                
                return;
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingBrushToChestHoldPoint;

            await _handMotion.MoveHandToAsync(
                _blushConfig.BrushChestHoldPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBrushDragStart;
        }

        public async UniTask ApplyBlushAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.DraggingBrushToFace)
                return;

            if (_runtimeState.SelectedBlushColorIndex < 0)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
                
                return;
            }

            if (_blushConfig.FaceApplyPoint == null || _blushConfig.BrushPickupPoint == null)
                return;

            _runtimeState.ProcessStageType = MakeupProcessStageType.ApplyingBlush;

            await _handMotion.MoveBrushTipToAsync(
                _blushConfig.FaceApplyPoint.position,
                _motionConfig.AutomaticMoveDuration * HALF_MOVE_DURATION_MULTIPLIER);

            await _handMotion.PlayBrushDipAnimationAsync();

            if (_playerFaceStateView != null)
            {
                await _playerFaceStateView.ShowBlushAsync(
                    _runtimeState.SelectedBlushColorIndex,
                    _motionConfig.ApplyDuration);
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningBrush;

            await _handMotion.MoveHandToAsync(_blushConfig.BrushPickupPoint.position, _motionConfig.AutomaticMoveDuration);

            _visualState.SetBrushInHandVisible(false);
            _visualState.SetBrushStandVisible(true);
            _visualState.ResetBrushTipColor();

            await _handMotion.MoveHandToDefaultPointAsync();

            _runtimeState.SelectedBlushColorIndex = -1;
            _runtimeState.DragVelocity = UnityEngine.Vector3.zero;
            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForBrushSelection;
        }
    }
}