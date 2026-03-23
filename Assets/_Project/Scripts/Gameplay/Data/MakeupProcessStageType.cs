namespace _Project.Gameplay
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
    }
}