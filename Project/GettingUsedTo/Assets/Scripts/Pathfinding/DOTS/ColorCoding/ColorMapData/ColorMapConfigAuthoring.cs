using System.Linq;
using System.Text.RegularExpressions;
using Pathfinding.DOTS.ColorCoding.Sprites;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding.DOTS.ColorCoding.ColorMapData
{
    public class ColorMapConfigAuthoring : MonoBehaviour
    {
        [System.Serializable]
        internal class DestData
        {
            public int2[] positions;
            public Sprite[] sprites;
        }
        
        public int2[] spawnPos;
        [SerializeField] internal DestData[] destPos;
        
        private class Baker : Baker<ColorMapConfigAuthoring>
        {
            public override void Bake(ColorMapConfigAuthoring authoring)
            {
                var blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var root = ref blobBuilder.ConstructRoot<ColorMapPositionsBlobAsset>();
                BlobBuilderArray<int2> spawn = blobBuilder.Allocate(ref root.spawnPos, authoring.spawnPos.Length);
                BlobBuilderArray<int2> destination = blobBuilder.Allocate(ref root.destPos, authoring.destPos.Sum(destData => destData.positions.Length));
                
                for (var i = 0; i < authoring.spawnPos.Length; i++)
                {
                    spawn[i] = authoring.spawnPos[i];
                }
                
                for (var x = 0; x < authoring.destPos.Length; x++)
                {
                    for (int y = 0; y < authoring.destPos[x].positions.Length; y++)
                    {
                        destination[x + y * 3] = authoring.destPos[x].positions[y];
                    }
                }
                
                var blobAssetReference = blobBuilder.CreateBlobAssetReference<ColorMapPositionsBlobAsset>(Allocator.Persistent);
                blobBuilder.Dispose();
                
                AddBlobAsset(ref blobAssetReference, out _);
                
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ColorMapPositionData
                {
                    blobAssetReference = blobAssetReference
                });

                var sprites = new Sprite[authoring.destPos.Sum(destData => destData.sprites.Length)];
                
                for (var x = 0; x < authoring.destPos.Length; x++)
                {
                    for (int y = 0; y < authoring.destPos[x].sprites.Length; y++)
                    {
                        sprites[x + y * 3] = authoring.destPos[x].sprites[y];
                    }
                }
                
                AddComponentObject(entity, new ColorSpritesData
                {
                    sprites = sprites
                });
            }
        }
        
        
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            DrawSpawners();
            DrawBuildings();
            return;
            
            void DrawSpawners()
            {
                if(spawnPos == null || spawnPos.Length == 0)
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
                
                for (var i = 0; i < spawnPos.Length; i++)
                {
                    UnityEditor.Handles.DrawSolidRectangleWithOutline(GetRectangleVertices(new Vector3(spawnPos[i].x, spawnPos[i].y), 1f), Color.black, Color.white);
                    UnityEditor.Handles.Label(new Vector3(spawnPos[i].x, spawnPos[i].y), $"s{i}", style);
                }
                
            }
            
            
            void DrawBuildings()
            {
                if(destPos == null || destPos.Length == 0)
                    return;

                for (var i = 0; i < destPos.Length; i++)
                {
                    Color color;

                    switch (i)
                    {
                        case 0:
                            color = Color.yellow;
                            break;
                        case 1:
                            color = Color.red;
                            break;
                        case 2:
                            color = Color.blue;
                            break;
                        default:
                            color = Color.magenta;
                            break;
                        
                    }
                    
                    Gizmos.color = color;
                    
                    for (var j = 0; j < destPos[i].positions.Length; j++)
                    {
                        Gizmos.DrawWireCube(new Vector3(destPos[i].positions[j].x, destPos[i].positions[j].y, 0f), Vector3.one);
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
                        case (>= 44 and <= 53):
                        {
                            AddItem(2);
                            continue;
                        }
                    }

                    continue;

                    void AddItem(int i)
                    {
                        var list = destPos[i].positions.ToList();
                        
                        if (list.Contains(new int2(x, y)))
                            return;
                        
                        list.Add(new int2(x, y));
                        destPos[i].positions = list.ToArray();
                    }
                }
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
#endif
    }
}