using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class MakeupHandMotion
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly MakeupMotionConfig _motionConfig;
        private readonly MakeupRuntimeState _runtimeState;

        public MakeupHandMotion(
            MakeupHandConfig handConfig,
            MakeupMotionConfig motionConfig,
            MakeupRuntimeState runtimeState)
        {
            _handConfig = handConfig;
            _motionConfig = motionConfig;
            _runtimeState = runtimeState;
        }

        public async UniTask MoveHandToAsync(Vector3 targetPosition, float duration)
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            if (duration <= 0f)
            {
                handRoot.position = targetPosition;
                
                return;
            }

            Vector3 startPosition = handRoot.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float easedProgress = _motionConfig.AutomaticMoveCurve.Evaluate(progress);
                handRoot.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedProgress);

                await UniTask.Yield();
            }

            handRoot.position = targetPosition;
        }

        public async UniTask MoveHandToDefaultPointAsync()
        {
            if (_handConfig.HandDefaultPoint == null)
                return;

            await MoveHandToAsync(_handConfig.HandDefaultPoint.position, _motionConfig.AutomaticMoveDuration);
        }

        public async UniTask MoveBrushTipToAsync(Vector3 targetBrushTipPosition, float duration)
        {
            Transform handRoot = _handConfig.HandRoot;
            Transform brushTipPoint = _handConfig.BrushTipPoint;

            if (handRoot == null || brushTipPoint == null)
            {
                await MoveHandToAsync(targetBrushTipPosition, duration);
                
                return;
            }

            Vector3 brushTipOffset = brushTipPoint.position - handRoot.position;
            Vector3 targetHandRootPosition = targetBrushTipPosition - brushTipOffset;

            await MoveHandToAsync(targetHandRootPosition, duration);
        }

        public async UniTask PlayBrushDipAnimationAsync()
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            _runtimeState.ActiveSequence?.Kill();

            Vector3 startPosition = handRoot.position;
            Vector3 leftPosition = startPosition + Vector3.left * _motionConfig.BrushDipOffset;
            Vector3 rightPosition = startPosition + Vector3.right * _motionConfig.BrushDipOffset;

            _runtimeState.ActiveSequence = DOTween.Sequence();
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                leftPosition,
                _motionConfig.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                rightPosition,
                _motionConfig.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                leftPosition,
                _motionConfig.BrushDipAnimationStepDuration));
            _runtimeState.ActiveSequence.Append(handRoot.DOMove(
                startPosition,
                _motionConfig.BrushDipAnimationStepDuration));

            await _runtimeState.ActiveSequence.AsyncWaitForCompletion();
        }

        public void UpdateDraggedHandPosition(Vector3 pointerWorldPosition)
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null)
                return;

            Vector3 targetPosition = new Vector3(
                pointerWorldPosition.x,
                pointerWorldPosition.y,
                handRoot.position.z);
            Vector3 dragVelocity = _runtimeState.DragVelocity;
            handRoot.position = Vector3.SmoothDamp(
                handRoot.position,
                targetPosition,
                ref dragVelocity,
                _motionConfig.DragSmoothTime);

            _runtimeState.DragVelocity = dragVelocity;
        }
    }
}