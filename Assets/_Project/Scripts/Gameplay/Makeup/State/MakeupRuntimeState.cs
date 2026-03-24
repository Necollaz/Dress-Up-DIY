using UnityEngine;
using DG.Tweening;
using _Project.Gameplay.Makeup.Data;
using _Project.Gameplay.Makeup.View;

namespace _Project.Gameplay.Makeup.State
{
    public sealed class MakeupRuntimeState
    {
        public LipstickPaletteColorView SelectedLipstickView { get; set; }
        public Sequence ActiveSequence { get; set; }

        public MakeupProcessStageType ProcessStageType { get; set; }
        public MakeupBookPageType OpenedBookPageType { get; set; }
        public MakeupToolType ActiveToolType { get; set; }

        public Vector3 DragVelocity { get; set; }

        public int SelectedBlushColorIndex { get; set; } = -1;
        public int SelectedLipstickColorIndex { get; set; } = -1;
        public int SelectedEyeshadowColorIndex { get; set; } = -1;

        public void ResetTransientState()
        {
            SelectedBlushColorIndex = -1;
            SelectedLipstickColorIndex = -1;
            SelectedEyeshadowColorIndex = -1;
            SelectedLipstickView = null;
            DragVelocity = Vector3.zero;
        }

        public MakeupProcessStageType GetWaitingStageForOpenedPage()
        {
            if (OpenedBookPageType == MakeupBookPageType.Blush)
                return MakeupProcessStageType.WaitingForBlushColorSelection;

            if (OpenedBookPageType == MakeupBookPageType.Lipstick)
                return MakeupProcessStageType.WaitingForLipstickSelection;

            if (OpenedBookPageType == MakeupBookPageType.Eyeshadow)
                return MakeupProcessStageType.WaitingForEyeshadowColorSelection;

            return MakeupProcessStageType.Idle;
        }
    }
}