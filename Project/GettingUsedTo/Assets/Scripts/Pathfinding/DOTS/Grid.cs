using Unity.Collections;
using Unity.Mathematics;

namespace Pathfinding.DOTS
{
    public struct Grid
    {
        public int2 gridSize;
        public NativeArray<PathNode> pathNodeArray;
    }
}