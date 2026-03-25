using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.Configs.Settings;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Boostrap
{
    public sealed class MakeupGameplayBootstrapData
    {
        public MakeupGameplayBootstrapData(
            Camera mainCamera,
            MakeupBookView makeupBookView,
            PlayerFaceStateView playerFaceStateView,
            MakeupHandConfig handConfig,
            SpongeMakeupConfig spongeConfig,
            CreamMakeupSceneReferences creamSceneReferences,
            BlushMakeupSceneReferences blushSceneReferences,
            LipstickMakeupSceneReferences lipstickSceneReferences,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences,
            MakeupMotionSettings motionSettings,
            CreamMakeupSettings creamSettings,
            BlushMakeupSettings blushSettings,
            LipstickMakeupSettings lipstickSettings,
            EyeshadowMakeupSettings eyeshadowSettings)
        {
            MainCamera = mainCamera;
            MakeupBookView = makeupBookView;
            PlayerFaceStateView = playerFaceStateView;
            HandConfig = handConfig;
            SpongeConfig = spongeConfig;
            CreamSceneReferences = creamSceneReferences;
            BlushSceneReferences = blushSceneReferences;
            LipstickSceneReferences = lipstickSceneReferences;
            EyeshadowSceneReferences = eyeshadowSceneReferences;
            MotionSettings = motionSettings;
            CreamSettings = creamSettings;
            BlushSettings = blushSettings;
            LipstickSettings = lipstickSettings;
            EyeshadowSettings = eyeshadowSettings;
        }

        public Camera MainCamera { get; }
        public MakeupBookView MakeupBookView { get; }
        public PlayerFaceStateView PlayerFaceStateView { get; }

        public MakeupHandConfig HandConfig { get; }
        public SpongeMakeupConfig SpongeConfig { get; }

        public CreamMakeupSceneReferences CreamSceneReferences { get; }
        public BlushMakeupSceneReferences BlushSceneReferences { get; }
        public LipstickMakeupSceneReferences LipstickSceneReferences { get; }
        public EyeshadowMakeupSceneReferences EyeshadowSceneReferences { get; }

        public MakeupMotionSettings MotionSettings { get; }

        public CreamMakeupSettings CreamSettings { get; }
        public BlushMakeupSettings BlushSettings { get; }
        public LipstickMakeupSettings LipstickSettings { get; }
        public EyeshadowMakeupSettings EyeshadowSettings { get; }
    }
}