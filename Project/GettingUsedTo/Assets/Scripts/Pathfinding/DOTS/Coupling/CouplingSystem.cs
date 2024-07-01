using Pathfinding.DOTS.ColorCoding;
using Pathfinding.DOTS.ColorCoding.ColorMapData;
using Pathfinding.DOTS.DotsSettings;
using Pathfinding.DOTS.RandomSystem;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Pathfinding.DOTS.Coupling
{
    [BurstCompile]
    public partial struct CouplingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ColorMapPositionData>();
            state.RequireForUpdate<SettingsData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<SettingsData>();
            var colorData = SystemAPI.GetSingleton<ColorMapPositionData>().blobAssetReference;

            int i = 0;

            int currentColor = 0;
            int2 spawnPosition = new int2();
            int2 destPosition = new int2();
            
            foreach (var (localTransform, pathData, randomData, colorDefined) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PathData>, RefRW<IndividualRandomData>, EnabledRefRW<HasColorDefined>>().WithDisabled<HasColorDefined>().WithAll<CoupleTag>())
            {
                ref var spawnPos = ref colorData.Value.spawnPos;
                ref var destPos = ref colorData.Value.destPos;
                
                if (i % settings.agentsPerCouple == 0)
                {
                    currentColor = randomData.ValueRW.random.NextInt(0, 3);
                    spawnPosition = spawnPos[randomData.ValueRW.random.NextInt(spawnPos.Length)];
                    
                    var randomIndex = randomData.ValueRW.random.NextInt(destPos.Length);
                    destPosition = destPos[currentColor  + randomIndex - (randomIndex % 3)];
                }
                
                pathData.ValueRW.colorIndex = currentColor;
                pathData.ValueRW.startPos = spawnPosition;
                pathData.ValueRW.endPos = destPosition;
                
                localTransform.ValueRW.Position.xy = math.float2(pathData.ValueRO.startPos);
                
                colorDefined.ValueRW = true;
                i++;
            }
        }
    }
}