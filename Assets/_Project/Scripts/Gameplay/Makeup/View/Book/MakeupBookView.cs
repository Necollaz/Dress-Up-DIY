using UnityEngine;
using _Project.Gameplay.Makeup.Data;

namespace _Project.Gameplay.Makeup.View
{
    public sealed class MakeupBookView : MonoBehaviour
    {
        [SerializeField] private MakeupBookTabView[] _tabViews;

        private void Awake()
        {
            SelectPage(MakeupBookPageType.None);
        }

        public void SelectPage(MakeupBookPageType pageType)
        {
            for (int index = 0; index < _tabViews.Length; index++)
            {
                MakeupBookTabView tabView = _tabViews[index];

                if (tabView == null)
                    continue;

                bool isSelected = tabView.PageType == pageType;
                tabView.SetSelected(isSelected);
            }
        }

        public bool TryGetTab(
            Collider2D[] collidersUnderPointer,
            int collidersUnderPointerCount,
            out MakeupBookTabView selectedTabView)
        {
            for (int colliderIndex = 0; colliderIndex < collidersUnderPointerCount; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int tabIndex = 0; tabIndex < _tabViews.Length; tabIndex++)
                {
                    MakeupBookTabView tabView = _tabViews[tabIndex];

                    if (tabView == null)
                        continue;

                    if (tabView.TapZone == currentCollider)
                    {
                        selectedTabView = tabView;

                        return true;
                    }
                }
            }

            selectedTabView = null;

            return false;
        }
    }
}