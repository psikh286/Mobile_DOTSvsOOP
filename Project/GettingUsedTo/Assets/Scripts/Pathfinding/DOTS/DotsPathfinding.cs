using NativeHeap;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding.DOTS
{
    public class DotsPathfinding : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        
        int2 gridSize = new int2(18, 18);
        
        public struct FindPathJob : IJob
        {
            public int2 startPos;
            public int2 endPos;
            
            public void Execute()
            {
                
            }
        }

        [ContextMenu("Test")]
        private void Start()
        {
            var isWalkable = new NativeArray<bool>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    isWalkable[GetIndex(x, y, gridSize.x)] = !_tilemap.HasTile(new Vector3Int(x, y, 0));
                }
            }
            
            var startTime = Time.realtimeSinceStartup;
            
            FindPath(new int2(0,0), new int2(2, 3), isWalkable);

            isWalkable.Dispose();
            
            Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));
        }

        private void FindPath(int2 startPos, int2 endPos, NativeArray<bool> isWalkable)
        {
            (startPos, endPos) = (endPos, startPos);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();

                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = GetIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPos);
                    pathNode.CalcFCost();

                    pathNode.isWalkable = isWalkable[pathNode.index];
                    pathNode.previousNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            isWalkable.Dispose();
            
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[] 
            {
                new int2(-1, 0), // Left
                new int2(+1, 0), // Right
                new int2(0, +1), // Up
                new int2(0, -1), // Down
                new int2(-1, -1), // Left Down
                new int2(-1, +1), // Left Up
                new int2(+1, -1), // Right Down
                new int2(+1, +1), // Right Up
            }, Allocator. Temp);
            
            
            int endPosIndex = GetIndex(endPos.x, endPos.y, gridSize.x);

            PathNode startNode = pathNodeArray[GetIndex(startPos.x, startPos.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalcFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);
            
            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];
                
                if (currentNodeIndex == endPosIndex)
                    break;

                for (int i = 0; i < openList.Length; i++)
                {
                    if(openList[i] != currentNodeIndex)
                        continue;
                    
                    openList.RemoveAtSwapBack(i);
                    break;
                }
                
                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition =
                        new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                    if (!IsInsideGrid(neighbourPosition, gridSize))
                        continue;


                    int neighbourNodeIndex = GetIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                    if (closedList.Contains(neighbourNodeIndex))
                        continue;

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];

                    if (!neighbourNode.isWalkable)
                        continue;


                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                    
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);

                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.previousNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalcFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;
                    }

                    if (!openList.Contains(neighbourNode.index))
                    {
                        openList.Add(neighbourNode.index);
                    }
                }
            }
            
            PathNode endNode = pathNodeArray[endPosIndex];
            if (endNode.previousNodeIndex == -1) 
            {
                //DIDNT
            }
            else
            {
                
            }

            neighbourOffsetArray.Dispose();
            pathNodeArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }
        
        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, int endPosIndex)
        {
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);

            PathNode endNode = pathNodeArray[endPosIndex];
                    
            path.Add(new int2 (endNode.x, endNode.y));
            
            PathNode currentNode = endNode;
                    
            while (currentNode.previousNodeIndex != -1) {
                        
                PathNode cameFromNode = pathNodeArray[currentNode.previousNodeIndex];
                        
                path. Add (new int2 (cameFromNode.x, cameFromNode.y)) ;
                        
                currentNode = cameFromNode;
                        
            }
            
            return path;
        }

        private bool IsInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.y >= 0 &&
                   gridPosition.x < gridSize.x &&
                   gridPosition.y < gridSize.y;
        }
        
        private int GetIndex(int x, int y, int gridWight) => x + y * gridWight;

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];

            for (int i = 1; i < openList.Length; i++)
            {
                PathNode currentNode = pathNodeArray[openList[i]];

                if (currentNode.fCost < lowestCostPathNode.fCost) 
                    lowestCostPathNode = currentNode;
            }

            return lowestCostPathNode.index;
        }
    }
}