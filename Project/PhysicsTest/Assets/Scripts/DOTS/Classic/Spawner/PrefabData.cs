using Unity.Entities;

namespace PhysicsBenchmark.DOTS.Classic.Spawner
{
    public struct PrefabData : IComponentData
    {
        public Entity cubePrefab;
        public Entity spherePrefab;
    }
}