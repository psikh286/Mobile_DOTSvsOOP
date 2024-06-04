using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding.DOTS
{
    // ReSharper disable once InconsistentNaming
    public class DOTSPathfindingBenchmark : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;

        

        // void Test()
        // {
        //     int2 gridSize;
        //     
        //     NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
        //
        //     for (int x = 0; x < gridSize.x; x++)
        //     {
        //         for (int y = 0; y < gridSize.y; y++)
        //         {
        //             PathNode pathNode = new PathNode();
        //
        //             pathNode.x = x;
        //             pathNode.y = y;
        //             pathNode.index = GetIndex(x, y, gridSize.x);
        //
        //             pathNode.gCost = int.MaxValue;
        //             pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPos);
        //             pathNode.CalcFCost();
        //
        //             pathNode.isWalkable = true;
        //             pathNode.previousNodeIndex = -1;
        //
        //             pathNodeArray[pathNode.index] = pathNode;
        //         }
        //     }
        // }
    }
}