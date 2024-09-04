using Unity.Entities;
using Unity.Mathematics;

public struct CircularSpawnData : IComponentData
{
    public Entity Prefab;
    public int Count;
    public float Radius;
    public float3 Normal;
    public SpawnerHierarchy Hierarchy;
}