using UnityEngine;
using DG.Tweening;

public class FlatConveyorBeltCurve : FlatConveyorBelt
{
    //magic number so that the speed of the moving package matches the speed of the moving texture - eye candy
    private float _eyeCandySpeedMultiplier = 0.7f;

    protected override void FixedUpdateMovement()
    {
        Quaternion rot = _rBody.rotation;
        _rBody.rotation *= Quaternion.Euler(0, Speed * _eyeCandySpeedMultiplier, 0);
        _rBody.MoveRotation(rot);
    }

    protected override void OnCollisionStay(Collision other)
    {
    }

    protected override void Turn()
    {
        _rotateTween = DOTween.Sequence().Append(
                this.gameObject.transform.parent.DORotate(
                    this.gameObject.transform.parent.rotation.eulerAngles + new Vector3(0, 90, 0), RotateInterval))
            .AppendCallback(
                () => { _rotateTween = null; });
    }
}