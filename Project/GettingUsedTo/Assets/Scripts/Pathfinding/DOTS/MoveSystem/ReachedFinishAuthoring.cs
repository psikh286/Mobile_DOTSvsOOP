using Unity.Entities;
using UnityEngine;

namespace Pathfinding.DOTS.MoveSystem
{
    public class ReachedFinishAuthoring : MonoBehaviour
    {
        private class Baker : Baker<ReachedFinishAuthoring>
        {
            public override void Bake(ReachedFinishAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent (entity, new ReachedFinish());
                SetComponentEnabled<ReachedFinish>(entity, true);
            }
        }
    }
}