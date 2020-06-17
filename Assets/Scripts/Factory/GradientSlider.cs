using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Factory
{
    [RequireComponent(typeof(Slider))]
    public class GradientSlider : MonoBehaviour
    {
        [Serializable]
        public class GradientPoint : IComparable<GradientPoint>
        {
            [Range(0f, 1f)]
            [SerializeField]
            private float _changePercentage = 0f;

            [SerializeField]
            private Color _color = Color.black;

            public float ChangePercentage => _changePercentage;

            public Color Color => _color;

            public int CompareTo(GradientPoint other)
            {
                return _changePercentage.CompareTo(other._changePercentage);
            }
        }

        [ReorderableList]
        [SerializeField]
        private GradientPoint[] _gradientPoints;

        private Slider _slider;
        private Image _sliderImage;
        private List<GradientPoint> _sortedGradientPoints;
        private int _currentPointId = 0;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(UpdateColor);

            _sliderImage = _slider.fillRect.GetComponent<Image>();

            _sortedGradientPoints = _gradientPoints.ToList();
            _sortedGradientPoints.Sort();
        }

        private void UpdateColor(float value)
        {
            print($"{value}:{_sortedGradientPoints[_currentPointId].ChangePercentage}");
            if (value > _sortedGradientPoints[_currentPointId].ChangePercentage)
            {
                _currentPointId++;
            }

            if (Mathf.Approximately(value, 0f))
            {
                print("true");
                _sliderImage.color = _sortedGradientPoints[0].Color;
                _currentPointId = 0;
                return;
            }

            if (_sortedGradientPoints.Count - 1 <= _currentPointId)
                return;

            float percentBeforeNewPoint;

            if (_sortedGradientPoints[_currentPointId].ChangePercentage == 0)
                percentBeforeNewPoint = 0;
            else
                percentBeforeNewPoint = Mathf.InverseLerp(_sortedGradientPoints[_currentPointId - 1].ChangePercentage,
                    _sortedGradientPoints[_currentPointId].ChangePercentage, value);

            var newColor = Color.LerpUnclamped(_sortedGradientPoints[_currentPointId].Color,
                _sortedGradientPoints[_currentPointId + 1].Color,
                percentBeforeNewPoint);

            // Debug.Log(
            //     $"{percentBeforeNewPoint}:{_sliderImage.color}:{_sortedGradientPoints[_currentPointId].Color}:{newColor.ToString()}",
            //     gameObject);

            _sliderImage.color = newColor;
        }
    }
}