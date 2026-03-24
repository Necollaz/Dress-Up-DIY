using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.State
{
    public sealed class MakeupToolVisibility
    {
        private readonly MakeupHandConfig _handConfig;
        private readonly CreamMakeupSceneReferences _creamSceneReferences;
        private readonly BlushMakeupSceneReferences _blushSceneReferences;
        private readonly EyeshadowMakeupSceneReferences _eyeshadowSceneReferences;

        public MakeupToolVisibility(
            MakeupHandConfig handConfig,
            CreamMakeupSceneReferences creamSceneReferences,
            BlushMakeupSceneReferences blushSceneReferences,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences)
        {
            _handConfig = handConfig;
            _creamSceneReferences = creamSceneReferences;
            _blushSceneReferences = blushSceneReferences;
            _eyeshadowSceneReferences = eyeshadowSceneReferences;
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

        public void SetCreamInHandVisible(bool isVisible)
        {
            SetGameObjectActiveIfChanged(_handConfig.CreamInHandVisual, isVisible);
        }

        public void SetCreamStandVisible(bool isVisible)
        {
            SetGameObjectActiveIfChanged(_creamSceneReferences.CreamStandVisual, isVisible);
        }

        public void SetBrushInHandVisible(bool isVisible)
        {
            SetRendererGameObjectActiveIfChanged(_handConfig.BrushInHandRenderer, isVisible);
        }

        public void SetBlushBrushStandVisible(bool isVisible)
        {
            SetRendererGameObjectActiveIfChanged(_blushSceneReferences.BrushStandRenderer, isVisible);
        }

        public void SetLipstickInHandVisible(bool isVisible)
        {
            SetRendererGameObjectActiveIfChanged(_handConfig.LipstickInHandRenderer, isVisible);
        }

        public void SetEyeshadowBrushInHandVisible(bool isVisible)
        {
            SetRendererGameObjectActiveIfChanged(_handConfig.EyeshadowBrushInHandRenderer, isVisible);
        }

        public void SetEyeshadowBrushStandVisible(bool isVisible)
        {
            SetRendererGameObjectActiveIfChanged(_eyeshadowSceneReferences.BrushStandRenderer, isVisible);
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

        private void SetGameObjectActiveIfChanged(GameObject targetObject, bool isActive)
        {
            if (targetObject == null || targetObject.activeSelf == isActive)
                return;

            targetObject.SetActive(isActive);
        }

        private void SetRendererGameObjectActiveIfChanged(SpriteRenderer spriteRenderer, bool isActive)
        {
            if (spriteRenderer == null)
                return;

            SetGameObjectActiveIfChanged(spriteRenderer.gameObject, isActive);
        }
    }
}