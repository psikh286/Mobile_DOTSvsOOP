using NativeHeap;
using Unity.Burst;
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
        
        [BurstCompile]
        public struct FindPathJob : IJob
        {
            public int2 gridSize;
            public int2 startPos;
            public int2 endPos;
            [DeallocateOnJobCompletion] public NativeArray<PathNode> pathNodeArray;
            
            public void Execute()
            {
                (startPos, endPos) = (endPos, startPos);
                
                NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
                neighbourOffsetArray[0] = new int2(-1, 0); // Left
                neighbourOffsetArray[1] = new int2(+1, 0); // Right
                neighbourOffsetArray[2] = new int2(0, +1); // Up
                neighbourOffsetArray[3] = new int2(0, -1); // Down
                neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
                neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
                neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
                neighbourOffsetArray[7] = new int2(+1, +1); // Right Up
                
                int endPosIndex = GetIndex(endPos.x, endPos.y, gridSize.x);

                PathNode startNode = pathNodeArray[GetIndex(startPos.x, startPos.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalcFCost();
                pathNodeArray[startNode.index] = startNode;

                var openSet = new NativeHeap<PathNode, PathfindingMin>(Allocator.Temp, gridSize.x * gridSize.y);

                NativeList<int> closedList = new NativeList<int>(Allocator.Temp);
                
                openSet.Insert(startNode);

                while (openSet.Count > 0)
                {
                    PathNode currentNode = openSet.Pop();
                    int currentNodeIndex = currentNode.index;
                    
                    closedList.Add(currentNodeIndex);
                    
                    if (currentNodeIndex == endPosIndex)
                        break;

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
                            neighbourNode.hCost = CalculateDistanceCost(neighbourPosition, endPos);
                            
                            neighbourNode.CalcFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;
                        }

                        if(!openSet.Contains(neighbourNode, out var heapIndex))
                        {
                            openSet.Insert(neighbourNode);
                        }
                        else if (heapIndex != -1)
                        {
                            openSet.UpdateItem(neighbourNode, heapIndex);
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
                openSet.Dispose();
                closedList.Dispose();
            }
        }

        [ContextMenu("Test")]
        private void Start()
        {
            var gridSize = new int2(18, 18);
            
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.TempJob);
            
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode
                    {
                        x = x,
                        y = y,
                        index = GetIndex(x, y, gridSize.x),
                        isWalkable = !_tilemap.HasTile(new Vector3Int(x, y, 0)),
                        previousNodeIndex = -1
                    };
                    
                    pathNodeArray[pathNode.index] = pathNode;
                }
            }
            
            var startTime = Time.realtimeSinceStartup;
            
            int findPathJobCount = 5;
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);

            for (int i = 0; i < findPathJobCount; i++)
            {
                FindPathJob findPathJob = new FindPathJob
                {
                    gridSize = gridSize,
                    startPos = new int2(0, 0), 
                    endPos = new int2(2, 3),
                    pathNodeArray = new NativeArray<PathNode>(pathNodeArray, Allocator.TempJob)
                };
                
                jobHandleArray[i] = findPathJob.Schedule();
            }
            
            JobHandle.CompleteAll(jobHandleArray);
            
            jobHandleArray.Dispose();
            pathNodeArray.Dispose();
            
            Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));
        }


        private static int GetIndex(int x, int y, int gridWight) => x + y * gridWight;
        
        private static bool IsInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.y >= 0 &&
                   gridPosition.x < gridSize.x &&
                   gridPosition.y < gridSize.y;
        }
        
        private static int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }
        
        private static NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, int endPosIndex)
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
    }
}