using Unity.Entities;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    public struct RaycastSpawnData : IComponentData
    {
        public Entity prefab;
    }
}