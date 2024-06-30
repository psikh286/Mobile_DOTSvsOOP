using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.ColorCoding.ColorMapData
{
    public struct ColorMapPositionsBlobAsset
    {
        public BlobArray<int2> spawnPos;
        public BlobArray<int2> destPos;
    }
}