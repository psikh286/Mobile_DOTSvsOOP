using Unity.Entities;
using UnityEngine;

namespace Pathfinding.DOTS.ColorCoding
{
    public class HasColorDefinedAuthoring : MonoBehaviour
    {
        private class HasColorDefinedBaker : Baker<HasColorDefinedAuthoring>
        {
            public override void Bake(HasColorDefinedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<HasColorDefined>(entity);
                SetComponentEnabled<HasColorDefined>(entity, false);
            }
        }
    }
}