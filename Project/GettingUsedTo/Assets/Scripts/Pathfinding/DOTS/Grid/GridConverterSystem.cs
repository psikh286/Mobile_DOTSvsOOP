using Unity.Collections;
using Unity.Entities;

namespace Pathfinding.DOTS.Grid
{
    public partial struct GridConverterSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridData>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var grid = SystemAPI.GetSingleton<GridData>();
            
            var isWalkable = new NativeArray<bool>(grid.gridSize.x * grid.gridSize.y, Allocator.Temp);
                
            for (int x = 0; x < grid.gridSize.x; x++)
            {
                for (int y = 0; y < grid.gridSize.y; y++)
                {
                    isWalkable[x + y * grid.gridSize.x] = grid.blobAssetReference.Value.isWalkable[x + y * grid.gridSize.x];
                }
            }
            
            if (!SystemAPI.TryGetSingletonEntity<NewGridData>(out var entity))
            {
                entity = state.EntityManager.CreateSingleton<NewGridData>();
            }
            
            state.EntityManager.SetComponentData(entity, new NewGridData
            {
                gridSize = grid.gridSize,
                isWalkable = new NativeArray<bool>(isWalkable, Allocator.Persistent),
            });

            isWalkable.Dispose();
        }
    }
}