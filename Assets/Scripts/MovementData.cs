using Unity.Entities;

public struct MovementData : IComponentData
{
    public float JumpSpeed;
    public float FallSpeed;
    public float CurrentVerticalVelocity;
}
