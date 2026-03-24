using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Domain
{
    public sealed class FaceRendererAlpha
    {
        private const float HIDDEN_ALPHA = 0f;

        public async UniTask FadeToAlphaAsync(SpriteRenderer targetRenderer, float targetAlpha, float duration)
        {
            if (targetRenderer == null)
                return;

            EnsureVisible(targetRenderer);

            Color rendererColor = targetRenderer.color;
            float startAlpha = rendererColor.a;

            if (duration <= 0f)
            {
                if (Mathf.Approximately(startAlpha, targetAlpha) == false)
                {
                    rendererColor.a = targetAlpha;
                    targetRenderer.color = rendererColor;
                }

                UpdateEnabledStateByAlpha(targetRenderer, targetAlpha);

                return;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

                rendererColor = targetRenderer.color;
                rendererColor.a = currentAlpha;

                if (Mathf.Approximately(targetRenderer.color.a, currentAlpha) == false)
                    targetRenderer.color = rendererColor;

                await UniTask.Yield();
            }

            rendererColor = targetRenderer.color;
            rendererColor.a = targetAlpha;
            targetRenderer.color = rendererColor;

            UpdateEnabledStateByAlpha(targetRenderer, targetAlpha);
        }

        public bool TryGetRenderer(SpriteRenderer[] renderers, int rendererIndex, out SpriteRenderer targetRenderer)
        {
            if (renderers == null || rendererIndex < 0 || rendererIndex >= renderers.Length)
            {
                targetRenderer = null;

                return false;
            }

            targetRenderer = renderers[rendererIndex];

            return targetRenderer != null;
        }
        
        public void HideImmediately(SpriteRenderer targetRenderer)
        {
            if (targetRenderer == null)
                return;

            if (targetRenderer.enabled)
                targetRenderer.enabled = false;

            SetAlpha(targetRenderer, HIDDEN_ALPHA);
        }

        public void HideAllImmediately(SpriteRenderer[] renderers)
        {
            if (renderers == null)
                return;

            for (int index = 0; index < renderers.Length; index++)
                HideImmediately(renderers[index]);
        }

        public void SetAlpha(SpriteRenderer targetRenderer, float alpha)
        {
            if (targetRenderer == null)
                return;

            Color rendererColor = targetRenderer.color;

            if (Mathf.Approximately(rendererColor.a, alpha))
                return;

            rendererColor.a = alpha;
            targetRenderer.color = rendererColor;
        }

        public void EnsureVisible(SpriteRenderer targetRenderer)
        {
            if (targetRenderer == null)
                return;

            if (targetRenderer.gameObject.activeSelf == false)
                targetRenderer.gameObject.SetActive(true);

            if (targetRenderer.enabled == false)
                targetRenderer.enabled = true;
        }

        public void HideRendererByIndex(SpriteRenderer[] renderers, int rendererIndex)
        {
            if (TryGetRenderer(renderers, rendererIndex, out SpriteRenderer targetRenderer) == false)
                return;

            HideImmediately(targetRenderer);
        }

        private void UpdateEnabledStateByAlpha(SpriteRenderer targetRenderer, float alpha)
        {
            if (targetRenderer == null)
                return;

            bool shouldBeEnabled = Mathf.Approximately(alpha, HIDDEN_ALPHA) == false;

            if (targetRenderer.enabled != shouldBeEnabled)
                targetRenderer.enabled = shouldBeEnabled;
        }
    }
}