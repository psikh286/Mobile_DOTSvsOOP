using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    [BurstCompile]
    public partial struct RaycastSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            
            var rayResults = new NativeArray<RaycastHit>(100000, Allocator.TempJob);
            
            var filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                GroupIndex = 0
            };

            var input = new RaycastInput()
            {
                Start = float3.zero,
                End = new float3(0f, 2f, 0f),
                Filter = filter
            };
            
            var handle = ScheduleBatchRayCast(collisionWorld, input, rayResults, ref state);
            handle.Complete();
            
            rayResults.Dispose();
        }
        
        public static JobHandle ScheduleBatchRayCast(CollisionWorld world, RaycastInput input, NativeArray<RaycastHit> results, ref SystemState state)
        {
            var job = new RaycastJob
            {
                input = input,
                results = results,
                world = world

            }.ScheduleParallel(results.Length, 4, state.Dependency);

            state.Dependency = job;
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
    }
}