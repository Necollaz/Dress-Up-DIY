using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Hand
{
    public sealed class HandMotionFacade
    {
        private readonly HandPositionTween _handPositionTween;
        private readonly HandGestureTween _handGestureTween;
        private readonly HandDragSmoothing _handDragSmoothing;

        public HandMotionFacade(
            HandPositionTween handPositionTween,
            HandGestureTween handGestureTween,
            HandDragSmoothing handDragSmoothing)
        {
            _handPositionTween = handPositionTween;
            _handGestureTween = handGestureTween;
            _handDragSmoothing = handDragSmoothing;
        }

        public UniTask MoveHandToAsync(Vector3 targetPosition, float duration) =>
            _handPositionTween.MoveHandToAsync(targetPosition, duration);

        public UniTask MoveHandToDefaultPointAsync() =>
            _handPositionTween.MoveHandToDefaultPointAsync();

        public UniTask MoveBrushTipToAsync(Vector3 targetBrushTipPosition, float duration) =>
            _handPositionTween.MoveBrushTipToAsync(targetBrushTipPosition, duration);

        public UniTask MoveLipstickApplyPointToAsync(Vector3 targetApplyPointPosition, float duration) =>
            _handPositionTween.MoveLipstickApplyPointToAsync(targetApplyPointPosition, duration);

        public UniTask MoveEyeshadowBrushTipToAsync(Vector3 targetBrushTipPosition, float duration) =>
            _handPositionTween.MoveEyeshadowBrushTipToAsync(targetBrushTipPosition, duration);

        public UniTask PlayBrushDipAnimationAsync() => _handGestureTween.PlayBrushDipAnimationAsync();

        public UniTask PlayLipstickApplyAnimationAsync(
            LipstickStrokePath lipstickStrokePath,
            float duration) =>
            _handGestureTween.PlayLipstickApplyAnimationAsync(lipstickStrokePath, duration);

        public void UpdateDraggedHandPosition(Vector3 pointerWorldPosition) =>
            _handDragSmoothing.UpdateDraggedHandPosition(pointerWorldPosition);
    }
}