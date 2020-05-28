using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _objects;
    [SerializeField] private float _spawnerRotationSpeed;

    [MinMaxSlider(0, 2000)] [SerializeField]
    private Vector2 _pushForce;

    [SerializeField] private float _spawnInterval;


    [SerializeField] private bool _spawnEndlessly;

    [HideIf(nameof(_spawnEndlessly))] [SerializeField]
    private int _objectCount;

    [SerializeField] private bool _isDestroyedAfterTime;

    [ShowIf(nameof(_isDestroyedAfterTime))] [SerializeField]
    private float _timeBeforeDestroy;

    private GameObject _clone;

    private void Start()
    {
        if (_objects.Length == 0)
            return;

        if (Mathf.Abs(_spawnerRotationSpeed) < .01f)
        {
            DOTween.Sequence()
                .AppendCallback(() => { transform.Rotate(new Vector3(0, _spawnerRotationSpeed, 0), Space.World); })
                .SetLoops(-1);
        }


        if (_spawnEndlessly)
        {
            DOTween.Sequence().AppendCallback(Spawn).AppendInterval(_spawnInterval).SetLoops(-1);
        }
        else
        {
            DOTween.Sequence().AppendCallback(Spawn).AppendInterval(_spawnInterval).SetLoops(_objectCount);
        }
    }

    private void Spawn()
    {
        var newObject = Instantiate(_objects[Random.Range(0, _objects.Length)], transform.position, transform.rotation);

        newObject.TryGetComponent(out Rigidbody rBody);

        if (!rBody.Equals(null))
            rBody.AddForce(transform.up * Random.Range(_pushForce.x, _pushForce.y));

        if (_isDestroyedAfterTime)
            Destroy(newObject, _timeBeforeDestroy);
    }
}