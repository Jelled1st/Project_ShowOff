﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class CookingPan : MonoBehaviour, IControllable
{
    [SerializeField] GameObject _foodNode;
    [SerializeField] Stove stove;
    [SerializeField] GameObject _stirringDevice;
    [SerializeField] float _stirBonusModifier = 0.5f;
    CookableFood _food;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (stove == null || stove.IsOn())
        {
            _food?.Cook();
        }
    }

    public void TrySetfood(CookableFood food)
    {
        if (_food != null) return;
        _food = food;
        food.transform.position = _foodNode.transform.position;
        food.cookingPan = this;
    }

    public void RemoveFood(CookableFood food)
    {
        if (food == null) return;
        _food = null;
        food.cookingPan = null;
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
        if (_food == null || !_food.IsCooked())
        {
            return null;
        }
        GameObject copy = Instantiate(_stirringDevice);
        GameObject foodCopy = _food.GetDragCopy();
        foodCopy.transform.SetParent(copy.transform);
        Destroy(copy.GetComponent<CookingPan>());
        Collider[] colliders = copy.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            Destroy(colliders[i]);
        }
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
        if(_food != null) droppedOn.OnDrop(_food.GetComponent<IControllable>(), hitInfo);
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
        if (dropped is CookableFood)
        {
            TrySetfood(dropped as CookableFood);
        }
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
        if(!DOTween.IsTweening(_stirringDevice.transform))_stirringDevice.transform.DORotate(new Vector3(0, 360, 0), 0.7f, RotateMode.LocalAxisAdd);
        _food?.Cook(_stirBonusModifier);
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
}
