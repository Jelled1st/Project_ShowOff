using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class CookingPan : MonoBehaviour, IControllable, ISubject
{
    [SerializeField] GameObject _foodNode;
    [SerializeField] Stove stove;
    [SerializeField] GameObject _stirringDeviceRotator;
    [SerializeField] GameObject _stirringDeviceBottom;
    [SerializeField] float _stirBonusModifier = 0.5f;
    CookableFood _food;
    private bool _foodIsCooked = false;

    List<IObserver> _observers = new List<IObserver>();


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (stove == null || stove.IsOn())
        {
            if (_food != null)
            {
                _food.Cook();
                if (_food.IsCooked() && !_foodIsCooked)
                {
                    _foodIsCooked = true;
                    Notify(new CookingDoneEvent(this, _food));
                }
            }
        }
    }

    public void TrySetfood(CookableFood food)
    {
        if (_food != null) return;
        _food = food;
        food.transform.position = _foodNode.transform.position;
        food.cookingPan = this;
        Notify(new CookingStartEvent(this, _food));
        _foodIsCooked = false;
    }

    public void RemoveFood()
    {
        if (_food == null) return;
        Notify(new CookingStopEvent(this, _food));
        _food = null;
        _food.cookingPan = null;
    }

    #region IControllable
    public GameObject GetDragCopy()
    {
        if (_food == null || !_food.IsCooked())
        {
            return null;
        }
        GameObject copy = Instantiate(_stirringDeviceRotator);
        copy.transform.localScale = _stirringDeviceRotator.transform.lossyScale;
        GameObject foodCopy = _food.GetDragCopy();
        foodCopy.transform.SetParent(copy.transform);
        foodCopy.transform.localPosition = _stirringDeviceBottom.transform.localPosition;
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
        if (!DOTween.IsTweening(_stirringDeviceRotator.transform))
        {
            _stirringDeviceRotator.transform.DORotate(new Vector3(0, 360, 0), 0.7f, RotateMode.LocalAxisAdd);
        }
        _food?.Cook(_stirBonusModifier);
        Notify(new CookingStirEvent(this, _food));
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
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
    #endregion
}
