using UnityEngine;

namespace _Project.Gameplay.Makeup.Configs.Settings
{
    [CreateAssetMenu(
        fileName = "EyeshadowMakeupSettings", 
        menuName = "Project/Gameplay/Makeup/EyeshadowMakeupSettings")]
    public sealed class EyeshadowMakeupSettings : ScriptableObject
    {
        [SerializeField] private float _moveToFaceDurationMultiplier = 0.5f;

        public float MoveToFaceDurationMultiplier => _moveToFaceDurationMultiplier;
    }
}