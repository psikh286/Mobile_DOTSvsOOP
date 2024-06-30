using System.Runtime.InteropServices;
using Pathfinding.DOTS.ColorCoding.ColorMapData;
using Pathfinding.DOTS.RandomSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pathfinding.DOTS.ColorCoding
{
    public partial struct ColorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ColorMapPositionData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var positions = SystemAPI.GetSingleton<ColorMapPositionData>();

            var job = new ColorDefineJob
            {
                colorData = positions.blobAssetReference
            };

            job.ScheduleParallel();
        }

        [BurstCompile]
        [StructLayout(LayoutKind.Auto)]
        [WithDisabled(typeof(HasColorDefined))]
        public partial struct ColorDefineJob : IJobEntity
        {
            [ReadOnly] public BlobAssetReference<ColorMapPositionsBlobAsset> colorData;
            
            public void Execute(ref LocalTransform localTransform, ref PathData pathData, ref IndividualRandomData randomData, EnabledRefRW<HasColorDefined> colorDefined)
            {
                ref var spawnPos = ref colorData.Value.spawnPos;
                ref var destPos = ref colorData.Value.destPos;
                
                pathData.colorIndex = randomData.random.NextInt(0, 3);
                
                pathData.startPos = spawnPos[randomData.random.NextInt(spawnPos.Length)];
                localTransform.Position.xy = math.float2(pathData.startPos);
                
                var randomIndex = randomData.random.NextInt(destPos.Length);
                pathData.endPos = destPos[pathData.colorIndex  + randomIndex - (randomIndex % 3)];
                
                colorDefined.ValueRW = true;
            }
        }
    }
}