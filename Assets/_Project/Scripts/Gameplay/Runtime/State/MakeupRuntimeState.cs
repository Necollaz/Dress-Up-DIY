using UnityEngine;
using DG.Tweening;

namespace _Project.Gameplay
{
    public sealed class MakeupRuntimeState
    {
        public LipstickPaletteColorView SelectedLipstickView { get; set; }
        public Sequence ActiveSequence { get; set; }
        
        public MakeupProcessStageType ProcessStageType { get; set; }
        public Vector3 DragVelocity { get; set; }
        
        public int SelectedBlushColorIndex { get; set; } = -1;
        public int SelectedLipstickColorIndex { get; set; } = -1;
        public int SelectedEyeshadowColorIndex { get; set; } = -1;
        
        public bool IsBlushPageOpened { get; set; }
        public bool IsLipstickPageOpened { get; set; }
        public bool IsEyeshadowPageOpened { get; set; }
    }
}