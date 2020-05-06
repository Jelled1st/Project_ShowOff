using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _packagePrefab;

    [SerializeField] private Transform _startPosition;

    [SerializeField] private float _groundLevel;

    [SerializeField] private float _respawnTime;

    [SerializeField] private float _pushForce;

    [SerializeField] private float _stuckCheckTime;

    [SerializeField] private float _stuckDistance;

    private GameObject _package;
    private bool _isRespawning;
    private Vector3 _lastPosition;

    private void OnEnable()
    {
        FindObjectOfType<FinishTrigger>().FinishTriggerHit.AddListener(OnFinishTriggerHit);
        InvokeRepeating(nameof(CheckStuck), _stuckCheckTime, _stuckCheckTime);
    }

    private void OnDisable()
    {
        FindObjectOfType<FinishTrigger>()?.FinishTriggerHit.RemoveListener(OnFinishTriggerHit);
        CancelInvoke();
    }

    private void CheckStuck()
    {
        var distanceDelta = (_package.transform.position - _lastPosition).magnitude;

        if (distanceDelta < _stuckDistance)
        {
            RespawnPackage();
        }

        _lastPosition = _package.transform.position;
    }

    private void OnFinishTriggerHit(GameObject hitGameObject)
    {
        if (gameObject == _package)
        {
            LevelFinished();
        }
    }

    private void LevelFinished()
    {
        Debug.Log("Package reached the end");
        RespawnPackage();
    }


    private void Start()
    {
        _package = Instantiate(_packagePrefab);
        RespawnPackage();
    }

    private void Update()
    {
        if (!_isRespawning && _package.transform.position.y <= _groundLevel)
        {
            _isRespawning = true;
            Invoke(nameof(RespawnPackage), _respawnTime);
        }
    }

    private void RespawnPackage()
    {
        var rigidbody = _package.GetComponentInChildren<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
        _package.transform.position = _startPosition.position;
        _package.transform.rotation = _packagePrefab.transform.rotation;

        rigidbody.AddForce(Vector3.left * _pushForce);

        _isRespawning = false;
    }
}