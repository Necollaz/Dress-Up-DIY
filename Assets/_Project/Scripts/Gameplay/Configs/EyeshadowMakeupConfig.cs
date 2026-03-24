using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class EyeshadowMakeupConfig
    {
        [SerializeField] private EyeshadowPaletteColorView[] _paletteColors;
        [SerializeField] private Collider2D _brushStandTapZone;
        [SerializeField] private SpriteRenderer _brushStandRenderer;
        [SerializeField] private Collider2D _faceZone;

        [Space(10)]
        [SerializeField] private Transform _brushPickupPoint;
        [SerializeField] private Transform _brushChestHoldPoint;

        [Space(10)]
        [SerializeField] private Transform _leftEyeApplyPoint;
        [SerializeField] private Transform _rightEyeApplyPoint;

        public EyeshadowPaletteColorView[] PaletteColors => _paletteColors;
        public Collider2D BrushStandTapZone => _brushStandTapZone;
        public SpriteRenderer BrushStandRenderer => _brushStandRenderer;
        public Collider2D FaceZone => _faceZone;

        public Transform BrushPickupPoint => _brushPickupPoint;
        public Transform BrushChestHoldPoint => _brushChestHoldPoint;

        public Transform LeftEyeApplyPoint => _leftEyeApplyPoint;
        public Transform RightEyeApplyPoint => _rightEyeApplyPoint;
    }
}