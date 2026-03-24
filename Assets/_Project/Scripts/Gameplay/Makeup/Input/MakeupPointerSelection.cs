using System;
using UnityEngine;
using _Project.Gameplay.Makeup.Configs.SceneRefs;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.Input
{
    public sealed class MakeupPointerSelection
    {
        private const int NOT_FOUND_INDEX = -1;

        private readonly MakeupBookView _makeupBookView;
        private readonly BlushMakeupSceneReferences _blushSceneReferences;
        private readonly LipstickMakeupSceneReferences _lipstickSceneReferences;
        private readonly EyeshadowMakeupSceneReferences _eyeshadowSceneReferences;

        public MakeupPointerSelection(
            MakeupBookView makeupBookView,
            BlushMakeupSceneReferences blushSceneReferences,
            LipstickMakeupSceneReferences lipstickSceneReferences,
            EyeshadowMakeupSceneReferences eyeshadowSceneReferences)
        {
            _makeupBookView = makeupBookView;
            _blushSceneReferences = blushSceneReferences;
            _lipstickSceneReferences = lipstickSceneReferences;
            _eyeshadowSceneReferences = eyeshadowSceneReferences;
        }

        public bool TryGetBookTab(
            Collider2D[] collidersUnderPointerBuffer,
            int collidersUnderPointerCount,
            out MakeupBookTabView selectedTabView)
        {
            selectedTabView = null;

            if (_makeupBookView == null)
                return false;

            return _makeupBookView.TryGetTab(
                collidersUnderPointerBuffer,
                collidersUnderPointerCount,
                out selectedTabView);
        }

        public bool TryGetBlushPaletteColor(
            Collider2D[] collidersUnderPointerBuffer,
            int collidersUnderPointerCount,
            out BlushPaletteColorView selectedColorView,
            out int selectedColorIndex)
        {
            return TryGetPaletteElement(
                collidersUnderPointerBuffer,
                collidersUnderPointerCount,
                _blushSceneReferences.PaletteColors,
                GetBlushTapZone,
                out selectedColorView,
                out selectedColorIndex);
        }

        public bool TryGetLipstickSample(
            Collider2D[] collidersUnderPointerBuffer,
            int collidersUnderPointerCount,
            out LipstickPaletteColorView selectedLipstickView,
            out int selectedLipstickIndex)
        {
            return TryGetPaletteElement(
                collidersUnderPointerBuffer,
                collidersUnderPointerCount,
                _lipstickSceneReferences.PaletteColors,
                GetLipstickTapZone,
                out selectedLipstickView,
                out selectedLipstickIndex);
        }

        public bool TryGetEyeshadowColor(
            Collider2D[] collidersUnderPointerBuffer,
            int collidersUnderPointerCount,
            out EyeshadowPaletteColorView selectedColorView,
            out int selectedColorIndex)
        {
            return TryGetPaletteElement(
                collidersUnderPointerBuffer,
                collidersUnderPointerCount,
                _eyeshadowSceneReferences.PaletteColors,
                GetEyeshadowTapZone,
                out selectedColorView,
                out selectedColorIndex);
        }

        private Collider2D GetBlushTapZone(BlushPaletteColorView blushPaletteColorView) =>
            blushPaletteColorView.TapZone;

        private Collider2D GetLipstickTapZone(LipstickPaletteColorView lipstickPaletteColorView) =>
            lipstickPaletteColorView.TapZone;

        private Collider2D GetEyeshadowTapZone(EyeshadowPaletteColorView eyeshadowPaletteColorView) =>
            eyeshadowPaletteColorView.TapZone;

        private bool TryGetPaletteElement<TPaletteElement>(
            Collider2D[] collidersUnderPointerBuffer,
            int collidersUnderPointerCount,
            TPaletteElement[] paletteElements,
            Func<TPaletteElement, Collider2D> tapZoneGetter,
            out TPaletteElement selectedPaletteElement,
            out int selectedPaletteElementIndex)
            where TPaletteElement : class
        {
            selectedPaletteElement = null;
            selectedPaletteElementIndex = NOT_FOUND_INDEX;

            if (paletteElements == null || tapZoneGetter == null)
                return false;

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointerCount; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointerBuffer[colliderIndex];

                for (int paletteElementIndex = 0; paletteElementIndex < paletteElements.Length; paletteElementIndex++)
                {
                    TPaletteElement paletteElement = paletteElements[paletteElementIndex];

                    if (paletteElement == null)
                        continue;

                    if (tapZoneGetter.Invoke(paletteElement) != currentCollider)
                        continue;

                    selectedPaletteElement = paletteElement;
                    selectedPaletteElementIndex = paletteElementIndex;

                    return true;
                }
            }

            return false;
        }
    }
}