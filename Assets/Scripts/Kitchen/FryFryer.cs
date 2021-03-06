﻿using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class FryFryer : MonoBehaviour, IControllable, ISubject
{
    [SerializeField]
    private GameObject _basket;

    [SerializeField]
    private GameObject _foodNode;

    [SerializeField]
    private GameObject _basketDownNode;

    [SerializeField]
    private ProgressBar _progressBar;

    [SerializeField]
    private GameObject _oil;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _oilTempIncrease;

    private FryableFood _food;
    private bool _basketIsUp = true;
    private Material _oilMat;
    private float _oilHeat = 0;

    private List<IObserver> _observers = new List<IObserver>();

    private void Start()
    {
        _progressBar.SetActive(false);
        _oilMat = _oil.GetComponent<Renderer>().materials[0];
    }

    private void Update()
    {
        if (_food != null && !_basketIsUp)
        {
            _oilHeat = Mathf.Min(_oilHeat + _oilTempIncrease, 1.0f);
            _oilMat.SetFloat("_OilCooking", _oilHeat);
            _food.Fry();
            var value = _food.GetTimeFried() / _food.GetFryTime();
            _progressBar.SetPercentage(value);
            _progressBar.SetFillColor(new Color(1 - value, value, 0, 1));
            if (_food.IsFried())
            {
                Notify(new FryerStopEvent(this, _food));
                MoveBasketUp();
            }
        }
        else
        {
            _oilHeat = Mathf.Max(_oilHeat - _oilTempIncrease, 0.0f);
            _oilMat.SetFloat("_OilCooking", _oilHeat);
        }
    }

    private void MoveBasketUp()
    {
        var pos = _basketDownNode.transform.position;
        var rot = _basketDownNode.transform.rotation.eulerAngles;
        if (_food != null)
        {
            var foodPos = _food.transform.position + pos - _basket.transform.position;
            var foodRotation =
                (Quaternion.FromToRotation(_basket.transform.rotation.eulerAngles,
                    _basketDownNode.transform.rotation.eulerAngles) * _food.transform.rotation).eulerAngles;
            _food.transform.DOMove(foodPos, 0.3f);
            _food.transform.DORotate(foodRotation, 0.3f);
        }

        _basketDownNode.transform.position = _basket.transform.position;
        _basketDownNode.transform.rotation = _basket.transform.rotation;
        _basket.transform.DOMove(pos, 0.3f);
        _basket.transform.DORotate(rot, 0.3f);
        _basketIsUp = true;
        _progressBar.SetActive(false);
    }

    private void MoveBasketDown()
    {
        var pos = _basketDownNode.transform.position;
        var rot = _basketDownNode.transform.rotation.eulerAngles;
        if (_food != null)
        {
            var foodPos = _food.transform.position + pos - _basket.transform.position;
            var foodRotation =
                (Quaternion.FromToRotation(_basket.transform.rotation.eulerAngles,
                    _basketDownNode.transform.rotation.eulerAngles) * _food.transform.rotation).eulerAngles;
            _food.transform.DOMove(foodPos, 0.3f);
            _food.transform.DORotate(foodRotation, 0.3f);
        }

        _basketDownNode.transform.position = _basket.transform.position;
        _basketDownNode.transform.rotation = _basket.transform.rotation;
        _basket.transform.DOMove(pos, 0.3f);
        _basket.transform.DORotate(rot, 0.3f);

        _basketIsUp = false;
        _progressBar.SetActive(true);
    }

    public void TrySetFood(FryableFood food)
    {
        if (_food != null) return;
        _food = food;
        food.transform.position = _foodNode.transform.position;
        food.fryer = this;
        Notify(new FryerStartEvent(this, food));
        MoveBasketDown();
    }

    public void RemoveFood(FryableFood food)
    {
        if (food == null) return;
        _food = null;
        food.fryer = null;
    }

    #region IControllable

    public GameObject GetDragCopy()
    {
        if (_food == null || !_basketIsUp)
        {
            Debug.Log("Returning null");
            return null;
        }

        var copy = _food.GetDragCopy();
        copy.transform.SetParent(null);
        return copy;
    }

    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        if (_food != null) droppedOn.OnDrop(_food.GetComponent<IControllable>(), hitInfo);
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is FryableFood)
        {
            TrySetFood(dropped as FryableFood);
        }
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
    }

    #endregion

    public void Register(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void UnRegister(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(AObserverEvent observerEvent)
    {
        for (var i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}