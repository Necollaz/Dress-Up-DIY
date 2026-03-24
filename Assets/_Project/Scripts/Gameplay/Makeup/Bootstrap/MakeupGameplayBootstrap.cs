using System;
using UnityEngine;
using DG.Tweening;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
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

        public MakeupGameplayBootstrap(
            Camera mainCamera,
            MakeupBookView makeupBookView,
            PlayerFaceStateView playerFaceStateView,
            MakeupHandConfig handConfig,
            SpongeMakeupConfig spongeConfig,
            CreamMakeupSceneReferences creamSceneReferences,
            BlushMakeupSceneReferences blushSceneReferences,
            LipstickMakeupSceneReferences lipstickSceneReferences,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences,
            MakeupMotionSettings motionSettings,
            CreamMakeupSettings creamSettings,
            BlushMakeupSettings blushSettings,
            LipstickMakeupSettings lipstickSettings,
            EyeshadowMakeupSettings eyeshadowSettings)
        {
            _playerFaceStateView = playerFaceStateView;

            RuntimeState = new MakeupRuntimeState();
            ActionSequencer = new MakeupActionSequencer();
            PointerInputSource = new MakeupPointerInputSource(mainCamera);
            MakeupPointerWorldHit pointerWorldHit = new MakeupPointerWorldHit();
            MakeupPointerSelection pointerSelection = new MakeupPointerSelection(
                makeupBookView,
                blushSceneReferences,
                lipstickSceneReferences,
                eyeshadowSceneReferences);
            PointerInput = new MakeupPointerInput(PointerInputSource, pointerWorldHit, pointerSelection);
            MakeupToolVisibility toolVisibility = new MakeupToolVisibility(
                handConfig,
                creamSceneReferences,
                blushSceneReferences,
                eyeshadowSceneReferences);
            MakeupToolAppearance toolAppearance = new MakeupToolAppearance(handConfig);
            HandDefaultPosePlacement handDefaultPosePlacement = new HandDefaultPosePlacement(handConfig);
            VisualState = new MakeupVisualState(
                RuntimeState,
                toolVisibility,
                toolAppearance,
                handDefaultPosePlacement);
            HandPositionTween handPositionTween = new HandPositionTween(handConfig, motionSettings);
            HandGestureTween handGestureTween = new HandGestureTween(handConfig, motionSettings, RuntimeState);
            HandDragSmoothing handDragSmoothing = new HandDragSmoothing(handConfig, motionSettings, RuntimeState);
            HandMotionFacade = new HandMotionFacade(handPositionTween, handGestureTween, handDragSmoothing);
            ToolReturnSequence = new ActiveToolReturnSequence(
                creamSceneReferences,
                blushSceneReferences,
                eyeshadowSceneReferences,
                motionSettings,
                RuntimeState,
                VisualState,
                HandMotionFacade);
            CreamToolFlow = new CreamToolFlow(
                creamSettings,
                creamSceneReferences,
                motionSettings,
                playerFaceStateView,
                RuntimeState,
                VisualState,
                HandMotionFacade,
                ToolReturnSequence);
            BlushToolFlow = new BlushToolFlow(
                blushSettings,
                blushSceneReferences,
                motionSettings,
                playerFaceStateView,
                RuntimeState,
                VisualState,
                HandMotionFacade,
                ToolReturnSequence);
            LipstickToolFlow = new LipstickToolFlow(
                lipstickSettings,
                lipstickSceneReferences,
                motionSettings,
                playerFaceStateView,
                RuntimeState,
                VisualState,
                HandMotionFacade,
                ToolReturnSequence);
            EyeshadowToolFlow = new EyeshadowToolFlow(
                eyeshadowSettings,
                eyeshadowSceneReferences,
                motionSettings,
                playerFaceStateView,
                RuntimeState,
                VisualState,
                HandMotionFacade,
                ToolReturnSequence);
            TapFlow = new MakeupTapFlow(
                RuntimeState,
                PointerInput,
                ActionSequencer,
                makeupBookView,
                playerFaceStateView,
                spongeConfig,
                creamSettings,
                creamSceneReferences,
                ToolReturnSequence,
                CreamToolFlow);
            StageInputFlow = new MakeupStageInputFlow(
                RuntimeState,
                PointerInput,
                ActionSequencer,
                handConfig,
                HandMotionFacade,
                CreamToolFlow,
                BlushToolFlow,
                LipstickToolFlow,
                EyeshadowToolFlow);
        }

        public MakeupRuntimeState RuntimeState { get; }
        public MakeupActionSequencer ActionSequencer { get; }
        public MakeupPointerInputSource PointerInputSource { get; }
        public MakeupPointerInput PointerInput { get; }
        public MakeupVisualState VisualState { get; }
        public HandMotionFacade HandMotionFacade { get; }
        public ActiveToolReturnSequence ToolReturnSequence { get; }
        public CreamToolFlow CreamToolFlow { get; }
        public BlushToolFlow BlushToolFlow { get; }
        public LipstickToolFlow LipstickToolFlow { get; }
        public EyeshadowToolFlow EyeshadowToolFlow { get; }
        public MakeupTapFlow TapFlow { get; }
        public MakeupStageInputFlow StageInputFlow { get; }

        public void Dispose()
        {
            RuntimeState.ActiveSequence?.Kill();
            PointerInputSource?.Dispose();
        }

        public void ApplyInitialState()
        {
            RuntimeState.ProcessStageType = MakeupProcessStageType.Idle;
            RuntimeState.OpenedBookPageType = MakeupBookPageType.None;
            RuntimeState.ActiveToolType = MakeupToolType.None;

            VisualState.ResetActiveToolState();
            ToolReturnSequence.ApplyIdleVisualStateForOpenedPage();
            _playerFaceStateView?.ResetFaceState();
        }
    }
}