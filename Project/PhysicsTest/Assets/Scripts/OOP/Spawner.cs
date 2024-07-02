using PhysicsBenchmark.DOTS.Classic.Spawner;
using PhysicsBenchmark.Settings;
using UnityEngine;

namespace PhysicsBenchmark.OOP
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform _cubePrefab;
        [SerializeField] private Transform _spherePrefab;
        
        private void Start()
        {
            if (ClassicSettings.formationIdentifier == FormationIdentifier.Perimeter)
            {
                SpawnPerimeter();
            }
            else
            {
                SpawnCheckerboard();
            }
        }

        private void SpawnPerimeter()
        {
            var prefab = ClassicSettings.enableSphere ? _spherePrefab : _cubePrefab;
            var offset = (ClassicSettings.length - 1f) * 0.5f;
            var height = ClassicSettings.heightOffset + ClassicSettings.height;
            
            for (int y = 0; y < ClassicSettings.height; y++)
            {
                for (int i = 0; i < ClassicSettings.length; i++)
                {
                    var x = i - offset;
                
                    Quaternion rot = Quaternion.Euler(0f, ClassicSettings.angle * y, 0f);
                
                    Instantiate(prefab, rot * new Vector3(x, height, -offset), rot);
                    Instantiate(prefab, rot * new Vector3(x, height, offset), rot);
                }

                for (int i = 1; i < ClassicSettings.length - 1; i++)
                {
                    var z = i - offset;
                
                    Quaternion rot = Quaternion.Euler(0f, ClassicSettings.angle * y, 0f);
                
                    Instantiate(prefab, rot * new Vector3(-offset, height, z), rot);
                    Instantiate(prefab, rot * new Vector3(offset, height, z), rot);
                }
            }
        }

        private void SpawnCheckerboard()
        {
            var prefab = ClassicSettings.enableSphere ? _spherePrefab : _cubePrefab;
            var offset = (ClassicSettings.length - 1f) * 0.5f;
            var height = ClassicSettings.heightOffset + ClassicSettings.height;
            
            for (int y = 0; y < ClassicSettings.height; y++)
            {
                for (int i = 0; i < ClassicSettings.length; i++)
                {
                    if ((y % 2 == 0 && i % 2 != 0) || (y % 2 != 0 && i % 2 == 0))
                        continue;
                    
                    var x = i - offset;
                    
                    Instantiate(prefab, new Vector3(x, height, -offset), Quaternion.identity);
                    Instantiate(prefab, new Vector3(x, height, offset), Quaternion.identity);
                }

                for (int i = 1; i < ClassicSettings.length - 1; i++)
                {
                    if ((y % 2 == 0 && i % 2 != 0) || (y % 2 != 0 && i % 2 == 0))
                        continue;
                    
                    var z = i - offset;
                
                    Instantiate(prefab, new Vector3(-offset, height, z), Quaternion.identity);
                    Instantiate(prefab, new Vector3(offset, height, z), Quaternion.identity);
                }
            }
        }
    }
}