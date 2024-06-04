using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding.OOP
{
	public class Grid : MonoBehaviour 
	{
		public int MaxSize => _gridSizeX * _gridSizeY;
		
		[SerializeField] private Vector2 _gridWorldSize;
		[SerializeField] private Tilemap _tilemap;
		
		private PathfindingNode[,] _grid;


		private int _gridSizeX;
		private int _gridSizeY;
		
		public void Init() 
		{
			_gridSizeX = Mathf.RoundToInt(_gridWorldSize.x);
			_gridSizeY = Mathf.RoundToInt(_gridWorldSize.y);
			CreateGrid();
		}
		
		private void CreateGrid()
		{
			_grid = new PathfindingNode[_gridSizeX, _gridSizeY];

			for (int x = 0; x < _gridSizeX; x ++) 
			{
				for (int y = 0; y < _gridSizeY; y ++) 
				{
					_grid[x, y] = new PathfindingNode(!_tilemap.HasTile( new Vector3Int(x, y, 0)), x, y);
				}
			}
		}
		
		public PathfindingNode NodeFromCoords(int x, int y) => _grid[x,y];

		public List<PathfindingNode> GetNeighbours(PathfindingNode node) 
		{
			List<PathfindingNode> neighbours = new List<PathfindingNode>();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					if (x == 0 && y == 0)
						continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;

					if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY) {
						neighbours.Add(_grid[checkX,checkY]);
					}
				}
			}

			return neighbours;
		}
	}
}