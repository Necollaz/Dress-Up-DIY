using UnityEngine;

namespace _Project.Gameplay.Makeup.Hand
{
    public readonly struct LipstickStrokePath
    {
        public readonly Vector3 UpperLipLeftPosition;
        public readonly Vector3 UpperLipRightPosition;
        public readonly Vector3 LowerLipRightPosition;
        public readonly Vector3 LowerLipLeftPosition;

        public LipstickStrokePath(
            Vector3 upperLipLeftPosition,
            Vector3 upperLipRightPosition,
            Vector3 lowerLipRightPosition,
            Vector3 lowerLipLeftPosition)
        {
            UpperLipLeftPosition = upperLipLeftPosition;
            UpperLipRightPosition = upperLipRightPosition;
            LowerLipRightPosition = lowerLipRightPosition;
            LowerLipLeftPosition = lowerLipLeftPosition;
        }
    }
}