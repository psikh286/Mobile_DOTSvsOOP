using Helpers;
using UnityEngine;

namespace Pathfinding.DOTS.DotsSettings
{
    public class SettingsInitializer : MonoBehaviour
    {
        private void Awake() => DataTransferHelper.CreateOrUpdateData();
    }
}