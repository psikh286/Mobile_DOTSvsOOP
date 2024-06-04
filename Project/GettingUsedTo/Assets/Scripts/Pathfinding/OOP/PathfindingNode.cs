namespace Pathfinding.OOP
{
	public class PathfindingNode : IHeapItem<PathfindingNode>
	{
		public readonly bool walkable;
		public readonly int gridX;
		public readonly int gridY;

		public int gCost;
		public int hCost;
		public int fCost => gCost + hCost;
		
		public PathfindingNode parent;
		
		public int HeapIndex { get; set; }

		public PathfindingNode(bool _walkable, int _gridX, int _gridY)
		{
			walkable = _walkable;
			gridX = _gridX;
			gridY = _gridY;
		}
		
		public int CompareTo(PathfindingNode nodeToCompare)
		{
			int compare = fCost.CompareTo(nodeToCompare.fCost);
			
			if (compare == 0) 
				compare = hCost.CompareTo(nodeToCompare.hCost);
			
			return -compare;
		}
	}
}
