using UnityEngine;

namespace PhysicsBenchmark.OOP.Raycasts
{
    public class RaycastBenchmark : MonoBehaviour
    {
        [SerializeField] private int _count = 10000;
        
        private void Update()
        {
            for (int i = 0; i < _count; i++)
            {
                Physics.Raycast(Vector3.zero, Vector3.up, out var hit, 5f);
            }
        }
    }
}