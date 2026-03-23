using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class MakeupBookTabView : MonoBehaviour
    {
        [SerializeField] private MakeupBookPageType _pageType;
        [SerializeField] private Collider2D _tapZone;
        [SerializeField] private SpriteRenderer _inactiveTabVisual;
        [SerializeField] private SpriteRenderer _activeTabVisual;
        [SerializeField] private Transform _pageRoot;

        public Collider2D TapZone => _tapZone;
        public MakeupBookPageType PageType => _pageType;

        public void SetSelected(bool isSelected)
        {
            _inactiveTabVisual?.gameObject.SetActive(isSelected == false);
            _activeTabVisual?.gameObject.SetActive(isSelected);
            _pageRoot?.gameObject.SetActive(isSelected);
        }
    }
}