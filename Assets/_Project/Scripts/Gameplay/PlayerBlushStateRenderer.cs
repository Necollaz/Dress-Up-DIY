using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay
{
    public sealed class PlayerBlushStateRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _blushRenderers;

        private void Awake()
        {
            HideAll();
        }

        public async UniTask ShowBlushAsync(int colorIndex, float duration)
        {
            if (TryGetBlushRenderer(colorIndex, out SpriteRenderer selectedBlushRenderer) == false)
                return;

            HideAll();

            selectedBlushRenderer.gameObject.SetActive(true);
            selectedBlushRenderer.enabled = true;

            Color startColor = selectedBlushRenderer.color;
            startColor.a = 0f;
            selectedBlushRenderer.color = startColor;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);

                Color currentColor = selectedBlushRenderer.color;
                currentColor.a = progress;
                selectedBlushRenderer.color = currentColor;

                await UniTask.Yield();
            }

            Color finalColor = selectedBlushRenderer.color;
            finalColor.a = 1f;
            selectedBlushRenderer.color = finalColor;
        }

        public void SetBlushImmediately(int colorIndex)
        {
            if (TryGetBlushRenderer(colorIndex, out SpriteRenderer selectedBlushRenderer) == false)
                return;

            HideAll();

            selectedBlushRenderer.gameObject.SetActive(true);
            selectedBlushRenderer.enabled = true;

            Color finalColor = selectedBlushRenderer.color;
            finalColor.a = 1f;
            selectedBlushRenderer.color = finalColor;
        }

        public void HideAll()
        {
            for (int index = 0; index < _blushRenderers.Length; index++)
            {
                SpriteRenderer blushRenderer = _blushRenderers[index];

                if (blushRenderer == null)
                    continue;

                blushRenderer.enabled = false;

                Color hiddenColor = blushRenderer.color;
                hiddenColor.a = 0f;
                blushRenderer.color = hiddenColor;
            }
        }

        private bool TryGetBlushRenderer(int colorIndex, out SpriteRenderer blushRenderer)
        {
            if (colorIndex < 0 || colorIndex >= _blushRenderers.Length)
            {
                blushRenderer = null;
                
                return false;
            }

            blushRenderer = _blushRenderers[colorIndex];
            
            return blushRenderer != null;
        }
    }
}