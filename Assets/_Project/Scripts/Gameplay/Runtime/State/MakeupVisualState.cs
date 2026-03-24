using UnityEngine;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class MakeupVisualState
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly CreamMakeupConfig _creamConfig;
        private readonly BlushMakeupConfig _blushConfig;
        private readonly EyeshadowMakeupConfig _eyeshadowConfig;
        private readonly MakeupRuntimeState _runtimeState;

        public MakeupVisualState(
            MakeupHandConfig handConfig,
            CreamMakeupConfig creamConfig,
            BlushMakeupConfig blushConfig,
            EyeshadowMakeupConfig eyeshadowConfig,
            MakeupRuntimeState runtimeState)
        {
            _handConfig = handConfig;
            _creamConfig = creamConfig;
            _blushConfig = blushConfig;
            _eyeshadowConfig = eyeshadowConfig;
            _runtimeState = runtimeState;
        }
        
        public bool IsCreamInHandVisible() =>
            _handConfig.CreamInHandVisual != null && _handConfig.CreamInHandVisual.activeSelf;

        public bool IsBrushInHandVisible() =>
            _handConfig.BrushInHandRenderer != null && _handConfig.BrushInHandRenderer.gameObject.activeSelf;

        public bool IsLipstickInHandVisible() =>
            _handConfig.LipstickInHandRenderer != null && _handConfig.LipstickInHandRenderer.gameObject.activeSelf;

        public bool IsEyeshadowBrushInHandVisible() =>
            _handConfig.EyeshadowBrushInHandRenderer != null &&
            _handConfig.EyeshadowBrushInHandRenderer.gameObject.activeSelf;

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

            _runtimeState.SelectedBlushColorIndex = -1;
            _runtimeState.SelectedLipstickColorIndex = -1;
            _runtimeState.SelectedEyeshadowColorIndex = -1;
            _runtimeState.SelectedLipstickView = null;
            _runtimeState.DragVelocity = Vector3.zero;
            _runtimeState.ProcessStageType = MakeupProcessStageType.Idle;
        }

        public void SetCreamInHandVisible(bool isVisible)
        {
            _handConfig.CreamInHandVisual?.SetActive(isVisible);
        }

        public void SetCreamStandVisible(bool isVisible)
        {
            _creamConfig.CreamStandVisual?.SetActive(isVisible);
        }

        public void SetBrushInHandVisible(bool isVisible)
        {
            _handConfig.BrushInHandRenderer?.gameObject.SetActive(isVisible);
        }

        public void SetBlushBrushStandVisible(bool isVisible)
        {
            _blushConfig.BrushStandRenderer?.gameObject.SetActive(isVisible);
        }

        public void SetLipstickInHandVisible(bool isVisible)
        {
            _handConfig.LipstickInHandRenderer?.gameObject.SetActive(isVisible);
        }

        public void SetEyeshadowBrushInHandVisible(bool isVisible)
        {
            _handConfig.EyeshadowBrushInHandRenderer?.gameObject.SetActive(isVisible);
        }

        public void SetEyeshadowBrushStandVisible(bool isVisible)
        {
            _eyeshadowConfig.BrushStandRenderer?.gameObject.SetActive(isVisible);
        }

        public void SetLipstickBookVisualVisible(LipstickPaletteColorView lipstickView, bool isVisible)
        {
            lipstickView?.SetBookLipstickVisible(isVisible);
        }

        public void HideAllBookBrushStands()
        {
            SetBlushBrushStandVisible(false);
            SetEyeshadowBrushStandVisible(false);
        }

        public void ResetBrushTipColor()
        {
            if (_handConfig.BrushTipRenderer != null)
                _handConfig.BrushTipRenderer.color = _handConfig.DefaultBrushTipColor;
        }

        public void ApplyBrushTipColor(BlushPaletteColorView colorView)
        {
            if (_handConfig.BrushTipRenderer == null || colorView == null)
                return;

            Color brushTipColor = colorView.BrushTipColor;
            brushTipColor.a = 1f;
            _handConfig.BrushTipRenderer.color = brushTipColor;
        }

        public void ApplyLipstickInHandSprite(LipstickPaletteColorView colorView)
        {
            if (_handConfig.LipstickInHandRenderer == null || colorView == null)
                return;

            _handConfig.LipstickInHandRenderer.sprite = colorView.LipstickInHandSprite;
        }

        public void ResetLipstickInHandSprite()
        {
            if (_handConfig.LipstickInHandRenderer == null)
                return;

            _handConfig.LipstickInHandRenderer.sprite = _handConfig.DefaultLipstickInHandSprite;
        }

        public void ApplyEyeshadowBrushTipColor(EyeshadowPaletteColorView colorView)
        {
            if (_handConfig.EyeshadowBrushTipRenderer == null || colorView == null)
                return;

            Color brushTipColor = colorView.BrushTipColor;
            brushTipColor.a = 1f;
            _handConfig.EyeshadowBrushTipRenderer.color = brushTipColor;
        }

        public void ResetEyeshadowBrushTipColor()
        {
            if (_handConfig.EyeshadowBrushTipRenderer != null)
                _handConfig.EyeshadowBrushTipRenderer.color = _handConfig.DefaultEyeshadowBrushTipColor;
        }

        public void MoveHandToDefaultPointImmediately()
        {
            if (_handConfig.HandRoot == null || _handConfig.HandDefaultPoint == null)
                return;

            _handConfig.HandRoot.position = _handConfig.HandDefaultPoint.position;
        }
    }
}