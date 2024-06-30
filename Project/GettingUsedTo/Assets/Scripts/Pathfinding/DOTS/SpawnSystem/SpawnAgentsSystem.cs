using System.Runtime.InteropServices;
using Pathfinding.DOTS.ColorCoding;
using Pathfinding.DOTS.DotsSettings;
using Pathfinding.DOTS.MoveSystem;
using Pathfinding.DOTS.Path;
using Pathfinding.DOTS.RandomSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Pathfinding.DOTS.SpawnSystem
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct SpawnAgentsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SettingsData>();
            state.RequireForUpdate<SpawnerConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<SpawnerConfigData>();
            var settings = SystemAPI.GetSingleton<SettingsData>();

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < settings.agentsCount; i++)
            {
                Entity entity =  ecb.Instantiate(config.prefabEntity);
                
                ecb.SetComponent(entity, LocalTransform.FromPosition(-1f, -1f, 0f));
                ecb.AddComponent(entity, new PathData());
                ecb.AddComponent(entity, new MovementData());
                ecb.AddComponent(entity, new IndividualRandomData(){random = new Random((uint)(i + 1) * settings.seed)});
                ecb.AddBuffer<PathPosition>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}