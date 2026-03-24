using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.Input;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.ToolFlow;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Interaction
{
    public sealed class MakeupStageInputFlow
    {
        private readonly ProfilerMarker _dragMarker = new("Makeup.Drag");
        private readonly ProfilerMarker _colorSelectionMarker = new("Makeup.ColorSelection");

        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupPointerInput _pointerInput;
        private readonly MakeupActionSequencer _actionSequencer;
        private readonly MakeupHandConfig _handConfig;
        private readonly HandMotionFacade _handMotionFacade;
        private readonly CreamToolFlow _creamToolFlow;
        private readonly BlushToolFlow _blushToolFlow;
        private readonly LipstickToolFlow _lipstickToolFlow;
        private readonly EyeshadowToolFlow _eyeshadowToolFlow;

        private readonly Dictionary<MakeupProcessStageType, Func<bool>> _pressActionsByStage;
        private readonly Dictionary<MakeupProcessStageType, IToolFlowContract> _dragFlowsByStage;

        public MakeupStageInputFlow(
            MakeupRuntimeState runtimeState,
            MakeupPointerInput pointerInput,
            MakeupActionSequencer actionSequencer,
            MakeupHandConfig handConfig,
            HandMotionFacade handMotionFacade,
            CreamToolFlow creamToolFlow,
            BlushToolFlow blushToolFlow,
            LipstickToolFlow lipstickToolFlow,
            EyeshadowToolFlow eyeshadowToolFlow)
        {
            _runtimeState = runtimeState;
            _pointerInput = pointerInput;
            _actionSequencer = actionSequencer;
            _handConfig = handConfig;
            _handMotionFacade = handMotionFacade;
            _creamToolFlow = creamToolFlow;
            _blushToolFlow = blushToolFlow;
            _lipstickToolFlow = lipstickToolFlow;
            _eyeshadowToolFlow = eyeshadowToolFlow;

            _pressActionsByStage = new Dictionary<MakeupProcessStageType, Func<bool>>
            {
                { MakeupProcessStageType.WaitingForCreamDragStart, ProcessCreamDragStartInput },

                { MakeupProcessStageType.WaitingForBlushColorSelection, ProcessBlushPaletteColorInput },
                { MakeupProcessStageType.WaitingForBrushDragStart, ProcessBrushWaitingStageInput },

                { MakeupProcessStageType.WaitingForLipstickSelection, ProcessLipstickPaletteColorInput },
                { MakeupProcessStageType.WaitingForLipstickDragStart, ProcessLipstickWaitingStageInput },

                { MakeupProcessStageType.WaitingForEyeshadowColorSelection, ProcessEyeshadowPaletteColorInput },
                { MakeupProcessStageType.WaitingForEyeshadowBrushDragStart, ProcessEyeshadowWaitingStageInput },
            };

            _dragFlowsByStage = new Dictionary<MakeupProcessStageType, IToolFlowContract>
            {
                { _creamToolFlow.DragStageType, _creamToolFlow },
                { _blushToolFlow.DragStageType, _blushToolFlow },
                { _lipstickToolFlow.DragStageType, _lipstickToolFlow },
                { _eyeshadowToolFlow.DragStageType, _eyeshadowToolFlow },
            };
        }

        public bool ProcessPressInput()
        {
            if (_pressActionsByStage.TryGetValue(_runtimeState.ProcessStageType, out Func<bool> pressAction) == false)
                return false;

            return pressAction.Invoke();
        }

        public void ProcessDragInput()
        {
            if (_dragFlowsByStage.TryGetValue(_runtimeState.ProcessStageType, out IToolFlowContract flow) == false)
                return;

            using (_dragMarker.Auto())
            {
                if (_pointerInput.IsPointerHeld())
                {
                    if (_pointerInput.TryGetPointerWorldPosition(out Vector3 pointerWorldPosition))
                        _handMotionFacade.UpdateDraggedHandPosition(pointerWorldPosition);
                }

                if (_pointerInput.IsPointerReleasedThisFrame() == false)
                    return;

                if (_pointerInput.TryGetPointerWorldPosition(out Vector3 releaseWorldPosition) == false)
                {
                    _runtimeState.ProcessStageType = flow.DragFallbackStageType;
                    return;
                }

                bool isReleasedInsideFaceZone =
                    flow.FaceZone != null &&
                    flow.FaceZone.OverlapPoint(releaseWorldPosition);

                if (isReleasedInsideFaceZone == false)
                {
                    _runtimeState.ProcessStageType = flow.DragFallbackStageType;
                    return;
                }

                bool isStarted = _actionSequencer.TryRun(flow.ApplyAsync);

                if (isStarted == false)
                    _runtimeState.ProcessStageType = flow.DragFallbackStageType;
            }
        }

        private bool ProcessCreamDragStartInput()
        {
            return TryStartDragForStage(MakeupProcessStageType.DraggingCreamToFace);
        }

        private bool ProcessBlushPaletteColorInput()
        {
            using (_colorSelectionMarker.Auto())
            {
                if (_pointerInput.TryGetPaletteColor(
                        out BlushPaletteColorView selectedColorView,
                        out int selectedColorIndex) == false)
                {
                    return false;
                }

                return _actionSequencer.TryRun(() =>
                    _blushToolFlow.SelectColorAsync(selectedColorView, selectedColorIndex));
            }
        }

        private bool ProcessBrushWaitingStageInput()
        {
            if (ProcessBlushPaletteColorInput())
                return true;

            return TryStartDragForStage(MakeupProcessStageType.DraggingBrushToFace);
        }

        private bool ProcessLipstickPaletteColorInput()
        {
            using (_colorSelectionMarker.Auto())
            {
                if (_pointerInput.TryGetLipstickSample(
                        out LipstickPaletteColorView selectedLipstickView,
                        out int selectedLipstickIndex) == false)
                {
                    return false;
                }

                return _actionSequencer.TryRun(() =>
                    _lipstickToolFlow.SelectLipstickAsync(selectedLipstickView, selectedLipstickIndex));
            }
        }

        private bool ProcessLipstickWaitingStageInput()
        {
            if (ProcessLipstickPaletteColorInput())
                return true;

            return TryStartDragForStage(MakeupProcessStageType.DraggingLipstickToFace);
        }

        private bool ProcessEyeshadowPaletteColorInput()
        {
            using (_colorSelectionMarker.Auto())
            {
                if (_pointerInput.TryGetEyeshadowColor(
                        out EyeshadowPaletteColorView selectedColorView,
                        out int selectedColorIndex) == false)
                {
                    return false;
                }

                return _actionSequencer.TryRun(() =>
                    _eyeshadowToolFlow.SelectColorAsync(selectedColorView, selectedColorIndex));
            }
        }

        private bool ProcessEyeshadowWaitingStageInput()
        {
            if (ProcessEyeshadowPaletteColorInput())
                return true;

            return TryStartDragForStage(MakeupProcessStageType.DraggingEyeshadowBrushToFace);
        }

        private bool TryStartDragForStage(MakeupProcessStageType draggingStageType)
        {
            if (_pointerInput.ContainsColliderUnderPointer(_handConfig.HandDragZone) == false)
                return false;

            _runtimeState.ProcessStageType = draggingStageType;
            _runtimeState.DragVelocity = Vector3.zero;

            return true;
        }
    }
}