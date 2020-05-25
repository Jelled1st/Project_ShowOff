﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BakableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private float _timeToBake;
    [SerializeField] private int _jumpHeight = 10;
    private float[] _bakedTimes = new float[2];
    private int _currentFace = 0;
    private bool _isBaking = false;
    public FryingPan fryingPan;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _isBaking = false;
    }

    //called by the thing that does the baking
    public void Bake()
    {
        _isBaking = true;
        _bakedTimes[_currentFace] += Time.deltaTime;
    }

    public void Flip()
    {
        if (_isBaking && !DOTween.IsTweening(this.transform))
        {
            _currentFace = (_currentFace+1) % 2;
            this.transform.DOPunchPosition(new Vector3(0, _ingredientHeight * _jumpHeight, 0), 0.7f, 0);
            this.transform.DORotate(new Vector3(180, 0, 0), 0.4f, RotateMode.WorldAxisAdd);
        }
    }

    #region IIngredient
    public bool ReadyForDish()
    {
        return _bakedTimes[0] > _timeToBake && _bakedTimes[1] > _timeToBake;
    }

    public void AddedToDish()
    {
        fryingPan.RemoveFood(this);
        Destroy(this.gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public GameObject GetDishMesh()
    {
        if (ReadyForDish())
        {
            return this.gameObject;
        }
        else return null;
    }

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    #endregion

    #region IControllable
    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<Collider>());
        Destroy(copy.GetComponent<BakableFood>());
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
        Flip();
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }
    #endregion
}
