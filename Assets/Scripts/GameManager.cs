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


    private GameObject _package;
    private bool _isRespawning;

    private void OnEnable()
    {
        FindObjectOfType<FinishTrigger>().FinishTriggerHit.AddListener(OnFinishTriggerHit);
    }

    private void OnDisable()
    {
        FindObjectOfType<FinishTrigger>().FinishTriggerHit.RemoveListener(OnFinishTriggerHit);
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
        
        rigidbody.velocity=Vector3.zero;
        _package.transform.position = _startPosition.position;
        _package.transform.rotation = Quaternion.identity;

        rigidbody.AddForce(Vector3.left * _pushForce);

        _isRespawning = false;
    }
}