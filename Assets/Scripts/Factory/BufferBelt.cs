using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    public class BufferBelt : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Transform _output;

        [SerializeField]
        private CollisionCallback _collisionCallback;

        [SerializeField]
        private float _duration = 1f;

        private Tween _releaseTween;
        private bool _allowedToRelease = false;
        private Queue<GameObject> _storedObjects = new Queue<GameObject>();

        private void OnEnable()
        {
            _collisionCallback.TriggerEnter += TriggerEnter;
        }

        private void OnDisable()
        {
            _collisionCallback.TriggerEnter -= TriggerEnter;
        }

        private void TriggerEnter(Collider other)
        {
            if (!_storedObjects.Contains(other.gameObject))
                _storedObjects.Enqueue(other.gameObject);

            if (_releaseTween == null && _allowedToRelease)
                ReleaseObjects();
        }

        public void ReleaseObjects()
        {
            _allowedToRelease = true;

            if (_releaseTween != null)
                return;

            _releaseTween = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    if (_storedObjects.Count > 0)
                        SpitItem(_storedObjects.Dequeue());
                })
                .AppendInterval(2f)
                .SetLoops(_storedObjects.Count);
            _releaseTween.OnComplete(() =>
            {
                _releaseTween = null;
                if (_storedObjects.Count > 0)
                    ReleaseObjects();
            });
        }

        private void SpitItem(GameObject inputGameObject)
        {
            if (inputGameObject.TryGetComponent(out Collider spitItem))
            {
                spitItem.enabled = false;
                DOTween.Sequence()
                    .Append(inputGameObject.transform.DOMove(
                        new Vector3(_output.position.x, inputGameObject.transform.position.y, _output.position.z),
                        _duration))
                    .AppendCallback(() => { spitItem.enabled = true; });
            }
        }
    }
}