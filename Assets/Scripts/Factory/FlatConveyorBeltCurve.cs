using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBeltCurve : FlatConveyorBelt
{
    //magic number so that the speed of the moving package matches the speed of the moving texture - eye candy
    private float _eyeCandySpeedMultiplier = 0.7f;
    private Tween _rotateTween;


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        Quaternion rot = _rBody.rotation;
        _rBody.rotation *= Quaternion.Euler(0, Speed * _eyeCandySpeedMultiplier, 0);
        _rBody.MoveRotation(rot);
    }

    public override void Turn()
    {
        if (_rotateTween == null || !_rotateTween.IsPlaying())
        {
            _rotateTween =
                this.gameObject.transform.parent.DORotate(
                    this.gameObject.transform.parent.rotation.eulerAngles + new Vector3(0, 90, 0), 0.2f);
        }
    }
}