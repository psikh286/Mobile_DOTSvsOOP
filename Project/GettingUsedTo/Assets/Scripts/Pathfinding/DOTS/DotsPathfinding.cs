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
        private readonly int2 _gridSize = new(20, 20);
        private NativeArray<PathNode> _pathNodeArray;
        
        private void Start()
        {
            _pathNodeArray = new NativeArray<PathNode>(_gridSize.x * _gridSize.y, Allocator.Persistent);

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++) 
                {
                    PathNode pathNode = new PathNode
                    {
                        x = x,
                        y = y,
                        index = GetIndex(x, y, _gridSize.x),
                        isWalkable = !_tilemap.HasTile(new Vector3Int(x, y)),
                        gCost = int.MaxValue,
                        fCost = int.MaxValue,
                        previousNodeIndex = -1
                    };

                    _pathNodeArray[pathNode.index] = pathNode;
                }
            }
            
            InvokeRepeating(nameof(Test), 1f, 3f);
        }

        private void OnDestroy() => _pathNodeArray.Dispose();

        [ContextMenu("Test")]
        private void Test()
        {
            float startTime = Time.realtimeSinceStartup;
            
            int findPathJobCount = 5;
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.Temp);

            for (int i = 0; i < findPathJobCount; i++)
            {
                FindPathJob findPathJob = new FindPathJob 
                { 
                    startPos = new int2(0, 0), 
                    endPos = new int2(12, 6),
                    gridSize = _gridSize,
                    pathNodeArray = new NativeArray<PathNode>(_pathNodeArray, Allocator.TempJob)
                };
                jobHandleArray[i] = findPathJob.Schedule();
            }

            JobHandle.CompleteAll(jobHandleArray);
            jobHandleArray.Dispose();

            Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));
        }
        
        [BurstCompile]
        private struct FindPathJob : IJob
        {
            public int2 startPos;
            public int2 endPos;
            public int2 gridSize;
            [DeallocateOnJobCompletion] public NativeArray<PathNode> pathNodeArray;
            
            public void Execute()
            {
                (startPos, endPos) = (endPos, startPos);
                
                int endPosIndex = GetIndex(endPos.x, endPos.y, gridSize.x);

                PathNode startNode = pathNodeArray[GetIndex(startPos.x, startPos.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.hCost = CalculateDistanceCost(startPos, endPos);
                startNode.CalcFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeHeap<PathNode, PathfindingMin> openSet = new NativeHeap<PathNode, PathfindingMin>(Allocator.Temp, gridSize.x * gridSize.y);
                NativeHashSet<int> closedList = new NativeHashSet<int>(gridSize.x * gridSize.y, Allocator.Temp);
                
                openSet.Insert(startNode);

                while (openSet.Count > 0)
                {
                    PathNode currentNode = openSet.Pop();
                    int currentNodeIndex = currentNode.index;
                    
                    closedList.Add(currentNodeIndex);
                    
                    if (currentNodeIndex == endPosIndex)
                        break;
                    
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0)
                                continue;
                            
                            int2 neighbourOffset = new int2(x, y);
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
                            
                            if (tentativeGCost >= neighbourNode.gCost)
                                continue;

                            neighbourNode.previousNodeIndex = currentNodeIndex;

                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistanceCost(neighbourPosition, endPos);
                            neighbourNode.CalcFCost();
                            
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;
                            
                            openSet.InsertOrUpdate(neighbourNode);
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
                    // var r = CalculatePath(pathNodeArray, endPosIndex);
                    //
                    // foreach (var pos in r)
                    // {
                    //     Debug.Log($"{pos}");
                    // }
                    //
                    // r.Dispose();
                }

                openSet.Dispose();
                closedList.Dispose();
            }
        }
        
        #region Helper Functions
        
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
        
        #endregion
    }
}