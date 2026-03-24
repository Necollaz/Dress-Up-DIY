using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class LipstickPaletteColorView : MonoBehaviour
    {
        [SerializeField] private Collider2D _tapZone;
        [SerializeField] private Transform _pickupPoint;
        [SerializeField] private GameObject _bookLipstickVisual;
        [SerializeField] private Sprite _lipstickInHandSprite;

        public Collider2D TapZone => _tapZone;
        public Transform PickupPoint => _pickupPoint;
        public Sprite LipstickInHandSprite => _lipstickInHandSprite;
        
        public void SetBookLipstickVisible(bool isVisible)
        {
            _bookLipstickVisual?.SetActive(isVisible);
        }
    }
}