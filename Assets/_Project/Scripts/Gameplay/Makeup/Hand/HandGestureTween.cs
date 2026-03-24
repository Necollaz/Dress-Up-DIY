using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.State;

namespace _Project.Gameplay.Makeup.Hand
{
    public sealed class HandGestureTween
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly MakeupMotionSettings _motionSettings;
        private readonly MakeupRuntimeState _runtimeState;

        public HandGestureTween(
            MakeupHandConfig handConfig,
            MakeupMotionSettings motionSettings,
            MakeupRuntimeState runtimeState)
        {
            _handConfig = handConfig;
            _motionSettings = motionSettings;
            _runtimeState = runtimeState;
        }

        public async UniTask PlayBrushDipAnimationAsync()
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            _runtimeState.ActiveSequence?.Kill();

            Vector3 startPosition = handRoot.position;
            Vector3 leftPosition = startPosition + Vector3.left * _motionSettings.BrushDipOffset;
            Vector3 rightPosition = startPosition + Vector3.right * _motionSettings.BrushDipOffset;

            _runtimeState.ActiveSequence = DOTween.Sequence();
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                leftPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                rightPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                leftPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                startPosition,
                _motionSettings.BrushDipAnimationStepDuration));

            await _runtimeState.ActiveSequence.AsyncWaitForCompletion();
        }

        public async UniTask PlayLipstickApplyAnimationAsync(
            LipstickStrokePath lipstickStrokePath,
            float duration)
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            if (_handConfig.LipstickApplyPoint == null)
            {
                await PlayLipstickApplyAnimationWithoutOffsetAsync(lipstickStrokePath, duration);

                return;
            }

            _runtimeState.ActiveSequence?.Kill();

            Vector3 applyPointOffset = _handConfig.LipstickApplyPoint.position - handRoot.position;
            Vector3 upperLipLeftHandPosition = lipstickStrokePath.UpperLipLeftPosition - applyPointOffset;
            Vector3 upperLipRightHandPosition = lipstickStrokePath.UpperLipRightPosition - applyPointOffset;
            Vector3 lowerLipRightHandPosition = lipstickStrokePath.LowerLipRightPosition - applyPointOffset;
            Vector3 lowerLipLeftHandPosition = lipstickStrokePath.LowerLipLeftPosition - applyPointOffset;

            float halfDuration = duration * 0.5f;

            _runtimeState.ActiveSequence = DOTween.Sequence();
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                upperLipLeftHandPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                upperLipRightHandPosition,
                halfDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lowerLipRightHandPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lowerLipLeftHandPosition,
                halfDuration));

            await _runtimeState.ActiveSequence.AsyncWaitForCompletion();
        }

        private async UniTask PlayLipstickApplyAnimationWithoutOffsetAsync(
            LipstickStrokePath lipstickStrokePath,
            float duration)
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            _runtimeState.ActiveSequence?.Kill();

            float halfDuration = duration * 0.5f;

            _runtimeState.ActiveSequence = DOTween.Sequence();
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lipstickStrokePath.UpperLipLeftPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lipstickStrokePath.UpperLipRightPosition,
                halfDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lipstickStrokePath.LowerLipRightPosition,
                _motionSettings.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                lipstickStrokePath.LowerLipLeftPosition,
                halfDuration));

            await _runtimeState.ActiveSequence.AsyncWaitForCompletion();
        }
    }
}