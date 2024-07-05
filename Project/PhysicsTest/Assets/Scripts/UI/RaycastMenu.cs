using System.Globalization;
using System.Linq;
using PhysicsBenchmark.DOTS.Raycasts;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PhysicsBenchmark.UI
{
    public class RaycastMenu : MonoBehaviour
    {
        [Header("Menu")] 
        [SerializeField] private TMP_InputField _numberInputField;
        [SerializeField] private TMP_InputField _distanceInputField;
        [SerializeField] private Button _testButton;
        [SerializeField] private Toggle _extraLayersToggle;
        [SerializeField] private Toggle _enableDotsToggle;
        
        [Header("World UI")] 
        [SerializeField] private TMP_Text _distanceText;

        [Header("Extra Layer")] 
        [SerializeField] private GameObject _extraLayers;

        [Header("Ref")]
        [SerializeField] private GameObject _cube;
        
        [Header("Result")]
        [SerializeField] private GameObject _resultScreenParent;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private Button _resultCloseButton;

        private bool _dotsEnabled;
        private int _count = 10000;
        private float _time;
        private float _distance = 10;
        private bool _layersOn;
        
        private void Awake()
        {
            _numberInputField.SetTextWithoutNotify(_count.ToString());
            _distanceInputField.SetTextWithoutNotify(_distance.ToString(CultureInfo.CurrentCulture));
            _distanceText.text = $"Distance: {_distance}";
            _cube.transform.position = Vector3.right * _distance;
            TransferData();
            
            _testButton.onClick.AddListener(OnTestPressed);
            _extraLayersToggle.onValueChanged.AddListener(OnLayerUpdated);
            
            _resultCloseButton.onClick.AddListener(() => _resultScreenParent.SetActive(false));
            
            _enableDotsToggle.onValueChanged.AddListener(isOn =>
            {
                _dotsEnabled = isOn;
            });
            
            _distanceInputField.onValueChanged.AddListener(distance =>
            {
                if(!float.TryParse(distance, out var clampedDist))
                    return;
                
                clampedDist = Mathf.Clamp(clampedDist, 0.001f, clampedDist);

                _distance = clampedDist;
                
                _cube.transform.position = Vector3.right * _distance;
                TransferData();
                
                _distanceText.text = $"Distance: {_distance}";
                _distanceInputField.SetTextWithoutNotify(clampedDist.ToString(CultureInfo.CurrentCulture));
            });
            
            _numberInputField.onValueChanged.AddListener(number =>
            {
                if(!int.TryParse(number, out var clampedNum))
                    return;
                
                clampedNum = Mathf.Clamp(clampedNum, 1, clampedNum);
                
                _count = clampedNum;

                _numberInputField.SetTextWithoutNotify(clampedNum.ToString(CultureInfo.CurrentCulture));
            });
        }

        private void OnDestroy()
        {
            _testButton.onClick.RemoveListener(OnTestPressed);
            _extraLayersToggle.onValueChanged.RemoveListener(OnLayerUpdated);
            
            _resultCloseButton.onClick.RemoveAllListeners();
            _enableDotsToggle.onValueChanged.RemoveAllListeners();
            _distanceInputField.onValueChanged.RemoveAllListeners();
            _numberInputField.onValueChanged.RemoveAllListeners();
        }

        
        private void OnTestPressed()
        {
            if (_dotsEnabled)
            {
                RaycastSystem.RaycastCompleteEvent += OnRaycastComplete;

                TransferData();
                
                _time = Time.realtimeSinceStartup;
                World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingSystemState<RaycastSystem>().Enabled = true;
            }
            else
            {
                _time = Time.realtimeSinceStartup;

                var hits = new RaycastHit[_count];
                
                for (int i = 0; i < _count; i++)
                {
                    if (Physics.Raycast(Vector3.zero, Vector3.right, out var hit, _distance))
                    {
                        hits[i] = hit;
                    }
                }
                
                var l =  hits.Count(r => r.rigidbody != null);
                 
                _time = (Time.realtimeSinceStartup - _time) * 1000f;
                ShowResult(l);
            }
        }

        private void OnRaycastComplete(RaycastResultInfo info)
        {
            RaycastSystem.RaycastCompleteEvent -= OnRaycastComplete;
            _time = (Time.realtimeSinceStartup - _time) * 1000f;
            ShowResult(info.successfulHits);
        }

        private void OnLayerUpdated(bool isOn)
        {
            _layersOn = isOn;
            
            _extraLayers.SetActive(isOn);
        }

        private void TransferData()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery query = entityManager.CreateEntityQuery(typeof(RaycastSettingsData));
            
            RaycastSettingsData settingsData = new RaycastSettingsData
            {
                count = _count,
                distance = _distance,
                layersOn = _layersOn
            };

            if (query.CalculateEntityCount() == 0)
            {
                // Singleton does not exist, create it
                Entity singletonEntity = entityManager.CreateEntity(typeof(RaycastSettingsData));
                entityManager.SetComponentData(singletonEntity, settingsData);
            }
            else
            {
                // Singleton exists, update it
                Entity singletonEntity = query.GetSingletonEntity();
                entityManager.SetComponentData(singletonEntity, settingsData);
            }
        }

        private void ShowResult(int successful)
        {
            _resultScreenParent.SetActive(true);
            _resultText.text = $"Architecture: {(_dotsEnabled ? "DOTS" : "OOP")} \nRay Count: {_count} \nDistance: {_distance} \nTime: {_time:F4}ms \nSuccessful Hits: {successful}";
        }
    }
}