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
    public sealed class BlushToolFlow : ToolFlowBase
    {
        private readonly BlushMakeupSettings _blushSettings;
        private readonly BlushMakeupSceneReferences _blushSceneReferences;

        public BlushToolFlow(
            BlushMakeupSettings blushSettings,
            BlushMakeupSceneReferences blushSceneReferences,
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
            _blushSettings = blushSettings;
            _blushSceneReferences = blushSceneReferences;
        }

        public override Collider2D FaceZone => _blushSceneReferences.FaceZone;

        public override MakeupProcessStageType DragStageType => MakeupProcessStageType.DraggingBrushToFace;

        public override MakeupProcessStageType DragFallbackStageType =>
            MakeupProcessStageType.WaitingForBrushDragStart;

        public override async UniTask ApplyAsync()
        {
            if (RuntimeState.ProcessStageType != DragStageType)
                return;

            if (RuntimeState.SelectedBlushColorIndex < 0)
            {
                RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
                return;
            }

            if (_blushSceneReferences.FaceApplyPoint == null)
                return;

            RuntimeState.ProcessStageType = MakeupProcessStageType.ApplyingBlush;

            float moveToFaceDuration =
                MotionSettings.AutomaticMoveDuration * _blushSettings.MoveToFaceDurationMultiplier;

            await HandMotionFacade.MoveBrushTipToAsync(
                _blushSceneReferences.FaceApplyPoint.position,
                moveToFaceDuration);
            await HandMotionFacade.PlayBrushDipAnimationAsync();

            if (PlayerFaceStateView != null)
            {
                await PlayerFaceStateView.ShowBlushAsync(
                    RuntimeState.SelectedBlushColorIndex,
                    MotionSettings.ApplyDuration);
            }

            await FinishApplyAndReturnAsync(
                MakeupProcessStageType.ReturningBrush,
                MakeupProcessStageType.WaitingForBlushColorSelection);
        }

        public async UniTask SelectColorAsync(BlushPaletteColorView colorView, int colorIndex)
        {
            bool canSelectColor =
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForBlushColorSelection ||
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForBrushDragStart;

            if (canSelectColor == false || colorView == null || colorView.BrushDipPoint == null)
                return;

            PrepareForManualStep();

            if (RuntimeState.ActiveToolType != MakeupToolType.BlushBrush)
                await TakeBrushForColorSelectionAsync();

            RuntimeState.SelectedBlushColorIndex = colorIndex;
            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingBrushToColor;

            await HandMotionFacade.MoveBrushTipToAsync(
                colorView.BrushDipPoint.position,
                MotionSettings.AutomaticMoveDuration);
            await HandMotionFacade.PlayBrushDipAnimationAsync();

            VisualState.ApplyBrushTipColor(colorView);

            await MoveToHoldPointOrEnterWaitingStageAsync(
                _blushSceneReferences.BrushChestHoldPoint,
                MakeupProcessStageType.MovingBrushToChestHoldPoint,
                MakeupProcessStageType.WaitingForBrushDragStart);
        }

        private async UniTask TakeBrushForColorSelectionAsync()
        {
            if (_blushSceneReferences.BrushPickupPoint == null)
                return;

            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToBrush;

            await HandMotionFacade.MoveHandToAsync(
                _blushSceneReferences.BrushPickupPoint.position,
                MotionSettings.AutomaticMoveDuration);

            VisualState.SetBlushBrushStandVisible(false);
            VisualState.SetBrushInHandVisible(true);
            RuntimeState.ActiveToolType = MakeupToolType.BlushBrush;

            RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
        }
        
        // private readonly BlushMakeupSettings _blushSettings;
        // private readonly BlushMakeupSceneReferences _blushSceneReferences;
        //
        // public BlushMakeupFlow(
        //     BlushMakeupSettings blushSettings,
        //     BlushMakeupSceneReferences blushSceneReferences,
        //     MakeupMotionSettings motionSettings,
        //     PlayerFaceStateView playerFaceStateView,
        //     MakeupRuntimeState runtimeState,
        //     MakeupVisualState visualState,
        //     MakeupHandMotion handMotion,
        //     MakeupToolReturnFlow toolReturnFlow)
        //     : base(
        //         motionSettings,
        //         playerFaceStateView,
        //         runtimeState,
        //         visualState,
        //         handMotion,
        //         toolReturnFlow)
        // {
        //     _blushSettings = blushSettings;
        //     _blushSceneReferences = blushSceneReferences;
        // }
        //
        // public override Collider2D FaceZone => _blushSceneReferences.FaceZone;
        //
        // public override MakeupProcessStageType DragStageType => MakeupProcessStageType.DraggingBrushToFace;
        //
        // public override MakeupProcessStageType DragFallbackStageType => MakeupProcessStageType.WaitingForBrushDragStart;
        //
        // public override async UniTask ApplyAsync()
        // {
        //     if (RuntimeState.ProcessStageType != DragStageType)
        //         return;
        //
        //     if (RuntimeState.SelectedBlushColorIndex < 0)
        //     {
        //         RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
        //
        //         return;
        //     }
        //
        //     if (_blushSceneReferences.FaceApplyPoint == null)
        //         return;
        //
        //     RuntimeState.ProcessStageType = MakeupProcessStageType.ApplyingBlush;
        //
        //     float moveToFaceDuration =
        //         MotionSettings.AutomaticMoveDuration * _blushSettings.MoveToFaceDurationMultiplier;
        //
        //     await HandMotion.MoveBrushTipToAsync(
        //         _blushSceneReferences.FaceApplyPoint.position,
        //         moveToFaceDuration);
        //     await HandMotion.PlayBrushDipAnimationAsync();
        //
        //     if (PlayerFaceStateView != null)
        //     {
        //         await PlayerFaceStateView.ShowBlushAsync(
        //             RuntimeState.SelectedBlushColorIndex,
        //             MotionSettings.ApplyDuration);
        //     }
        //
        //     await FinishApplyAndReturnAsync(
        //         MakeupProcessStageType.ReturningBrush,
        //         MakeupProcessStageType.WaitingForBrushSelection);
        // }
        //
        // public async UniTask TakeBrushAsync()
        // {
        //     if (RuntimeState.ProcessStageType != MakeupProcessStageType.WaitingForBrushSelection)
        //         return;
        //
        //     if (_blushSceneReferences.BrushPickupPoint == null)
        //         return;
        //
        //     RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToBrush;
        //
        //     await HandMotion.MoveHandToAsync(
        //         _blushSceneReferences.BrushPickupPoint.position,
        //         MotionSettings.AutomaticMoveDuration);
        //
        //     VisualState.SetBlushBrushStandVisible(false);
        //     VisualState.SetBrushInHandVisible(true);
        //     RuntimeState.ActiveToolType = MakeupToolType.BlushBrush;
        //
        //     RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForBlushColorSelection;
        // }
        //
        // public async UniTask SelectColorAsync(BlushPaletteColorView colorView, int colorIndex)
        // {
        //     bool canSelectColor =
        //         RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForBlushColorSelection ||
        //         RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForBrushDragStart;
        //
        //     if (canSelectColor == false || colorView == null || colorView.BrushDipPoint == null)
        //         return;
        //
        //     PrepareForManualStep();
        //
        //     RuntimeState.SelectedBlushColorIndex = colorIndex;
        //     RuntimeState.ProcessStageType = MakeupProcessStageType.MovingBrushToColor;
        //
        //     await HandMotion.MoveBrushTipToAsync(
        //         colorView.BrushDipPoint.position,
        //         MotionSettings.AutomaticMoveDuration);
        //     await HandMotion.PlayBrushDipAnimationAsync();
        //
        //     VisualState.ApplyBrushTipColor(colorView);
        //
        //     await MoveToHoldPointOrEnterWaitingStageAsync(
        //         _blushSceneReferences.BrushChestHoldPoint,
        //         MakeupProcessStageType.MovingBrushToChestHoldPoint,
        //         MakeupProcessStageType.WaitingForBrushDragStart);
        // }
    }
}