using UnityEngine;
using Unity.Profiling;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Boostrap
{
    public sealed class MakeupGameplay : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private MakeupBookView _makeupBookView;
        [SerializeField] private PlayerFaceStateView _playerFaceStateView;

        [Header("Configs")]
        [SerializeField] private MakeupHandConfig _handConfig;
        [SerializeField] private SpongeMakeupConfig _spongeConfig;
        [SerializeField] private CreamMakeupSceneReferences _creamSceneReferences;
        [SerializeField] private BlushMakeupSceneReferences _blushSceneReferences;
        [SerializeField] private LipstickMakeupSceneReferences _lipstickSceneReferences;
        [SerializeField] private EyeshadowMakeupSceneReferences _eyeshadowSceneReferences;

        [Header("Settings")]
        [SerializeField] private MakeupMotionSettings _motionSettings;
        [SerializeField] private CreamMakeupSettings _creamSettings;
        [SerializeField] private BlushMakeupSettings _blushSettings;
        [SerializeField] private LipstickMakeupSettings _lipstickSettings;
        [SerializeField] private EyeshadowMakeupSettings _eyeshadowSettings;

        private readonly ProfilerMarker _pointerFrameMarker = new("Makeup.PointerFrame");

        private MakeupGameplayBootstrap _bootstrap;

        private void Awake()
        {
            _bootstrap = new MakeupGameplayBootstrap(
                _mainCamera,
                _makeupBookView,
                _playerFaceStateView,
                _handConfig,
                _spongeConfig,
                _creamSceneReferences,
                _blushSceneReferences,
                _lipstickSceneReferences,
                _eyeshadowSceneReferences,
                _motionSettings,
                _creamSettings,
                _blushSettings,
                _lipstickSettings,
                _eyeshadowSettings);

            _bootstrap.ApplyInitialState();
        }

        private void OnDestroy()
        {
            _bootstrap?.Dispose();
        }

        private void Update()
        {
            if (_bootstrap == null)
                return;

            using (_pointerFrameMarker.Auto())
                _bootstrap.PointerInput.BeginFrame();

            if (_bootstrap.PointerInput.IsPointerPressAllowedThisFrame())
            {
                bool isInputConsumed = _bootstrap.TapFlow.ProcessTapInput();

                if (isInputConsumed == false)
                    _bootstrap.StageInputFlow.ProcessPressInput();
            }

            _bootstrap.StageInputFlow.ProcessDragInput();
        }
    }
}