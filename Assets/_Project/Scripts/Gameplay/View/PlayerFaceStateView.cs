using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay
{
    public sealed class PlayerFaceStateView : MonoBehaviour
    {
        private const float HIDDEN_ALPHA = 0f;
        private const float VISIBLE_ALPHA = 1f;

        [Header("Cream")]
        [SerializeField] private SpriteRenderer _bodyWithAcneRenderer;
        [SerializeField] private SpriteRenderer _bodyClearRenderer;
        [SerializeField] private SpriteRenderer[] _acneRenderers;

        [Header("Blush")]
        [SerializeField] private SpriteRenderer[] _blushRenderers;
        
        [Header("Lipstick")]
        [SerializeField] private SpriteRenderer[] _lipstickRenderers;

        private void Awake()
        {
            ResetFaceState();
        }

        public void ResetFaceState()
        {
            ResetCreamState();
            HideAllBlush();
            HideAllLipstick();
        }

        public async UniTask HideAcneAsync(float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);
                float acneAlpha = 1f - progress;
                float clearAlpha = progress;

                if (_bodyWithAcneRenderer == null || _bodyClearRenderer == null)
                    return;
                
                SetRendererAlpha(_bodyWithAcneRenderer, acneAlpha);
                SetRendererAlpha(_bodyClearRenderer, clearAlpha);

                for (int index = 0; index < _acneRenderers.Length; index++)
                {
                    if (_acneRenderers[index] == null)
                        continue;

                    SetRendererAlpha(_acneRenderers[index], acneAlpha);
                }

                await UniTask.Yield();
            }

            if (_bodyWithAcneRenderer == null || _bodyClearRenderer == null)
                return;
            
            SetRendererAlpha(_bodyWithAcneRenderer, HIDDEN_ALPHA);
            SetRendererAlpha(_bodyClearRenderer, VISIBLE_ALPHA);

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] == null)
                    continue;

                SetRendererAlpha(_acneRenderers[index], HIDDEN_ALPHA);
            }
        }

        public async UniTask ShowBlushAsync(int colorIndex, float duration)
        {
            if (TryGetBlushRenderer(colorIndex, out SpriteRenderer selectedBlushRenderer) == false)
                return;

            HideAllBlush();

            selectedBlushRenderer.enabled = true;
            selectedBlushRenderer.gameObject.SetActive(true);

            Color startColor = selectedBlushRenderer.color;
            startColor.a = HIDDEN_ALPHA;
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
            finalColor.a = VISIBLE_ALPHA;
            selectedBlushRenderer.color = finalColor;
        }
        
        public async UniTask ShowLipstickAsync(int colorIndex, float duration)
        {
            if (TryGetLipstickRenderer(colorIndex, out SpriteRenderer selectedLipstickRenderer) == false)
                return;

            HideAllLipstick();

            selectedLipstickRenderer.enabled = true;
            selectedLipstickRenderer.gameObject.SetActive(true);

            Color startColor = selectedLipstickRenderer.color;
            startColor.a = HIDDEN_ALPHA;
            selectedLipstickRenderer.color = startColor;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float progress = Mathf.Clamp01(elapsedTime / duration);

                Color currentColor = selectedLipstickRenderer.color;
                currentColor.a = progress;
                selectedLipstickRenderer.color = currentColor;

                await UniTask.Yield();
            }

            Color finalColor = selectedLipstickRenderer.color;
            finalColor.a = VISIBLE_ALPHA;
            selectedLipstickRenderer.color = finalColor;
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
        
        private bool TryGetLipstickRenderer(int colorIndex, out SpriteRenderer lipstickRenderer)
        {
            if (colorIndex < 0 || colorIndex >= _lipstickRenderers.Length)
            {
                lipstickRenderer = null;
                return false;
            }

            lipstickRenderer = _lipstickRenderers[colorIndex];
            return lipstickRenderer != null;
        }
        
        private void ResetCreamState()
        {
            if (_bodyWithAcneRenderer == null || _bodyClearRenderer == null)
                return;
            
            SetRendererAlpha(_bodyWithAcneRenderer, VISIBLE_ALPHA);
            SetRendererAlpha(_bodyClearRenderer, HIDDEN_ALPHA);

            for (int index = 0; index < _acneRenderers.Length; index++)
            {
                if (_acneRenderers[index] == null)
                    continue;

                SetRendererAlpha(_acneRenderers[index], VISIBLE_ALPHA);
            }
        }
        
        private void HideAllBlush()
        {
            for (int index = 0; index < _blushRenderers.Length; index++)
            {
                SpriteRenderer blushRenderer = _blushRenderers[index];

                if (blushRenderer == null)
                    continue;

                blushRenderer.enabled = false;

                Color hiddenColor = blushRenderer.color;
                hiddenColor.a = HIDDEN_ALPHA;
                blushRenderer.color = hiddenColor;
            }
        }
        
        private void HideAllLipstick()
        {
            for (int index = 0; index < _lipstickRenderers.Length; index++)
            {
                SpriteRenderer lipstickRenderer = _lipstickRenderers[index];

                if (lipstickRenderer == null)
                    continue;

                lipstickRenderer.enabled = false;

                Color hiddenColor = lipstickRenderer.color;
                hiddenColor.a = HIDDEN_ALPHA;
                lipstickRenderer.color = hiddenColor;
            }
        }

        private void SetRendererAlpha(SpriteRenderer spriteRenderer, float alpha)
        {
            if (spriteRenderer == null)
                return;

            Color rendererColor = spriteRenderer.color;
            rendererColor.a = alpha;
            spriteRenderer.color = rendererColor;
        }
    }
}