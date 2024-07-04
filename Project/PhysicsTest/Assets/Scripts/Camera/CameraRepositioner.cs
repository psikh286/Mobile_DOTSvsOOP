using PhysicsBenchmark.Settings;
using UnityEngine;

namespace PhysicsBenchmark.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraRepositioner : MonoBehaviour
    {
        private void Start()
        {
            var cubeSize = new Vector3(ClassicSettings.length, ClassicSettings.height, ClassicSettings.length);
            
            var cubeDiagonal = Mathf.Sqrt(cubeSize.x * cubeSize.x + cubeSize.y * cubeSize.y + cubeSize.z * cubeSize.z);

            var orthographicSize = cubeDiagonal / (2f * Mathf.Sqrt(3f) / 2f);

            GetComponent<UnityEngine.Camera>().orthographicSize = orthographicSize;
        }
    }
}