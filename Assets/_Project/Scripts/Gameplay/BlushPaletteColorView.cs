using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class BlushPaletteColorView : MonoBehaviour
    {
        [SerializeField] private int _colorIndex;
        [SerializeField] private Collider2D _tapZone;
        [SerializeField] private SpriteRenderer _colorRenderer;
        [SerializeField] private Transform _brushDipPoint;
        [SerializeField] private Color _brushTipColor = Color.white;

        public int ColorIndex => _colorIndex;
        public Collider2D TapZone => _tapZone;
        public SpriteRenderer ColorRenderer => _colorRenderer;
        public Transform BrushDipPoint => _brushDipPoint;
        public Color BrushTipColor => _brushTipColor;
    }
}