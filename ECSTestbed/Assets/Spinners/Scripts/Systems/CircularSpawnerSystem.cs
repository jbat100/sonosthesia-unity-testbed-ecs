using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct CircularSpawnerSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BufferedSpawn(ref state);
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        
    }

    public void OnStopRunning(ref SystemState state)
    {
        
    }

    private void BufferedSpawn(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach(var (spawnerR0, positionRO, entity) in SystemAPI
                    .Query<RefRO<CircularSpawnData>, RefRO<LocalToWorld>>()
                    .WithAll<Simulate>()
                    .WithEntityAccess()) 
        {
            ref readonly CircularSpawnData spawner = ref spawnerR0.ValueRO;
            ref readonly LocalToWorld localToWorld = ref positionRO.ValueRO;

            float angleStep = (2 * math.PI) / spawner.Count;

            Parent rootParent = new Parent()
            {
                Value = entity
            };
            
            for (int i = 0; i < spawner.Count; i++)
            {
                float angle = angleStep * i;
                Entity spawnedEntity = ecb.Instantiate(spawner.Prefab);
                quaternion rotation = quaternion.AxisAngle(spawner.Normal, angle);
                float3 position = localToWorld.Position + math.mul(rotation, math.forward() * spawner.Radius);
                
                if (spawner.Hierarchy == SpawnerHierarchy.Nested)
                {
                    Entity intermediate = ecb.CreateEntity();
                    
                    ecb.AddComponent(intermediate, rootParent);
                    ecb.AddComponent(intermediate, new LocalToWorld());
                    ecb.AddComponent(intermediate, LocalTransform.FromPositionRotation(position, rotation));

                    Parent intermediateParent = new Parent()
                    {
                        Value = intermediate
                    };
                    
                    ecb.AddComponent(spawnedEntity, intermediateParent);
                    ecb.SetComponent(spawnedEntity, LocalTransform.Identity);
                }
                else if (spawner.Hierarchy == SpawnerHierarchy.Child)
                {
                    
                    ecb.AddComponent(spawnedEntity, rootParent);
                    ecb.SetComponent(spawnedEntity, LocalTransform.FromPositionRotation(position, rotation));
                }
                else if (spawner.Hierarchy == SpawnerHierarchy.Root)
                {
                    ecb.SetComponent(spawnedEntity, LocalTransform.FromPositionRotation(position, rotation));
                }
            }
            
            ecb.RemoveComponent<CircularSpawnData>(entity);
        }
    }
}