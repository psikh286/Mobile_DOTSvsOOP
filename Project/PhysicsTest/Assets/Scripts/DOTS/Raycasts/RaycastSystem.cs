using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    [BurstCompile]
    public partial class RaycastSystem : SystemBase
    {
        public static event Action<RaycastResultInfo> RaycastCompleteEvent;
        private Entity _entity;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<RaycastSettingsData>();
            RequireForUpdate<RaycastSpawnData>();
        }

        protected override void OnUpdate()
        {
            Enabled = false;
            
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var settings = SystemAPI.GetSingleton<RaycastSettingsData>();
            var rayResults = new NativeArray<RaycastHit>(settings.count, Allocator.TempJob);
            
            var filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            };

            var input = new RaycastInput()
            {
                Start = float3.zero,
                End = new float3(1f * settings.distance, 0f, 0f),
                Filter = filter
            };
            
            var handle = ScheduleBatchRayCast(collisionWorld, input, rayResults, Dependency);
            Dependency = handle;
            handle.Complete();
            
            RaycastCompleteEvent?.Invoke(new RaycastResultInfo()
            {
                successfulHits = rayResults.Count(r => r.Entity != Entity.Null)
            });
            
            rayResults.Dispose();
        }
        
        #region Jobs
        
        public static JobHandle ScheduleBatchRayCast(CollisionWorld world, RaycastInput input, NativeArray<RaycastHit> results, JobHandle dependency)
        {
            var job = new RaycastJob
            {
                input = input,
                results = results,
                world = world
            }.ScheduleParallel(results.Length, 4, dependency);

            return job;
        }
        
        [BurstCompile]
        public struct RaycastJob : IJobFor
        {
            [ReadOnly] public CollisionWorld world;
            [ReadOnly] public RaycastInput input;
            public NativeArray<RaycastHit> results;

            public void Execute(int index)
            {
                world.CastRay(input, out var hit);
                results[index] = hit;
            }
        }
        
        #endregion
    }
}
