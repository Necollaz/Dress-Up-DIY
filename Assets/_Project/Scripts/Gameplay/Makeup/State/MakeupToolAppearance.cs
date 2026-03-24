using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.State
{
    public sealed class MakeupToolAppearance
    {
        private readonly MakeupHandConfig _handConfig;

        public MakeupToolAppearance(MakeupHandConfig handConfig)
        {
            _handConfig = handConfig;
        }

        public void ResetBrushTipColor()
        {
            SetRendererColorIfChanged(_handConfig.BrushTipRenderer, _handConfig.DefaultBrushTipColor);
        }

        public void ApplyBrushTipColor(BlushPaletteColorView colorView)
        {
            if (_handConfig.BrushTipRenderer == null || colorView == null)
                return;

            Color brushTipColor = colorView.BrushTipColor;
            brushTipColor.a = 1f;

            SetRendererColorIfChanged(_handConfig.BrushTipRenderer, brushTipColor);
        }

        public void ApplyLipstickInHandSprite(LipstickPaletteColorView colorView)
        {
            if (_handConfig.LipstickInHandRenderer == null || colorView == null)
                return;

            if (_handConfig.LipstickInHandRenderer.sprite == colorView.LipstickInHandSprite)
                return;

            _handConfig.LipstickInHandRenderer.sprite = colorView.LipstickInHandSprite;
        }

        public void ResetLipstickInHandSprite()
        {
            if (_handConfig.LipstickInHandRenderer == null)
                return;

            if (_handConfig.LipstickInHandRenderer.sprite == _handConfig.DefaultLipstickInHandSprite)
                return;

            _handConfig.LipstickInHandRenderer.sprite = _handConfig.DefaultLipstickInHandSprite;
        }

        public void ApplyEyeshadowBrushTipColor(EyeshadowPaletteColorView colorView)
        {
            if (_handConfig.EyeshadowBrushTipRenderer == null || colorView == null)
                return;

            Color brushTipColor = colorView.BrushTipColor;
            brushTipColor.a = 1f;

            SetRendererColorIfChanged(_handConfig.EyeshadowBrushTipRenderer, brushTipColor);
        }

        public void ResetEyeshadowBrushTipColor()
        {
            SetRendererColorIfChanged(
                _handConfig.EyeshadowBrushTipRenderer,
                _handConfig.DefaultEyeshadowBrushTipColor);
        }

        private void SetRendererColorIfChanged(SpriteRenderer spriteRenderer, Color targetColor)
        {
            if (spriteRenderer == null || spriteRenderer.color == targetColor)
                return;

            spriteRenderer.color = targetColor;
        }
    }
}