using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Theme")] 
        [SerializeField] private GameObject _darkThemeParent;
        [SerializeField] private GameObject _lightThemeParent;
        [SerializeField] private Button _switchThemeButton;
        
        [Header("Main")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _creditsButton;
        [Space]
        [SerializeField] private GameObject _mainMenuParent;
        [SerializeField] private Button _backToMainButton;
        [SerializeField] private GameObject _backToMainButtonParent;
        
        [Header("Info")] 
        [SerializeField] private GameObject _infoParent; 

        [Header("Credits")] 
        [SerializeField] private GameObject _creditsParent;

        [Header("Settings")] 
        [SerializeField] private Button _settingsBackButton;
        [SerializeField] private GameObject _settingsBackButtonParent;
        [SerializeField] private GameObject _settingsParent; 
        [SerializeField] private GameObject _locationParent; 
        [SerializeField] private GameObject _diagonalParent; 
        [SerializeField] private GameObject _couplingParent; 
        [SerializeField] private GameObject _lastPageParent; 
        
        [Header("Settings Header")]
        [SerializeField] private GameObject _settingsHeaderParent; 
        [SerializeField] private TMP_Text _settingsHeaderText;
        [SerializeField] private Button _settingsHeaderInfoButton;
        
        [Header("Info")]
        [SerializeField] private GameObject _settingsInfoParent;
        [SerializeField] private TMP_Text _settingsInfoText;
        [SerializeField] private Button _settingsInfoCloseButton;
        
        [Header("Choice Buttons")]
        [SerializeField] private Button _oopButton;
        [SerializeField] private Button _dotsButton;
        [SerializeField] private Button _carsButton;
        [SerializeField] private Button _boatsButton;
        [SerializeField] private Button _diagonalYesButton;
        [SerializeField] private Button _diagonalNoButton;
        [SerializeField] private Button _couplingYesButton;
        [SerializeField] private Button _couplingNoButton;
        
        [Header("Diagonal")]
        [SerializeField] private Image _diagonalYesImage;
        [SerializeField] private Image _diagonalNoImage;
        [SerializeField] private Sprite _diagonalYesBoatSprite;
        [SerializeField] private Sprite _diagonalNoBoatSprite;
        [SerializeField] private Sprite _diagonalYesCarSprite;
        [SerializeField] private Sprite _diagonalNoCarSprite;
        
        [Header("Coupling")]
        [SerializeField] private Image _couplingYesImage;
        [SerializeField] private Image _couplingNoImage;
        [SerializeField] private Sprite _couplingYesBoatSprite;
        [SerializeField] private Sprite _couplingNoBoatSprite;
        [SerializeField] private Sprite _couplingYesCarSprite;
        [SerializeField] private Sprite _couplingNoCarSprite;

        [Header("Last Page")] 
        [SerializeField] private Button _actualPlayButton;
        
        private bool _lightThemeOn;

        private bool _dotsSelected;
        private bool _carsSelected;
        private bool _diagonalsAllowed;
        private bool _couplingAllowed;
        
        private int _agentCount;
        private int _agentsPerCouple;
        private int _seed;

        private void Awake()
        {
            if (PlayerPrefs.HasKey("Theme"))
            {
                _lightThemeOn = PlayerPrefs.GetInt("Theme") == 0;
                OnSwitchTheme();
            }
            
            _switchThemeButton.onClick.AddListener(OnSwitchTheme);
            
            _creditsButton.onClick.AddListener(OnCreditsChanged);
            _infoButton.onClick.AddListener(OnInfoChanged);
            
            _backToMainButton.onClick.AddListener(OnMainMenuOpen);
            
            _playButton.onClick.AddListener(OnSettingsOpened);
            
            _settingsHeaderInfoButton.onClick.AddListener(() => _settingsInfoParent.SetActive(true));
            _settingsInfoCloseButton.onClick.AddListener(() => _settingsInfoParent.SetActive(false));
            
            _actualPlayButton.onClick.AddListener(OnTestStart);
            
            _oopButton.onClick.AddListener(() =>
            {
                _dotsSelected = false;
                OnLocationOpened();
            });
            
            _dotsButton.onClick.AddListener(() =>
            {
                _dotsSelected = true;
                OnLocationOpened();
            });
            
            _boatsButton.onClick.AddListener(() =>
            {
                _carsSelected = false;
                OnDiagonalOpened();
            });
            
            _carsButton.onClick.AddListener(() =>
            {
                _carsSelected = true;
                OnDiagonalOpened();
            });
            
            _diagonalNoButton.onClick.AddListener(() =>
            {
                _diagonalsAllowed = false;
                OnCouplingOpened();
            });
            
            _diagonalYesButton.onClick.AddListener(() =>
            {
                _diagonalsAllowed = true;
                OnCouplingOpened();
            });
            
            _couplingYesButton.onClick.AddListener(() =>
            {
                _couplingAllowed = false;
                OnLastPageOpened();
            });
            
            _couplingNoButton.onClick.AddListener(() =>
            {
                _couplingAllowed = true;
                OnLastPageOpened();
            });
        }
        
        private void OnDestroy()
        {
            _switchThemeButton.onClick.RemoveListener(OnSwitchTheme);
            _creditsButton.onClick.RemoveListener(OnCreditsChanged);
            _infoButton.onClick.RemoveListener(OnInfoChanged);
            _backToMainButton.onClick.RemoveListener(OnMainMenuOpen);
            _playButton.onClick.RemoveListener(OnSettingsOpened);
            
            _actualPlayButton.onClick.RemoveListener(OnTestStart);
            
            _settingsHeaderInfoButton.onClick.RemoveAllListeners();
            _settingsInfoCloseButton.onClick.RemoveAllListeners();
            
            _oopButton.onClick.RemoveAllListeners();
            _dotsButton.onClick.RemoveAllListeners();
            _carsButton.onClick.RemoveAllListeners();
            _boatsButton.onClick.RemoveAllListeners();
            _diagonalYesButton.onClick.RemoveAllListeners();
            _diagonalNoButton.onClick.RemoveAllListeners();
            _couplingYesButton.onClick.RemoveAllListeners();
            _couplingNoButton.onClick.RemoveAllListeners();
        }

        #region Main Menu

        
        private void OnSwitchTheme()
        {
            _lightThemeOn = !_lightThemeOn;
            
            _darkThemeParent.SetActive(!_lightThemeOn);
            _lightThemeParent.SetActive(_lightThemeOn);

            PlayerPrefs.SetInt("Theme", _lightThemeOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnCreditsChanged()
        {
            _creditsParent.SetActive(true);
            _backToMainButtonParent.SetActive(true);
            _mainMenuParent.SetActive(false);
        }
        
        private void OnInfoChanged()
        {
            _infoParent.SetActive(true);
            _backToMainButtonParent.SetActive(true);
            _mainMenuParent.SetActive(false);
        }

        private void OnMainMenuOpen()
        {
            _mainMenuParent.SetActive(true);
            
            _backToMainButtonParent.SetActive(false);
            _infoParent.SetActive(false);
            _creditsParent.SetActive(false);
            _settingsParent.SetActive(false);
            _settingsHeaderParent.SetActive(false);
        }

        #endregion

        #region Settings

        private void OnSettingsOpened()
        {
            _settingsParent.SetActive(true);
            _settingsHeaderParent.SetActive(true);
            _backToMainButtonParent.SetActive(true);
            _mainMenuParent.SetActive(false);
            _locationParent.SetActive(false);
            _settingsBackButtonParent.SetActive(false);

            _settingsHeaderText.text = "Architecture";
            _settingsInfoText.text = "OOP - Object Oriented; DOTS - Data...";
        }

        private void OnLocationOpened()
        {
            _locationParent.SetActive(true);
            _settingsParent.SetActive(false);
            _diagonalParent.SetActive(false);
            _settingsBackButtonParent.SetActive(true);
            _backToMainButtonParent.SetActive(false);
            
            _settingsBackButton.onClick.RemoveAllListeners();
            _settingsBackButton.onClick.AddListener(OnSettingsOpened);
            
            _settingsHeaderText.text = "MOVING TYPE";
            _settingsInfoText.text = "roads are narrow, water is open space";
        }

        private void OnDiagonalOpened()
        {
            _diagonalParent.SetActive(true);
            _locationParent.SetActive(false);
            _couplingParent.SetActive(false);

            _diagonalYesImage.sprite = _carsSelected ? _diagonalYesCarSprite : _diagonalYesBoatSprite;
            _diagonalNoImage.sprite = _carsSelected ? _diagonalNoCarSprite : _diagonalNoBoatSprite;
            
            _settingsBackButton.onClick.RemoveAllListeners();
            _settingsBackButton.onClick.AddListener(OnLocationOpened);
            
            _settingsHeaderText.text = "Diagonals Allowed";
            _settingsInfoText.text = "disables enables diogonal";
        }

        private void OnCouplingOpened()
        {
            _couplingParent.SetActive(true);
            _diagonalParent.SetActive(false);
            _lastPageParent.SetActive(false);

            _couplingYesImage.sprite = _carsSelected ? _couplingYesCarSprite : _couplingYesBoatSprite;
            _couplingNoImage.sprite = _carsSelected ? _couplingNoCarSprite : _couplingNoBoatSprite;
            
            _settingsBackButton.onClick.RemoveAllListeners();
            _settingsBackButton.onClick.AddListener(OnDiagonalOpened);
            
            _settingsHeaderText.text = "Coupling Allowed";
            _settingsInfoText.text = "coupling so multiple at one spot";
        }

        private void OnLastPageOpened()
        {
            _lastPageParent.SetActive(true);
            _couplingParent.SetActive(false);
            
            _settingsBackButton.onClick.RemoveAllListeners();
            _settingsBackButton.onClick.AddListener(OnCouplingOpened);
            
            _settingsHeaderText.text = "Settings";
            _settingsInfoText.text = "1. Number: how many agents";
        }

        #endregion

        private void OnTestStart()
        {
            PathfindingSettings.AllowCoupling = _couplingAllowed;
            PathfindingSettings.AllowDiagonal = _diagonalsAllowed;
            PathfindingSettings.AgentsCount = _agentCount;
            PathfindingSettings.AgentsPerCouple = _agentsPerCouple;
            PathfindingSettings.Seed = _seed;
            
            int sceneIndex = 1;

            sceneIndex += _dotsSelected ? 1 : 0;
            sceneIndex += _carsSelected ? 0 : 1;
            
            SceneManager.LoadScene(sceneIndex);
        }
    }
}