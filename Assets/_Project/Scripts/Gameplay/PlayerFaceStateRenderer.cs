using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay
{
    public sealed class PlayerFaceStateRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bodyWithAcneRenderer;
        [SerializeField] private SpriteRenderer _bodyClearRenderer;
        [SerializeField] private SpriteRenderer[] _acneRenderers;

        private Color _bodyWithAcneColor;
        private Color _bodyClearColor;
        private Color[] _acneColors;

        private void Awake()
        {
            if (_bodyWithAcneRenderer != null)
            {
                _bodyWithAcneColor = _bodyWithAcneRenderer.color;
            }

            if (_bodyClearRenderer != null)
            {
                _bodyClearColor = _bodyClearRenderer.color;
                SetRendererAlpha(_bodyClearRenderer, 0f);
            }

            _acneColors = new Color[_acneRenderers.Length];

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                _acneColors[index] = _acneRenderers[index].color;
            }
        }

        public void ResetState()
        {
            if (_bodyWithAcneRenderer != null)
            {
                SetRendererAlpha(_bodyWithAcneRenderer, 1f);
            }

            if (_bodyClearRenderer != null)
            {
                SetRendererAlpha(_bodyClearRenderer, 0f);
            }

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] != null)
                {
                    SetRendererAlpha(_acneRenderers[index], 1f);
                }
            }
        }

        public async UniTask HideAcneAsync(float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float acneAlpha = 1f - progress;
                float cleanAlpha = progress;

                if (_bodyWithAcneRenderer != null)
                {
                    SetRendererAlpha(_bodyWithAcneRenderer, acneAlpha);
                }

                if (_bodyClearRenderer != null)
                {
                    SetRendererAlpha(_bodyClearRenderer, cleanAlpha);
                }

                for (int index = 0; index < _acneRenderers.Length; index++)
                {
                    if (_acneRenderers[index] != null)
                    {
                        SetRendererAlpha(_acneRenderers[index], acneAlpha);
                    }
                }

                await UniTask.Yield();
            }

            if (_bodyWithAcneRenderer != null)
            {
                SetRendererAlpha(_bodyWithAcneRenderer, 0f);
            }

            if (_bodyClearRenderer != null)
            {
                SetRendererAlpha(_bodyClearRenderer, 1f);
            }

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] != null)
                {
                    SetRendererAlpha(_acneRenderers[index], 0f);
                }
            }
        }

        private void SetRendererAlpha(SpriteRenderer spriteRenderer, float alpha)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}