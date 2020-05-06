using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatConveyorBeltCurve : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Material _conveyorMaterial;

    private Rigidbody _rBody;
    //magic number so that the speed of the moving package matches the speed of the moving texture - eye candy
    private float _eyeCandySpeedMultiplier = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<Rigidbody>(out _rBody))
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
        Quaternion rot = _rBody.rotation;
        _rBody.rotation *= Quaternion.Euler(0, _speed*_eyeCandySpeedMultiplier, 0);
        _rBody.MoveRotation(rot);
    }
}
