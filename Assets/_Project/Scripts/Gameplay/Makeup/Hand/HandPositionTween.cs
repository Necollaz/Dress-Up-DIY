using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;

namespace _Project.Gameplay.Makeup.Hand
{
    public sealed class HandPositionTween
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly MakeupMotionSettings _motionSettings;

        public HandPositionTween(MakeupHandConfig handConfig, MakeupMotionSettings motionSettings)
        {
            _handConfig = handConfig;
            _motionSettings = motionSettings;
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
                float easedProgress = _motionSettings.AutomaticMoveCurve.Evaluate(progress);
                handRoot.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedProgress);

                await UniTask.Yield();
            }

            handRoot.position = targetPosition;
        }

        public async UniTask MoveHandToDefaultPointAsync()
        {
            if (_handConfig.HandDefaultPoint == null)
                return;

            await MoveHandToAsync(_handConfig.HandDefaultPoint.position, _motionSettings.AutomaticMoveDuration);
        }

        public async UniTask MoveBrushTipToAsync(Vector3 targetBrushTipPosition, float duration)
        {
            await MoveByPointOffsetAsync(_handConfig.BrushTipPoint, targetBrushTipPosition, duration);
        }

        public async UniTask MoveLipstickApplyPointToAsync(Vector3 targetApplyPointPosition, float duration)
        {
            await MoveByPointOffsetAsync(_handConfig.LipstickApplyPoint, targetApplyPointPosition, duration);
        }

        public async UniTask MoveEyeshadowBrushTipToAsync(Vector3 targetBrushTipPosition, float duration)
        {
            await MoveByPointOffsetAsync(_handConfig.EyeshadowBrushTipPoint, targetBrushTipPosition, duration);
        }

        private async UniTask MoveByPointOffsetAsync(Transform handPoint, Vector3 targetPointPosition, float duration)
        {
            Transform handRoot = _handConfig.HandRoot;

            if (handRoot == null || handPoint == null)
            {
                await MoveHandToAsync(targetPointPosition, duration);

                return;
            }

            Vector3 pointOffset = handPoint.position - handRoot.position;
            Vector3 targetHandRootPosition = targetPointPosition - pointOffset;

            await MoveHandToAsync(targetHandRootPosition, duration);
        }
    }
}