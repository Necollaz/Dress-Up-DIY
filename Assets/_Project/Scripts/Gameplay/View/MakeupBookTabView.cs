using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class MakeupBookTabView : MonoBehaviour
    {
        [SerializeField] private MakeupBookPageType _pageType;
        [SerializeField] private Collider2D _tapZone;
        [SerializeField] private SpriteRenderer _inactiveTabRenderer;
        [SerializeField] private SpriteRenderer _activeTabRenderer;
        [SerializeField] private Transform _pageRoot;

        public MakeupBookPageType PageType => _pageType;
        public Collider2D TapZone => _tapZone;

        public void SetSelected(bool isSelected)
        {
            _inactiveTabRenderer?.gameObject.SetActive(isSelected == false);
            _activeTabRenderer?.gameObject.SetActive(isSelected);
            _pageRoot?.gameObject.SetActive(isSelected);
        }
    }
}