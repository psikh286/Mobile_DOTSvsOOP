using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PhysicsBenchmark.UI
{
    [RequireComponent(typeof(Button))]
    public class MainMenuButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => SceneManager.LoadScene(0));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}