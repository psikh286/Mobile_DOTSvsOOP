using Unity.Entities;

namespace Pathfinding.DOTS.Grid
{
    public struct GridDataBlobAsset
    {
        public BlobArray<bool> isWalkable;
    }
}