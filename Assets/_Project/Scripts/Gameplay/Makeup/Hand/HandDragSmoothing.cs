using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.State;

namespace _Project.Gameplay.Makeup.Hand
{
    public sealed class HandDragSmoothing
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly MakeupMotionSettings _motionSettings;
        private readonly MakeupRuntimeState _runtimeState;

        public HandDragSmoothing(
            MakeupHandConfig handConfig,
            MakeupMotionSettings motionSettings,
            MakeupRuntimeState runtimeState)
        {
            _handConfig = handConfig;
            _motionSettings = motionSettings;
            _runtimeState = runtimeState;
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
                _motionSettings.DragSmoothTime);

            _runtimeState.DragVelocity = dragVelocity;
        }
    }
}