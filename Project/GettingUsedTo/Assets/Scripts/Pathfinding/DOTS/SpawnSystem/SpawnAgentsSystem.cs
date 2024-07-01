using System.Runtime.InteropServices;
using Pathfinding.DOTS.ColorCoding;
using Pathfinding.DOTS.Coupling;
using Pathfinding.DOTS.Debug;
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
        private float timePassed;
        private int totalAmountSpawned;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SettingsData>();
            state.RequireForUpdate<SpawnerConfigData>();
            
            timePassed = 1f;
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<SpawnerConfigData>();
            var settings = SystemAPI.GetSingleton<SettingsData>();

            timePassed += SystemAPI.Time.DeltaTime;
            
            if (timePassed < 1f) 
                return;
            
            timePassed -= 1f;

            if (totalAmountSpawned >= settings.agentsCount)
            {
                timePassed = 1f;
                totalAmountSpawned = 0;
                state.Enabled = false;
                return;
            }
            
            var ecb = new EntityCommandBuffer(Allocator.Persistent);
            
            var totalAmountLeft = settings.agentsCount - totalAmountSpawned;
            var amountToSpawn = math.min(totalAmountLeft, 1000);
            totalAmountSpawned += amountToSpawn;
            
            for (int i = 0; i < amountToSpawn; i++)
            {
                Entity entity =  ecb.Instantiate(config.prefabEntity);
                
                ecb.SetComponent(entity, LocalTransform.FromPosition(10f, 10f, 0f));
                ecb.AddComponent(entity, new PathData());
                ecb.AddComponent(entity, new MovementData());
                ecb.AddComponent(entity, new IndividualRandomData
                {
                    random = new Random((uint)(i + 1) * settings.seed),
                    spriteRandom = new Random((uint)(i + 1) * settings.seed),
                });
                
                if (settings.allowCoupling) 
                    ecb.AddComponent(entity, new CoupleTag());

                
                ecb.AddBuffer<PathPosition>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}