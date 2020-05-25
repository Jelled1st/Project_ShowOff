using System;
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
        [SerializeField] private float _angle;

        private Sequence _sequence;
        private bool _direction;

        protected override void Start()
        {
            base.Start();

            transform.rotation =
                Quaternion.Euler(transform.rotation.eulerAngles.z, _angle / 2f, transform.rotation.eulerAngles.z);
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
            var rotation = transform.parent.rotation.eulerAngles;

            if (_direction)
                rotation.y += _angle / 2f;
            else
                rotation.y -= _angle / 2f;


            _sequence = DOTween.Sequence();
            _sequence.AppendCallback(() => { _direction = !_direction; });
            _sequence.Append(_rBody.DORotate(rotation, 1 / _rotationSpeed));
            _sequence.AppendCallback(() => { });
            _sequence.AppendInterval(_delay);
        }
    }
}