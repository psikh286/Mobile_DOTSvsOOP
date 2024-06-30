using Unity.Entities;
using UnityEngine;

namespace Pathfinding.DOTS.ColorCoding.SpriteApplying
{
    public class HasSpriteAppliedAuthoring : MonoBehaviour
    {
        public class HasSpriteAppliedBaker : Baker<HasSpriteAppliedAuthoring>
        {
            public override void Bake(HasSpriteAppliedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<HasSpriteApplied>(entity);
                SetComponentEnabled<HasSpriteApplied>(entity, false);
            }
        }
    }
}