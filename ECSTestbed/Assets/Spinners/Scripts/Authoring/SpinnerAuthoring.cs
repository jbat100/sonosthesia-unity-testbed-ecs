using UnityEngine;
using Unity.Entities;

public class SpinnerAuthoring : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed;
    public float offset;
    
    public class Baker : Baker<SpinnerAuthoring>
    {
        public override void Bake(SpinnerAuthoring authoring)
        {
            var bakingEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(bakingEntity, new SpinnerConfig()
            {
                Axis = authoring.axis,
                Speed = authoring.speed,
                Offset = authoring.offset
            });
            
            AddComponent(bakingEntity, new SpinnerData()
            {
                Angle = 0f
            });
        }
    }
}