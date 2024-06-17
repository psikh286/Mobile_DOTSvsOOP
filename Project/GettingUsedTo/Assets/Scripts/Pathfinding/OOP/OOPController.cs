using UnityEngine;

namespace Pathfinding.OOP
{
    // ReSharper disable once InconsistentNaming
    public class OOPController : MonoBehaviour
    {
        [SerializeField] private OOPathfindingController _controller;
        [SerializeField] private OOPathfindingCoupledController _coupledController;
        
        private void Awake()
        {
            if (PathfindingSettings.AllowCoupling)
                _coupledController.Init();
            else
                _controller.Init();
        }
    }
}