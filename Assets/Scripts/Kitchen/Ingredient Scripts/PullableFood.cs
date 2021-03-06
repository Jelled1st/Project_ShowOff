﻿using System.Collections.Generic;
using UnityEngine;

public class PullableFood : MonoBehaviour, IIngredient, IControllable, ISubject
{
    [SerializeField]
    private IngredientType _ingredientType;

    [SerializeField]
    private float _ingredientHeight;

    [SerializeField]
    private List<PullableFoodPullable> _pullables;

    [SerializeField]
    private InvisibleOnDrag _invisibleOnDrag;

    private PullableFoodPullable currentPullable;

    private List<IObserver> _observers = new List<IObserver>();

    private void Awake()
    {
        tag = "Ingredient";
    }

    private void Start()
    {
        if (_invisibleOnDrag != null) _invisibleOnDrag.active = false;
        for (var i = 0; i < _pullables.Count; ++i)
        {
            _pullables[i].foodParent = this;
        }
    }

    public void OnDisable()
    {
        for (var i = 0; i < _pullables.Count; ++i)
        {
            _pullables[i].gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        for (var i = 0; i < _pullables.Count; ++i)
        {
            _pullables[i].gameObject.SetActive(true);
        }
    }

    public void OnDestroy()
    {
        for (var i = 0; i < _pullables.Count; ++i)
        {
            Destroy(_pullables[i].gameObject);
        }
    }

    public void Pull(PullableFoodPullable pulled)
    {
        if (_pullables.Contains(pulled))
        {
            currentPullable = pulled;
            Notify(new PullablePulledEvent(this));
        }
    }

    #region IIngredient

    public void AddedToDish()
    {
        if (currentPullable != null)
        {
            _pullables.Remove(currentPullable);
            Destroy(currentPullable.gameObject);
            currentPullable = null;
            if (_pullables.Count == 0) Notify(new IngredientDoneEvent(this));
        }
    }

    public GameObject GetDishMesh()
    {
        //    GameObject copy = Instantiate(this.gameObject);
        //    Destroy(copy.GetComponent<PullableFood>());
        //    Destroy(copy.GetComponent<Collider>());
        //    copy.transform.localScale = this.transform.lossyScale;
        //    copy.GetComponent<Renderer>().enabled = true;
        //    return copy;
        return currentPullable.GetDishMesh();
    }

    public float GetHeight()
    {
        return _ingredientHeight;
    }

    public IngredientType GetIngredientType()
    {
        return _ingredientType;
    }

    public bool ReadyForDish()
    {
        return currentPullable != null;
    }

    #endregion

    #region IControllable

    public GameObject GetDragCopy()
    {
        if (ReadyForDish())
        {
            if (_invisibleOnDrag != null) _invisibleOnDrag.active = true;
            var copy = Instantiate(gameObject);
            Destroy(copy.GetComponent<PullableFood>());
            Destroy(copy.GetComponent<Collider>());
            copy.transform.localScale = transform.lossyScale;
            return copy;
        }

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
        if (droppedOn is Dish)
        {
            droppedOn.OnDrop(this, hitInfo);
        }
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
    }

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

    #endregion
}