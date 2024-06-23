using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding.OOP
{
    // ReSharper disable once InconsistentNaming
    public class OOPathfindingBenchmark : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private ObjectOrientedPathfinding _pathfinding;

        [SerializeField] private int2 startPos;
        [SerializeField] private int2 endPos;

        [SerializeField] private bool _drawPath;

        private List<Vector2> _path;
        
        private void Start()
        {
            _pathfinding.Init();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                InvokeRepeating(nameof(Test), 1f, 3f);
        }

        [ContextMenu("Test")]
        private void Test()
        { 
            float startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < 5; i++)
            {
                _path = _pathfinding.FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
            }
            
            Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));
        }

        private void OnDrawGizmos()
        {
            if(!_drawPath || _path == null)
                return;

            for (var i = 0; i < _path.Count; i++)
            {
                Vector3 pos = new Vector3(_path[i].x, _path[i].y, 0f);

                if (i == 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(pos, Vector3.one);
                }
                
                Gizmos.color = Color.green;
                
                if (i == _path.Count - 1)
                {
                    var lastPos = new Vector3(endPos.x, endPos.y, 0f);
                    
                    Gizmos.DrawLine(pos, lastPos);
                    
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(lastPos, Vector3.one);
                    
                    return;
                }
                
                Gizmos.DrawLine(pos, new Vector3(_path[i+1].x, _path[i+1].y, 0f));
            }
        }
    }
}