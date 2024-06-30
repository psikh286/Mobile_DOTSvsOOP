using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.Grid
{
    public struct GridData : IComponentData
    {
        public int2 gridSize;
        public BlobAssetReference<GridDataBlobAsset> blobAssetReference;
    }
}