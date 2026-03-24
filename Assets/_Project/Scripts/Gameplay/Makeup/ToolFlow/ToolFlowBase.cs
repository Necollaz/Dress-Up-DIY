using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.ToolFlow
{
    public abstract class ToolFlowBase : IToolFlowContract
    {
        protected readonly MakeupMotionSettings MotionSettings;
        protected readonly PlayerFaceStateView PlayerFaceStateView;
        protected readonly MakeupRuntimeState RuntimeState;
        protected readonly MakeupVisualState VisualState;
        protected readonly HandMotionFacade HandMotionFacade;
        protected readonly ActiveToolReturnSequence ToolReturnSequence;

        protected ToolFlowBase(
            MakeupMotionSettings motionSettings,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            HandMotionFacade handMotionFacade,
            ActiveToolReturnSequence toolReturnSequence)
        {
            MotionSettings = motionSettings;
            PlayerFaceStateView = playerFaceStateView;
            RuntimeState = runtimeState;
            VisualState = visualState;
            HandMotionFacade = handMotionFacade;
            ToolReturnSequence = toolReturnSequence;
        }

        public abstract Collider2D FaceZone { get; }
        public abstract MakeupProcessStageType DragStageType { get; }
        public abstract MakeupProcessStageType DragFallbackStageType { get; }

        public abstract UniTask ApplyAsync();

        protected async UniTask MoveToHoldPointOrEnterWaitingStageAsync(
            Transform holdPoint,
            MakeupProcessStageType movingStageType,
            MakeupProcessStageType waitingStageType)
        {
            if (holdPoint == null)
            {
                RuntimeState.ProcessStageType = waitingStageType;

                return;
            }

            RuntimeState.ProcessStageType = movingStageType;

            await HandMotionFacade.MoveHandToAsync(holdPoint.position, MotionSettings.AutomaticMoveDuration);

            RuntimeState.ProcessStageType = waitingStageType;
        }

        protected async UniTask FinishApplyAndReturnAsync(
            MakeupProcessStageType returningStageType,
            MakeupProcessStageType waitingStageType)
        {
            RuntimeState.ProcessStageType = returningStageType;

            await ToolReturnSequence.ReturnActiveToolAsync();

            RuntimeState.ProcessStageType = waitingStageType;
        }
        
        protected void PrepareForManualStep()
        {
            RuntimeState.ActiveSequence?.Kill();
            RuntimeState.DragVelocity = Vector3.zero;
        }
    }
}