using Unity.Entities;

namespace PhysicsBenchmark.DOTS.Classic.Spawner
{
    public struct ClassicSettingsData : IComponentData
    {
        public int height;
        public int length;
        public float heightOffset;
        public float angle;
        public bool enableSphere;
        public FormationIdentifier formationIdentifier;
    }
}