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
    public sealed class EyeshadowToolFlow : ToolFlowBase
    {
        private readonly EyeshadowMakeupSettings _eyeshadowSettings;
        private readonly EyeshadowMakeupSceneReferences _eyeshadowSceneReferences;

        public EyeshadowToolFlow(
            EyeshadowMakeupSettings eyeshadowSettings,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences,
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
            _eyeshadowSettings = eyeshadowSettings;
            _eyeshadowSceneReferences = eyeshadowSceneReferences;
        }

        public override Collider2D FaceZone => _eyeshadowSceneReferences.FaceZone;

        public override MakeupProcessStageType DragStageType => MakeupProcessStageType.DraggingEyeshadowBrushToFace;

        public override MakeupProcessStageType DragFallbackStageType =>
            MakeupProcessStageType.WaitingForEyeshadowBrushDragStart;

        public override async UniTask ApplyAsync()
        {
            if (RuntimeState.ProcessStageType != DragStageType)
                return;

            if (RuntimeState.SelectedEyeshadowColorIndex < 0)
            {
                RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowColorSelection;
                return;
            }

            if (_eyeshadowSceneReferences.LeftEyeApplyPoint == null ||
                _eyeshadowSceneReferences.RightEyeApplyPoint == null)
            {
                return;
            }

            RuntimeState.ProcessStageType = MakeupProcessStageType.ApplyingEyeshadow;

            float moveToFaceDuration =
                MotionSettings.AutomaticMoveDuration * _eyeshadowSettings.MoveToFaceDurationMultiplier;

            await HandMotionFacade.MoveEyeshadowBrushTipToAsync(
                _eyeshadowSceneReferences.LeftEyeApplyPoint.position,
                moveToFaceDuration);
            await HandMotionFacade.PlayBrushDipAnimationAsync();

            if (PlayerFaceStateView != null)
            {
                await PlayerFaceStateView.ShowLeftEyeshadowAsync(
                    RuntimeState.SelectedEyeshadowColorIndex,
                    MotionSettings.ApplyDuration);
            }

            await HandMotionFacade.MoveEyeshadowBrushTipToAsync(
                _eyeshadowSceneReferences.RightEyeApplyPoint.position,
                moveToFaceDuration);
            await HandMotionFacade.PlayBrushDipAnimationAsync();

            if (PlayerFaceStateView != null)
            {
                await PlayerFaceStateView.ShowRightEyeshadowAsync(
                    RuntimeState.SelectedEyeshadowColorIndex,
                    MotionSettings.ApplyDuration);
            }

            await FinishApplyAndReturnAsync(
                MakeupProcessStageType.ReturningEyeshadowBrush,
                MakeupProcessStageType.WaitingForEyeshadowColorSelection);
        }

        public async UniTask SelectColorAsync(EyeshadowPaletteColorView colorView, int colorIndex)
        {
            bool canSelectColor =
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForEyeshadowColorSelection ||
                RuntimeState.ProcessStageType == MakeupProcessStageType.WaitingForEyeshadowBrushDragStart;

            if (canSelectColor == false || colorView == null || colorView.BrushDipPoint == null)
                return;

            PrepareForManualStep();

            if (RuntimeState.ActiveToolType != MakeupToolType.EyeshadowBrush)
                await TakeBrushForColorSelectionAsync();

            RuntimeState.SelectedEyeshadowColorIndex = colorIndex;
            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingEyeshadowBrushToColor;

            await HandMotionFacade.MoveEyeshadowBrushTipToAsync(
                colorView.BrushDipPoint.position,
                MotionSettings.AutomaticMoveDuration);
            await HandMotionFacade.PlayBrushDipAnimationAsync();

            VisualState.ApplyEyeshadowBrushTipColor(colorView);

            await MoveToHoldPointOrEnterWaitingStageAsync(
                _eyeshadowSceneReferences.BrushChestHoldPoint,
                MakeupProcessStageType.MovingEyeshadowBrushToChestHoldPoint,
                MakeupProcessStageType.WaitingForEyeshadowBrushDragStart);
        }

        private async UniTask TakeBrushForColorSelectionAsync()
        {
            if (_eyeshadowSceneReferences.BrushPickupPoint == null)
                return;

            RuntimeState.ProcessStageType = MakeupProcessStageType.MovingHandToEyeshadowBrush;

            await HandMotionFacade.MoveHandToAsync(
                _eyeshadowSceneReferences.BrushPickupPoint.position,
                MotionSettings.AutomaticMoveDuration);

            VisualState.SetEyeshadowBrushInHandVisible(true);
            VisualState.SetEyeshadowBrushStandVisible(false);
            RuntimeState.ActiveToolType = MakeupToolType.EyeshadowBrush;

            RuntimeState.ProcessStageType = MakeupProcessStageType.WaitingForEyeshadowColorSelection;
        }
    }
}