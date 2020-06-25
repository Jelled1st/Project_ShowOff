using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    public class BufferBelt : MonoBehaviour
    {
        public event Action FinishedOutputting = delegate { };

        [Required]
        [SerializeField]
        private Transform _output;

        [Tag]
        [SerializeField]
        private string _allowInputItemTag;

        [SerializeField]
        private Transform _throwPosition;

        [SerializeField]
        private CollisionCallback _collisionCallback;

        [SerializeField]
        private float _duration = 1f;

        private Tween _releaseTween;
        private bool _allowedToRelease = false;
        private readonly Queue<GameObject> _storedObjects = new Queue<GameObject>();

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
            if (!other.CompareTag(_allowInputItemTag))
            {
                other.transform.DOJump(_throwPosition.position, 2f, 1, 0.5f);
            }

            if (!_storedObjects.Contains(other.gameObject))
                _storedObjects.Enqueue(other.gameObject);

            if (_allowedToRelease)
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
                .AppendInterval(_duration)
                .SetLoops(_storedObjects.Count);
            _releaseTween.OnComplete(() =>
            {
                _releaseTween.Kill();
                _releaseTween = null;

                if (_storedObjects.Count > 0)
                {
                    ReleaseObjects();
                }
                else
                {
                    FinishedOutputting();
                }
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