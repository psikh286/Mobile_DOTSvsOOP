using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace PhysicsBenchmark.DOTS.Classic.Spawner
{
    [BurstCompile]
    public partial struct SpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PrefabData>();
            state.RequireForUpdate<ClassicSettingsData>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            var prefabData = SystemAPI.GetSingleton<PrefabData>();
            var settings = SystemAPI.GetSingleton<ClassicSettingsData>();
            
            var entityPrefab = settings.enableSphere ? prefabData.spherePrefab : prefabData.cubePrefab;
            var ecb = GetEntityCommandBuffer(ref state);
            
            switch (settings.formationIdentifier)
            {
                case FormationIdentifier.Checkerboard:
                    var checkerboardJob = new CheckerboardJob
                    {
                        entityPrefab = entityPrefab,
                        length = settings.length,
                        heightOffset = settings.heightOffset,
                        ecb = ecb
                    };

                    state.Dependency = checkerboardJob.ScheduleParallel(settings.height, 32, state.Dependency);
                    break;
                
                case FormationIdentifier.Perimeter:
                    var perimeterJob = new PerimeterJob
                    {
                        entityPrefab = entityPrefab,
                        length = settings.length,
                        heightOffset = settings.heightOffset,
                        ecb = ecb,
                        angle = settings.angle
                    };

                    state.Dependency = perimeterJob.ScheduleParallel(settings.height, 32, state.Dependency);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [BurstCompile]
        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }

        [BurstCompile]
        public struct PerimeterJob : IJobFor
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            public Entity entityPrefab;
            public int length;
            public float heightOffset;
            public float angle;
            
            [BurstCompile]
            public void Execute(int index)
            {
                var offset = (length - 1f) * 0.5f;
                var height = heightOffset + index;

                for (int i = 0; i < length; i++)
                {
                    var x = i - offset;
                    InstantiatePair(index, new float3(x, height, -offset), new float3(x, height, offset));
                }

                for (int i = 1; i < length - 1; i++)
                {
                    var y = i - offset;
                    InstantiatePair(index, new float3(-offset, height, y), new float3(offset, height, y));
                }
            }

            [BurstCompile]
            void InstantiatePair(int index, float3 posA, float3 posB)
            {
                var entityA = ecb.Instantiate(index, entityPrefab);
                var entityB = ecb.Instantiate(index, entityPrefab);

                var rot = quaternion.AxisAngle(new float3(0, 1, 0), math.radians(angle * index));
                var entityRotation = quaternion.RotateY(math.radians(angle * index));
                
                ecb.SetComponent(index, entityA, LocalTransform.FromPositionRotation(math.mul(rot, posA), rot));
                ecb.SetComponent(index, entityB, LocalTransform.FromPositionRotation(math.mul(rot, posB), rot));
            }
        }
        
        [BurstCompile]
        public struct CheckerboardJob : IJobFor
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            public Entity entityPrefab;
            public int length;
            public float heightOffset;
            
            [BurstCompile]
            public void Execute(int index)
            {
                var offset = (length - 1f) * 0.5f;
                var height = heightOffset + index;

                for (int i = 0; i < length; i++)
                {
                    if ((index % 2 == 0 && i % 2 != 0) || (index % 2 != 0 && i % 2 == 0))
                        continue;
                    
                    var x = i - offset;
                    InstantiatePair(index, new float3(x, height, -offset), new float3(x, height, offset));
                }

                for (int i = 1; i < length - 1; i++)
                {
                    if ((index % 2 == 0 && i % 2 != 0) || (index % 2 != 0 && i % 2 == 0))
                        continue;
                    
                    var y = i - offset;
                    InstantiatePair(index, new float3(-offset, height, y), new float3(offset, height, y));
                }
            }

            [BurstCompile]
            void InstantiatePair(int index, float3 posA, float3 posB)
            {
                var entityA = ecb.Instantiate(index, entityPrefab);
                var entityB = ecb.Instantiate(index, entityPrefab);

                ecb.SetComponent(index, entityA, LocalTransform.FromPosition(posA));
                ecb.SetComponent(index, entityB, LocalTransform.FromPosition(posB));
            }
        }
    }
}
