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
    public sealed class LipstickToolFlow : ToolFlowBase
    {
        private readonly LipstickMakeupSettings _lipstickSettings;
        private readonly LipstickMakeupSceneReferences _lipstickSceneReferences;

        public LipstickToolFlow(
            LipstickMakeupSettings lipstickSettings,
            LipstickMakeupSceneReferences lipstickSceneReferences,
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
            _lipstickSettings = lipstickSettings;
            _lipstickSceneReferences = lipstickSceneReferences;
        }

        public override Collider2D FaceZone => _lipstickSceneReferences.FaceZone;
        
        public override MakeupProcessStageType DragStageType => MakeupProcessStageType.DraggingLipstickToFace;

        public override MakeupProcessStageType DragFallbackStageType =>
            MakeupProcessStageType.WaitingForLipstickDragStart;
        
        public override async UniTask ApplyAsync()
        {
            if (RuntimeState.ProcessStageType != DragStageType)
                return;

            if (RuntimeState.SelectedLipstickColorIndex < 0 || RuntimeState.SelectedLipstickView == null)
            {
                RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForLipstickSelection;

                return;
            }

            if (_lipstickSceneReferences.UpperLipLeftApplyPoint == null ||
                _lipstickSceneReferences.UpperLipRightApplyPoint == null ||
                _lipstickSceneReferences.LowerLipRightApplyPoint == null ||
                _lipstickSceneReferences.LowerLipLeftApplyPoint == null)
            {
                return;
            }

            RuntimeState.ProcessStageType = MakeupProcessStageType.ApplyingLipstick;

            float moveToFaceDuration =
                MotionSettings.AutomaticMoveDuration * _lipstickSettings.MoveToFaceDurationMultiplier;
            LipstickStrokePath lipstickStrokePath = new LipstickStrokePath(
                _lipstickSceneReferences.UpperLipLeftApplyPoint.position,
                _lipstickSceneReferences.UpperLipRightApplyPoint.position,
                _lipstickSceneReferences.LowerLipRightApplyPoint.position,
                _lipstickSceneReferences.LowerLipLeftApplyPoint.position);
            
            await HandMotionFacade.MoveLipstickApplyPointToAsync(lipstickStrokePath.UpperLipLeftPosition, moveToFaceDuration);
            await HandMotionFacade.PlayLipstickApplyAnimationAsync(lipstickStrokePath, MotionSettings.ApplyDuration);

            if (PlayerFaceStateView != null)
            {
                await PlayerFaceStateView.ShowLipstickAsync(
                    RuntimeState.SelectedLipstickColorIndex,
                    MotionSettings.ApplyDuration);
            }

            await FinishApplyAndReturnAsync(
                MakeupProcessStageType.ReturningLipstick,
                MakeupProcessStageType.WaitingForLipstickSelection);
        }

        public async UniTask SelectLipstickAsync(
            LipstickPaletteColorView selectedLipstickView,
            int selectedLipstickIndex)
        {
            bool canSelectLipstick =
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickSelection ||
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickDragStart;

            if (canSelectLipstick == false ||
                selectedLipstickView == null ||
                selectedLipstickView.PickupPoint == null)
            {
                return;
            }

            if (RuntimeState.SelectedLipstickView == selectedLipstickView &&
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForLipstickDragStart)
            {
                return;
            }

            PrepareForManualStep();

            if (RuntimeState.ActiveToolType == MakeupToolType.Lipstick)
                await ToolReturnSequence.ReturnActiveToolAsync();

            RuntimeState.SelectedLipstickColorIndex = selectedLipstickIndex;
            RuntimeState.SelectedLipstickView = selectedLipstickView;
            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToLipstick;

            await HandMotionFacade.MoveHandToAsync(
                selectedLipstickView.PickupPoint.position,
                MotionSettings.AutomaticMoveDuration);

            VisualState.SetLipstickBookVisualVisible(selectedLipstickView, false);
            VisualState.ApplyLipstickInHandSprite(selectedLipstickView);
            VisualState.SetLipstickInHandVisible(true);
            RuntimeState.ActiveToolType = MakeupToolType.Lipstick;

            await MoveToHoldPointOrEnterWaitingStageAsync(
                _lipstickSceneReferences.LipstickChestHoldPoint,
                MakeupProcessStageType.MovingLipstickToChestHoldPoint,
                MakeupProcessStageType.WaitingForLipstickDragStart);
        }
    }
}