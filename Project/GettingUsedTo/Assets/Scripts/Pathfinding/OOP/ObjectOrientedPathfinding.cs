using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.OOP
{
	public class ObjectOrientedPathfinding : MonoBehaviour 
	{
		[SerializeField] private Grid _grid;
		
		private BinaryHeap<PathfindingNode> _openSet;
		private readonly HashSet<PathfindingNode> _closedSet = new();
		
		public void Init()
		{
			_grid.Init();
			_openSet = new BinaryHeap<PathfindingNode>(_grid.MaxSize);
		}

		public List<Vector2> FindPath( int startPosX, int startPosY, int endPosX, int endPosY)
		{
			//reversed to avoid reversing path list after
			PathfindingNode startNode = _grid.NodeFromCoords(endPosX, endPosY);
			PathfindingNode targetNode = _grid.NodeFromCoords(startPosX, startPosY);

			startNode.gCost = 0;
			startNode.hCost = GetDistance(startNode, targetNode);

			_openSet.Clear();
			_closedSet.Clear();
			
			_openSet.Add(startNode);

			while (_openSet.Count > 0)
			{
				PathfindingNode currentNode = _openSet.RemoveFirst();
				_closedSet.Add(currentNode);

				if (currentNode == targetNode)
					return RetracePath(startNode, targetNode);

				foreach (PathfindingNode neighbour in _grid.GetNeighbours(currentNode)) 
				{
					if (!neighbour.walkable || _closedSet.Contains(neighbour))
						continue;

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

					var neighbourContained = _openSet.Contains(neighbour);
					
					if (newMovementCostToNeighbour >= neighbour.gCost && neighbourContained) 
						continue;
					
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!neighbourContained)
						_openSet.Add(neighbour);
					else
						_openSet.UpdateItem(neighbour);
				}
			}

			return null;
		}

		private List<Vector2> RetracePath(PathfindingNode startNode, PathfindingNode endNode) 
		{
			List<Vector2> path = new List<Vector2>();
			PathfindingNode currentNode = endNode;

			while (currentNode != startNode) {
				path.Add(new Vector2(currentNode.gridX, currentNode.gridY));
				currentNode = currentNode.parent;
			}
			
			return path;
		}

		private int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB) 
		{
			int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

			if (dstX > dstY)
				return 14*dstY + 10* (dstX-dstY);
			
			return 14*dstX + 10 * (dstY-dstX);
		}
	}
}