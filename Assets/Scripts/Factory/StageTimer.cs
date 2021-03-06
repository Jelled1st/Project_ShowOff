using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Factory
{
    public class StageTimer : MonoBehaviour
    {
        public event Action TimeEnded = delegate { };

        [SerializeField]
        private float _roundTime;

        private ISetTimer _timerImplementation;
        private Sequence _timerSequence;
        private float _timeRemaining;

        public float TimeRemaining => _timeRemaining;

        private void Awake()
        {
            _timerImplementation =
                (ISetTimer) FindObjectsOfType<MonoBehaviour>().FirstOrDefault(t => t is ISetTimer);

            if (_timerImplementation == null)
            {
                Debug.LogError("Can't find any object that sets the timer!");
            }
        }

        public void StartTimer()
        {
            if (_timerSequence != null)
            {
                _timerSequence.Kill();
                _timerSequence = null;
            }

            _timerSequence = DOTween.Sequence()
                .AppendInterval(_roundTime)
                .OnUpdate(delegate
                {
                    _timeRemaining = _roundTime - _timerSequence.Elapsed();
                    _timerImplementation.SetTimer((int) TimeRemaining);
                })
                .OnComplete(delegate { TimeEnded(); });
        }

        public void StopTimer()
        {
            _timerSequence.Kill();
        }
    }
}