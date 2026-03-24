using Cysharp.Threading.Tasks;
using DG.Tweening;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.State;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.ToolFlow
{
    public sealed class ActiveToolReturnSequence
    {
        private readonly CreamMakeupSceneReferences _creamSceneReferences;
        private readonly BlushMakeupSceneReferences _blushSceneReferences;
        private readonly EyeshadowMakeupSceneReferences _eyeshadowSceneReferences;
        private readonly MakeupMotionSettings _motionSettings;
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupVisualState _visualState;
        private readonly HandMotionFacade _handMotionFacade;

        public ActiveToolReturnSequence(
            CreamMakeupSceneReferences creamSceneReferences,
            BlushMakeupSceneReferences blushSceneReferences,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences,
            MakeupMotionSettings motionSettings,
            MakeupRuntimeState runtimeState,
            MakeupVisualState visualState,
            HandMotionFacade handMotionFacade)
        {
            _creamSceneReferences = creamSceneReferences;
            _blushSceneReferences = blushSceneReferences;
            _eyeshadowSceneReferences = eyeshadowSceneReferences;
            _motionSettings = motionSettings;
            _runtimeState = runtimeState;
            _visualState = visualState;
            _handMotionFacade = handMotionFacade;
        }

        public async UniTask ReturnActiveToolAsync()
        {
            _runtimeState.ActiveSequence?.Kill();

            switch (_runtimeState.ActiveToolType)
            {
                case MakeupToolType.Cream:
                    await ReturnCreamAsync();
                    break;

                case MakeupToolType.BlushBrush:
                    await ReturnBlushBrushAsync();
                    break;

                case MakeupToolType.Lipstick:
                    await ReturnLipstickAsync();
                    break;

                case MakeupToolType.EyeshadowBrush:
                    await ReturnEyeshadowBrushAsync();
                    break;

                default:
                    ApplyIdleVisualStateForOpenedPage();
                    _visualState.MoveHandToDefaultPointImmediately();
                    break;
            }

            _runtimeState.ActiveToolType = MakeupToolType.None;
            _runtimeState.ResetTransientState();
        }

        public void ApplyIdleVisualStateForOpenedPage()
        {
            _visualState.SetCreamStandVisible(true);
            _visualState.HideAllBookBrushStands();

            if (_runtimeState.OpenedBookPageType == MakeupBookPageType.Blush)
                _visualState.SetBlushBrushStandVisible(true);

            if (_runtimeState.OpenedBookPageType == MakeupBookPageType.Eyeshadow)
                _visualState.SetEyeshadowBrushStandVisible(true);
        }

        private async UniTask ReturnCreamAsync()
        {
            if (_visualState.IsCreamInHandVisible() && _creamSceneReferences.CreamPickupPoint != null)
            {
                await _handMotionFacade.MoveHandToAsync(
                    _creamSceneReferences.CreamPickupPoint.position,
                    _motionSettings.AutomaticMoveDuration);
            }

            _visualState.SetCreamInHandVisible(false);
            _visualState.SetCreamStandVisible(true);

            await _handMotionFacade.MoveHandToDefaultPointAsync();

            ApplyIdleVisualStateForOpenedPage();
        }

        private async UniTask ReturnBlushBrushAsync()
        {
            if (_visualState.IsBrushInHandVisible() && _blushSceneReferences.BrushPickupPoint != null)
            {
                await _handMotionFacade.MoveHandToAsync(
                    _blushSceneReferences.BrushPickupPoint.position,
                    _motionSettings.AutomaticMoveDuration);
            }

            _visualState.SetBrushInHandVisible(false);
            _visualState.ResetBrushTipColor();

            await _handMotionFacade.MoveHandToDefaultPointAsync();

            ApplyIdleVisualStateForOpenedPage();
        }

        private async UniTask ReturnLipstickAsync()
        {
            LipstickPaletteColorView selectedLipstickView = _runtimeState.SelectedLipstickView;

            if (_visualState.IsLipstickInHandVisible() &&
                selectedLipstickView != null &&
                selectedLipstickView.PickupPoint != null)
            {
                await _handMotionFacade.MoveHandToAsync(
                    selectedLipstickView.PickupPoint.position,
                    _motionSettings.AutomaticMoveDuration);
            }

            if (selectedLipstickView != null)
                _visualState.SetLipstickBookVisualVisible(selectedLipstickView, true);

            _visualState.SetLipstickInHandVisible(false);
            _visualState.ResetLipstickInHandSprite();

            await _handMotionFacade.MoveHandToDefaultPointAsync();

            ApplyIdleVisualStateForOpenedPage();
        }

        private async UniTask ReturnEyeshadowBrushAsync()
        {
            if (_visualState.IsEyeshadowBrushInHandVisible() && _eyeshadowSceneReferences.BrushPickupPoint != null)
            {
                await _handMotionFacade.MoveHandToAsync(
                    _eyeshadowSceneReferences.BrushPickupPoint.position,
                    _motionSettings.AutomaticMoveDuration);
            }

            _visualState.SetEyeshadowBrushInHandVisible(false);
            _visualState.ResetEyeshadowBrushTipColor();

            await _handMotionFacade.MoveHandToDefaultPointAsync();

            ApplyIdleVisualStateForOpenedPage();
        }
    }
}