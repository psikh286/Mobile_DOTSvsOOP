using PhysicsBenchmark.DOTS.Classic.Spawner;
using PhysicsBenchmark.Settings;
using Unity.Entities;

namespace PhysicsBenchmark.Helpers
{
    public static class DataTransferHelper
    {
        public static void CreateOrUpdateData()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery query = entityManager.CreateEntityQuery(typeof(ClassicSettingsData));
            
            ClassicSettingsData settingsData = new ClassicSettingsData
            {
                height = ClassicSettings.height,
                length = ClassicSettings.length,
                heightOffset = ClassicSettings.heightOffset,
                enableSphere = ClassicSettings.enableSphere,
                formationIdentifier = ClassicSettings.formationIdentifier,
                angle = ClassicSettings.angle,
            };

            if (query.CalculateEntityCount() == 0)
            {
                // Singleton does not exist, create it
                Entity singletonEntity = entityManager.CreateEntity(typeof(ClassicSettingsData));
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