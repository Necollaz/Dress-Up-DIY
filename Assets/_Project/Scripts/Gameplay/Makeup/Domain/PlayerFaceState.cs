using Cysharp.Threading.Tasks;

namespace _Project.Gameplay.Makeup.Domain
{
    public sealed class PlayerFaceState
    {
        private readonly FaceCreamState _faceCreamState;
        private readonly FaceIndexedLayerGroup _blushLayerGroup;
        private readonly FaceIndexedLayerGroup _lipstickLayerGroup;
        private readonly FaceIndexedLayerGroup _leftEyeshadowLayerGroup;
        private readonly FaceIndexedLayerGroup _rightEyeshadowLayerGroup;

        public PlayerFaceState(
            FaceCreamState faceCreamState,
            FaceIndexedLayerGroup blushLayerGroup,
            FaceIndexedLayerGroup lipstickLayerGroup,
            FaceIndexedLayerGroup leftEyeshadowLayerGroup,
            FaceIndexedLayerGroup rightEyeshadowLayerGroup)
        {
            _faceCreamState = faceCreamState;
            _blushLayerGroup = blushLayerGroup;
            _lipstickLayerGroup = lipstickLayerGroup;
            _leftEyeshadowLayerGroup = leftEyeshadowLayerGroup;
            _rightEyeshadowLayerGroup = rightEyeshadowLayerGroup;
        }

        public void Reset()
        {
            _faceCreamState.Reset();
            _blushLayerGroup.Reset();
            _lipstickLayerGroup.Reset();
            _leftEyeshadowLayerGroup.Reset();
            _rightEyeshadowLayerGroup.Reset();
        }

        public UniTask HideAcneAsync(float duration) => _faceCreamState.HideAcneAsync(duration);

        public UniTask ShowBlushAsync(int colorIndex, float duration) => 
            _blushLayerGroup.ShowAsync(colorIndex, duration);

        public UniTask ShowLipstickAsync(int colorIndex, float duration) =>
            _lipstickLayerGroup.ShowAsync(colorIndex, duration);

        public UniTask ShowLeftEyeshadowAsync(int colorIndex, float duration) =>
            _leftEyeshadowLayerGroup.ShowAsync(colorIndex, duration);

        public UniTask ShowRightEyeshadowAsync(int colorIndex, float duration) =>
            _rightEyeshadowLayerGroup.ShowAsync(colorIndex, duration);
    }
}