using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct BounceSystem : ISystem
{
    private const bool PARALLEL = true;
    
    // This attribute means that the code will be compiled using the Burst compiler
    // The burst compiler enable great performance improvement applying some 
    // optimization automatically such as loop vectorization using SIMD instructions
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (PARALLEL)
        {
            state.Dependency = new BounceSystemJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(state.Dependency);
        }
        else
        {
            // To query a dynamic buffer you can use RefRO or RefRW, you just have to reference it directly.
            // Note that this implies the dynamic buffer is always accessed as Read/Write !
            foreach (var (configRO, dataRW, localTransformRW) in SystemAPI.Query<RefRO<BounceConfig>, RefRW<BounceData>, RefRW<LocalTransform>>())
            {
                // This is a different syntax to avoid repeating ValueRO and ValueRW everytime, making the code a bit cleaner
                ref readonly BounceConfig config = ref configRO.ValueRO;
                ref LocalTransform localTransform = ref localTransformRW.ValueRW;
                ref BounceData data = ref dataRW.ValueRW;
                data.Distance += SystemAPI.Time.DeltaTime * config.Speed;
                float fraction = math.abs((data.Distance % 2f) - 1 + config.Offset);
                localTransform.Position = config.Direction * fraction;
            }
        }
    }

    [BurstCompile]
    private partial struct BounceSystemJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        public void Execute(in BounceConfig config, ref BounceData data, ref LocalTransform transform)
        {
            data.Distance += DeltaTime * config.Speed;
            float fraction = math.abs((data.Distance % 2f) - 1 + config.Offset);
            transform.Position = config.Direction * fraction;
        }
    }
}