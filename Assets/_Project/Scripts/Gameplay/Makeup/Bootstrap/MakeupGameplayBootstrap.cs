using System;
using DG.Tweening;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.Input;
using _Project.Gameplay.Makeup.Interaction;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.ToolFlow;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Boostrap
{
    public sealed class MakeupGameplayBootstrap : IDisposable
    {
        private readonly PlayerFaceStateView _playerFaceStateView;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupActionSequencer _actionSequencer;
        private readonly MakeupPointerInputSource _pointerInputSource;
        private readonly MakeupVisualState _visualState;
        private readonly HandMotionFacade _handMotionFacade;
        private readonly ActiveToolReturnSequence _toolReturnSequence;
        private readonly CreamToolFlow _creamToolFlow;
        private readonly BlushToolFlow _blushToolFlow;
        private readonly LipstickToolFlow _lipstickToolFlow;
        private readonly EyeshadowToolFlow _eyeshadowToolFlow;

        public MakeupGameplayBootstrap(MakeupGameplayBootstrapData bootstrapData)
        {
            _playerFaceStateView = bootstrapData.PlayerFaceStateView;

            _runtimeState = new MakeupRuntimeState();
            _actionSequencer = new MakeupActionSequencer();
            _pointerInputSource = new MakeupPointerInputSource(bootstrapData.MainCamera);
            MakeupPointerWorldHit pointerWorldHit = new MakeupPointerWorldHit();
            MakeupPointerSelection pointerSelection = new MakeupPointerSelection(
                bootstrapData.MakeupBookView,
                bootstrapData.BlushSceneReferences,
                bootstrapData.LipstickSceneReferences,
                bootstrapData.EyeshadowSceneReferences);
            PointerInput = new MakeupPointerInput(_pointerInputSource, pointerWorldHit, pointerSelection);
            MakeupToolVisibility toolVisibility = new MakeupToolVisibility(
                bootstrapData.HandConfig,
                bootstrapData.CreamSceneReferences,
                bootstrapData.BlushSceneReferences,
                bootstrapData.EyeshadowSceneReferences);
            MakeupToolAppearance toolAppearance = new MakeupToolAppearance(bootstrapData.HandConfig);
            HandDefaultPosePlacement handDefaultPosePlacement = new HandDefaultPosePlacement(bootstrapData.HandConfig);
            _visualState = new MakeupVisualState(_runtimeState, toolVisibility, toolAppearance, handDefaultPosePlacement);
            HandPositionTween handPositionTween = new HandPositionTween(
                bootstrapData.HandConfig, 
                bootstrapData.MotionSettings);
            HandGestureTween handGestureTween = new HandGestureTween(
                bootstrapData.HandConfig,
                bootstrapData.MotionSettings,
                _runtimeState);
            HandDragSmoothing handDragSmoothing = new HandDragSmoothing(
                bootstrapData.HandConfig,
                bootstrapData.MotionSettings,
                _runtimeState);
            _handMotionFacade = new HandMotionFacade(
                handPositionTween,
                handGestureTween,
                handDragSmoothing);
            _toolReturnSequence = new ActiveToolReturnSequence(
                bootstrapData.CreamSceneReferences,
                bootstrapData.BlushSceneReferences,
                bootstrapData.EyeshadowSceneReferences,
                bootstrapData.MotionSettings,
                _runtimeState,
                _visualState,
                _handMotionFacade);
            _creamToolFlow = new CreamToolFlow(
                bootstrapData.CreamSettings,
                bootstrapData.CreamSceneReferences,
                bootstrapData.MotionSettings,
                bootstrapData.PlayerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotionFacade,
                _toolReturnSequence);
            _blushToolFlow = new BlushToolFlow(
                bootstrapData.BlushSettings,
                bootstrapData.BlushSceneReferences,
                bootstrapData.MotionSettings,
                bootstrapData.PlayerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotionFacade,
                _toolReturnSequence);
            _lipstickToolFlow = new LipstickToolFlow(
                bootstrapData.LipstickSettings,
                bootstrapData.LipstickSceneReferences,
                bootstrapData.MotionSettings,
                bootstrapData.PlayerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotionFacade,
                _toolReturnSequence);
            _eyeshadowToolFlow = new EyeshadowToolFlow(
                bootstrapData.EyeshadowSettings,
                bootstrapData.EyeshadowSceneReferences,
                bootstrapData.MotionSettings,
                bootstrapData.PlayerFaceStateView,
                _runtimeState,
                _visualState,
                _handMotionFacade,
                _toolReturnSequence);
            TapFlow = new MakeupTapFlow(
                _runtimeState,
                PointerInput,
                _actionSequencer,
                bootstrapData.MakeupBookView,
                bootstrapData.PlayerFaceStateView,
                bootstrapData.SpongeConfig,
                bootstrapData.CreamSettings,
                bootstrapData.CreamSceneReferences,
                _toolReturnSequence,
                _creamToolFlow);
            StageInputFlow = new MakeupStageInputFlow(
                _runtimeState,
                PointerInput,
                _actionSequencer,
                bootstrapData.HandConfig,
                _handMotionFacade,
                _creamToolFlow,
                _blushToolFlow,
                _lipstickToolFlow,
                _eyeshadowToolFlow);
        }
        
        public MakeupPointerInput PointerInput { get; }
        public MakeupTapFlow TapFlow { get; }
        public MakeupStageInputFlow StageInputFlow { get; }

        public void Dispose()
        {
            _runtimeState.ActiveSequence?.Kill();
            _pointerInputSource?.Dispose();
        }

        public void ApplyInitialState()
        {
            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
            _runtimeState.OpenedBookPageType = MakeupBookPageType.None;
            _runtimeState.ActiveToolType = MakeupToolType.None;

            _visualState.ResetActiveToolState();
            _toolReturnSequence.ApplyIdleVisualStateForOpenedPage();
            _playerFaceStateView?.ResetFaceState();
        }
    }
}