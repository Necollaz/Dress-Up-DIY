using UnityEngine;
using _Project.Gameplay.Makeup.Data;

namespace _Project.Gameplay.Makeup.Configs.Settings
{
    [CreateAssetMenu(fileName = "CreamMakeupSettings", menuName = "Project/Gameplay/Makeup/CreamMakeupSettings")]
    public sealed class CreamMakeupSettings : ScriptableObject
    {
        [SerializeField] private MakeupProcessStageType[] _allowedStartStages =
        {
            MakeupProcessStageType.Idle,
            MakeupProcessStageType.WaitingForCreamDragStart,

            MakeupProcessStageType.WaitingForBrushSelection,
            MakeupProcessStageType.WaitingForBlushColorSelection,
            MakeupProcessStageType.WaitingForBrushDragStart,

            MakeupProcessStageType.WaitingForLipstickSelection,
            MakeupProcessStageType.WaitingForLipstickDragStart,

            MakeupProcessStageType.WaitingForEyeshadowBrushSelection,
            MakeupProcessStageType.WaitingForEyeshadowColorSelection,
            MakeupProcessStageType.WaitingForEyeshadowBrushDragStart,
        };

        [SerializeField] private float _moveToFaceDurationMultiplier = 0.5f;

        public float MoveToFaceDurationMultiplier => _moveToFaceDurationMultiplier;

        public bool CanStartFromStage(MakeupProcessStageType processStageType)
        {
            for (int index = 0; index < _allowedStartStages.Length; index++)
            {
                if (_allowedStartStages[index] == processStageType)
                    return true;
            }

            return false;
        }
    }
}