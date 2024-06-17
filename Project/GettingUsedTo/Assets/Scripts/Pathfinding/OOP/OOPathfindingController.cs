using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Pathfinding.OOP
{
    // ReSharper disable once InconsistentNaming
    public class OOPathfindingController : MonoBehaviour
    {
        [System.Serializable]
        private struct ColorBuildingData
        {
            public Color color;
            public int2 pos;
        }
        
        private class AgentData
        {
            public List<Vector2> path = new();
            public int nodeCount;
            public bool inProgress;
        }
        
        [SerializeField] private SpriteRenderer _agentPrefab;
        [SerializeField] private ObjectOrientedPathfinding _pathfinding;
        [SerializeField] private Vector2[] _spawnersPositions;
        [SerializeField] private ColorBuildingData[] _destinationPositions;

        [Header("Temp")] 
        [SerializeField] private float _carSpeed;
        [SerializeField] private int _capacity;

        private SpriteRenderer[] _agents;
        private AgentData[] _agentData;
        
        
        private void Start()
        {
            _pathfinding.Init();
            
            Random.InitState(7);

            _agents = new SpriteRenderer[_capacity];
            _agentData = new AgentData[_capacity];
            
            for (int i = 0; i < _capacity; i++)
            {
                _agents[i] = Instantiate(_agentPrefab, GetSpawnPosition(), Quaternion.identity);
                _agentData[i] = new AgentData();
                
            }
        }

        private void Update()
        {
            for (var i = 0; i < _agents.Length; i++)
            {
                if (_agentData[i].inProgress)
                {
                    if (_agentData[i].nodeCount >= _agentData[i].path.Count)
                    {
                        _agentData[i] = new AgentData();
                        _agents[i].transform.position = GetSpawnPosition();
                        return;
                    }
                    
                    _agents[i].transform.position = Vector3.MoveTowards(_agents[i].transform.position, _agentData[i].path[_agentData[i].nodeCount], _carSpeed * Time.deltaTime);
                    
                    if (_agents[i].transform.position == (Vector3)_agentData[i].path[_agentData[i].nodeCount])
                        _agentData[i].nodeCount++;
                }
                else
                {
                    var pos = _agents[i].transform.position;
                    var dest = GetDestinationPosition();
                    var path = _pathfinding.FindPath((int)pos.x, (int)pos.y, dest.x, dest.y);
                    
                    _agentData[i].path = path;
                    _agentData[i].inProgress = true;
                }
            }
        }

        private Vector2 GetSpawnPosition() => _spawnersPositions[Mathf.RoundToInt(Random.value *  (_spawnersPositions.Length - 1))];
        private int2 GetDestinationPosition()
        {
            return _destinationPositions[Mathf.RoundToInt(Random.value * (_destinationPositions.Length - 1))].pos;
        }

        private void OnDrawGizmosSelected()
        {
            DrawSpawners();
            DrawBuildings();
            return;
            
            void DrawSpawners()
            {
                if(_spawnersPositions == null || _spawnersPositions.Length == 0)
                    return;

                var style = new GUIStyle
                {
                    fontStyle = FontStyle.Bold, 
                    fontSize = 12, 
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        textColor = Color.red
                    }
                };
                
                for (var i = 0; i < _spawnersPositions.Length; i++)
                {
                    UnityEditor.Handles.DrawSolidRectangleWithOutline(GetRectangleVertices(_spawnersPositions[i], 1f), Color.black, Color.white);
                    UnityEditor.Handles.Label(_spawnersPositions[i], $"s{i}", style);
                }
                
            }
            
            
            void DrawBuildings()
            {
                if(_destinationPositions == null || _destinationPositions.Length == 0)
                    return;

                for (var i = 0; i < _destinationPositions.Length; i++)
                {
                    Gizmos.color = _destinationPositions[i].color;
                    Gizmos.DrawWireCube(new Vector3(_destinationPositions[i].pos.x, _destinationPositions[i].pos.y, 0f), Vector3.one);
                }
            }
            
            Vector3[] GetRectangleVertices(Vector3 labelPosition, float size)
            {
                float halfWidth = size / 2;
                float halfHeight = size / 2;

                return new Vector3[]
                {
                    labelPosition + new Vector3(-halfWidth, -halfHeight, 0),
                    labelPosition + new Vector3(halfWidth, -halfHeight, 0),
                    labelPosition + new Vector3(halfWidth, halfHeight, 0),
                    labelPosition + new Vector3(-halfWidth, halfHeight, 0)
                };
            }
        }
    }
}
