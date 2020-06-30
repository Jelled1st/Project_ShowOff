using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AlphaOmega.Conveyors
{
    [SelectionBase]
    public class SplitterBelt : FlatConveyorBelt
    {
        [Header("Splitter settings")]
        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private List<float> angles = new List<float> {-45f, 45f};

        protected override Transform RotateTarget => transform.GetChild(0);

        private int _currentAngleIndex;
        private Sequence _sequence;

        private void Start()
        {
            var localRotation = RotateTarget.localRotation;

            localRotation =
                Quaternion.Euler(localRotation.eulerAngles.x, angles[0],
                    localRotation.eulerAngles.z);

            RotateTarget.localRotation = localRotation;
        }

        protected override void Turn()
        {
            var rotation = transform.localRotation.eulerAngles;

            _currentAngleIndex++;
            if (_currentAngleIndex >= angles.Count)
                _currentAngleIndex = 0;

            rotation.y += angles[_currentAngleIndex];

            RotateTween = DOTween.Sequence()
                .Append(RotateTarget.DORotate(rotation, 1f / rotationSpeed))
                .OnComplete(delegate { RotateTween = null; });
        }
    }
}