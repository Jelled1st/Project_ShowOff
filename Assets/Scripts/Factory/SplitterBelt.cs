using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Factory
{
    [SelectionBase]
    public class SplitterBelt : FlatConveyorBelt
    {
        [Header("Splitter settings")] [SerializeField]
        private float _rotationSpeed;

        [SerializeField] private float _delay;
        [SerializeField] private List<float> _angles = new List<float> {-45f, 45f};

        private int _currentAngleId = 0;
        private Sequence _sequence;
        private bool _direction;

        protected override void Start()
        {
            base.Start();

            var localRotation = transform.localRotation;

            localRotation =
                Quaternion.Euler(localRotation.eulerAngles.x, localRotation.eulerAngles.y + _angles[0],
                    localRotation.eulerAngles.z);

            transform.localRotation = localRotation;
        }


        public override void OnPress(Vector3 hitPoint)
        {
            Move();
        }

        public override void OnHold(float holdTime, Vector3 hitPoint)
        {
        }

        public override void OnHoldRelease(float timeHeld)
        {
        }

        private void Move()
        {
            var rotation = transform.parent.localRotation.eulerAngles;

            _currentAngleId++;
            if (_currentAngleId >= _angles.Count)
                _currentAngleId = 0;

            rotation.y += _angles[_currentAngleId];

            _sequence = DOTween.Sequence();
            _sequence.AppendCallback(() => { _direction = !_direction; });
            _sequence.Append(_rBody.DORotate(rotation, 1 / _rotationSpeed));
            _sequence.AppendCallback(() => { });
            _sequence.AppendInterval(_delay);
        }
    }
}