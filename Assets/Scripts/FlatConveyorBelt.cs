using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBelt : MonoBehaviour, IControllable
{
    [SerializeField] protected float _speed = 1;
    [SerializeField] protected Material _conveyorMaterial;

    protected Rigidbody _rBody;
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

    public virtual void Turn()
    {
        if (_rotateTween == null || !_rotateTween.IsPlaying())
        {
            _rotateTween = this.gameObject.transform.DORotate(
                this.gameObject.transform.rotation.eulerAngles + new Vector3(0, 90, 0),
                0.2f);
        }
    }

    public void OnPress()
    {
        Turn();
    }

    public void OnHold(float holdTime)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnSwipe(Vector3 direction)
    {
    }
}