using Unity.Entities;

namespace Pathfinding.DOTS.DotsSettings
{
    public struct SettingsData : IComponentData
    {
        public int agentsCount;
        public int agentsPerCouple;
        public uint seed;
        public bool allowCoupling;
        public bool allowDiagonal;
    }
}