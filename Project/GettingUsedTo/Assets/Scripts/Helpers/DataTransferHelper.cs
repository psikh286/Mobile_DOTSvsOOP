using Pathfinding;
using Pathfinding.DOTS.DotsSettings;
using Unity.Entities;

namespace Helpers
{
    public static class DataTransferHelper
    {
        public static void CreateOrUpdateData()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery query = entityManager.CreateEntityQuery(typeof(SettingsData));
            
            SettingsData settingsData = new SettingsData
            {
                allowCoupling = PathfindingSettings.AllowCoupling,
                allowDiagonal = PathfindingSettings.AllowDiagonal,
                agentsCount = PathfindingSettings.AgentsCount,
                agentsPerCouple = PathfindingSettings.AgentsPerCouple,
                seed = PathfindingSettings.Seed
            };

            if (query.CalculateEntityCount() == 0)
            {
                // Singleton does not exist, create it
                Entity singletonEntity = entityManager.CreateEntity(typeof(SettingsData));
                entityManager.SetComponentData(singletonEntity, settingsData);
            }
            else
            {
                // Singleton exists, update it
                Entity singletonEntity = query.GetSingletonEntity();
                entityManager.SetComponentData(singletonEntity, settingsData);
            }
        }
    }
}