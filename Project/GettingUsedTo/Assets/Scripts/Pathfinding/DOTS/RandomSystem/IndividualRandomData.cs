using Unity.Entities;

namespace Pathfinding.DOTS.RandomSystem
{
    public struct IndividualRandomData : IComponentData
    {
        public Unity.Mathematics.Random random;
    }
}