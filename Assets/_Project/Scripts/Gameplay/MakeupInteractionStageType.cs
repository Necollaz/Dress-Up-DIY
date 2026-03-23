namespace _Project.Gameplay
{
    public enum MakeupInteractionStageType
    {
        Idle = 0,
        MovingHandToCream = 1,
        MovingHandToCreamDragStartPoint = 2,
        WaitingForHandDragStart = 3,
        DraggingHandToFace = 4,
        ApplyingCream = 5,
        ReturningHandWithCream = 6,
    }
}