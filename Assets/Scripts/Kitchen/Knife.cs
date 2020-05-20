using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Knife : MonoBehaviour, IControllable
{
    [Tooltip("1 is most precise (complete equal to the rotation), 0 is perpedicular, -1 is opposite direction")]
    [SerializeField] private float _swipePrecision = 0.8f;
    private Rigidbody _rigidBody;
    private bool _isCutting = false;
    private bool _isResettingCut = false;
    private Vector3 _rotationBeforeCut;

    private List<CuttableFood> _foodsToCut = null;

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
        if(_isCutting)
        {
            Cut();
        }
        else if(_isResettingCut)
        {
            ResetCut();
        }
    }

    private void Cut()
    {
        if (!DOTween.IsTweening(this.transform))
        {
            if (!_isCutting)
            {
                _isCutting = true;
                this.transform.DORotate(new Vector3(0, 0, 0), 0.4f);
            }
            else
            {
                TryCutFood();
                _isCutting = false;
                ResetCut();
            }
        }
    }

    private void ResetCut()
    {
        if (!DOTween.IsTweening(this.transform))
        {
            if (!_isResettingCut)
            {
                _isResettingCut = true;
                this.transform.DORotate(_rotationBeforeCut, 0.4f);
            }
            else
            {
                _isResettingCut = false;
            }
        }
    }

    private void TryCutFood()
    {
        if (_foodsToCut == null) return;
        for (int i = 0; i < _foodsToCut.Count; ++i)
        {
            _foodsToCut[i].Cut();
            Debug.Log("Cut meself some food");
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
            Cut();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        CuttableFood food;
        if(other.TryGetComponent<CuttableFood>(out food))
        {
            if (_foodsToCut == null) _foodsToCut = new List<CuttableFood>();
            _foodsToCut.Add(food);
            Debug.Log("Food entered");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        CuttableFood food;
        if (other.TryGetComponent<CuttableFood>(out food))
        {
            _foodsToCut.Remove(food);
            Debug.Log("Food exited");
        }
    }
}
