using Unity.Entities;
using UnityEngine;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    public class RaycastSpawnDataAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        
        public class RaycastSpawnDataBaker : Baker<RaycastSpawnDataAuthoring>
        {
            public override void Bake(RaycastSpawnDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<RaycastSpawnData>(entity, new RaycastSpawnData()
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}