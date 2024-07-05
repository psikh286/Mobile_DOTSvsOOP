using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.DOTS.Grid
{
    public struct NewGridData : IComponentData, IDisposable
    {
        public int2 gridSize;
        public NativeArray<bool> isWalkable;

        public void Dispose()
        {
            isWalkable.Dispose();
        }
    }
}