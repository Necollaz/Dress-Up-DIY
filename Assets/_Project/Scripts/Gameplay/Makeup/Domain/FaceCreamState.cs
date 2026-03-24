using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Domain
{
    public sealed class FaceCreamState
    {
        private const float HIDDEN_ALPHA = 0f;
        private const float VISIBLE_ALPHA = 1f;

        private readonly SpriteRenderer _bodyWithAcneRenderer;
        private readonly SpriteRenderer _bodyClearRenderer;
        private readonly SpriteRenderer[] _acneRenderers;
        private readonly FaceRendererAlpha _faceRendererAlpha;

        public FaceCreamState(
            SpriteRenderer bodyWithAcneRenderer,
            SpriteRenderer bodyClearRenderer,
            SpriteRenderer[] acneRenderers,
            FaceRendererAlpha faceRendererAlpha)
        {
            _bodyWithAcneRenderer = bodyWithAcneRenderer;
            _bodyClearRenderer = bodyClearRenderer;
            _acneRenderers = acneRenderers;
            _faceRendererAlpha = faceRendererAlpha;
        }

        public void Reset()
        {
            if (_bodyWithAcneRenderer == null || _bodyClearRenderer == null)
                return;

            _faceRendererAlpha.SetAlpha(_bodyWithAcneRenderer, VISIBLE_ALPHA);
            _faceRendererAlpha.SetAlpha(_bodyClearRenderer, HIDDEN_ALPHA);

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] == null)
                    continue;

                _faceRendererAlpha.SetAlpha(_acneRenderers[index], VISIBLE_ALPHA);
            }
        }

        public async UniTask HideAcneAsync(float duration)
        {
            if (_bodyWithAcneRenderer == null || _bodyClearRenderer == null)
                return;

            if (duration <= 0f)
            {
                _faceRendererAlpha.SetAlpha(_bodyWithAcneRenderer, HIDDEN_ALPHA);
                _faceRendererAlpha.SetAlpha(_bodyClearRenderer, VISIBLE_ALPHA);

                for (int index = 0; index < _acneRenderers.Length; index++)
                {
                    if (_acneRenderers[index] == null)
                        continue;

                    _faceRendererAlpha.SetAlpha(_acneRenderers[index], HIDDEN_ALPHA);
                }

                return;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float acneAlpha = 1f - progress;
                float clearAlpha = progress;

                _faceRendererAlpha.SetAlpha(_bodyWithAcneRenderer, acneAlpha);
                _faceRendererAlpha.SetAlpha(_bodyClearRenderer, clearAlpha);

                for (int index = 0; index < _acneRenderers.Length; index++)
                {
                    if (_acneRenderers[index] == null)
                        continue;

                    _faceRendererAlpha.SetAlpha(_acneRenderers[index], acneAlpha);
                }

                await UniTask.Yield();
            }

            _faceRendererAlpha.SetAlpha(_bodyWithAcneRenderer, HIDDEN_ALPHA);
            _faceRendererAlpha.SetAlpha(_bodyClearRenderer, VISIBLE_ALPHA);

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] == null)
                    continue;

                _faceRendererAlpha.SetAlpha(_acneRenderers[index], HIDDEN_ALPHA);
            }
        }
    }
}