using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class MakeupMotionConfig
    {
        [SerializeField] private float _automaticMoveDuration = 0.35f;
        [SerializeField] private float _dragSmoothTime = 0.04f;
        [SerializeField] private float _applyDuration = 0.35f;
        [SerializeField] private float _pauseAfterApplyDuration = 0.1f;
        [SerializeField] private float _brushDipAnimationStepDuration = 0.08f;
        [SerializeField] private float _brushDipOffset = 0.12f;
        [SerializeField] private AnimationCurve _automaticMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public float AutomaticMoveDuration => _automaticMoveDuration;
        public float DragSmoothTime => _dragSmoothTime;
        public float ApplyDuration => _applyDuration;
        public float PauseAfterApplyDuration => _pauseAfterApplyDuration;
        public float BrushDipAnimationStepDuration => _brushDipAnimationStepDuration;
        public float BrushDipOffset => _brushDipOffset;
        public AnimationCurve AutomaticMoveCurve => _automaticMoveCurve;
    }
}