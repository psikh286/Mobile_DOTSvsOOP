namespace Pathfinding.DOTS
{
    public struct PathNode
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
    }
}