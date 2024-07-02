using Unity.Entities;
using UnityEngine;

namespace PhysicsBenchmark.DOTS.Classic.Spawner
{
    public class PrefabDataAuthoring : MonoBehaviour
    {
        public GameObject cubePrefab;
        public GameObject spherePrefab;

        public class PrefabDataBaker : Baker<PrefabDataAuthoring>
        {
            public override void Bake(PrefabDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PrefabData
                {
                    cubePrefab = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                    spherePrefab = GetEntity(authoring.spherePrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}