using Unity.Entities;
using Unity.Mathematics;

public struct BounceConfig : IComponentData
{
    public float3 Direction;
    public float Speed;
    public float Offset;
}