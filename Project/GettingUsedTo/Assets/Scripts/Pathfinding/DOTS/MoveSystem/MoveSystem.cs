using System.Runtime.InteropServices;
using Pathfinding.DOTS.ColorCoding;
using Pathfinding.DOTS.ColorCoding.SpriteApplying;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using PathPosition = Pathfinding.DOTS.Path.PathPosition;

namespace Pathfinding.DOTS.MoveSystem
{
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveAgentJob
            {
                deltaTime = SystemAPI.Time.DeltaTime * 5f
            };
            
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct MoveAgentJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        
        [BurstCompile]
        public void Execute(ref LocalTransform localTransform, ref MovementData movementData, in DynamicBuffer<PathPosition> nodes, EnabledRefRW<ReachedFinish> reachedFinish,  EnabledRefRW<HasColorDefined> colorDefined, EnabledRefRW<HasSpriteApplied> spriteApplied)
        {
            if(reachedFinish.ValueRO)
                return;
            
            if (movementData.index >= nodes.Length)
            {
                reachedFinish.ValueRW = true;
                colorDefined.ValueRW = false;
                spriteApplied.ValueRW = false;
                movementData.index = 0;
                return;
            }

            var target = math.float2(nodes[movementData.index].position);
            localTransform.Position.xy = Vector2.MoveTowards(localTransform.Position.xy, target, deltaTime);
            
            if(target.Equals(localTransform.Position.xy))
                movementData.index += 1;
        }
    }
}