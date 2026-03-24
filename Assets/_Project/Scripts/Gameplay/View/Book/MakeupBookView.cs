using UnityEngine;

namespace _Project.Gameplay
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

        public bool TryGetTab(Vector3 worldPoint, out MakeupBookTabView selectedTabView)
        {
            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(worldPoint);

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
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