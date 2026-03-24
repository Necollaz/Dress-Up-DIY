using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class LipstickMakeupFlow
    {
        private const float HALF_MOVE_DURATION_MULTIPLIER = 0.5f;

        private readonly LipstickMakeupConfig _lipstickConfig;
        private readonly MakeupMotionConfig _motionConfig;
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupVisualState _visualState;
        private readonly MakeupHandMotion _handMotion;

        public LipstickMakeupFlow(
            LipstickMakeupConfig lipstickConfig,
            MakeupMotionConfig motionConfig,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            MakeupHandMotion handMotion)
        {
            _lipstickConfig = lipstickConfig;
            _motionConfig = motionConfig;
            _playerFaceStateView = playerFaceStateView;
            _runtimeState = runtimeState;
            _visualState = visualState;
            _handMotion = handMotion;
        }

        public async UniTask SelectLipstickAsync(LipstickPaletteColorView selectedLipstickView, int selectedLipstickIndex)
        {
            bool canSelectLipstick =
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickSelection ||
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickDragStart;

            if (canSelectLipstick == false || selectedLipstickView == null || selectedLipstickView.PickupPoint == null)
                return;

            if (_runtimeState.SelectedLipstickView == selectedLipstickView &&
                _runtimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickDragStart)
            {
                return;
            }

            _runtimeState.ActiveSequence?.Kill();
            _runtimeState.DragVelocity = Vector3.zero;

            if (_visualState.IsLipstickInHandVisible())
                await ReturnSelectedLipstickToBookAsync();

            _runtimeState.SelectedLipstickColorIndex = selectedLipstickIndex;
            _runtimeState.SelectedLipstickView = selectedLipstickView;
            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingHandToLipstick;

            await _handMotion.MoveHandToAsync(
                selectedLipstickView.PickupPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _visualState.SetLipstickBookVisualVisible(selectedLipstickView, false);
            _visualState.ApplyLipstickInHandSprite(selectedLipstickView);
            _visualState.SetLipstickInHandVisible(true);

            if (_lipstickConfig.LipstickChestHoldPoint == null)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForLipstickDragStart;
                
                return;
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.MovingLipstickToChestHoldPoint;

            await _handMotion.MoveHandToAsync(
                _lipstickConfig.LipstickChestHoldPoint.position,
                _motionConfig.AutomaticMoveDuration);

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForLipstickDragStart;
        }

        public async UniTask ApplyLipstickAsync()
        {
            if (_runtimeState.ProcessStageType != MakeupProcessStageType.DraggingLipstickToFace)
                return;

            if (_runtimeState.SelectedLipstickColorIndex < 0 || _runtimeState.SelectedLipstickView == null)
            {
                _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForLipstickSelection;
                
                return;
            }

            if (_lipstickConfig.UpperLipLeftApplyPoint == null ||
                _lipstickConfig.UpperLipRightApplyPoint == null ||
                _lipstickConfig.LowerLipRightApplyPoint == null ||
                _lipstickConfig.LowerLipLeftApplyPoint == null)
            {
                return;
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.ApplyingLipstick;

            await _handMotion.MoveLipstickApplyPointToAsync(
                _lipstickConfig.UpperLipLeftApplyPoint.position,
                _motionConfig.AutomaticMoveDuration * HALF_MOVE_DURATION_MULTIPLIER);
            await _handMotion.PlayLipstickApplyAnimationAsync(
                _lipstickConfig.UpperLipLeftApplyPoint.position,
                _lipstickConfig.UpperLipRightApplyPoint.position,
                _lipstickConfig.LowerLipRightApplyPoint.position,
                _lipstickConfig.LowerLipLeftApplyPoint.position,
                _motionConfig.ApplyDuration);

            if (_playerFaceStateView != null)
            {
                await _playerFaceStateView.ShowLipstickAsync(
                    _runtimeState.SelectedLipstickColorIndex,
                    _motionConfig.ApplyDuration);
            }

            _runtimeState.ProcessStageType = MakeupProcessStageType.ReturningLipstick;

            await ReturnSelectedLipstickToBookAsync();

            _runtimeState.ProcessStageType = MakeupProcessStageType.WaitingForLipstickSelection;
        }

        public async UniTask ReturnSelectedLipstickToBookAsync()
        {
            LipstickPaletteColorView selectedLipstickView = _runtimeState.SelectedLipstickView;

            if (selectedLipstickView != null && 
                selectedLipstickView.PickupPoint != null &&
                _visualState.IsLipstickInHandVisible())
            {
                await _handMotion.MoveHandToAsync(
                    selectedLipstickView.PickupPoint.position,
                    _motionConfig.AutomaticMoveDuration);
            }

            if (selectedLipstickView != null)
                _visualState.SetLipstickBookVisualVisible(selectedLipstickView, true);

            _visualState.SetLipstickInHandVisible(false);
            _visualState.ResetLipstickInHandSprite();

            await _handMotion.MoveHandToDefaultPointAsync();

            _runtimeState.SelectedLipstickColorIndex = -1;
            _runtimeState.SelectedLipstickView = null;
            _runtimeState.DragVelocity = Vector3.zero;
        }
    }
}