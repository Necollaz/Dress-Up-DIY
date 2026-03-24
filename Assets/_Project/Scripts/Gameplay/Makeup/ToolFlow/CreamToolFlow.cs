using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.ToolFlow
{
    public sealed class CreamToolFlow : ToolFlowBase
    {
        private const int MILLISECONDS_IN_SECOND = 1000;

        private readonly CreamMakeupSettings _creamSettings;
        private readonly CreamMakeupSceneReferences _creamSceneReferences;

        public CreamToolFlow(
            CreamMakeupSettings creamSettings,
            CreamMakeupSceneReferences creamSceneReferences,
            MakeupMotionSettings motionSettings,
            PlayerFaceStateView playerFaceStateView,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            HandMotionFacade handMotionFacade,
            ActiveToolReturnSequence toolReturnSequence)
            : base(
                motionSettings,
                playerFaceStateView,
                runtimeState,
                visualState,
                handMotionFacade,
                toolReturnSequence)
        {
            _creamSettings = creamSettings;
            _creamSceneReferences = creamSceneReferences;
        }

        public override Collider2D FaceZone => _creamSceneReferences.FaceZone;
        
        public override MakeupProcessStageType DragStageType => MakeupProcessStageType.DraggingCreamToFace;

        public override MakeupProcessStageType DragFallbackStageType => MakeupProcessStageType.WaitingForCreamDragStart;

        public override async UniTask ApplyAsync()
        {
            if (RuntimeState.ProcessStageType != DragStageType)
                return;

            if (_creamSceneReferences.FaceApplyPoint == null)
                return;

            RuntimeState.ProcessStageType = MakeupProcessStageType.ApplyingCream;

            await HandMotionFacade.MoveHandToAsync(
                _creamSceneReferences.FaceApplyPoint.position,
                MotionSettings.AutomaticMoveDuration * _creamSettings.MoveToFaceDurationMultiplier);

            if (PlayerFaceStateView != null)
                await PlayerFaceStateView.HideAcneAsync(MotionSettings.ApplyDuration);

            int pauseDurationMilliseconds =
                Mathf.RoundToInt(MotionSettings.PauseAfterApplyDuration * MILLISECONDS_IN_SECOND);

            await UniTask.Delay(pauseDurationMilliseconds);
            await FinishApplyAndReturnAsync(MakeupProcessStageType.ReturningCream, MakeupProcessStageType.Idle);
        }
        
        public async UniTask StartSequenceAsync()
        {
            if (RuntimeState.ProcessStageType != MakeupProcessStageType.Idle &&
                RuntimeState.ProcessStageType != MakeupProcessStageType.ReturningToolBeforeSwitch)
            {
                return;
            }

            if (_creamSceneReferences.CreamPickupPoint == null ||
                _creamSceneReferences.CreamDragStartPoint == null)
                return;

            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToCream;

            await HandMotionFacade.MoveHandToAsync(
                _creamSceneReferences.CreamPickupPoint.position,
                MotionSettings.AutomaticMoveDuration);

            VisualState.SetCreamStandVisible(false);
            VisualState.SetCreamInHandVisible(true);
            RuntimeState.ActiveToolType = MakeupToolType.Cream;

            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToCreamDragStartPoint;

            await HandMotionFacade.MoveHandToAsync(
                _creamSceneReferences.CreamDragStartPoint.position,
                MotionSettings.AutomaticMoveDuration);

            RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForCreamDragStart;
        }
    }
}