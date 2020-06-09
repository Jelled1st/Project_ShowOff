
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Stove : MonoBehaviour, IControllable, ISubject
{
    private List<IObserver> _observers = new List<IObserver>();
    private bool _turnedOn = false;

    public GameObject GetDragCopy()
    {
        return null;
    }

    public bool IsOn()
    {
        return _turnedOn;
    }

    #region IControllable
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
        _turnedOn = !_turnedOn;
        Notify(new StoveToggleEvent(this));
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
        for(int i = 0; i < _observers.Count; ++i)
        {
            _observers[i].OnNotify(observerEvent);
        }
    }
}
