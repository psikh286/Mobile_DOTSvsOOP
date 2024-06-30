using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.Path
{
    [InternalBufferCapacity(80)]
    public struct PathPosition : IBufferElementData
    {
        public int2 position;
    }
}