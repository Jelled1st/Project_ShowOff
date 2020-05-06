﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatConveyorBelt : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Material _conveyorMaterial;

    private Rigidbody _rBody;
    // Start is called before the first frame update
    void Start()
    {
        if(!TryGetComponent<Rigidbody>(out _rBody))
        {
            _rBody = this.gameObject.AddComponent<Rigidbody>();
        }
        _rBody.useGravity = true;
        _rBody.isKinematic = true;
        _conveyorMaterial.SetFloat("_scrollingSpeed", _speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = _rBody.position;
        _rBody.position += _rBody.transform.right * -_speed * Time.deltaTime;
        _rBody.MovePosition(pos);
    }
}
