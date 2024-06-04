using System.Collections.Generic;

namespace Pathfinding.DOTS
{
    public struct PathfindingMin : IComparer<PathNode>
    {
        public int Compare(PathNode x, PathNode y)
        {
            int compare = x.fCost.CompareTo(y.fCost);
			
            if (compare == 0)
                compare = x.hCost.CompareTo(y.hCost);
			
            return -compare;
        }
    }
    
}