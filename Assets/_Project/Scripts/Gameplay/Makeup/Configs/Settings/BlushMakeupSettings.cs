using UnityEngine;

namespace _Project.Gameplay.Makeup.Configs.Settings
{
    [CreateAssetMenu(fileName = "BlushMakeupSettings", menuName = "Project/Gameplay/Makeup/BlushMakeupSettings")]
    public sealed class BlushMakeupSettings : ScriptableObject
    {
        [SerializeField] private float _moveToFaceDurationMultiplier = 0.5f;

        public float MoveToFaceDurationMultiplier => _moveToFaceDurationMultiplier;
    }
}