using PhysicsBenchmark.Helpers;
using UnityEngine;

namespace PhysicsBenchmark.Settings
{
    public class ClassicSettingsInitializer : MonoBehaviour
    {
        private void Awake()
        {
            DataTransferHelper.CreateOrUpdateData();
        }
    }
}