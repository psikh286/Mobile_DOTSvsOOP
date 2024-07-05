using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;

namespace PhysicsBenchmark.UI
{
    [BurstCompile]
    public struct RaycastJob : IJobFor
    {
        [ReadOnly] public CollisionWorld world;
        [ReadOnly] public RaycastInput input;
        public NativeArray<RaycastHit> results;

        public void Execute(int index)
        {
            world.CastRay(input, out RaycastHit hit);
            results[index] = hit;
        }
    }
}