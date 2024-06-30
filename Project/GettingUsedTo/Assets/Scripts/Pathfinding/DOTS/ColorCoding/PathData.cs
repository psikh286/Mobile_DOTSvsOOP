using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.ColorCoding
{
    public struct PathData : IComponentData
    {
        public int2 startPos;
        public int2 endPos;
        public int colorIndex;
    }
}