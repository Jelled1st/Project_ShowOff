using System;
using DG.Tweening;
using UnityEngine;

namespace Factory
{
    public class FactoryTimer : MonoBehaviour
    {
        public event Action TimeEnded = delegate { };

        [SerializeField]
        private float _roundTime;

        private FactoryUiManager _factoryUiManager;
        private Sequence _timerSequence;

        private void Awake()
        {
            _factoryUiManager = FindObjectOfType<FactoryUiManager>();
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
                .OnUpdate(delegate { _factoryUiManager.SetTimer((int) (_roundTime - _timerSequence.Elapsed())); })
                .OnComplete(delegate { TimeEnded(); });
        }
    }
}