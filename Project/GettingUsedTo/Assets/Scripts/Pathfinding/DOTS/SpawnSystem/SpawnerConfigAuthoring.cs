using Unity.Entities;
using UnityEngine;

namespace Pathfinding.DOTS.SpawnSystem
{
    public class SpawnerConfigAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public int amountToSpawn;
        
        private class Baker : Baker<SpawnerConfigAuthoring>
        {
            public override void Bake(SpawnerConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpawnerConfigData()
                {
                    prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    amountToSpawn = authoring.amountToSpawn
                });
            }
        }
    }
}