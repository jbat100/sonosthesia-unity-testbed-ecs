using Unity.Entities;
using UnityEngine;

public enum SpawnerHierarchy : int
{
    Root,
    Child,
    Nested
}

public class CircularSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float radius = 1;
    public int count = 10;
    public Vector3 normal = Vector3.up;
    public SpawnerHierarchy hierarchy;
    
    public class Baker : Baker<CircularSpawnerAuthoring>
    {
        public override void Bake(CircularSpawnerAuthoring authoring)
        {
            var bakingEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Store the entity in a custom component that we'll use to instantiate later
            AddComponent(bakingEntity, new CircularSpawnData()
            {
                Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                Radius = authoring.radius,
                Count = authoring.count,
                Normal = authoring.normal,
                Hierarchy = authoring.hierarchy
            });
        }
    }
}
