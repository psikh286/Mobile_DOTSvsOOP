using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = Unity.Mathematics.Random;

namespace Pathfinding.OOP
{
    // ReSharper disable once InconsistentNaming
    public class OOPathfindingCoupledController : MonoBehaviour
    {
        [System.Serializable]
        private struct ColorData
        {
            public Color color;
            public CarColor carColor;
            public int2[] pos;
            public Sprite[] sprites;
        }
        
        private enum CarColor : byte
        {
            None = 0,
            Orange = 1,
            Red = 2,
            Blue =3,
        }
        
        private struct Agent
        {
            public SpriteRenderer spriteRenderer;
            public List<Vector2> path;
            public int nodeCount;
            public CarColor color;
        }
        
        [SerializeField] private SpriteRenderer _agentPrefab;
        [SerializeField] private ObjectOrientedPathfinding _pathfinding;
        [SerializeField] private Vector2[] _spawnersPositions;
        [SerializeField] private ColorData[] _colorData;

        [Header("Temp")] 
        [SerializeField] private float _carSpeed;
        private int _capacity;
        private int _coupleNumber;
        

        private Agent[] _agents;
        
        private Dictionary<CarColor, int2[]> _buildingPositions;
        private Dictionary<CarColor, Sprite[]> _carSprites;

        private CarColor _currentColor;
        private Vector2 _currentSpawnPosition;
        private int2 _currentDestinationPosition;
        private Sprite _currentSprite;

        private Random _random = new(PathfindingSettings.Seed);
        

        private void Awake()
        {
            _buildingPositions = new Dictionary<CarColor, int2[]>();
            _carSprites = new Dictionary<CarColor, Sprite[]>();
            
            foreach (var data in _colorData)
            {
                _buildingPositions.Add(data.carColor, data.pos);
                _carSprites.Add(data.carColor, data.sprites);
            }
        }

        public void Init()
        {
            enabled = true;
            _pathfinding.Init();

            _capacity = PathfindingSettings.AgentsCount;
            _coupleNumber = PathfindingSettings.AgentsPerCouple;
            _agents = new Agent[_capacity];
            
            for (int i = 0; i < _capacity; i++)
            {
                _agents[i] = new Agent()
                {
                    spriteRenderer = Instantiate(_agentPrefab, Vector3.one * -10f, Quaternion.identity),
                    path = new List<Vector2>(),
                };
            }
        }
        
        private void Update()
        {
            for (var i = 0; i < _agents.Length; i++)
            {
                if (_agents[i].nodeCount >= _agents[i].path.Count)
                {
                    if (i % _coupleNumber == 0)
                    {
                        _currentColor = (CarColor)_random.NextInt(1, 4);
                        _currentSprite = GetCarSprite(_currentColor);
                        _currentSpawnPosition = GetSpawnPosition();
                        _currentDestinationPosition = GetDestinationPosition(_currentColor);
                    }
                    
                    _agents[i].nodeCount = 0;
                    
                    var pos = _currentSpawnPosition;
                    var dest = _currentDestinationPosition;
                    var path = _pathfinding.FindPath((int)pos.x, (int)pos.y, dest.x, dest.y);
                    
                    if (path == null)
                    {
                        _agents[i].path = new List<Vector2>(0);
                        continue;
                    }
                    
                    _agents[i].color = _currentColor;
                    _agents[i].spriteRenderer.transform.position = _currentSpawnPosition;
                    _agents[i].spriteRenderer.sprite = _currentSprite;
                    _agents[i].path = path;
                }
                    
                var moveTo = Vector3.MoveTowards(_agents[i].spriteRenderer.transform.position, _agents[i].path[_agents[i].nodeCount], _carSpeed * Time.deltaTime);

                _agents[i].spriteRenderer.flipX = moveTo.x < _agents[i].spriteRenderer.transform.position.x;
                    
                _agents[i].spriteRenderer.transform.position = moveTo;
                    
                if (_agents[i].spriteRenderer.transform.position == (Vector3)_agents[i].path[_agents[i].nodeCount])
                    _agents[i].nodeCount++;
            }
        }
        

        private Vector2 GetSpawnPosition()
        {
            return _spawnersPositions[_random.NextInt(_spawnersPositions.Length)];
        }

        private int2 GetDestinationPosition(CarColor color)
        {
            if (_buildingPositions.TryGetValue(color, out var positions))
                return positions[_random.NextInt(positions.Length)];
            
            
            Debug.LogError($"[OOPathfindingController] couldn't find a destination with color: {color}");
            return new int2(-1, -1);
        }

        private Sprite GetCarSprite(CarColor color)
        {
            if (_carSprites.TryGetValue(color, out var sprites))
                return sprites[_random.NextInt(sprites.Length)];
            
            
            Debug.LogError($"[OOPathfindingController] couldn't find a sprite with color: {color}");
            return null;
        }
        
#if UNITY_EDITOR
        
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
                if(_colorData == null || _colorData.Length == 0)
                    return;

                for (var i = 0; i < _colorData.Length; i++)
                {
                    Gizmos.color = _colorData[i].color;
                    
                    for (var j = 0; j < _colorData[i].pos.Length; j++)
                    {
                        Gizmos.DrawWireCube(new Vector3(_colorData[i].pos[j].x, _colorData[i].pos[j].y, 0f), Vector3.one);
                    }
                }
            }
            
            Vector3[] GetRectangleVertices(Vector3 labelPosition, float size)
            {
                float halfWidth = size / 2;
                float halfHeight = size / 2;

                return new[]
                {
                    labelPosition + new Vector3(-halfWidth, -halfHeight, 0),
                    labelPosition + new Vector3(halfWidth, -halfHeight, 0),
                    labelPosition + new Vector3(halfWidth, halfHeight, 0),
                    labelPosition + new Vector3(-halfWidth, halfHeight, 0)
                };
            }
        }
        
        [SerializeField] private Tilemap _buildingTilemap;
        
        [ContextMenu("FillBuildings")]
        private void FillBuildings()
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    var pos = new Vector3Int(x, y);
                    
                    if(!_buildingTilemap.HasTile(pos))
                        continue;
                    
                    var id= int.Parse(Regex.Match(_buildingTilemap.GetSprite(pos).name, @"\d+").Value);

                    switch (id)
                    {
                        case >= 80 and <= 89:
                        {
                            AddItem(0);
                            continue;
                        }
                        case >= 62 and <= 71:
                        {
                            AddItem(1);
                            continue;
                        }
                        case >= 44 and <= 53:
                        {
                            AddItem(2);
                            continue;
                        }
                    }

                    continue;

                    void AddItem(int i)
                    {
                        var list = _colorData[i].pos.ToList();
                        
                        if (list.Contains(new int2(x, y)))
                            return;
                        
                        list.Add(new int2(x, y));
                        _colorData[i].pos = list.ToArray();
                    }
                }
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
#endif
    }
}