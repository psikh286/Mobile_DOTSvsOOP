using PhysicsBenchmark.DOTS.Classic.Spawner;
using PhysicsBenchmark.Helpers;
using PhysicsBenchmark.Settings;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PhysicsBenchmark.UI
{
    public class ClassicMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private TMP_Text _cubeCountText;
        [SerializeField] private Button _playButton;
        
        [Header("Settings")]
        [SerializeField] private TMP_InputField _heightInput;
        [SerializeField] private TMP_InputField _lengthInput;
        [SerializeField] private TMP_InputField _angleInput;
        [SerializeField] private TMP_InputField _heightOffsetInput;
        [SerializeField] private Toggle _dotsToggle;
        [SerializeField] private Toggle _sphereToggle;
        //[SerializeField] private Toggle _checkerboardToggle;

        private const int HEIGHT_LIMIT = 1000;
        private const int LENGTH_LIMIT = 1000;
        
        private const int HEIGHT_OFFSET_LOWER_LIMIT = 5;
        private const int HEIGHT_OFFSET_UPPER_LIMIT = 50;

        private bool _dotsEnabled;
        
        private void Awake()
        {
            World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingSystemState<SpawnSystem>().Enabled = true;

            if (PlayerPrefs.HasKey("dotsEnabled"))
                _dotsEnabled = PlayerPrefs.GetInt("dotsEnabled") != 0;
            
            MainMenu.MenuTestSelectedEvent += OnMenuSelected;
            
            _heightInput.onValueChanged.AddListener(height =>
            {
                ClassicSettings.height = Mathf.Clamp(int.Parse(height), 1, HEIGHT_LIMIT);
                _heightInput.SetTextWithoutNotify(ClassicSettings.height.ToString());
                UpdateCubeCount();
            });
            
            _lengthInput.onValueChanged.AddListener(length =>
            {
                ClassicSettings.length = Mathf.Clamp(int.Parse(length), 1, LENGTH_LIMIT);
                _lengthInput.SetTextWithoutNotify(ClassicSettings.length.ToString());
                UpdateCubeCount();
            });
            
            _heightInput.onSubmit.AddListener(_ => UpdateLengthAndHeight());
            _lengthInput.onSubmit.AddListener(_ => UpdateLengthAndHeight());
            
            _lengthInput.onValueChanged.AddListener(length =>
            {
                ClassicSettings.length = Mathf.Clamp(int.Parse(length), 1, LENGTH_LIMIT);
                _lengthInput.SetTextWithoutNotify(ClassicSettings.length.ToString());
                UpdateCubeCount();
            });

            
            _heightOffsetInput.onValueChanged.AddListener(offset =>
            {
                ClassicSettings.heightOffset = Mathf.Clamp(int.Parse(offset), HEIGHT_OFFSET_LOWER_LIMIT, HEIGHT_OFFSET_UPPER_LIMIT);
                _heightOffsetInput.SetTextWithoutNotify(ClassicSettings.heightOffset.ToString("F0"));
            });
            
            _angleInput.onValueChanged.AddListener(angle =>
            {
                ClassicSettings.angle = Mathf.Clamp(int.Parse(angle), 0, 45);
                _angleInput.SetTextWithoutNotify(ClassicSettings.angle.ToString("F0"));
            });
            
            _dotsToggle.onValueChanged.AddListener(dots =>
            {
                _dotsEnabled = dots;
            });
            
            _sphereToggle.onValueChanged.AddListener(sphere =>
            {
                ClassicSettings.enableSphere = sphere;
            });
            
            _playButton.onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("dotsEnabled", _dotsEnabled ? 1 : 0);
                UpdateLengthAndHeight();
                SceneManager.LoadScene(1 + (_dotsEnabled ? 1 : 0));
            });
        }

        private void OnDestroy()
        {
            MainMenu.MenuTestSelectedEvent -= OnMenuSelected;
            
            _heightInput.onValueChanged.RemoveAllListeners();
            _angleInput.onValueChanged.RemoveAllListeners();
            _heightOffsetInput.onValueChanged.RemoveAllListeners();
            _lengthInput.onValueChanged.RemoveAllListeners();
            _dotsToggle.onValueChanged.RemoveAllListeners();
            _sphereToggle.onValueChanged.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();

            _heightInput.onSubmit.RemoveAllListeners();
            _lengthInput.onSubmit.RemoveAllListeners();
        }

        private void OnMenuSelected(MainMenu.TestIdentifier identifier)
        {
            _parent.SetActive(identifier == MainMenu.TestIdentifier.Classic);
            UpdateMenu();
        }
        
        private void UpdateMenu()
        {
            _heightInput.SetTextWithoutNotify(ClassicSettings.height.ToString());
            _lengthInput.SetTextWithoutNotify(ClassicSettings.length.ToString());
            _angleInput.SetTextWithoutNotify(ClassicSettings.angle.ToString("F0"));
            _heightOffsetInput.SetTextWithoutNotify(ClassicSettings.heightOffset.ToString("F0"));
            
            _dotsToggle.SetIsOnWithoutNotify(_dotsEnabled);
            _sphereToggle.SetIsOnWithoutNotify(ClassicSettings.enableSphere);
            
            UpdateCubeCount();
        }

        private void UpdateCubeCount()
        {
            _cubeCountText.text = "#" + CubeCountHelper.GetNumber();
        }

        private void UpdateLengthAndHeight()
        {
            ClassicSettings.height = Mathf.Clamp(int.Parse(_heightInput.text), 3, HEIGHT_LIMIT);
            _heightInput.SetTextWithoutNotify(ClassicSettings.height.ToString());
            
            ClassicSettings.length = Mathf.Clamp(int.Parse(_lengthInput.text), 3, LENGTH_LIMIT);
            _lengthInput.SetTextWithoutNotify(ClassicSettings.length.ToString());
            
            UpdateCubeCount();
        }
    }
}