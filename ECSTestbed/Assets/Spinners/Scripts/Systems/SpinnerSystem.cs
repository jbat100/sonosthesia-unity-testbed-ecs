using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpinnerSystem : ISystem
{
    private const bool PARALLEL = true;
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (PARALLEL)
        {
            state.Dependency = new SpinnerSystemJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel(state.Dependency);
        }
        else
        {
            // To query a dynamic buffer you can use RefRO or RefRW, you just have to reference it directly.
            // Note that this implies the dynamic buffer is always accessed as Read/Write !
            foreach (var (configRO, dataRW, localTransformRW) in SystemAPI.Query<RefRO<SpinnerConfig>, RefRW<SpinnerData>, RefRW<LocalTransform>>())
            {
                // This is a different syntax to avoid repeating ValueRO and ValueRW everytime, making the code a bit cleaner
                ref readonly SpinnerConfig config = ref configRO.ValueRO;
                ref LocalTransform localTransform = ref localTransformRW.ValueRW;
                ref SpinnerData data = ref dataRW.ValueRW;
                data.Angle += SystemAPI.Time.DeltaTime * config.Speed;
                localTransform.Rotation = quaternion.AxisAngle(config.Axis, data.Angle + config.Offset);
            }    
        }
    }
    
    [BurstCompile]
    private partial struct SpinnerSystemJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        public void Execute(in SpinnerConfig config, ref SpinnerData data, ref LocalTransform transform)
        {
            data.Angle += DeltaTime * config.Speed;
            transform.Rotation = quaternion.AxisAngle(config.Axis, data.Angle + config.Offset);
        }
    }
}