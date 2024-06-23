using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Pathfinding.DOTS.SpawnSystem
{
    public partial struct SpawnAgentsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            SpawnerConfigData config = SystemAPI.GetSingleton<SpawnerConfigData>();

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < config.amountToSpawn; i++)
            {
                Entity entity =  ecb.Instantiate(config.prefabEntity);
                
                ecb.SetComponent(entity, LocalTransform.FromPosition(-10f, -10f, 0f));
            }

            ecb.Playback(state.EntityManager);
        }
    }
}