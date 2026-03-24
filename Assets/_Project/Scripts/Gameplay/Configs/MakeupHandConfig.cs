using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class MakeupHandConfig
    {
        [SerializeField] private Transform _handRoot;
        [SerializeField] private Transform _brushTipPoint;
        [SerializeField] private Collider2D _handDragZone;

        [Space(10)]
        [SerializeField] private GameObject _creamInHandVisual;
        [SerializeField] private SpriteRenderer _brushInHandRenderer;
        [SerializeField] private SpriteRenderer _brushTipRenderer;
        
        [Space(10)]
        [SerializeField] private Transform _lipstickApplyPoint;
        [SerializeField] private SpriteRenderer _lipstickInHandRenderer;
        [SerializeField] private Sprite _defaultLipstickInHandSprite;
        
        [Space(10)]
        [SerializeField] private Transform _handDefaultPoint;

        [Space(10)]
        [SerializeField] private Color _defaultBrushTipColor = Color.white;

        public Transform HandRoot => _handRoot;
        public Transform BrushTipPoint => _brushTipPoint;
        public Collider2D HandDragZone => _handDragZone;

        public GameObject CreamInHandVisual => _creamInHandVisual;
        public SpriteRenderer BrushInHandRenderer => _brushInHandRenderer;
        public SpriteRenderer BrushTipRenderer => _brushTipRenderer;
        
        public Transform LipstickApplyPoint => _lipstickApplyPoint;
        public SpriteRenderer LipstickInHandRenderer => _lipstickInHandRenderer;
        public Sprite DefaultLipstickInHandSprite => _defaultLipstickInHandSprite;

        public Transform HandDefaultPoint => _handDefaultPoint;

        public Color DefaultBrushTipColor => _defaultBrushTipColor;
    }
}