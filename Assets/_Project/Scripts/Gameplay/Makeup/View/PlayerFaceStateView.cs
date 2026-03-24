using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Gameplay.Makeup.Domain;

namespace _Project.Gameplay.Makeup.View
{
    public sealed class PlayerFaceStateView : MonoBehaviour
    {
        [Header("Cream")]
        [SerializeField] private SpriteRenderer _bodyWithAcneRenderer;
        [SerializeField] private SpriteRenderer _bodyClearRenderer;
        [SerializeField] private SpriteRenderer[] _acneRenderers;

        [Header("Blush")]
        [SerializeField] private SpriteRenderer[] _blushRenderers;
        
        [Header("Lipstick")]
        [SerializeField] private SpriteRenderer[] _lipstickRenderers;
        
        [Header("Eyeshadow")]
        [SerializeField] private SpriteRenderer[] _leftEyeshadowRenderers;
        [SerializeField] private SpriteRenderer[] _rightEyeshadowRenderers;
        
        private PlayerFaceState _playerFaceState;

        private void Awake()
        {
            FaceRendererAlpha faceRendererAlpha = new FaceRendererAlpha();
            FaceCreamState faceCreamState = new FaceCreamState(
                _bodyWithAcneRenderer,
                _bodyClearRenderer,
                _acneRenderers,
                faceRendererAlpha);
            FaceIndexedLayerGroup blushLayerGroup = new FaceIndexedLayerGroup(
                _blushRenderers,
                faceRendererAlpha);
            FaceIndexedLayerGroup lipstickLayerGroup = new FaceIndexedLayerGroup(
                _lipstickRenderers,
                faceRendererAlpha);
            FaceIndexedLayerGroup leftEyeshadowLayerGroup = new FaceIndexedLayerGroup(
                _leftEyeshadowRenderers,
                faceRendererAlpha);
            FaceIndexedLayerGroup rightEyeshadowLayerGroup = new FaceIndexedLayerGroup(
                _rightEyeshadowRenderers,
                faceRendererAlpha);
            _playerFaceState = new PlayerFaceState(
                faceCreamState,
                blushLayerGroup,
                lipstickLayerGroup,
                leftEyeshadowLayerGroup,
                rightEyeshadowLayerGroup);

            ResetFaceState();
        }

        public void ResetFaceState()
        {
            _playerFaceState?.Reset();
        }

        public UniTask HideAcneAsync(float duration)
        {
            if (_playerFaceState == null)
                return UniTask.CompletedTask;

            return _playerFaceState.HideAcneAsync(duration);
        }

        public UniTask ShowBlushAsync(int colorIndex, float duration)
        {
            if (_playerFaceState == null)
                return UniTask.CompletedTask;

            return _playerFaceState.ShowBlushAsync(colorIndex, duration);
        }

        public UniTask ShowLipstickAsync(int colorIndex, float duration)
        {
            if (_playerFaceState == null)
                return UniTask.CompletedTask;

            return _playerFaceState.ShowLipstickAsync(colorIndex, duration);
        }

        public UniTask ShowLeftEyeshadowAsync(int colorIndex, float duration)
        {
            if (_playerFaceState == null)
                return UniTask.CompletedTask;

            return _playerFaceState.ShowLeftEyeshadowAsync(colorIndex, duration);
        }

        public UniTask ShowRightEyeshadowAsync(int colorIndex, float duration)
        {
            if (_playerFaceState == null)
                return UniTask.CompletedTask;

            return _playerFaceState.ShowRightEyeshadowAsync(colorIndex, duration);
        }
    }
}