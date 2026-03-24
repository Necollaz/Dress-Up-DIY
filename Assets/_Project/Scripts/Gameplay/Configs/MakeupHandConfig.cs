using System;
using UnityEngine;

namespace _Project.Gameplay
{
    [Serializable]
    public sealed class MakeupHandConfig
    {
        [SerializeField] private Transform _handRoot;
        [SerializeField] private Transform _handDefaultPoint;
        [SerializeField] private Transform _brushTipPoint;
        [SerializeField] private Transform _eyeshadowBrushTipPoint;
        [SerializeField] private Collider2D _handDragZone;

        [Header("Cream"), Space(10)]
        [SerializeField] private GameObject _creamInHandVisual;
        
        [Header("Blush"), Space(10)]
        [SerializeField] private SpriteRenderer _brushInHandRenderer;
        [SerializeField] private SpriteRenderer _brushTipRenderer;
        [SerializeField] private Color _defaultBrushTipColor = Color.white;
        
        [Header("Lipstick"), Space(10)]
        [SerializeField] private Transform _lipstickApplyPoint;
        [SerializeField] private SpriteRenderer _lipstickInHandRenderer;
        [SerializeField] private Sprite _defaultLipstickInHandSprite;
        
        [Header("Eyeshadow"), Space(10)]
        [SerializeField] private SpriteRenderer _eyeshadowBrushInHandRenderer;
        [SerializeField] private SpriteRenderer _eyeshadowBrushTipRenderer;
        [SerializeField] private Color _defaultEyeshadowBrushTipColor = Color.white;
        
        public Transform HandRoot => _handRoot;
        public Transform HandDefaultPoint => _handDefaultPoint;
        public Transform BrushTipPoint => _brushTipPoint;
        public Transform EyeshadowBrushTipPoint => _eyeshadowBrushTipPoint;
        public Collider2D HandDragZone => _handDragZone;

        public GameObject CreamInHandVisual => _creamInHandVisual;
        
        public SpriteRenderer BrushInHandRenderer => _brushInHandRenderer;
        public SpriteRenderer BrushTipRenderer => _brushTipRenderer;
        public Color DefaultBrushTipColor => _defaultBrushTipColor;
        
        public Transform LipstickApplyPoint => _lipstickApplyPoint;
        public SpriteRenderer LipstickInHandRenderer => _lipstickInHandRenderer;
        public Sprite DefaultLipstickInHandSprite => _defaultLipstickInHandSprite;
        
        public SpriteRenderer EyeshadowBrushInHandRenderer => _eyeshadowBrushInHandRenderer;
        public SpriteRenderer EyeshadowBrushTipRenderer => _eyeshadowBrushTipRenderer;
        public Color DefaultEyeshadowBrushTipColor => _defaultEyeshadowBrushTipColor;
    }
}