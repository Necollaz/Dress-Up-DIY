using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class MakeupBookViewNew : MonoBehaviour
    {
        [SerializeField] private MakeupBookTabViewNew[] _tabViews;

        private void Awake()
        {
            SelectPage(MakeupBookPageType.None);
        }

        public void SelectPage(MakeupBookPageType pageType)
        {
            for (int index = 0; index < _tabViews.Length; index++)
            {
                MakeupBookTabViewNew tabViewNew = _tabViews[index];

                if (tabViewNew == null)
                    continue;

                bool isSelected = tabViewNew.PageType == pageType;
                
                tabViewNew.SetSelected(isSelected);
            }
        }

        public bool TryGetTab(Vector3 worldPoint, out MakeupBookTabViewNew selectedTabViewNew)
        {
            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(worldPoint);

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int tabIndex = 0; tabIndex < _tabViews.Length; tabIndex++)
                {
                    MakeupBookTabViewNew tabViewNew = _tabViews[tabIndex];

                    if (tabViewNew == null)
                        continue;

                    if (tabViewNew.TapZone == currentCollider)
                    {
                        selectedTabViewNew = tabViewNew;
                        
                        return true;
                    }
                }
            }

            selectedTabViewNew = null;
            
            return false;
        }
    }
}