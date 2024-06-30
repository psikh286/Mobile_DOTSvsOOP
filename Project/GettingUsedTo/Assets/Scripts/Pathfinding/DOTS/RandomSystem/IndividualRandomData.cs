using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.RandomSystem
{
    public struct IndividualRandomData : IComponentData
    {
        public Random random;
        public Random spriteRandom;
    }
}