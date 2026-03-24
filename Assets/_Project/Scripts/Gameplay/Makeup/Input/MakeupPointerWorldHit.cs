using UnityEngine;

namespace _Project.Gameplay.Makeup.Input
{
    public sealed class MakeupPointerWorldHit
    {
        private const int DEFAULT_COLLIDER_BUFFER_SIZE = 16;

        private Collider2D[] _collidersUnderPointerBuffer = new Collider2D[DEFAULT_COLLIDER_BUFFER_SIZE];
        
        private int _collidersUnderPointerCount;

        public Collider2D[] CollidersUnderPointerBuffer => _collidersUnderPointerBuffer;
        public int CollidersUnderPointerCount => _collidersUnderPointerCount;

        public bool ContainsCollider(Collider2D targetCollider)
        {
            if (targetCollider == null)
                return false;

            for (int colliderIndex = 0; colliderIndex < _collidersUnderPointerCount; colliderIndex++)
            {
                if (_collidersUnderPointerBuffer[colliderIndex] == targetCollider)
                    return true;
            }

            return false;
        }
        
        public void Update(Vector3 pointerWorldPosition)
        {
            _collidersUnderPointerCount =
                Physics2D.OverlapPointNonAlloc(pointerWorldPosition, _collidersUnderPointerBuffer);

            if (_collidersUnderPointerCount < _collidersUnderPointerBuffer.Length)
                return;

            int expandedBufferSize = _collidersUnderPointerBuffer.Length * 2;
            _collidersUnderPointerBuffer = new Collider2D[expandedBufferSize];

            _collidersUnderPointerCount =
                Physics2D.OverlapPointNonAlloc(pointerWorldPosition, _collidersUnderPointerBuffer);
        }
        
        public void Clear()
        {
            _collidersUnderPointerCount = 0;
        }
    }
}