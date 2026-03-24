using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay
{
    public sealed class CreamMakeupFlow
    {
        private const float HALF_MOVE_DURATION_MULTIPLIER = 0.5f;
        private const int MILLISECONDS_IN_SECOND = 1000;

        private readonly CreamMakeupConfig _creamConfig;
        private readonly MakeupMotionConfig _motionConfig;
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupVisualState _visualState;
        private readonly MakeupHandMotion _handMotion;

        public CreamMakeupFlow(
            CreamMakeupConfig creamConfig,
            MakeupMotionConfig motionConfig,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            MakeupHandMotion handMotion)
        {
            _creamConfig = creamConfig;
            _motionConfig = motionConfig;
            _playerFaceStateView = playerFaceStateView;
            _runtimeState = runtimeState;
            _visualState = visualState;
            _handMotion = handMotion;
        }

        public bool CanSwitchToCream()
        {
            switch (_runtimeState.ProcessStageType)
            {
                case MakeupProcessStageType.Idle:
                case MakeupProcessStageType.WaitingForCreamDragStart:

                case MakeupProcessStageType.WaitingForBrushSelection:
                case MakeupProcessStageType.WaitingForBlushColorSelection:
                case MakeupProcessStageType.WaitingForBrushDragStart:

                case MakeupProcessStageType.WaitingForLipstickSelection:
                case MakeupProcessStageType.WaitingForLipstickDragStart:
                    return true;

                default:
                    return false;
            }
        }

        public async UniTask StartCreamSequenceAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.Idle &&
                _runtimeState.ProcessStageType != MakeupProcessStageType.ReturningToolBeforeSwitch)
            {
                return;
            }

            if (_creamConfig.CreamPickupPoint == null || _creamConfig.CreamDragStartPoint == null)
                return;

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingHandToCream;

            await _handMotion.MoveHandToAsync(
                _creamConfig.CreamPickupPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _visualState.SetCreamStandVisible(false);
            _visualState.SetCreamInHandVisible(true);

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingHandToCreamDragStartPoint;

            await _handMotion.MoveHandToAsync(
                _creamConfig.CreamDragStartPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForCreamDragStart;
        }

        public async UniTask ApplyCreamAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.DraggingCreamToFace)
                return;

            if (_creamConfig.FaceApplyPoint == null || _creamConfig.CreamPickupPoint == null)
                return;

            _runtimeState.ProcessStageType = MakeupProcessStageType.ApplyingCream;

            await _handMotion.MoveHandToAsync(
                _creamConfig.FaceApplyPoint.position,
                _motionConfig.AutomaticMoveDuration * HALF_MOVE_DURATION_MULTIPLIER);

            if (_playerFaceStateView != null)
                await _playerFaceStateView.HideAcneAsync(_motionConfig.ApplyDuration);

            int pauseDurationMilliseconds =
                Mathf.RoundToInt(_motionConfig.PauseAfterApplyDuration * MILLISECONDS_IN_SECOND);

            await UniTask.Delay(pauseDurationMilliseconds);

            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningCream;

            await _handMotion.MoveHandToAsync(_creamConfig.CreamPickupPoint.position, _motionConfig.AutomaticMoveDuration);

            _visualState.SetCreamInHandVisible(false);
            _visualState.SetCreamStandVisible(true);

            await _handMotion.MoveHandToDefaultPointAsync();

            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
        }
    }
}