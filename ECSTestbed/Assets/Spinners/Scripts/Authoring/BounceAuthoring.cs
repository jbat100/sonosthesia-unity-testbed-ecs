using UnityEngine;
using Unity.Entities;

public class BounceAuthoring : MonoBehaviour
{
    public Vector3 direction = Vector3.up;
    public float speed;
    public float offset;
    
    public class Baker : Baker<BounceAuthoring>
    {
        public override void Bake(BounceAuthoring authoring)
        {
            var bakingEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(bakingEntity, new BounceConfig()
            {
                Direction = authoring.direction,
                Speed = authoring.speed,
                Offset = authoring.offset
            });
            
            AddComponent(bakingEntity, new BounceData()
            {
                Distance = 0f
            });
        }
    }
}
