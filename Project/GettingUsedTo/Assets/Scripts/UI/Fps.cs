using System.Collections;
using UnityEngine;

namespace UI
{
    public class Fps : MonoBehaviour
    {
        private float _count;
        private WaitForSecondsRealtime _delay = new(0.1f);
        
        private IEnumerator Start()
        {
            GUI.depth = 2;
            while (true)
            {
                _count = 1f / Time.unscaledDeltaTime;
                yield return _delay;
            }
        }
        
        private void OnGUI()
        {
            GUI.Label(new Rect(5, 40, 100, 25), "FPS: " + Mathf.Round(_count), new GUIStyle(){fontSize = 100});
        }
    }
}