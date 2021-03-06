﻿using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Knife : MonoBehaviour, IControllable
{
    [Tooltip("1 is most precise (complete equal to the rotation), 0 is perpedicular, -1 is opposite direction")]
    [SerializeField]
    private float _swipePrecision = 0.8f;

    [SerializeField]
    private int _swipeFramesLength = 5;

    //private Rigidbody _rigidBody;
    private bool _isSwiping = false;
    private int _swipeFramesCount = 0;
    private bool _isCutting = false;
    private bool _isResettingCut = false;
    private Vector3 _rotationBeforeCut;

    private List<CuttableFood> _foodsToCut = null;

    private void Awake()
    {
        var rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
    }

    private void Start()
    {
        _rotationBeforeCut = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (_swipeFramesCount >= _swipeFramesLength || _isCutting)
        {
            _swipeFramesCount = 0;
            Cut();
        }
        else if (_isResettingCut)
        {
            _swipeFramesCount = 0;
            ResetCut();
        }

        if (!_isSwiping)
        {
            _swipeFramesCount = 0;
        }

        _isSwiping = false;
    }

    private void Cut()
    {
        if (!DOTween.IsTweening(transform))
        {
            if (!_isCutting)
            {
                _isCutting = true;
                transform.DORotate(new Vector3(0, 0, 0), 0.4f);
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
        if (!DOTween.IsTweening(transform))
        {
            if (!_isResettingCut)
            {
                _isResettingCut = true;
                transform.DORotate(_rotationBeforeCut, 0.4f);
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
        for (var i = 0; i < _foodsToCut.Count; ++i)
        {
            _foodsToCut[i].Cut();
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
        if (_isCutting || _isResettingCut) return;
        var directionRotationDiff = Vector3.Dot(direction.normalized, transform.forward);
        if (Mathf.Abs(directionRotationDiff) >= _swipePrecision)
        {
            _isSwiping = true;
            ++_swipeFramesCount;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        CuttableFood food;
        if (other.TryGetComponent<CuttableFood>(out food))
        {
            if (_foodsToCut == null) _foodsToCut = new List<CuttableFood>();
            _foodsToCut.Add(food);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        CuttableFood food;
        if (other.TryGetComponent<CuttableFood>(out food))
        {
            _foodsToCut.Remove(food);
        }
    }
}