using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBelt : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Material _conveyorMaterial;

    private Rigidbody _rBody;

    //magic number so that the speed of the moving package matches the speed of the moving texture - eye candy
    private float _eyeCandySpeedMultiplier = 0.7f;
    private Tween _rotateTween;


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
        Vector3 pos = _rBody.position;
        _rBody.position += _rBody.transform.right * -_speed * Time.deltaTime;
        _rBody.MovePosition(pos);
    }


    public void Turn()
    {
        if (_rotateTween == null || !_rotateTween.IsPlaying())
        {
            _rotateTween = this.gameObject.transform.DORotate(
                this.gameObject.transform.rotation.eulerAngles + new Vector3(0, 90, 0),
                0.2f);
        }
    }
}