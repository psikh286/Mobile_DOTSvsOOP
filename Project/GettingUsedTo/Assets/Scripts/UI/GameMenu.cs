using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameMenu : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _homeButton;
        
        [Header("Info")] 
        [SerializeField] private GameObject _infoBoardParent;
        [SerializeField] private TMP_Text _agentCountText;
        [SerializeField] private TMP_Text _agentPerGroupText;
        [SerializeField] private TMP_Text _seedText;

        [Header("Speed")] 
        [SerializeField] private Button _slowButton;
        [SerializeField] private Button _fastButton;
        [SerializeField] private CanvasGroup _fastButtonCanvas;
        [SerializeField] private CanvasGroup _slowButtonCanvas;
        [SerializeField] private TMP_Text _speedText;
        
        [Header("Pause")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Image _pauseIconImage;
        [SerializeField] private Sprite _playIcon;
        [SerializeField] private Sprite _pauseIcon;

        private readonly float[] _speedValues = {0.5f, 1f, 2f, 5f, 10f};
        private float _lastTimeScale;
        private int _speedIndex = 1;
        
        private void Awake()
        {
            UpdateStats();
            
            _pauseButton.onClick.AddListener(OnGamePaused);
            
            _slowButton.onClick.AddListener(() => OnGameSpeedChanged(-1));
            _fastButton.onClick.AddListener(() => OnGameSpeedChanged(+1));
            
            _homeButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            });
            
            _infoButton.onClick.AddListener(() =>
            {
                _infoBoardParent.SetActive(!_infoBoardParent.activeInHierarchy);
            });
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnGamePaused);
            
            _slowButton.onClick.RemoveAllListeners();
            _fastButton.onClick.RemoveAllListeners();

            _homeButton.onClick.RemoveAllListeners();

            _infoButton.onClick.RemoveAllListeners();
        }


        private void OnGameSpeedChanged(int i)
        {
            _speedIndex = Mathf.Clamp(_speedIndex + i, 0, _speedValues.Length - 1);
            
            
            if (Time.timeScale == 0f)
                _lastTimeScale = _speedValues[_speedIndex];
            else
                Time.timeScale = _speedValues[_speedIndex];
            
            _speedText.text = $"SPEED: {_speedValues[_speedIndex]}";
            
            
            if (_speedIndex == 0)
            {
                _slowButtonCanvas.interactable = false;
                _slowButtonCanvas.alpha = 0f;
                return;
            }

            if (_speedIndex >= _speedValues.Length - 1)
            {
                _fastButtonCanvas.interactable = false;
                _fastButtonCanvas.alpha = 0f;
                return;
            }

            _slowButtonCanvas.interactable = true;
            _fastButtonCanvas.interactable = true;
            _slowButtonCanvas.alpha = 1f;
            _fastButtonCanvas.alpha = 1f;
        }

        private void OnGamePaused()
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = _lastTimeScale;
                _pauseIconImage.sprite = _pauseIcon;
            }
            else
            {
                _lastTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                
                _pauseIconImage.sprite = _playIcon;
            }
        }
        
        private void UpdateStats()
        {
            var agentsPerCouple = PathfindingSettings.AllowCoupling ? PathfindingSettings.AgentsPerCouple : 1;
            
            _agentCountText.text = $"AGENTS COUNT: {PathfindingSettings.AgentsCount}";
            _agentPerGroupText.text = $"AGENTS PER GROUP: {agentsPerCouple}";
            _seedText.text =  $"SEED: {PathfindingSettings.Seed}";
            
            
            _speedText.text = $"SPEED: {_speedValues[_speedIndex]}";
        }
    }
}