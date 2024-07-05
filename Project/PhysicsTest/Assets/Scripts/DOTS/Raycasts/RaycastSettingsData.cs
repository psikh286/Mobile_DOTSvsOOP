using Unity.Entities;

namespace PhysicsBenchmark.DOTS.Raycasts
{
    public struct RaycastSettingsData : IComponentData
    {
        public int count;
        public float distance;
        public bool layersOn;
    }
}