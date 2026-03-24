using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class LipstickMakeupConfig
    {
        [SerializeField] private LipstickPaletteColorView[] _paletteColors;
        [SerializeField] private Collider2D _faceZone;

        [Space(10)]
        [SerializeField] private Transform _lipstickChestHoldPoint;

        [Space(10)]
        [SerializeField] private Transform _upperLipLeftApplyPoint;
        [SerializeField] private Transform _upperLipRightApplyPoint;
        [SerializeField] private Transform _lowerLipRightApplyPoint;
        [SerializeField] private Transform _lowerLipLeftApplyPoint;

        public LipstickPaletteColorView[] PaletteColors => _paletteColors;
        public Collider2D FaceZone => _faceZone;
        
        public Transform LipstickChestHoldPoint => _lipstickChestHoldPoint;

        public Transform UpperLipLeftApplyPoint => _upperLipLeftApplyPoint;
        public Transform UpperLipRightApplyPoint => _upperLipRightApplyPoint;
        public Transform LowerLipRightApplyPoint => _lowerLipRightApplyPoint;
        public Transform LowerLipLeftApplyPoint => _lowerLipLeftApplyPoint;
    }
}