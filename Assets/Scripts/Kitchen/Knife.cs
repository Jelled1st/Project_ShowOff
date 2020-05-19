using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class Knife : MonoBehaviour, IControllable
{
    [Tooltip("1 is most precise (complete equal to the rotation), 0 is perpedicular, -1 is opposite direction")]
    [SerializeField] private float _swipePrecision = 0.8f;
    private Rigidbody _rigidBody;
    private int _swipeFramesCount = 0;
    private bool _isSwiping = false;
    private bool _isCutting = false;
    private bool _isResettingCut = false;
    private Vector3 _rotationBeforeCut;

    void Awake()
    {
        _rigidBody = this.gameObject.GetComponent<Rigidbody>();
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rotationBeforeCut = this.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(_swipeFramesCount >= 5 || _isCutting)
        {
            Cut();
            _swipeFramesCount = 0;
        }
        else if(_isResettingCut)
        {
            _swipeFramesCount = 0; // make sure a new cut cannot happen whilst resetting
            ResetCut();
        }
        if(!_isSwiping)
        {
            _swipeFramesCount = 0;
        }
        _isSwiping = false; // if swiping is active it will be set true again (set it to false because it would otherwise never be false)
    }

    private void Cut()
    {
        if (!DOTween.IsTweening(this.transform))
        {
            Debug.Log("Not tweening");
            if (!_isCutting)
            {
                _isCutting = true;
                this.transform.DORotate(new Vector3(0, 0, 0), 0.4f);
                Debug.Log("tweening down");
            }
            else
            {
                _isCutting = false;
                ResetCut();
                Debug.Log("Bottom cut");
            }
        }
    }

    private void ResetCut()
    {
        if (!DOTween.IsTweening(this.transform))
        {
            Debug.Log("Not tweening");
            if (!_isResettingCut)
            {
                _isResettingCut = true;
                this.transform.DORotate(_rotationBeforeCut, 0.4f);
                Debug.Log("tweening down");
            }
            else
            {
                _isResettingCut = false;
                Debug.Log("End o' cut");
            }
        }
    }

    public GameObject GetDragCopy()
    {
        return null;
    }

    public void OnClick(Vector3 hitPoint)
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

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
        float directionRotationDiff = Vector3.Dot(direction.normalized, this.transform.forward);
        if(directionRotationDiff >= _swipePrecision)
        {
            _isSwiping = true;
            ++_swipeFramesCount;
        }
    }
}
