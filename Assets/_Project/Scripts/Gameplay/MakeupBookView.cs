using UnityEngine;

namespace _Project.Gameplay
{
    public sealed class MakeupBookView : MonoBehaviour
    {
        [SerializeField] private MakeupBookTabView[] _bookTabs;

        private void Awake()
        {
            SelectPage(MakeupBookPageType.None);
        }

        public void SelectPage(MakeupBookPageType pageType)
        {
            for (int index = 0; index < _bookTabs.Length; index++)
            {
                MakeupBookTabView bookTab = _bookTabs[index];

                if (bookTab == null)
                    continue;

                bool isSelected = bookTab.PageType == pageType;
                bookTab.SetSelected(isSelected);
            }
        }

        public bool TryGetTabByWorldPoint(Vector3 worldPoint, out MakeupBookTabView selectedBookTab)
        {
            Collider2D[] collidersUnderPointer = Physics2D.OverlapPointAll(worldPoint);

            for (int colliderIndex = 0; colliderIndex < collidersUnderPointer.Length; colliderIndex++)
            {
                Collider2D currentCollider = collidersUnderPointer[colliderIndex];

                for (int tabIndex = 0; tabIndex < _bookTabs.Length; tabIndex++)
                {
                    MakeupBookTabView bookTab = _bookTabs[tabIndex];

                    if (bookTab == null)
                        continue;

                    if (bookTab.TapZone == currentCollider)
                    {
                        selectedBookTab = bookTab;
                        
                        return true;
                    }
                }
            }

            selectedBookTab = null;
            
            return false;
        }
    }
}