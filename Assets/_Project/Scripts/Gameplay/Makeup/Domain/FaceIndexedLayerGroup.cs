using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Domain
{
    public sealed class FaceIndexedLayerGroup
    {
        private const float VISIBLE_ALPHA = 1f;
        private const int NO_ACTIVE_LAYER_INDEX = -1;

        private readonly SpriteRenderer[] _renderers;
        private readonly FaceRendererAlpha _faceRendererAlpha;

        private int _activeLayerIndex = NO_ACTIVE_LAYER_INDEX;

        public FaceIndexedLayerGroup(SpriteRenderer[] renderers, FaceRendererAlpha faceRendererAlpha)
        {
            _renderers = renderers;
            _faceRendererAlpha = faceRendererAlpha;
        }

        public void Reset()
        {
            _faceRendererAlpha.HideAllImmediately(_renderers);
            _activeLayerIndex = NO_ACTIVE_LAYER_INDEX;
        }

        public async UniTask ShowAsync(int layerIndex, float duration)
        {
            if (TryPrepareLayer(layerIndex, out SpriteRenderer targetRenderer) == false)
                return;

            await _faceRendererAlpha.FadeToAlphaAsync(targetRenderer, VISIBLE_ALPHA, duration);
        }

        private bool TryPrepareLayer(int targetLayerIndex, out SpriteRenderer targetRenderer)
        {
            if (_faceRendererAlpha.TryGetRenderer(_renderers, targetLayerIndex, out targetRenderer) == false)
                return false;

            if (_activeLayerIndex != targetLayerIndex)
            {
                _faceRendererAlpha.HideRendererByIndex(_renderers, _activeLayerIndex);
                _activeLayerIndex = targetLayerIndex;
            }

            return true;
        }
    }
}