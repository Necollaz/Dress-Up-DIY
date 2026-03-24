using DG.Tweening;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.Hand;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.State
{
    public sealed class MakeupVisualState
    {
        private readonly MakeupRuntimeState _runtimeState;
        private readonly MakeupToolVisibility _toolVisibility;
        private readonly MakeupToolAppearance _toolAppearance;
        private readonly HandDefaultPosePlacement _handDefaultPosePlacement;

        public MakeupVisualState(
            MakeupRuntimeState runtimeState,
            MakeupToolVisibility toolVisibility,
            MakeupToolAppearance toolAppearance,
            HandDefaultPosePlacement handDefaultPosePlacement)
        {
            _runtimeState = runtimeState;
            _toolVisibility = toolVisibility;
            _toolAppearance = toolAppearance;
            _handDefaultPosePlacement = handDefaultPosePlacement;
        }

        public bool IsCreamInHandVisible() => _toolVisibility.IsCreamInHandVisible();

        public bool IsBrushInHandVisible() => _toolVisibility.IsBrushInHandVisible();

        public bool IsLipstickInHandVisible() => _toolVisibility.IsLipstickInHandVisible();

        public bool IsEyeshadowBrushInHandVisible() => _toolVisibility.IsEyeshadowBrushInHandVisible();

        public void ResetActiveToolState()
        {
            _runtimeState.ActiveSequence?.Kill();

            SetCreamInHandVisible(false);
            SetCreamStandVisible(true);
            SetBrushInHandVisible(false);
            SetBlushBrushStandVisible(false);
            ResetBrushTipColor();
            SetLipstickInHandVisible(false);
            ResetLipstickInHandSprite();
            SetEyeshadowBrushInHandVisible(false);
            SetEyeshadowBrushStandVisible(false);
            ResetEyeshadowBrushTipColor();

            _runtimeState.SelectedLipstickView?.SetBookLipstickVisible(true);

            MoveHandToDefaultPointImmediately();

            _runtimeState.ResetTransientState();
            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
        }

        public void SetCreamInHandVisible(bool isVisible)
        {
            _toolVisibility.SetCreamInHandVisible(isVisible);
        }

        public void SetCreamStandVisible(bool isVisible)
        {
            _toolVisibility.SetCreamStandVisible(isVisible);
        }

        public void SetBrushInHandVisible(bool isVisible)
        {
            _toolVisibility.SetBrushInHandVisible(isVisible);
        }

        public void SetBlushBrushStandVisible(bool isVisible)
        {
            _toolVisibility.SetBlushBrushStandVisible(isVisible);
        }

        public void SetLipstickInHandVisible(bool isVisible)
        {
            _toolVisibility.SetLipstickInHandVisible(isVisible);
        }

        public void SetEyeshadowBrushInHandVisible(bool isVisible)
        {
            _toolVisibility.SetEyeshadowBrushInHandVisible(isVisible);
        }

        public void SetEyeshadowBrushStandVisible(bool isVisible)
        {
            _toolVisibility.SetEyeshadowBrushStandVisible(isVisible);
        }

        public void SetLipstickBookVisualVisible(LipstickPaletteColorView lipstickView, bool isVisible)
        {
            _toolVisibility.SetLipstickBookVisualVisible(lipstickView, isVisible);
        }

        public void HideAllBookBrushStands()
        {
            _toolVisibility.HideAllBookBrushStands();
        }

        public void ResetBrushTipColor()
        {
            _toolAppearance.ResetBrushTipColor();
        }

        public void ApplyBrushTipColor(BlushPaletteColorView colorView)
        {
            _toolAppearance.ApplyBrushTipColor(colorView);
        }

        public void ApplyLipstickInHandSprite(LipstickPaletteColorView colorView)
        {
            _toolAppearance.ApplyLipstickInHandSprite(colorView);
        }

        public void ResetLipstickInHandSprite()
        {
            _toolAppearance.ResetLipstickInHandSprite();
        }

        public void ApplyEyeshadowBrushTipColor(EyeshadowPaletteColorView colorView)
        {
            _toolAppearance.ApplyEyeshadowBrushTipColor(colorView);
        }

        public void ResetEyeshadowBrushTipColor()
        {
            _toolAppearance.ResetEyeshadowBrushTipColor();
        }

        public void MoveHandToDefaultPointImmediately()
        {
            _handDefaultPosePlacement.MoveHandToDefaultPointImmediately();
        }
    }
}