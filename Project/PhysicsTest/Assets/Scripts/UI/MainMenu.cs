using System;
using UnityEngine;
using UnityEngine.UI;

namespace PhysicsBenchmark.UI
{
    public class MainMenu : MonoBehaviour
    {
        public enum TestIdentifier
        {
            None,
            Classic,
        }

        public static event Action<TestIdentifier> MenuTestSelectedEvent;
        
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
        
        [Header("Tests")] 
        [SerializeField] private GameObject _testsParent;
        [SerializeField] private Button _classicButton;
        

        private void Awake()
        {
            _playButton.onClick.AddListener(() =>
            {
                _mainMenuParent.SetActive(false);
                _testsParent.SetActive(true);
                _backToMainButtonParent.SetActive(true);
            });
            _backToMainButton.onClick.AddListener(() =>
            {
                _mainMenuParent.SetActive(true);
                _testsParent.SetActive(false);
                _backToMainButtonParent.SetActive(false);
                MenuTestSelectedEvent?.Invoke(TestIdentifier.None);
            });
            _infoButton.onClick.AddListener(() =>
            {
                _mainMenuParent.SetActive(false);
                _infoParent.SetActive(true);
                _backToMainButtonParent.SetActive(true);
            });
            _creditsButton.onClick.AddListener(() =>
            {
                _mainMenuParent.SetActive(false);
                _creditsParent.SetActive(true);
                _backToMainButtonParent.SetActive(true);
            });
            
            _classicButton.onClick.AddListener(() =>
            {
                _mainMenuParent.SetActive(false);
                _testsParent.SetActive(false);
                MenuTestSelectedEvent?.Invoke(TestIdentifier.Classic);
            });
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
            _backToMainButton.onClick.RemoveAllListeners();
            _infoButton.onClick.RemoveAllListeners();
            _creditsButton.onClick.RemoveAllListeners();
            _classicButton.onClick.RemoveAllListeners();
        }
    }
}