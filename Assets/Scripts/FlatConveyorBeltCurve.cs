using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBeltCurve : FlatConveyorBelt
{
    private Transform _childConveyor;
    //magic number so that the speed of the moving package matches the speed of the moving texture - eye candy
    private float _eyeCandySpeedMultiplier = 0.7f;

    private bool _turning = false;
    private float _totalTurned = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<Rigidbody>(out _rBody))
        {
            _rBody = this.gameObject.AddComponent<Rigidbody>();
        }
        _rBody.useGravity = true;
        _rBody.isKinematic = true;
        
        SetConveyorSpeed();

        _childConveyor = this.gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion rot = _rBody.rotation;
        _rBody.rotation *= Quaternion.Euler(0, _speed * _eyeCandySpeedMultiplier, 0);
        _rBody.MoveRotation(rot);

        if(_turning)
        {
            Turn();
            if (_totalTurned >= 90)
            {
                _totalTurned = 0;
                _turning = false;
            }
        }
    }

    public override void Turn()
    {
        float turnAmount = 90 * 0.2f;
        if(_totalTurned + turnAmount > 90)
        {
            turnAmount = 90 - _totalTurned;
        }
        _totalTurned += turnAmount;
        this.gameObject.transform.RotateAround(new Vector3(-1, 0, -1), new Vector3(0, 1, 0), turnAmount); // -> correct
        _turning = true;
    }
}
