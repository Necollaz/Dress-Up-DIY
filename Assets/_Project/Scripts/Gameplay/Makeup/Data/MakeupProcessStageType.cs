namespace _Project.Gameplay.Makeup.Data
{
    public enum MakeupProcessStageType
    {
        Idle = 0,

        MovingHandToCream = 1,
        MovingHandToCreamDragStartPoint = 2,
        WaitingForCreamDragStart = 3,
        DraggingCreamToFace = 4,
        ApplyingCream = 5,
        ReturningCream = 6,

        WaitingForBrushSelection = 7,
        MovingHandToBrush = 8,
        WaitingForBlushColorSelection = 9,
        MovingBrushToColor = 10,
        MovingBrushToChestHoldPoint = 11,
        WaitingForBrushDragStart = 12,
        DraggingBrushToFace = 13,
        ApplyingBlush = 14,
        ReturningBrush = 15,

        ReturningToolBeforeSwitch = 16,
        
        WaitingForLipstickSelection = 17,
        MovingHandToLipstick = 18,
        MovingLipstickToChestHoldPoint = 19,
        WaitingForLipstickDragStart = 20,
        DraggingLipstickToFace = 21,
        ApplyingLipstick = 22,
        ReturningLipstick = 23,
        
        WaitingForEyeshadowBrushSelection = 24,
        MovingHandToEyeshadowBrush = 25,
        WaitingForEyeshadowColorSelection = 26,
        MovingEyeshadowBrushToColor = 27,
        MovingEyeshadowBrushToChestHoldPoint = 28,
        WaitingForEyeshadowBrushDragStart = 29,
        DraggingEyeshadowBrushToFace = 30,
        ApplyingEyeshadow = 31,
        ReturningEyeshadowBrush = 32,
    }
}