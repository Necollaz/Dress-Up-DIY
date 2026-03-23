namespace _Project.Gameplay
{
    public enum BlushInteractionStageType
    {
        Idle = 0,
        WaitingForBrushSelection = 1,
        MovingHandToBrush = 2,
        WaitingForColorSelection = 3,
        MovingHandToColor = 4,
        MovingHandToChestHoldPoint = 5,
        WaitingForHandDragStart = 6,
        DraggingHandToFace = 7,
        ApplyingBlush = 8,
        ReturningBrush = 9,
    }
}