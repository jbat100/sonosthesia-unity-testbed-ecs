using Unity.Entities;
using Unity.Mathematics;

public struct SpinnerConfig : IComponentData
{
    public float3 Axis;
    public float Speed;
    public float Offset;
}