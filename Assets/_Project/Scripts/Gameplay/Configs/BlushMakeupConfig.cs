using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class BlushMakeupConfig
    {
        [SerializeField] private BlushPaletteColorView[] _paletteColors;
        [SerializeField] private Collider2D _brushStandTapZone;
        [SerializeField] private SpriteRenderer _brushStandRenderer;
        [SerializeField] private Collider2D _faceZone;

        [Space(10)]
        [SerializeField] private Transform _brushPickupPoint;
        [SerializeField] private Transform _brushChestHoldPoint;
        [SerializeField] private Transform _faceApplyPoint;

        public BlushPaletteColorView[] PaletteColors => _paletteColors;
        public Collider2D BrushStandTapZone => _brushStandTapZone;
        public SpriteRenderer BrushStandRenderer => _brushStandRenderer;
        public Collider2D FaceZone => _faceZone;

        public Transform BrushPickupPoint => _brushPickupPoint;
        public Transform BrushChestHoldPoint => _brushChestHoldPoint;
        public Transform FaceApplyPoint => _faceApplyPoint;
    }
}