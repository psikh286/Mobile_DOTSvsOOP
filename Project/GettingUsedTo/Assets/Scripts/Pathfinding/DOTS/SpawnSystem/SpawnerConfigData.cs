using Unity.Entities;

namespace Pathfinding.DOTS.SpawnSystem
{
    public struct SpawnerConfigData : IComponentData
    {
        public Entity prefabEntity;
        public int amountToSpawn;
    }
}