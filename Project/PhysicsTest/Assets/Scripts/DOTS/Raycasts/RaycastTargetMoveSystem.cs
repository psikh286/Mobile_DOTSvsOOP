using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    public partial struct RaycastTargetMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RaycastSpawnData>();
            state.RequireForUpdate<RaycastSettingsData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var settings = SystemAPI.GetSingleton<RaycastSettingsData>();
            var spawnData = SystemAPI.GetSingleton<RaycastSpawnData>();
            
            if(!SystemAPI.TryGetSingletonEntity<RaycastTargetTag>(out var entity))
            {
                entity = state.EntityManager.Instantiate(spawnData.prefab);
            }
            
            state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(new float3(1f * settings.distance, 0f, 0f)));
        }
    }
}