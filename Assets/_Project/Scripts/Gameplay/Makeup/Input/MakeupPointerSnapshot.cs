using UnityEngine;

namespace _Project.Gameplay.Makeup.Input
{
    public readonly struct MakeupPointerSnapshot
    {
        public readonly Vector3 WorldPosition;

        public readonly bool IsPressedThisFrame;
        public readonly bool IsHeld;
        public readonly bool IsReleasedThisFrame;
        public readonly bool IsPointerOverUi;
        public readonly bool HasWorldPosition;

        public MakeupPointerSnapshot(
            Vector3 worldPosition,
            bool isPressedThisFrame,
            bool isHeld,
            bool isReleasedThisFrame,
            bool isPointerOverUi,
            bool hasWorldPosition)
        {
            WorldPosition = worldPosition;
            IsPressedThisFrame = isPressedThisFrame;
            IsHeld = isHeld;
            IsReleasedThisFrame = isReleasedThisFrame;
            IsPointerOverUi = isPointerOverUi;
            HasWorldPosition = hasWorldPosition;
        }
    }
}