using PhysicsBenchmark.Settings;
using UnityEngine;

namespace PhysicsBenchmark.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraRepositioner : MonoBehaviour
    {
        private void Start()
        {
            float cubeSize = Mathf.Max(ClassicSettings.height, ClassicSettings.length);
            
            float distance = cubeSize / 1.1547006f; 

            distance *= 3f; 

            float angle = Mathf.Deg2Rad * 45f;
            float x = distance * Mathf.Cos(angle);
            float z = distance * Mathf.Sin(angle);

            transform.position = new Vector3(x, distance, z);
            transform.rotation = Quaternion.Euler(45, 45, 0);

            transform.LookAt(Vector3.zero);
        }
    }
}