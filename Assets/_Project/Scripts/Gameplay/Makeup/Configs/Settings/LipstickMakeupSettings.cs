using UnityEngine;

namespace _Project.Gameplay.Makeup.Configs.Settings
{
    [CreateAssetMenu(fileName = "LipstickMakeupSettings", menuName = "Project/Gameplay/Makeup/LipstickMakeupSettings")]
    public sealed class LipstickMakeupSettings : ScriptableObject
    {
        [SerializeField] private float _moveToFaceDurationMultiplier = 0.5f;

        public float MoveToFaceDurationMultiplier => _moveToFaceDurationMultiplier;
    }
}