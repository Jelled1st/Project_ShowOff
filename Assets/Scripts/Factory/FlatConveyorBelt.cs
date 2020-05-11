using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlatConveyorBelt : MonoBehaviour, IControllable
{
    [SerializeField] protected float _speed = 1;
    [SerializeField] protected MeshRenderer _conveyorRenderer;

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

        SetConveyorSpeed();
    }

    protected void SetConveyorSpeed()
    {
        if (!_conveyorRenderer.Equals(null))
        {
            for (var i = 0; i < _conveyorRenderer.materials.Length; i++)
            {
                if (_conveyorRenderer.materials[i].shader.name.Equals("Shader Graphs/shdr_textureScroll"))
                {
                    _conveyorRenderer.materials[i] = new Material(_conveyorRenderer.materials[i]);
                    _conveyorRenderer.materials[i].SetFloat("_scrollingSpeed", _speed);
                }
            }
        }
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

    public void OnClick(Vector3 hitPoint)
    {

    }

    public void OnPress(Vector3 hitPoint)
    {
        Turn();
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPosition)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }
}