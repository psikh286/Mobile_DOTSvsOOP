using PhysicsBenchmark.Helpers;
using PhysicsBenchmark.Settings;
using TMPro;
using UnityEngine;

namespace PhysicsBenchmark.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class CubeCounter : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<TMP_Text>().text = (ClassicSettings.enableSphere ? "Sphere" : "Cube") + $" Count: {CubeCountHelper.GetNumber()}";
        }
    }
}