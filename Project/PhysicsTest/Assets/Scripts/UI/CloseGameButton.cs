using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class CloseGameButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Application.Quit();
        }
    }
}
