using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class BlushPaletteColorViewNew : MonoBehaviour
    {
        [SerializeField] private Collider2D _tapZone;
        [SerializeField] private Transform _brushDipPoint;
        [SerializeField] private Color _brushTipColor = Color.white;

        public Collider2D TapZone => _tapZone;
        public Transform BrushDipPoint => _brushDipPoint;
        public Color BrushTipColor => _brushTipColor;
    }
}