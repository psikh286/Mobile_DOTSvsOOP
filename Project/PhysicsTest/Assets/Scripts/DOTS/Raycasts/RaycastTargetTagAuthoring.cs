using Unity.Entities;
using UnityEngine;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    public class RaycastTargetTagAuthoring : MonoBehaviour
    {
        public class RaycastTargetTagBaker : Baker<RaycastTargetTagAuthoring>
        {
            public override void Bake(RaycastTargetTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<RaycastTargetTag>(entity);
            }
        }
    }
}