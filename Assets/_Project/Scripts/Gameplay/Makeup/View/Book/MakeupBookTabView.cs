using UnityEngine;
using _Project.Gameplay.Makeup.Data;

namespace _Project.Gameplay.Makeup.View
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
            SetGameObjectActiveIfChanged(
                _inactiveTabRenderer != null ? _inactiveTabRenderer.gameObject : null,
                isSelected == false);
            SetGameObjectActiveIfChanged(
                _activeTabRenderer != null ? _activeTabRenderer.gameObject : null, isSelected);
            SetGameObjectActiveIfChanged(_pageRoot != null ? _pageRoot.gameObject : null, isSelected);
        }

        private void SetGameObjectActiveIfChanged(GameObject targetObject, bool isActive)
        {
            if (targetObject == null || targetObject.activeSelf == isActive)
                return;

            targetObject.SetActive(isActive);
        }
    }
}