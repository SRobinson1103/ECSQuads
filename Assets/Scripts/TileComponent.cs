using Unity.Entities;
using Unity.Mathematics;

public struct TileComponent : IComponentData
{
    public float3 Position;
}

public struct TileTag : IComponentData { }
