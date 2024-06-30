using Unity.Entities;

namespace Pathfinding.DOTS.ColorCoding.ColorMapData
{
    public struct ColorMapPositionData : IComponentData
    {
        public BlobAssetReference<ColorMapPositionsBlobAsset> blobAssetReference;
    }
}