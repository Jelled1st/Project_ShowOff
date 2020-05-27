﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))] 
public class CuttableFood : MonoBehaviour, IControllable, IIngredient
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private float _ingredientHeight;
    [SerializeField] private GameObject _currentState;
    [SerializeField] private List<GameObject> _cutStates;
    public CuttingBoard cuttingBoard = null;
    int _currentStateIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(_cutStates != null && _cutStates[0] !=_currentState)
        {
            _cutStates.Insert(0, _currentState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Cut()
    {
        if (_currentStateIndex < _cutStates.Count - 1)
        {
            Destroy(_currentState);
            _currentState = Instantiate(_cutStates[++_currentStateIndex], this.transform);
            _currentState.transform.localPosition = new Vector3(0, 0, 0);
            return true;
        }
        else return false;
    }

    #region IIngredient
    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return _currentStateIndex >= _cutStates.Count - 1;
    }

    public void AddedToDish()
    {

        Destroy(this.gameObject);
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public GameObject GetDishMesh()
    {
        if(ReadyForDish())
        {
            GameObject copy = Instantiate(this.gameObject);
            Destroy(copy.GetComponent<CuttableFood>());
            Destroy(copy.GetComponent<Collider>());
            copy.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return copy;
        }
        return _cutStates[_cutStates.Count-1];
    }

    #endregion

    #region IControllable
    public void OnClick(Vector3 hitPoint)
    {
    }

    public void OnPress(Vector3 hitPoint)
    {
    }

    public void OnHold(float holdTime, Vector3 hitPoint)
    {
    }

    public void OnHoldRelease(float timeHeld)
    {
    }

    public void OnSwipe(Vector3 direction, Vector3 lastPoint)
    {
    }

    public void OnDrag(Vector3 position)
    {
    }

    public void OnDragDrop(Vector3 position, IControllable droppedOn, ControllerHitInfo hitInfo)
    {
        if(!(droppedOn is CuttingBoard))
        {
            cuttingBoard?.RequestRemoveSelected(this);
        }
    }

    public void OnDragDropFailed(Vector3 position)
    {
    }

    public void OnDrop(IControllable dropped, ControllerHitInfo hitInfo)
    {
    }

    public GameObject GetDragCopy()
    {
        GameObject copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<CuttableFood>());
        Destroy(copy.GetComponent<Collider>());
        copy.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return copy;
    }
    #endregion
}
