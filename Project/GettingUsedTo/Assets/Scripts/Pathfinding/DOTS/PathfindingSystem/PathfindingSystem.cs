using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using NativeHeap;
using Pathfinding.DOTS.ColorCoding;
using Pathfinding.DOTS.DotsSettings;
using Pathfinding.DOTS.Grid;
using Pathfinding.DOTS.MoveSystem;
using Pathfinding.DOTS.Path;
using Unity.Burst;

namespace Pathfinding.DOTS.PathfindingSystem
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct PathfindingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridData>();
            state.RequireForUpdate<SettingsData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = SystemAPI.GetSingleton<GridData>();
            var settings = SystemAPI.GetSingleton<SettingsData>();
            
            var job = new PathfindingJob()
            {
                isWalkable = grid.blobAssetReference,
                gridSize = grid.gridSize,
                allowDiagonals = settings.allowDiagonal
            };
            
            job.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [WithAll(typeof(HasColorDefined))]
    public partial struct PathfindingJob : IJobEntity
    {
        private const int moveStraightCost = 10;
        private const int moveDiagonalCost = 14;
        
        public int2 gridSize;
        public bool allowDiagonals;
        
        [ReadOnly] public BlobAssetReference<GridDataBlobAsset> isWalkable;
        
        [BurstCompile]
        public void Execute(in PathData pathData, ref DynamicBuffer<PathPosition> pathPositionBuffer, EnabledRefRW<ReachedFinish> reachedFinish, [EntityIndexInQuery] in int i)
        {
            if(i >= 1000)
                return;
            
            int2 startPos = pathData.endPos;
            int2 endPos = pathData.startPos;

            var pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode
                    {
                        x = x,
                        y = y,
                        index = x + y * gridSize.x,
                        gCost = int.MaxValue,
                        fCost = int.MaxValue,
                        previousNodeIndex = -1
                    };

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }
            
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
                        
                        if(!allowDiagonals && x != 0 && y != 0)
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
        
                        if (!isWalkable.Value.isWalkable[neighbourNode.index]) 
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
            
            pathPositionBuffer.Clear();


            if (endNode.previousNodeIndex != -1)
            {
                CalculatePath(pathNodeArray, endPosIndex, pathPositionBuffer);
                reachedFinish.ValueRW = false;
            }

            openSet.Dispose();
            closedList.Dispose();
        }
        
        #region Helper
        
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
        
            return moveDiagonalCost * math.min(xDistance, yDistance) + moveStraightCost * remaining;
        }
        
        private static void CalculatePath(NativeArray<PathNode> pathNodeArray, int endPosIndex, DynamicBuffer<PathPosition> pathPositionBuffer)
        {
            PathNode endNode = pathNodeArray[endPosIndex];
                    
            pathPositionBuffer.Add(new PathPosition {position = new int2 (endNode.x, endNode.y)});
            
            PathNode currentNode = endNode;
                    
            while (currentNode.previousNodeIndex != -1) {
                        
                PathNode cameFromNode = pathNodeArray[currentNode.previousNodeIndex];
                
                pathPositionBuffer.Add(new PathPosition {position = new int2 (cameFromNode.x, cameFromNode.y)});
                        
                currentNode = cameFromNode;
            }
            
            pathPositionBuffer.RemoveAt(pathPositionBuffer.Length - 1);
        }
        
        #endregion
    }
}