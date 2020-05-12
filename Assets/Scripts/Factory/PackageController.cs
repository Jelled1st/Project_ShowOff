using System;
using StateMachine;
using UnityEngine;

namespace Factory
{
    public class PackageController : MonoBehaviour
    {
        public static event Action<PackageController> PackageStuck = delegate { };
        public static event Action<PackageController> PackageWentUnderGround = delegate { };

        private static FactoryStageSettings FactoryStageSettings =>
            GameStageManager.GetStageSettings<FactoryStageSettings>();

        private Vector3 _lastPosition;
        private bool _isUnderGroundLevel;


        private void Awake()
        {
            InvokeRepeating(nameof(CheckStuck), 0f, FactoryStageSettings.StuckCheckTime);
        }

        private void Update()
        {
            if (!_isUnderGroundLevel && transform.position.y <= FactoryStageSettings.GroundLevel)
            {
                _isUnderGroundLevel = true;
                PackageWentUnderGround.Invoke(this);
            }
            else if (transform.position.y >= FactoryStageSettings.GroundLevel)
            {
                _isUnderGroundLevel = false;
            }
        }

        private void CheckStuck()
        {
            var distanceDelta = (transform.position - _lastPosition).magnitude;

            if (distanceDelta < FactoryStageSettings.StuckDistance)
            {
                PackageStuck.Invoke(this);
            }

            _lastPosition = transform.position;
        }
    }
}