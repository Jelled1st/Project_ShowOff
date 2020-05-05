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

    private void Start()
    {
        _package = Instantiate(_packagePrefab, _startPosition);
        _package.GetComponentInChildren<Rigidbody>().AddForce(Vector3.left * _pushForce);
    }

    private void Update()
    {
        if (_package.transform.position.y <= _groundLevel)
        {
            Invoke(nameof(RespawnPackage), _respawnTime);
        }
    }

    private void RespawnPackage()
    {
        _package.transform.position = _startPosition.position;
        _package.transform.rotation = Quaternion.identity;
    }
}