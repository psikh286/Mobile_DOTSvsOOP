using System;

namespace Pathfinding.DOTS
{
    public struct PathNode : IEquatable<PathNode>
    {
        public int x;
        public int y; 
        
        public int index;
        
        public int gCost; 
        public int hCost;
        public int fCost;
        
        public bool isWalkable;
        public int previousNodeIndex;

        public void CalcFCost() => fCost = hCost + gCost;

        public override bool Equals(object obj) => obj is PathNode other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(index);
        public bool Equals(PathNode other) => index == other.index;
    }
    
}