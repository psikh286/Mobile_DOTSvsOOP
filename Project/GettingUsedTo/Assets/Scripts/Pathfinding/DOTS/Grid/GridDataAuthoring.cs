using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding.DOTS.Grid
{
    public class GridDataAuthoring : MonoBehaviour
    {
        public Tilemap tilemap;
        public bool tilesWalkable;
        public int2 gridSize;

        private class Baker : Baker<GridDataAuthoring>
        {
            public override void Bake(GridDataAuthoring authoring)
            {
                var blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var root = ref blobBuilder.ConstructRoot<GridDataBlobAsset>();
                BlobBuilderArray<bool> pathNodeArray = blobBuilder.Allocate(ref root.isWalkable, authoring.gridSize.x * authoring.gridSize.y);
                    
                for (int x = 0; x < authoring.gridSize.x; x++)
                {
                    for (int y = 0; y < authoring.gridSize.y; y++)
                    {
                        pathNodeArray[x + y * authoring.gridSize.x] = authoring.tilesWalkable == authoring.tilemap.HasTile(new Vector3Int(x, y));
                    }
                }
                    
                BlobAssetReference<GridDataBlobAsset> blobAssetReference = blobBuilder.CreateBlobAssetReference<GridDataBlobAsset>(Allocator.Persistent);
                blobBuilder.Dispose();
                
                AddBlobAsset(ref blobAssetReference, out _);
                
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GridData
                {
                    blobAssetReference = blobAssetReference,
                    gridSize = authoring.gridSize
                });
            }
        }
    }
}